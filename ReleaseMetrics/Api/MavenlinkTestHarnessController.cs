using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReleaseMetrics.Core.Configuration;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.TimeEntries.MavenlinkApi;
using ReleaseMetrics.Core.TimeEntries;

namespace ReleaseMetrics.Api {

	/// <summary>
	/// Exposes endpoints for manually testing facets of the Jira API
	/// </summary>
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class MavenlinkTestHarnessController : AppControllerBase {

		private MavenlinkApiProxy ApiProxy => new MavenlinkApiProxy(Config.MavenlinkApiSettings.OauthToken);

		public MavenlinkTestHarnessController(AppSettings config, MetricsDbContext db) : base(config, db) {
		}

		[HttpGet]
		[Route("GetAllTasksForInnovationTimeTracking")]
		public async Task<ActionResult<List<MavenlinkTaskSearchApiResponse.StoryData>>> GetAllTasksForInnovationTimeTrackingAsync(string releaseNum) {
			var rawApiResponse = await ApiProxy.GetAllTasksForInnovationTimeTrackingAsync(releaseNum);

			return rawApiResponse;
		}

		[HttpGet]
		[Route("GetAllTimeEntriesForInnovations")]
		public async Task<ActionResult<List<MavenlinkTimeEntry>>> GetAllTimeEntriesForInnovationsAsync(string releaseNum) {
			var release = Database.GetReleaseByNumber(releaseNum);

			var rawApiResponse = await ApiProxy.GetAllTimeEntriesForInnovationsAsync(releaseNum, release.StartDate, release.EndDate);

			return rawApiResponse;
		}

		//[HttpGet]
		//[Route("GetAllTimeEntriesInRange")]
		//public async Task<ActionResult<List<MavenlinkTimeEntry>>> GetAllTimeEntriesInRange(DateTime startDate, DateTime endDate, string user = null) {

		//}
	}
}