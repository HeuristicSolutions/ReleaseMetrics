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
				case "DEFECT": return WorkItemTypeEnum.LegacyDefect;
				default: throw new NotImplementedException($"Jira issue type '{issueType}' is not mapped to a work item type");
			}
		}
	}
}
