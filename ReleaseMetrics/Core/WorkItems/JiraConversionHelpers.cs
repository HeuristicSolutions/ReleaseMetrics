using Heuristics.Library.Extensions;
using ReleaseMetrics.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.WorkItems {

	public static class JiraHelper {

		public static WorkItemTypeEnum GetWorkItemType(JiraStory story) {
			var issueType = story.IssueType.ToUpperNullSafe();

			switch (issueType) {
				case "ARCHITECTURAL ISSUE": return WorkItemTypeEnum.ArchitecturalIssue;
				case "EPIC": return WorkItemTypeEnum.Epic;
				case "STORY": return WorkItemTypeEnum.Feature;
				case "CHORE": return WorkItemTypeEnum.Chore;
				case "CONTINGENCY": return WorkItemTypeEnum.Contingency;
				case "DEFECT" when story.DefectType == "Current": return WorkItemTypeEnum.NewDefect;
				case "DEFECT" when story.DefectType != "Current": return WorkItemTypeEnum.LegacyDefect;		// default to legacy, but maybe could benefit from more analysis here

				default: throw new NotImplementedException($"Could not map Jira issue {story.Id} (a '{issueType}') to a work item type");
			}
		}

		public static WorkItemStatusEnum GetWorkItemStatus(JiraStory story) {
			var status = story.Status.ToUpperNullSafe();

			switch (status) {
				case "DECLINED":
				case "TEMPLATE":
					return WorkItemStatusEnum.NotShipped;
				
				case "DEFECT BACKLOG":
				case "NEEDS TRIAGED":
				case "IN TRIAGE":
				case "RELEASE COMMITMENT":
				case "COMMITTED":
				case "TO DO":
					return WorkItemStatusEnum.NotStarted;
				
				// for the purposes of running metrics, anything that's in process is counted as "shipped". That's because the stories sometimes
				// linger in the final stages of the flow at the end of a release, and we want to be able to run metrics without waiting for 
				// everything to be fully in "done done"
				case "ARCHIVED":				
				case "CODE REVIEW":
				case "DEVELOPMENT":
				case "DEVELOPMENT DONE":
				case "DOCUMENTATION":
				case "DONE DONE":
				case "IN PROGRESS":
				case "READY FOR DOCUMENTATION":
				case "TESTING":
				case "TESTING DONE":
				case "UI / UX":
					return WorkItemStatusEnum.Shipped;

				default: throw new NotImplementedException($"Could not map Jira issue {story.Id} (status '{status}') to a work item status");
			}
		}
	}
}
