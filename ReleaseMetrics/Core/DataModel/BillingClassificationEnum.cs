using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ReleaseMetrics.Core.DataModel {

	public enum BillingClassificationEnum {

		Planned,
		Unplanned,
		Overhead
	}
}