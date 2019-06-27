using Heuristics.Library.Extensions;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.WorkItems.JiraApi;
using ReleaseMetrics.Core.TimeEntries.MavenlinkApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseMetrics.Core.WorkItems;
using ReleaseMetrics.Core.Helpers;

namespace ReleaseMetrics.Core.TimeEntries {
	
	/// <summary>
	/// Service class for consuming and processing locally-stored time entries.
	/// </summary>
	public class MavenlinkTimeEntryProcessor {

		private MetricsDbContext Database { get; set; }

		public MavenlinkTimeEntryProcessor(MetricsDbContext db) {
			Database = db;
		}
	}
}
