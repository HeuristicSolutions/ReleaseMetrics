using Heuristics.Library.Extensions;
using Newtonsoft.Json;
using ReleaseMetrics.Core.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ReleaseMetrics.Core.WorkItems.JiraApi {

	/// <summary>
	/// Convenience wrapper around the Jira API
	/// </summary>
	public class JiraApiProxy {

		public static string JIRA_API_HOST = "https://heuristicsolutions.atlassian.net";
		public static int JIRA_MAX_RESULTS = 100;	// DO NOT INCREASE; Jira only accepts a max of 100

		public string ApiUsername { get; set; }
		public string ApiToken { get; set; }

		public string ApiAuthHeader {
			get {
				var base64EncodedCreds = GenericUtils.Base64Encode($"{this.ApiUsername}:{this.ApiToken}");
				return $"Basic {base64EncodedCreds}";
			}
		}

		private Dictionary<string, JiraStory> StoryCache { get; set; }

		public JiraApiProxy(string apiUsername, string apiToken) {
			this.ApiUsername = apiUsername;
			this.ApiToken = apiToken;
			this.StoryCache = new Dictionary<string, JiraStory>();
		}

		/// <summary>
		/// Returns a list of all stories flagged with the specified "fix version" in Jira.
		/// </summary>
		public async Task<List<JiraStory>> GetStoriesInReleaseAsync(string releaseNum) {
			var hasMorePages = true;
			var startAt = 0;
			var perPage = JIRA_MAX_RESULTS;

			var allStories = new List<JiraStory>();

			while (hasMorePages) {
				string json = null;
				try {
					json = await GetJiraSearchResultJsonAsync("fixversion=" + HttpUtility.UrlEncode(releaseNum), startAt: startAt);
					var payload = JsonConvert.DeserializeObject<JiraSearchApiResponse>(json);
					var storiesInPayload = payload.Issues;

					// make sure we're counting everything
					if (payload.MaxResults > perPage) {
						throw new Exception($"Requested page size of {perPage}, but a limit of {payload.MaxResults} was applied instead.");
					}

					allStories.AddRange(
						storiesInPayload
							.Select(x => new JiraStory(x))
							.ToList()
					);

					hasMorePages = payload.TotalResults > (startAt + perPage);
					startAt += perPage;
				}
				catch (JsonSerializationException ex) {
					throw new ApplicationException("Error processing this response: " + json, ex);
				}
			}

			return allStories;
		}

		/// <summary>
		/// Hits the Jira API to retrieve a single story. Utilizes a per-class-instance in-memory cache so 
		/// that subsequent calls for the same ID  do not result in additional network traffic.
		/// </summary>
		public async Task<JiraStory> GetStoryAsync(string storyId, bool refreshCache = false) {

			if (!refreshCache && StoryCache.ContainsKey(storyId)) {
				return StoryCache[storyId];
			}

			var json = await GetJiraSearchResultJsonAsync(HttpUtility.UrlEncode("id=" + storyId));

			try {
				var payload = JsonConvert.DeserializeObject<JiraSearchApiResponse>(json);
				var allStories = payload.Issues;

				// reality check
				if (payload.TotalResults == 0) {
					throw new NotFoundException($"Jira story '{storyId}' was not found.");
				}
				if (payload.TotalResults > 1) {
					throw new Exception($"Expected to find a single result for {storyId}. Found {payload.TotalResults} results instead.");
				}

				var story = new JiraStory(
					allStories.Single()
				);

				StoryCache[storyId] = story;

				return story;
			}
			catch (JsonSerializationException ex) {
				throw new ApplicationException("Error processing this response: " + json, ex);
			}
		}

		/// <summary>
		/// Performs a Jira API story search and returns the raw JSON returned by the endpoint.
		/// </summary>
		public async Task<string> GetJiraSearchResultJsonAsync(string urlEncodedQuery, int startAt = 0) {
			var client = new RestClient(JIRA_API_HOST);

			// To find ID of custom fields: https://confluence.atlassian.com/jirakb/how-to-find-id-for-custom-field-s-744522503.html
			// customfield_10018 = Epic Link
			// customfield_10022 = Story Points
			// customfield_10843 = Defect Type
			var request = new RestRequest($"rest/api/2/search?jql={urlEncodedQuery}&maxResults={JIRA_MAX_RESULTS}&startAt={startAt}&fields=id,key,status,epic,fixVersions,issuetype,summary,labels,customfield_10022,customfield_10018,customfield_10843", Method.GET);
			request.AddHeader("Authorization", this.ApiAuthHeader);
			request.RequestFormat = DataFormat.Json;

			var response = await Task.Run(() => client.Execute(request));

			switch (response.StatusCode) {
				case System.Net.HttpStatusCode.OK:
					return response.Content;

				case System.Net.HttpStatusCode.BadRequest:
					if (response.Content.ContainsIgnoringCase("does not exist")) {
						throw new NotFoundException($"No Jira stories were found matching the query '{urlEncodedQuery}'");
					}
					else {
						throw new ApplicationException($"Received response code {response.StatusCode} from \"{client.BuildUri(request)}\", Authorization: {this.ApiAuthHeader}");
					}

				default:
					throw new ApplicationException($"Received response code {response.StatusCode} from \"{client.BuildUri(request)}\", Authorization: {this.ApiAuthHeader}");
			}
		}
	}
}
