using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ReleaseMetrics.Core.DataModel {

	public class Release {

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[MaxLength(25)]
		public string ReleaseNumber { get; set; }

		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		public DateTime EndDate { get; set; }

		public string Notes { get; set; }

		/// <summary>
		/// All work items associated with the release. 
		/// </summary>
		[JsonIgnore]
		[IgnoreDataMember]
		public virtual ICollection<WorkItem> WorkItems { get; set; }

		/// <summary>
		/// All time entries billed to the release. Not all time entries must be associated with a work item!
		/// </summary>
		[JsonIgnore]
		[IgnoreDataMember]
		public virtual ICollection<TimeEntry> TimeEntries { get; set; }
	}
}