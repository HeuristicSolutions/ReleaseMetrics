using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseMetrics.Core.DataModel {

	/// <summary>
	/// In Mavenlink, a single time entry can reference multiple work items (via the Notes field mentioning multiple
	/// Jira stories). When this happens the time is allocated  between those work items, with each item getting a 
	/// portion of the overall duration. This is a one-to-many relationship with some extra metadata representing 
	/// the allocated time.
	/// 
	/// These entries are ONLY CREATED FOR VALID WORK ITEMS. It is possible that a Mavenlink time entry references
	/// a Jira story that doesn't exist. In that case, the work item allocation object will not be created; the validation
	/// process confirms that every story mentioned in a note has a corresponding allocation item.
	/// </summary>
	public class TimeEntryWorkItemAllocation {

		[MaxLength(50)]
		public string TimeEntryId { get; set; }
		public virtual TimeEntry TimeEntry { get; set; }

		[MaxLength(50)]
		public string WorkItemId { get; set; }
		public virtual WorkItem WorkItem { get; set; }

		/// <summary>
		/// The duration allocated to this work item. Can be fractional because sometimes an integer duration
		/// is divided between multiple referenced work items.
		/// </summary>
		[Column(TypeName = "decimal(6,3)")]
		public decimal DurationMinutes { get; set; }
	}
}
