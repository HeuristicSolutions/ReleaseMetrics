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
	}
}
