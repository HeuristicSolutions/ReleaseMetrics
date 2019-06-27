using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ReleaseMetrics.Core.DataModel {

	public enum WorkItemTypeEnum {

		/// <summary>
		/// Epics contain features and stories. They generally don't have planned time billed to them.
		/// </summary>
		Epic,

		/// <summary>
		/// Chores are things that need done and count towards velocity, but they don't necessarily represent
		/// "business value". Examples would include updating our release scripts.
		/// </summary>
		Chore,

		/// <summary>
		/// Features represent business value being delivered
		/// </summary>
		Feature,

		/// <summary>
		/// Defects represent that some previously delivered, and "counted", feature was incomplete. They do
		/// not count towards velocity
		/// </summary>
		Defect,

		/// <summary>
		/// Items on the architecture backlog don't count towards metrics
		/// </summary>
		ArchitecturalIssue
	}
}