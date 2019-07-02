using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heuristics.Library.Extensions;
using Microsoft.AspNetCore.Mvc;
using ReleaseMetrics.Core.Configuration;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.WorkItems.JiraApi;
using ReleaseMetrics.Core.WorkItems;
using ReleaseMetrics.Core;

namespace ReleaseMetrics.Api {

	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class WorkItemsController : AppControllerBase {

		private JiraApiProxy JiraApi => new JiraApiProxy(Config.JiraApiSettings.Username, Config.JiraApiSettings.AuthToken);
		private JiraLoader Loader => new JiraLoader(Database, JiraApi);

		public WorkItemsController(AppSettings config, MetricsDbContext db) : base(config, db) {
		}

		[HttpGet]
		[Route("Get")]
		public async Task<WorkItem> GetAsync(string id) {
			var workItem = await Task.Run(() => Database.GetWorkItem(id));

			return workItem;
		}

		[HttpGet]
		[Route("GetForRelease")]
		public async Task<List<WorkItem>> GetForReleaseAsync(string releaseNum) {
			var workItems = await Task.Run(() =>
				Database.WorkItems
					.Where(x => x.ReleaseNumber == releaseNum)
					.OrderBy(x => x.StoryNumber)
					.ToList()
			);

			return workItems;
		}

		[HttpPost]
		[HttpGet] // TODO remove
		[Route("RefreshJiraCacheForRelease")]
		public async Task<ApiResult<List<WorkItem>>> RefreshJiraCacheForRelease(string releaseNum) {
			var (workItems, messages) = await Loader.RefreshLocalJiraStoryCacheForReleaseAsync(releaseNum);

			return new ApiResult<List<WorkItem>>(workItems, messages);
		}
	}
}