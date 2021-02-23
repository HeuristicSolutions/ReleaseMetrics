using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.TimeEntries.MavenlinkApi;
using Z.EntityFramework.Plus;

namespace ReleaseMetrics.Core.TimeEntries {

	/// <summary>
	/// Handles the ETL tasks pertaining to the Mavenlink system. Mavenlink time entries are stored locally where they
	/// can be edited before metrics are processed. (This is because metrics are sometimes run after timesheets are approved,
	/// and entries are not easily modified in ML after approved and invoiced)
	/// </summary>
	public class MavenlinkLoader {

		protected MetricsDbContext Database { get; set; }
		protected MavenlinkApiProxy MavenlinkApi { get; set; }

		public MavenlinkLoader(MetricsDbContext database, MavenlinkApiProxy mavenlinkApi) {
			Database = database;
			MavenlinkApi = mavenlinkApi;
		}

		/// <summary>
		/// Talks to the Mavenlink API and pulls ALL time entries for the specified release. Each record is converted into
		/// a normalized TimeEntry record and cached locally. Entries are linked to WorkItems AS LONG AS the referenced Jira
		/// story is already locally cached. (Generally, speaking, you should refresh the local WorkItem cache of Jira stories
		/// BEFORE pulling the latest time entries)
		/// 
		/// Time Entries in the Mavenlink dataset that do not already exist in the local database are created.
		/// 
		/// Entries in the local database and tagged to the release, but not found in the Mavenlink dataset, are deleted locally.
		/// 
		/// Entries that already exist locally, and have a local modification timestamp that is OLDER than the Mavenlink modification
		/// timestamp are REPLACED with the Mavenlink version. (i.e. if the server record was updated more recently, it will overwrite
		/// any local modifications)
		/// 
		/// Entries that already exist locally, and have a local modification timestamp that is NEWER than the Mavenlink modification
		/// timestamp are NOT MODIFIED. (if the local copy is newest, its tweaks are preserved locally)
		/// 
		/// NOTE: if a time entry is removed from Release X, and added to Release Y (perhaps because an entry was originally mis-billed
		/// and later corrected), this system will correctly update only the release for which it is executed. This code would need run
		/// for BOTH Release X and Release Y in order to fully synchronize the local database.
		/// </summary>
		public async Task<List<TimeEntry>> RefreshTimeDataFromMavenlink(Release release) {
			// to ensure that all "stragglers" are accounted for, we run the time query for 2 weeks prior to the release start and
			// 2 weeks after the release end.
			var rangeStart = release.StartDate.AddDays(-14);
			var rangeEnd = release.EndDate.AddDays(14);
			var rawTimeData = await MavenlinkApi.GetAllTimeEntriesForInnovationsAsync(release.ReleaseNumber, rangeStart, rangeEnd);

			var returnData = new List<TimeEntry>();

			foreach (var mlEntry in rawTimeData) {
				var existingLocalRecord = Database.GetTimeEntry($"ML:{mlEntry.MavenlinkTimeId}");
				var newLocalRecord = this.SaveLocalCopy(release.ReleaseNumber, mlEntry, existingLocalRecord);

				returnData.Add(newLocalRecord);
			}

			// flush the adds/updates
			await Database.SaveChangesAsync();

			// delete local entries for the related innovation projects NOT in the server payload
			var projectIds = rawTimeData.Select(x => x.ProjectId).Distinct().ToList();

			// TODO: delete anything removed from Release X and added to Release Y?

			return returnData;
		}

		/// <summary>
		/// Re-examines the local Time Entry database and updates the Work Item Time Allocation data.
		/// 
		/// Call this after refreshing the local Jira cache to re-process the existing Time Entry data.
		/// </summary>
		public async Task<List<TimeEntry>> RefreshWorkItemTimeAllocationAsync(string releaseNum) {
			// delete all time allocations for the release
			await Task.Run(() => 
				Database.TimeEntryWorkItemAllocations
					.Where(x => x.TimeEntry.ReleaseNumber == releaseNum)
					.Delete()
			);

			await Task.Run(() => Database.SaveChanges());

			// reprocess
			var timeEntries = await Task.Run(() => Database.GetTimeEntriesForRelease(releaseNum));

			foreach (var timeEntry in timeEntries) {
				this.AllocateTimeToWorkItems(timeEntry);
			}

			await Task.Run(() => Database.SaveChanges());

			return timeEntries;
		}

		/// <summary>
		/// Refreshes the work item time allocation for a specific time entry.
		/// 
		/// Call this after modifying a local time entry's notes or duration to re-allocate the billed time against the
		/// referenced stories.
		/// </summary>
		public async Task<TimeEntry> RefreshTimeEntryWorkItemTimeAllocationAsync(string timeEntryId) {
			// delete all time allocations for the time entry
			await Task.Run(() =>
				Database.TimeEntryWorkItemAllocations
					.Where(x => x.TimeEntry.Id == timeEntryId)
					.Delete()
			);

			await Task.Run(() => Database.SaveChanges());

			// reprocess
			var timeEntry = await Task.Run(() => Database.GetTimeEntry(timeEntryId));
			this.AllocateTimeToWorkItems(timeEntry);

			await Task.Run(() => Database.SaveChanges());

			return timeEntry;
		}

		private TimeEntry SaveLocalCopy(string releaseNum, MavenlinkTimeEntry mlEntry, TimeEntry existingLocalRecord) {
			if (existingLocalRecord == null) {
				// new record in ML, no local copy
				var newLocalRecord = this.CreateTimeEntry(releaseNum, mlEntry);
				Database.TimeEntries.Add(newLocalRecord);

				return newLocalRecord;
			}
			else if (mlEntry.MavenlinkUpdatedAt > existingLocalRecord.LocallyUpdatedAt) {
				// exists locally but ML is newer; replace local copy.
				// HACK: EF doesn't give us control over the sequence in which operations are flushed, so a .Remove() followed by .Add() will throw
				// a PK exception b/c the insert gets executed first. So instead, we just do an update
				// It would be better to do a drop/create so that we don't have to worry about columns being added in
				// the future not being part of the update statement
				existingLocalRecord.ProjectIdOrig = mlEntry.ProjectId;
				existingLocalRecord.ProjectIdOverride = mlEntry.ProjectId;
				existingLocalRecord.ProjectTitleOrig = mlEntry.ProjectTitle;
				existingLocalRecord.ProjectTitleOverride = mlEntry.ProjectTitle;
				existingLocalRecord.TaskIdOrig = mlEntry.TaskId;
				existingLocalRecord.TaskIdOverride = mlEntry.TaskId;
				existingLocalRecord.DatePerformed = mlEntry.DatePerformed;
				existingLocalRecord.DurationMinutesOrig = mlEntry.DurationMinutes;
				existingLocalRecord.DurationMinutesOverride = mlEntry.DurationMinutes;
				existingLocalRecord.NotesOrig = mlEntry.Notes;
				existingLocalRecord.NotesOverride = mlEntry.Notes;
				existingLocalRecord.Billable = mlEntry.Billable;
				existingLocalRecord.SourceRecordCreatedAt = mlEntry.MavenlinkCreatedAt;
				existingLocalRecord.SourceRecordUpdatedAt = mlEntry.MavenlinkUpdatedAt;
				existingLocalRecord.LocallyUpdatedAt = DateTime.Now;

				return existingLocalRecord;
			}
			else if (existingLocalRecord.LocallyUpdatedAt >= mlEntry.MavenlinkUpdatedAt) {
				// exists locally and local copy was updated more recently; ignore the server record
				return existingLocalRecord;
			}
			else {
				throw new NotImplementedException("Should not get here: can't figure out how to merge server data w/ local data");
			}
		}

		private TimeEntry CreateTimeEntry(string releaseNum, MavenlinkTimeEntry mlEntry) {
			var timeEntry = new TimeEntry(releaseNum, mlEntry);

			this.AllocateTimeToWorkItems(timeEntry);

			return timeEntry;
		}

		private void AllocateTimeToWorkItems(TimeEntry timeEntry) {
			var referencedStoryIds = MavenlinkHelper.GetJiraStoryIds(timeEntry.NotesOverride);

			if (referencedStoryIds.Count == 0)
				return;

			decimal timePerStory = (referencedStoryIds.Count == 0)
				? (decimal)timeEntry.DurationMinutesOverride
				: (decimal)((decimal)timeEntry.DurationMinutesOverride / referencedStoryIds.Count);

			var workItemsInLocalCache = Database.WorkItems
				.Where(x => referencedStoryIds.Contains(x.StoryNumber))
				.OrderBy(x => x.StoryNumber)
				.ToList();

			foreach (var workItem in workItemsInLocalCache) {
				timeEntry.WorkItems.Add(
					new TimeEntryWorkItemAllocation {
						TimeEntry = timeEntry,
						WorkItem = workItem,
						DurationMinutes = timePerStory
					}
				);
			}

		}
	}
}
