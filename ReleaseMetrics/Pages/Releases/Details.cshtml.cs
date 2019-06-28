using System;
using Heuristics.Library.Extensions;
using ReleaseMetrics.Core.Configuration;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.Releases;
using ReleaseMetrics.Pages.Shared;

namespace ReleaseMetrics.Pages.Releases {

	public class DetailsModel : BasePageModel {

		public string ReleaseNumber { get; set; }

		public DetailsModel(AppSettings config, MetricsDbContext db) : base(config, db) {
		}

		public void OnGet(string releaseNum) {
			if (releaseNum.IsNullOrEmpty()) {
				throw new ArgumentException("The 'releaseNum' parameter is required");
			}

			ReleaseNumber = releaseNum.Trim();
		}
	}
}
