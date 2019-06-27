using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseMetrics.Core.DataModel;

namespace ReleaseMetrics.Core.Releases {

	/// <summary>
	/// JSON-serialization-safe DTO representing a Release. 
	/// </summary>
	public class ReleaseSummary {

		public string ReleaseNumber { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Notes { get; set; }
		public int TotalPoints { get; set; }
		public int TimeEntryCount { get; set; }
		public int WorkItemCount { get; set; }

		public ReleaseSummary() { }

		public ReleaseSummary(Release release) {
			this.ReleaseNumber = release.ReleaseNumber;
			this.StartDate = release.StartDate;
			this.EndDate = release.EndDate;
			this.Notes = release.Notes;
			this.TotalPoints = release.WorkItems.Sum(x => x.StoryPoints);
			this.TimeEntryCount = release.TimeEntries.Count();
			this.WorkItemCount = release.WorkItems.Count();
		}
	}
}
