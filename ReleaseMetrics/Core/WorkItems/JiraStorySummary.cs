using ReleaseMetrics.Core.WorkItems.JiraApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.WorkItems {
	
	/// <summary>
	/// Web-safe DTO summarizing a Jira story referenced by a time entry.
	/// </summary>
	public class JiraStorySummary {

		public string Id { get; set; }
		public string EpicStoryId { get; set; }
		public string IssueType { get; set; }
		public string Status { get; set; }
		public string Title { get; set; }
		public decimal? StoryPoints { get; set; }
		public List<string> Labels { get; set; }

		public JiraStorySummary(JiraStory story) {
			this.Id = story.Id;
			this.EpicStoryId = story.EpicStoryId;
			this.IssueType = story.IssueType;
			this.Status = story.Status;
			this.Title = story.Title;
			this.StoryPoints = story.StoryPoints;
			this.Labels = story.Labels;
		}
	}
}
