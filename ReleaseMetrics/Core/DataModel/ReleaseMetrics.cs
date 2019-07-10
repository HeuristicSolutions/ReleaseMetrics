using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Heuristics.Library.Extensions;
using Newtonsoft.Json;
using ReleaseMetrics.Core.DataModel.EfCoreExtensions;
using ReleaseMetrics.Core.TimeEntries;

namespace ReleaseMetrics.Core.DataModel {

	public class ReleaseMetrics {

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[MaxLength(25)]
		public string ReleaseNumber { get; set; }

		[SqlDefaultValue("getutcdate()")]
		public DateTime GeneratedAt { get; set; }

		public virtual Release Release { get; set; }

		// Stories and chores combined
		public int TotalStoryCount { get; set; }
		public int TotalStoryPoints { get; set; }

		// All Features, planned and unplanned combined
		public int TotalFeatureCount { get; set; }
		public int TotalFeaturePoints { get; set; }

		// All Chores, planned and unplanned combined
		public int TotalChoreCount { get; set; }
		public int TotalChorePoints { get; set; }

		// Planned work, client-funded, includes Features and Chores combined
		public int PlannedClientStoryCount { get; set; }
		public int PlannedClientStoryPoints { get; set; }

		// Planned work, R&D-funded, includes Features and Chores combined
		public int PlannedRDStoryCount { get; set; }
		public int PlannedRDStoryPoints { get; set; }

		// Unplanned work, client-funded, includes Features and Chores combined
		public int UnplannedClientStoryCount { get; set; }
		public int UnplannedClientStoryPoints { get; set; }

		// Unplanned work, R&D-funded, includes Features and Chores combined
		public int UnplannedRDStoryCount { get; set; }
		public int UnplannedRDStoryPoints { get; set; }

		// Points assigned to Contingency cases that have not been re-assigned elsewhere. (Represents unused buffer time from the planned bucket)
		public int UnusedContingencyPoints { get; set; }

		/// <summary>
		/// Total time billed to shipped work / total shipped points
		/// </summary>
		public decimal FullyLoadedHoursPerPoint { get; set; }

		/// <summary>
		/// The number of defects that were fixed, that were not introduced or part of the planned development in the release
		/// </summary>
		public int LegacyDefectCount { get; set; }

		/// <summary>
		/// The total number of hours tracked against legacy defects / total count of legacy defects. Since we don't count "points" for defects,
		/// this gives us a way of quantifying the time we spend maintaining what we've already built.
		/// </summary>
		[Column(TypeName = "decimal(6,3)")]
		public decimal HoursPerLegacyDefect { get; set; }

		/// <summary>
		/// The number of defects that were fixed that were introduced by, or part of, the development for the release
		/// </summary>
		public int NewDefectCount { get; set; }

		/// <summary>
		/// # of NEW defects / # of stories. Helps indicate the "churn" encountered when stories did not pass QA on the first pass.
		/// </summary>
		[Column(TypeName = "decimal(6,3)")]
		public decimal ReworkRatio { get; set; }

		public ReleaseMetrics() {
		}

	}
}
