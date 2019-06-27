using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReleaseMetrics.Core.Configuration;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.WorkItems.JiraApi;
using ReleaseMetrics.Core.TimeEntries.MavenlinkApi;
using ReleaseMetrics.Core.TimeEntries;

namespace ReleaseMetrics.Api {

	/// <summary>
	/// Exposes endpoints used by the local application to get and modify the locally cached TimeEntry data
	/// </summary>
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class TimeEntriesController : AppControllerBase {

		private JiraApiProxy JiraApiProxy => new JiraApiProxy(Config.JiraApiSettings.Username, Config.JiraApiSettings.AuthToken);
		private MavenlinkApiProxy MavenlinkApiProxy => new MavenlinkApiProxy(Config.MavenlinkApiSettings.OauthToken);
		private MavenlinkLoader Loader => new MavenlinkLoader(Database, MavenlinkApiProxy);
		private TimeEntryValidator Validator => new TimeEntryValidator(Database, JiraApiProxy, MavenlinkApiProxy);
		private MavenlinkTimeEntryProcessor Processor => new MavenlinkTimeEntryProcessor(Database);

		public TimeEntriesController(AppSettings config, MetricsDbContext db) : base(config, db) {
		}

		[HttpGet]
		[Route("Get")]
		public async Task<TimeEntry> GetAsync(string id) {
			var entry = await Task.Run(() => Database.GetTimeEntry(id));

			return entry;
		}

		/// <summary>
		/// Returns all of the locally-cached time entries for the specified release
		/// </summary>
		[HttpGet]
		[Route("GetForRelease")]
		public async Task<List<TimeEntry>> GetForReleaseAsync(string releaseNum) {
			return await Task.Run(() => Database.GetTimeEntriesForRelease(releaseNum));
		}

		/// <summary>
		/// Loads new time data from Mavenlink and merges it into the local data store, updating/adding/removing entries
		/// as necessary.
		/// 
		/// WARNING: Time entries that mention Jira stories in the notes will only be allocated to those stories that
		/// already exist in the local Work Item cache. For best results, refresh the work item cache first.
		/// </summary>
		[HttpPost]
		[Route("LoadTimeFromMavenlink")]
		public async Task<List<TimeEntry>> LoadTimeFromMavenlink(string releaseNum) {
			var release = Database.GetReleaseByNumber(releaseNum);
			var timeEntries = await Loader.RefreshTimeDataFromMavenlink(release);

			return timeEntries;
		}

		/// <summary>
		/// Re-examines the local Time Entry database and updates the Work Item Time Allocation data.
		/// 
		/// Call this after refreshing the local Jira cache to re-process the existing Time Entry data.
		/// </summary>
		[HttpPost]
		[Route("RefreshWorkItemTimeAllocation")]
		public async Task<List<TimeEntry>> RefreshWorkItemTimeAllocationAsync(string releaseNum) {
			return await Loader.RefreshWorkItemTimeAllocationAsync(releaseNum);
		}

		/// <summary>
		/// Erases all local overrides and restores the time entry to the data in the original mavenlink entry
		/// </summary>
		[HttpPost]
		[Route("RevertToOriginal")]
		public async Task<TimeEntry> RevertToOriginalAsync(string timeEntryId) {
			Database.RevertToOriginal(timeEntryId);
			return await Loader.RefreshTimeEntryWorkItemTimeAllocationAsync(timeEntryId);
		}

		/// <summary>
		/// Sets or clears the "ignored" flag (without updating the "locally updated" flag)
		/// </summary>
		[HttpPost]
		[Route("SetIgnoredStatus")]
		public async Task<TimeEntry> SetIgnoredStatusAsync(string timeEntryId, bool ignored) {
			return await Task.Run(() => Database.SetTimeEntryIgnoredFlag(timeEntryId, ignored));
		}

		/// <summary>
		/// Saves the changes from the Edit Time Entry modal
		/// </summary>
		[HttpPost]
		[Route("Update")]
		public async Task<TimeEntry> UpdateAsync([FromBody]EditTimeEntryModel data) {
			Database.UpdateTimeEntry(data);
			return await Loader.RefreshTimeEntryWorkItemTimeAllocationAsync(data.TimeEntryId);
		}

		/// <summary>
		/// Loads the time entries for the specified release and runs them through the validation and consistency checks,
		/// returning a list of warnings and errors that were found.
		/// </summary>
		[HttpGet]
		[Route("ValidateEntriesForRelease")]
		public async Task<List<TimeEntryValidationResult>> ValidateEntriesForReleaseAsync(string releaseNum) {
			var release = Database.GetReleaseByNumber(releaseNum);
			
			var timeEntries = Database.GetTimeEntriesForRelease(releaseNum)
				.Where(x => x.Ignore == false)
				.ToList();

			return await Validator.Validate(release, timeEntries);
		}
	}
}