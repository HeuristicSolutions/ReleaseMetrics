using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heuristics.Library.Extensions;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.Helpers;
using ReleaseMetrics.Core.WorkItems.JiraApi;
using ReleaseMetrics.Core.TimeEntries.MavenlinkApi;
using ReleaseMetrics.Core.WorkItems;

namespace ReleaseMetrics.Core.TimeEntries {

	/// <summary>
	/// Responsible for performing validation and consistency checks on the Mavenlink and Jira data that have been stored locally
	/// in the TimeEntry and WorkItem tables, respectively.
	/// 
	/// All of these rules must be satisifed before the metrics can be run.
	/// </summary>
	public class TimeEntryValidator {

		protected MetricsDbContext Database { get; set; }
		protected JiraApiProxy JiraApi { get; set; }
		protected MavenlinkApiProxy MavenlinkApi { get; set; }

		public TimeEntryValidator(MetricsDbContext database, JiraApiProxy jiraApi, MavenlinkApiProxy mavenlinkApi) {
			Database = database;
			JiraApi = jiraApi;
			MavenlinkApi = mavenlinkApi;
		}

		public async Task<List<TimeEntryValidationResult>> Validate(Release release, List<TimeEntry> timeEntries) {
			var jiraStoriesInRelease = (await JiraApi.GetStoriesInReleaseAsync(release.ReleaseNumber));

			var results = new List<TimeEntryValidationResult>();

			foreach (var timeEntry in timeEntries) {
				var errors = new List<string>();
				var warnings = new List<string>();

				var isPlanned = (MavenlinkHelper.GetBillingClassification(timeEntry.TaskTitleOverride) == BillingClassificationEnum.Planned);
				var referencedStoryIds = MavenlinkHelper.GetJiraStoryIds(timeEntry.NotesOverride);

				if (referencedStoryIds.Count <= 1 && timeEntry.DurationMinutesOverride < 15) {
					warnings.Add($"Minimum billing increment is 15 minutes");
				}
				else if (referencedStoryIds.Count > 1) {
					var timePerStory = (timeEntry.DurationMinutesOverride / referencedStoryIds.Count);

					if (timePerStory < 15m) {
						warnings.Add($"Each Jira story would receive {timePerStory} minutes of elapsed time, less than the 15-min minimum billing increment.");
					}
				}

				foreach (var jiraStoryId in referencedStoryIds) {
					var jiraStory = jiraStoriesInRelease
						.Where(x => x.Id == jiraStoryId)
						.FirstOrDefault();

					// we're loading all of the stories in the release in a single load for performance, but if time is
					// billed to a story NOT in the release we still need to load it as a 1-off. 
					if (jiraStory == null) {
						try {
							jiraStory = await JiraApi.GetStoryAsync(jiraStoryId);
						}
						catch (NotFoundException) {
							errors.Add($"Jira ID '{jiraStoryId}' was not found in Jira");
							continue;
						}
					}

					// Billing time to a story not associated with the release is a warning; the time will be counted towards
					// the "undelivered" category and will not affect the metrics
					if (!jiraStory.FixVersions.Contains(release.ReleaseNumber)) { 
						warnings.Add($"Jira {jiraStory.IssueType} '{jiraStoryId}' is not tagged to release {release.ReleaseNumber}. Time will be counted towards 'undelivered'.");
					}

					var isEpic = (JiraHelper.GetWorkItemType(jiraStory) == WorkItemTypeEnum.Epic);
					var isDeclined = jiraStory.Status.EqualsIgnoringCase("Declined");

					if (isDeclined) {
						warnings.Add($"Jira {jiraStory.IssueType} '{jiraStoryId}' is marked as 'DECLINED'. Time will be tracked towards 'undelivered'.");
					}

					if (isEpic && isPlanned) {
						warnings.Add($"'{jiraStoryId}' is an Epic; epics should generally have Overhead or Unplanned time instead of Planned.");
					}
				}

				if (errors.Any() || warnings.Any()) {
					results.Add(
						new TimeEntryValidationResult(timeEntry.Id, errors, warnings)
					);
				}
			}

			return results;
		}

		private static string GetEpicName(JiraSearchApiResponse.JiraIssue storyData) {
			return "TODO";
		}

		private static string GetBillToClient(JiraSearchApiResponse.JiraIssue storyData) {
			return "TODO";
		}
	}
}
