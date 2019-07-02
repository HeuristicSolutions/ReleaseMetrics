using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using ReleaseMetrics.Core;
using ReleaseMetrics.Core.Configuration;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.Helpers;
using ReleaseMetrics.Core.WorkItems;
using ReleaseMetrics.Core.WorkItems.JiraApi;

namespace ReleaseMetrics.Api {

	/// <summary>
	/// Exposes endpoints for manually testing facets of the Jira API
	/// </summary>
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class JiraTestHarnessController : AppControllerBase {

		private JiraApiProxy ApiProxy => new JiraApiProxy(Config.JiraApiSettings.Username, Config.JiraApiSettings.AuthToken);
		private JiraLoader Loader => new JiraLoader(Database, ApiProxy);

		public JiraTestHarnessController(AppSettings config, MetricsDbContext db) : base(config, db) {
		}

		[HttpGet]
		[Route("GetStory")]
		public async Task<ActionResult<JiraStory>> GetStoryAsync(string storyId) {
			return await ApiProxy.GetStoryAsync(storyId);
		}

		[HttpGet]
		[Route("GetStory_Raw")]
		public async Task<ActionResult<string>> GetStory_RawAsync(string storyId) {
			var rawApiResponse = await ApiProxy.GetJiraSearchResultJsonAsync(HttpUtility.UrlEncode("id=" + storyId));

			return rawApiResponse.ToJsonIndentedFormat();
		}

		[HttpGet]
		[Route("GetStoriesInRelease")]
		public async Task<ActionResult<List<JiraStory>>> GetStoriesInReleaseAsync(string releaseNum) {
			return await ApiProxy.GetStoriesInReleaseAsync(releaseNum);
		}

		[HttpGet]
		[Route("GetStoriesInRelease_Raw")]
		public async Task<ActionResult<string>> GetStoriesInRelease_RawAsync(string releaseNum) {
			var rawApiResponse = await ApiProxy.GetJiraSearchResultJsonAsync(HttpUtility.UrlEncode("fixversion=" + HttpUtility.UrlEncode(releaseNum)));

			return rawApiResponse.ToJsonIndentedFormat();
		}

		[HttpPost]
		[Route("RefreshLocalJiraStoryCacheForRelease")]
		public async Task<object> RefreshLocalJiraStoryCacheForReleaseAsync(string releaseNum) {
			var (workItems, messages) = await Loader.RefreshLocalJiraStoryCacheForReleaseAsync(releaseNum);
			return new { workItems = workItems, messages = messages };
		}
	}
}