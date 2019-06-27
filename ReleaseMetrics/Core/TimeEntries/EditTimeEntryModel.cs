using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.TimeEntries {

	public class EditTimeEntryModel {
		public string TimeEntryId { get; set; }
		public string TaskTitle { get; set; }
		public string Notes { get; set; }
		public int Duration { get; set; }

	}
}
