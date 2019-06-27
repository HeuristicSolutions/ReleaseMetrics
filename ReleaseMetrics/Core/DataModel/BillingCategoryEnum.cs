using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ReleaseMetrics.Core.DataModel {

	public enum BillingCategoryEnum {

		Analysis,
		Implementation,
		TestCaseManagement,
		TestExecution,
		Documentation
	}
}