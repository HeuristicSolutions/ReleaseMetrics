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
	public class TimeEntry {

		/// <summary>
		/// Indicates both the source of the record and ID in the source system, separated by a colon. For instance, an entry
		/// imported from Mavenlink will have an ID like "ML:12345" where 12345 is the ML time entry id.
		/// </summary>
		[Key]
		[MaxLength(50)]
		public string Id { get; set; }

		[MaxLength(25)]
		public string ReleaseNumber { get; set; }
		public virtual Release Release { get; set; }

		/// <summary>
		/// The project Id that the time entry was originally billed to
		/// </summary>
		[MaxLength(50)]
		public string ProjectIdOrig { get; set; }

		/// <summary>
		/// The project Id to use for local calculations. (Will be equal to ProjectIdOrig unless the local record was modified)
		/// </summary>
		[MaxLength(50)]
		public string ProjectIdOverride { get; set; }

		/// <summary>
		/// The project Title that the time entry was originally billed to
		/// </summary>
		[MaxLength(250)]
		public string ProjectTitleOrig { get; set; }

		/// <summary>
		/// The project title to use for all local calculations. (Will be equal to ProjectTitleOrig unless the local record was modified)
		/// </summary>
		[MaxLength(250)]
		public string ProjectTitleOverride { get; set; }

		/// <summary>
		/// The task Id that the time entry was originally billed to
		/// </summary>
		[MaxLength(50)]
		public string TaskIdOrig { get; set; }

		/// <summary>
		/// The task id to use for local calculations. (Will be equal to TaskIdOrig unless the local record was modified)
		/// </summary>
		[MaxLength(50)]
		public string TaskIdOverride { get; set; }

		/// <summary>
		/// The task Title that the time entry was originally billed to
		/// </summary>
		[MaxLength(250)]
		public string TaskTitleOrig { get; set; }

		/// <summary>
		/// The task Title to use for local calculations. (Will be equal to TaskTitleOrig unless the local record was modified)
		/// </summary>
		[MaxLength(250)]
		public string TaskTitleOverride { get; set; }

		[MaxLength(50)]
		public string UserName { get; set; }
		
		public DateTime DatePerformed { get; set; }
		
		/// <summary>
		/// The original notes recorded for the time entry
		/// </summary>
		public string NotesOrig { get; set; }

		/// <summary>
		/// The notes to use for local calculations. (Will be equal to NotesOrig unless the local record was modified)
		/// </summary>
		public string NotesOverride { get; set; }

		public bool Billable { get; set; }

		public DateTime SourceRecordCreatedAt { get; set; }
		public DateTime SourceRecordUpdatedAt { get; set; }
		public DateTime LocallyCreatedAt { get; set; }
		public DateTime LocallyUpdatedAt { get; set; }

		/// <summary>
		/// The total time, in minutes, originally billed in the timekeeping system 
		/// </summary>
		public int DurationMinutesOrig { get; set; }

		/// <summary>
		/// The total time, in minutes, to use in local calculations. (Will equal DurationMinutesOrig unless the record was modified locally)
		/// </summary>
		public int DurationMinutesOverride { get; set; }

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

		public TimeEntry() {
			WorkItems = new List<TimeEntryWorkItemAllocation>();
		}

		public TimeEntry(string releaseNum, MavenlinkTimeEntry mlEntry, List<TimeEntryWorkItemAllocation> workItemAllocations = null) {
			if (releaseNum.IsNullOrEmpty())
				throw new ArgumentNullException("releaseNum");

			if (mlEntry == null)
				throw new ArgumentNullException("mlEntry");

			this.Id = "ML:" + mlEntry.MavenlinkTimeId;

			this.ReleaseNumber = releaseNum;

			this.ProjectIdOrig = mlEntry.ProjectId;
			this.ProjectTitleOrig = mlEntry.ProjectTitle;
			this.ProjectTitleOverride = mlEntry.ProjectTitle;
			this.TaskIdOrig = mlEntry.TaskId;
			this.TaskIdOverride = mlEntry.TaskId;
			this.TaskTitleOrig = mlEntry.TaskTitle;
			this.TaskTitleOverride = mlEntry.TaskTitle;
			this.UserName = mlEntry.UserName;
			this.DatePerformed = mlEntry.DatePerformed;
			this.NotesOrig = mlEntry.Notes;
			this.NotesOverride = mlEntry.Notes;
			this.Billable = mlEntry.Billable;

			this.SourceRecordCreatedAt = mlEntry.MavenlinkCreatedAt;
			this.SourceRecordUpdatedAt = mlEntry.MavenlinkUpdatedAt;
			this.LocallyCreatedAt = DateTime.Now;
			this.LocallyUpdatedAt = DateTime.Now;

			this.DurationMinutesOrig = mlEntry.DurationMinutes;
			this.DurationMinutesOverride = mlEntry.DurationMinutes;
			this.WorkItems = workItemAllocations ?? new List<TimeEntryWorkItemAllocation>();

			this.Ignore = false;
		}
	}
}
