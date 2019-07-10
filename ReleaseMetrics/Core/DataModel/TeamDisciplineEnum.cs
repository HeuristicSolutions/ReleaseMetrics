using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ReleaseMetrics.Core.DataModel {

	/// <remarks>
	/// NOTE: Do not rename these enums w/out updating the database. They are mapped using their string representation
	/// to the data model.
	/// </remarks>
	public enum TeamDisciplineEnum {
		Dev, QA, UIUX, Other
	}
}