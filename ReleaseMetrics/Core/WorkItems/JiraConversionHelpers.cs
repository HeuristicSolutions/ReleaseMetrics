using Heuristics.Library.Extensions;
using ReleaseMetrics.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.WorkItems {

	public static class JiraHelper {

		public static WorkItemTypeEnum GetWorkItemType(string jiraIssueType) {
			switch (jiraIssueType.ToUpperNullSafe()) {
				case "ARCHITECTURAL ISSUE": return WorkItemTypeEnum.ArchitecturalIssue;
				case "EPIC": return WorkItemTypeEnum.Epic;
				case "STORY": return WorkItemTypeEnum.Feature;
				case "CHORE": return WorkItemTypeEnum.Chore;
				case "DEFECT": return WorkItemTypeEnum.Defect;
				default: throw new NotImplementedException($"Jira issue type '{jiraIssueType}' is not mapped to a work item type");
			}
		}
	}
}
