using Microsoft.AspNetCore.Mvc.RazorPages;
using ReleaseMetrics.Core.Configuration;
using ReleaseMetrics.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseMetrics.Pages.Shared {

	public class BasePageModel : PageModel {

		protected AppSettings AppSettings { get; set; }
		protected MetricsDbContext Database { get; set; }

		public BasePageModel(AppSettings config, MetricsDbContext db) {
			this.AppSettings = config;
			this.Database = db;
		}
	}
}
