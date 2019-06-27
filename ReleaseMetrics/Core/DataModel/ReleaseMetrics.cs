using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Heuristics.Library.Extensions;
using Newtonsoft.Json;
using ReleaseMetrics.Core.TimeEntries;

namespace ReleaseMetrics.Core.DataModel {

	/// <summary>
	/// Data is imported from the external timekeeping source and stored in the local database as a historical record. This gives us the 
	/// ability to manually adjust the data as needed to correct typos and make other adjustments that do not impact invoicing (and thus
	/// do not need ported back into the external source) but DO impact the processing of the data for release metrics.
	/// </summary>
	public class ReleaseMetrics {

		/// <summary>
		/// Indicates both the source of the record and ID in the source system, separated by a colon. For instance, an entry
		/// imported from Mavenlink will have an ID like "ML:12345" where 12345 is the ML time entry id.
		/// </summary>
		[Key]
		public string Id { get; set; }

		public string ReleaseNumber { get; set; }
		public virtual Release Release { get; set; }

		public string ProjectId { get; set; }
		public string ProjectTitle { get; set; }
		public string TaskId { get; set; }
		public string TaskTitle { get; set; }
		public string UserName { get; set; }
		public DateTime DatePerformed { get; set; }
		public string Notes { get; set; }
		public bool Billable { get; set; }

		public DateTime SourceRecordCreatedAt { get; set; }
		public DateTime SourceRecordUpdatedAt { get; set; }
		public DateTime LocallyCreatedAt { get; set; }
		public DateTime LocallyUpdatedAt { get; set; }

		/// <summary>
		/// The total time, in minutes, billed in the timekeeping system
		/// </summary>
		public int DurationMinutes { get; set; }

		/// <summary>
		/// The elapsed time, allocated to the relvant Work Items. (These records only exist for work items that are both
		/// referenced by the time entry AND found in the local cache when the record is imported. If a time entry contains
		/// a comment referencing an invalid work item, then no Work Item Allocation will be created. It's the job of the 
		/// validation system to spot these issues)
		/// </summary>
		[JsonIgnore]
		public virtual List<TimeEntryWorkItemAllocation> WorkItems { get; set; }

		/// <summary>
		/// If TRUE then the metrics system should ignore this entry. This is used to indicate that an entry was imported 
		/// from the timekeeping system but was determined to be irrelevant or otherwise unwanted. (These things are not 
		/// hard deleted so that we can retain a historical record of it)
		/// </summary>
		public bool Ignore { get; set; }

		public ReleaseMetrics() {
			WorkItems = new List<TimeEntryWorkItemAllocation>();
		}

		public ReleaseMetrics(string releaseNum, MavenlinkTimeEntry mlEntry, List<TimeEntryWorkItemAllocation> workItemAllocations = null) {
			if (releaseNum.IsNullOrEmpty())
				throw new ArgumentNullException("releaseNum");

			if (mlEntry == null)
				throw new ArgumentNullException("mlEntry");

			this.Id = "ML:" + mlEntry.MavenlinkTimeId;

			this.ReleaseNumber = releaseNum;

			this.ProjectId = mlEntry.ProjectId;
			this.ProjectTitle = mlEntry.ProjectTitle;
			this.TaskId = mlEntry.TaskId;
			this.TaskTitle = mlEntry.TaskTitle;
			this.UserName = mlEntry.UserName;
			this.DatePerformed = mlEntry.DatePerformed;
			this.Notes = mlEntry.Notes;
			this.Billable = mlEntry.Billable;

			this.SourceRecordCreatedAt = mlEntry.MavenlinkCreatedAt;
			this.SourceRecordUpdatedAt = mlEntry.MavenlinkUpdatedAt;
			this.LocallyCreatedAt = DateTime.Now;
			this.LocallyUpdatedAt = DateTime.Now;

			this.DurationMinutes = mlEntry.DurationMinutes;
			this.WorkItems = workItemAllocations ?? new List<TimeEntryWorkItemAllocation>();

			this.Ignore = false;
		}
	}
}
