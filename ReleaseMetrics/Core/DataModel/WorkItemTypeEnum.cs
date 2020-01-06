using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ReleaseMetrics.Core.DataModel {

	/// <remarks>
	/// NOTE: Do not rename these enums w/out updating the database. They are mapped using their string representation
	/// to the data model.
	/// </remarks>
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
		ArchitecturalIssue,

		/// <summary>
		/// Contingency cases are a way of recording "planned contingency". Typically, as we find issues, we decrease
		/// the points on these stories and use those points to create the actual stories. Generally speaking there
		/// shouldn't be any time billed directly to these cases. Contingency cases with points > 0 indicate that we
		/// shipped a planned scope of work with less-than-estimated complexity.
		/// </summary>
		Contingency,

		/// <summary>
		/// We have begun counting UI tests as "points" to reflect that they are a discrete item of complexity that
		/// we didn't use to do, and lumping them in w/ feature points would result in bloating the "hours per point"
		/// while obscuring that a "point" now contains more "stuff". So, we count them as specific items.
		/// </summary>
		UITest,

		/// <summary>
		/// TODO: Determine how to handle these
		/// </summary>
		SubTask,

		/// <summary>
		/// Feature Requests should count towards "undelivered"; ideally, the time should be billed as analysis.
		/// </summary>
		FeatureRequest
	}
}