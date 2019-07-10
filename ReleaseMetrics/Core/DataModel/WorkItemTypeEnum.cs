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
		/// Legacy Defects are defects fixed in a release that were introduced in an earlier release. These are NOT
		/// related to the work we'd planned to do in the release.
		/// </summary>
		LegacyDefect,

		/// <summary>
		/// New Defects are defects created by, and fixed within, the current release. They indicate that a story was
		/// pushed to QA with some deficiency that was later caught and fixed as part of the release process.
		/// </summary>
		NewDefect,

		/// <summary>
		/// Items on the architecture backlog don't count towards metrics
		/// </summary>
		ArchitecturalIssue
	}
}