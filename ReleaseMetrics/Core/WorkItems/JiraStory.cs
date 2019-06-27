using ReleaseMetrics.Core.WorkItems.JiraApi;
using System.Collections.Generic;
using System.Linq;

namespace ReleaseMetrics.Core.WorkItems {

	/// <summary>
	/// Simplified DTO representing Jira data loaded via the API.
	/// </summary>
	public class JiraStory {

		public string Id { get; set; }
		public string EpicStoryId { get; set; }
		public string IssueType { get; set; }
		public string Status { get; set; }
		public string Title { get; set; }
		public decimal? StoryPoints { get; set; }
		public List<string> Labels { get; set; }
		public List<string> FixVersions { get; set; }

		public JiraStory() { }

		public JiraStory(JiraSearchApiResponse.JiraIssue rawApiResponse) {
			Id = rawApiResponse.StoryNumber;
			EpicStoryId = rawApiResponse.Details.EpicStoryId;
			IssueType = rawApiResponse.Details.IssueType.Name;
			Status = rawApiResponse.Details.Status.Name;
			Title = rawApiResponse.Details.Summary;
			StoryPoints = rawApiResponse.Details.StoryPoints;
			Labels = rawApiResponse.Details.Labels;
			FixVersions = rawApiResponse.Details.FixVersions.Select(x => x.ReleaseNumber).ToList();
		}
	}
}
