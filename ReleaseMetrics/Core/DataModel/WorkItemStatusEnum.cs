using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ReleaseMetrics.Core.DataModel {

	/// <remarks>
	/// NOTE: Do not rename these enums w/out updating the database. They are mapped using their string representation
	/// to the data model.
	/// </remarks>
	public enum WorkItemStatusEnum {

		/// <summary>
		/// Work item was never started and should be completely ignored
		/// </summary>
		NotStarted,

		/// <summary>
		/// Work item was started but was abandonded for some reason. Not counted as "release points", but the elapsed
		/// time IS reported towards "undelivered".
		/// </summary>
		NotShipped,

		/// <summary>
		/// Work item was included in the release and should be counted.
		/// </summary>
		Shipped
	}
}