using Heuristics.Library.Extensions;
using Newtonsoft.Json;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ReleaseMetrics.Core.TimeEntries.MavenlinkApi {

	public class MavenlinkApiProxy {

		public static string ML_API_HOST = "https://api.mavenlink.com";
		public static int ML_PAGESIZE = 200;
		public static Dictionary<string, MavenlinkUserApiResponse.UserData> USER_CACHE = new Dictionary<string, MavenlinkUserApiResponse.UserData>();
		public static Dictionary<string, MavenlinkWorkspaceApiResponse.Project> PROJECT_CACHE = new Dictionary<string, MavenlinkWorkspaceApiResponse.Project>();
		public static Dictionary<string, MavenlinkTaskSearchApiResponse.StoryData> TASK_CACHE = new Dictionary<string, MavenlinkTaskSearchApiResponse.StoryData>();

		public string ApiAuthToken { get; set; }
		public string ApiAuthHeader => $"Bearer {this.ApiAuthToken}";

		public MavenlinkApiProxy(string oauthAccessToken) {
			this.ApiAuthToken = oauthAccessToken;
		}

		/// <summary>
		/// Returns the standard tasks used for billing fixed-cost innovation work, in any project. (i.e. returns the full
		/// set of tasks for which all innovation work in a release should have been billed to)
		/// 
		/// This also includes the "Defect Resolution" task so that we can count time billed to defects, and also because
		/// feature stories are sometimes mis-billed to defect res and we want the ability to fix that locally.
		/// </summary>
		public async Task<List<MavenlinkTaskSearchApiResponse.StoryData>> GetAllTasksForInnovationTimeTrackingAsync(string releaseNum) {

			releaseNum = releaseNum.Trim();

			// the billing tasks for major releases omit the trailing ".0"
			if (releaseNum.MatchesRegex("[0-9]+\\.[0-9]+\\.[0]$")) {
				releaseNum = releaseNum.RemoveTrailing(".0");
			}

			var searchStrings = new List<string> {
				$"{releaseNum}: Planned",
				$"{releaseNum} Planned",
				$"{releaseNum}: Unplanned",
				$"{releaseNum} Unplanned",
				$"{releaseNum}: Overhead",
				$"{releaseNum} Overhead",
				"Defect Resolution",			// need to count time billed to defects as well as stories
				"Unplanned Defect Resolution"
			};

			var allTasks = new List<MavenlinkTaskSearchApiResponse.StoryData>();
			foreach (var searchString in searchStrings) {
				// the search method targets fields other than title, but we only want things where the *title* matches the pattern,
				// so we first 
				var tasks = (await GetMavenlinkTasksAsync(searchString))
					.Where(x => x.Title.StartsWithIgnoringCase(searchString))
					.ToList();

				allTasks.AddRange(tasks);
			}

			return allTasks
				.OrderBy(x => x.ProjectId)
				.ThenBy(x => x.Title)
				.ToList();
		}

		/// <summary>
		/// Given a milestone and a date range, returns a list of all time entries for all of the standard
		/// time tracking projects (planned, unplanned, etc) w/ entries for the specified release.
		/// </summary>
		public async Task<List<MavenlinkTimeEntry>> GetAllTimeEntriesForInnovationsAsync(string releaseNum, DateTime startDate, DateTime endDate) {
			// we can't seem to easily filter the time entries by project, only by date range, so we have to manually build a list
			// of projects we care about, run the query for all time entries in the period, and then filter out just the projects
			// we care about
			var tasksToTrack = await this.GetAllTasksForInnovationTimeTrackingAsync(releaseNum);
			var taskIdsToTrack = tasksToTrack.Select(x => x.Id).ToList();
			var projectIdsToTrack = tasksToTrack.Select(x => x.ProjectId).Distinct().ToList();

			var allTimeEntriesInPeriod = new List<MavenlinkTimeEntry>();

			foreach (var projectId in projectIdsToTrack) {
				var timeForProject = await this.GetMavenlinkTimeSummariesAsync(projectId, startDate, endDate, taskIdsToTrack);

				allTimeEntriesInPeriod.AddRange(
					timeForProject
				);
			}

			return allTimeEntriesInPeriod;

		}

		/// <summary>
		/// Performs a text-based search of Mavenlink tasks. The search targets the task title and a few other fields.
		/// 
		/// Updates the task cache, so that any future ID lookups can avoid hitting the API again.
		/// </summary>
		public async Task<List<MavenlinkTaskSearchApiResponse.StoryData>> GetMavenlinkTasksAsync(string searchString) {
			var urlEncodedQuery = HttpUtility.UrlEncode(searchString);
			var client = new RestClient(ML_API_HOST);
			var request = new RestRequest($"api/v1/stories.json?search={urlEncodedQuery}&page_size=200", Method.GET);
			request.AddHeader("Authorization", this.ApiAuthHeader);
			request.RequestFormat = DataFormat.Json;

			var json = await Task.Run(() => client.Execute(request).Content);
			var payload = JsonConvert.DeserializeObject<MavenlinkTaskSearchApiResponse>(json);

			if (payload.Meta.PageCount > 1) {
				throw new Exception("Found multiple pages of task search results - this tool doesn't support multiple result sets for task search data");
			}

			foreach (var taskId in payload.Stories.Keys) {
				if (!TASK_CACHE.ContainsKey(taskId)) {
					TASK_CACHE.Add(taskId, payload.Stories[taskId]);
				}
			}

			return payload.Stories.Values.ToList();
		}

		/// <summary>
		/// Calls the ML time API and filters the results by the specified task IDs. Converts the results into the denormalized
		/// DTO that can be stored locally as a historical record.
		/// </summary>
		public async Task<List<MavenlinkTimeEntry>> GetMavenlinkTimeSummariesAsync(string projectId, DateTime start, DateTime end, List<string> taskIds) {
			var allEntriesInDateRange = (await GetMavenlinkTimeEntriesAsync(start, end, projectId)).ToList();

			var filteredEntries = allEntriesInDateRange
				.SelectMany(x => x.TimeEntries.Values)
				.Where(x => x.TaskId.IsIn(taskIds))
				.ToList();

			var timeData = new List<MavenlinkTimeEntry>();

			foreach (var entry in filteredEntries) {
				var project = await GetMavenlinkProjectAsync(entry.ProjectId);
				var user = await GetMavenlinkUserAsync(entry.UserId);

				// the task data should already be in the cache, since we had to hit the API to get the task list in the first place
				var task = TASK_CACHE[entry.TaskId].ThrowIfNull($"Task ${entry.TaskId} was not found in the task cache");

				timeData.Add(
					new MavenlinkTimeEntry(entry, user, project, task)
				);
			}

			return timeData;
		}

		/// <summary>
		/// Calls the ML time API and returns all pages of data. (ML doesn't let us request time entries for a specific task,
		/// so in order to get all entries for an entire release we have to pull a lot of data. This handles the pagination
		/// and streams the results to the caller.
		/// </summary>
		public async Task<List<MavenlinkTimesheetApiResponse>> GetMavenlinkTimeEntriesAsync(DateTime start, DateTime end, string projectId) {
			var client = new RestClient(ML_API_HOST);
			var hasMorePages = true;
			var pageNum = 1;
			var perPage = 200;

			var timeEntryList = new List<MavenlinkTimesheetApiResponse>();

			while (hasMorePages) {
				var formattedStartDate = start.ToString("yyyy-MM-dd") + "T000000";
				var formattedEndDate = end.ToString("yyyy-MM-dd") + "T115959";
				var datesBetween = $"{formattedStartDate}:{formattedEndDate}";

				var request = new RestRequest($"api/v1/time_entries.json?workspace_id={projectId}&date_performed_between={datesBetween}&per_page={perPage}&page={pageNum}", Method.GET);
				request.AddHeader("Authorization", this.ApiAuthHeader);
				request.RequestFormat = DataFormat.Json;
				var json = await Task.Run(() => client.Execute(request).Content);

				var payload = JsonConvert.DeserializeObject<MavenlinkTimesheetApiResponse>(json);
				timeEntryList.Add(payload);

				hasMorePages = (payload.Meta.PageNumber < payload.Meta.PageCount);
				pageNum++;
			}

			return timeEntryList;
		}

		/// <summary>
		/// Retrieves the user data from the local cache, if found, or else obtains it from the ML user API (and then adds to cache)
		/// </summary>
		public async Task<MavenlinkUserApiResponse.UserData> GetMavenlinkUserAsync(string userId) {
			if (!USER_CACHE.ContainsKey(userId)) {
				var client = new RestClient(ML_API_HOST);
				var request = new RestRequest($"api/v1/users/{userId}.json", Method.GET);
				request.AddHeader("Authorization", this.ApiAuthHeader);
				request.RequestFormat = DataFormat.Json;

				var json = await Task.Run(() => client.Execute(request).Content);
				var payload = JsonConvert.DeserializeObject<MavenlinkUserApiResponse>(json);

				if (payload.Count != 1) {
					throw new Exception($"Found {payload.Count} records for user ID {userId}");
				}

				var user = payload.Users[userId];

				if (user.Id != userId) {
					throw new Exception($"Expected user with ID {userId}, actually is {user.Id}");
				}

				USER_CACHE.Add(userId, user);
			}

			return USER_CACHE[userId];
		}

		/// <summary>
		/// Retrieves the project data from the local cache, if found, or else obtains it from the ML workspace API (and then adds to cache)
		/// </summary>
		public async Task<MavenlinkWorkspaceApiResponse.Project> GetMavenlinkProjectAsync(string workspaceId) {
			if (!PROJECT_CACHE.ContainsKey(workspaceId)) {
				var client = new RestClient(ML_API_HOST);
				var request = new RestRequest($"api/v1/workspaces/{workspaceId}.json", Method.GET);
				request.AddHeader("Authorization", this.ApiAuthHeader);
				request.RequestFormat = DataFormat.Json;

				var json = await Task.Run(() => client.Execute(request).Content);
				var payload = JsonConvert.DeserializeObject<MavenlinkWorkspaceApiResponse>(json);

				if (payload.Count != 1) {
					throw new Exception($"Found {payload.Count} records for workspace ID {workspaceId}");
				}

				var project = payload.Projects[workspaceId];

				if (project.Id != workspaceId) {
					throw new Exception($"Expected project with ID {workspaceId}, actually is {project.Id}");
				}

				PROJECT_CACHE.Add(workspaceId, project);
			}

			return PROJECT_CACHE[workspaceId];
		}
	}
}
