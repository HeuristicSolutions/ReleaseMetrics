﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseMetrics.Core.DataModel;

namespace ReleaseMetrics.Core.Releases {

	/// <summary>
	/// JSON-serialization-safe DTO representing the release metrics. (These are calculated by a DB view and mapped into EF)
	/// </summary>
	public class ReleaseSummary {

		public string ReleaseNumber { get; set; }
		public string Notes { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int WeeksInRelease { get; set; }

		public decimal ChoreHours { get; set; }
		public decimal FeatureHours { get; set; }
		public decimal NewDefectHours { get; set; }
		public decimal LegacyDefectHours { get; set; }
		public decimal TotalBilledHours { get; set; }


		public int ShippedFeatureAndChoreCount { get; set; }
		public int ShippedFeatureAndChorePoints { get; set; }
		public decimal ShippedFeatureAndChoreHours { get; set; }
		public decimal AvgHoursPerFeatureAndChorePoint { get; set; }
		
		public int LegacyDefectCount { get; set; }
		public decimal AvgHoursPerLegacyDefect { get; set; }

		public decimal UnshippedHours { get; set; }

		public int UnusedContingencyPoints { get; set; }

		public decimal FullyLoadedAvgHoursPerPoint { get; set; }

		public ReleaseSummary() {
		}
	}
}
