using System;
using System.Collections.Generic;
using System.Linq;
using Heuristics.Library.Extensions;
using Microsoft.AspNetCore.Mvc;
using ReleaseMetrics.Core.Configuration;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.Releases;

namespace ReleaseMetrics.Api {

	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class ReleasesController : AppControllerBase {

		public ReleasesController(AppSettings config, MetricsDbContext db) : base(config, db) {
		}

		// TODO: find a way to encapsulate ReleaseSummary better. Map a view?
		[HttpGet]
		[Route("List")]
		public List<ReleaseSummary> List() {
			var list = Database.ReleaseSummaries.ToList();

			return list;
		}

		[HttpGet]
		[Route("Get")]
		public ReleaseSummary Get(string releaseNum) {
			var release = Database.GetReleaseSummaryByNumber(releaseNum)
				.ThrowIfNull($"Release '{releaseNum}' not found");

			return release;
		}
	}
}