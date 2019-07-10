using Microsoft.EntityFrameworkCore;
using ReleaseMetrics.Core.WorkItems;
using ReleaseMetrics.Core.WorkItems.JiraApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseMetrics.Core.DataModel {

	/// <summary>
	/// Canonical/standardized representation of a story or case that went into a release. These data are cached locally 
	/// to improve performance but we do NOT expect local modifications to be made. Unlike time entries, which may be 
	/// difficult to modify once an invoice is created, work items are generally easily updated so if a change needs to 
	/// be made it will just be made to the external system and re-imported, rather than being modified locally.
	/// 
	/// Can represent both FogBugz (legacy) and Jira (modern) work items.
	/// </summary>
	public class WorkItem {

		/// <summary>
		/// Either "FB-xxxx" if this is a FogBugz item or "LB-xxxx" if this is a Jira item.
		/// </summary>
		[Key]
		[MaxLength(50)]
		public string StoryNumber { get; set; }

		[Required]
		[MaxLength(25)]
		public string ReleaseNumber { get; set; }

		public virtual Release Release { get; set; }

		/// <summary>
		/// The ID of the epic this belongs to, or null.
		/// </summary>
		[MaxLength(50)]
		public string EpicWorkItemId { get; set; }

		/// <summary>
		/// The name of the epic (for historical record), or null.
		/// </summary>
		[MaxLength(250)]
		public string EpicName { get; set; }

		[Required]
		[MaxLength(250)]
		public string Title { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(25)")]
		public WorkItemTypeEnum Type { get; set; }

		public int? StoryPointsOriginal { get; set; }

		[Required]
		public int StoryPoints { get; set; }

		/// <summary>
		/// Which client was the work billed to? Should be a client ID or "R&D" if we funded. 
		/// </summary>
		[Required]
		[MaxLength(150)]
		public string BillToClient { get; set; }

		/// <summary>
		/// The time entries associated with this work item.
		public virtual List<TimeEntryWorkItemAllocation> TimeEntries { get; set; }

		public WorkItem() { }

		public WorkItem(string releaseNum, JiraStory story, JiraStory epic) {
			this.StoryNumber = story.Id;
			this.ReleaseNumber = releaseNum;
			this.EpicWorkItemId = story.EpicStoryId;
			this.EpicName = (epic != null) ? epic.Title : null;
			this.Title = story.Title;
			this.Type = JiraHelper.GetWorkItemType(story);
			this.StoryPointsOriginal = story.StoryPoints.HasValue ? (int)story.StoryPoints.Value : (int?)null;
			this.StoryPoints = story.StoryPoints.HasValue ? (int)story.StoryPoints.Value : 0;
			this.BillToClient = "TODO";
		}

	}
}