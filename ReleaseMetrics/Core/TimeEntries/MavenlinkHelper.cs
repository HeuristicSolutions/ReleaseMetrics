using Heuristics.Library.Extensions;
using ReleaseMetrics.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.TimeEntries {

	public static class MavenlinkHelper {

		public static BillingClassificationEnum GetBillingClassification(string taskTitle) {
			if (taskTitle.ContainsIgnoringCase("unplanned"))
				return BillingClassificationEnum.Unplanned;

			if (taskTitle.ContainsIgnoringCase("overhead"))
				return BillingClassificationEnum.Overhead;

			if (taskTitle.ContainsIgnoringCase("planned"))
				return BillingClassificationEnum.Planned;

			throw new ApplicationException($"Could not map task '{taskTitle}' to Planned, Unplanned, or Overhead");
		}

		public static List<string> GetJiraStoryIds(string timeEntryComment) {
			return Regex.Matches(timeEntryComment ?? "", "LB[- ]?([0-9]+)", RegexOptions.IgnoreCase)
				.Cast<Match>()
				.Select(x => "LB-" + x.Groups[1].Value)
				.Distinct()
				.Where(x => x.IsNotNullOrEmpty())
				.OrderBy(x => x)
				.ToList();
		}

		public static TeamDisciplineEnum GetTeamDiscipline(string userName) {
			switch (userName.ToUpper()) {
				case "BRAD PHIPPS":
				case "MIKE FEIMSTER":
				case "CALVIN ALLEN":
				case "SCOTT REED":
					return TeamDisciplineEnum.Dev;

				case "SETH PETRY-JOHNSON":
					return TeamDisciplineEnum.Dev;

				case "BRIAN LYTLE":
				case "CHRISTY MOORE":
					return TeamDisciplineEnum.QA;

				case "SARA NICHOLSON":
					return TeamDisciplineEnum.UIUX;

				default:
					return TeamDisciplineEnum.Other;
			}
		}
	}
}
