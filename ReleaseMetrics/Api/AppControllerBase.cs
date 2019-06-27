using Microsoft.AspNetCore.Mvc;
using ReleaseMetrics.Core.Configuration;
using ReleaseMetrics.Core.DataModel;

namespace ReleaseMetrics.Api {

	public class AppControllerBase : ControllerBase {

		protected AppSettings Config { get; set; }
		protected MetricsDbContext Database { get; set; }

		public AppControllerBase(AppSettings config, MetricsDbContext db) {
			Config = config;
			Database = db;
		}
	}
}