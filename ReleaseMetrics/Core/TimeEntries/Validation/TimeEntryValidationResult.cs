using System.Collections.Generic;

namespace ReleaseMetrics.Core.TimeEntries {

	public class TimeEntryValidationResult {

		public string TimeEntryId { get; set; }
		public List<string> Errors { get; set; }
		public List<string> Warnings { get; set; }

		public TimeEntryValidationResult(string timeEntryId, List<string> errors = null, List<string> warnings = null) {
			TimeEntryId = timeEntryId;
			Errors = errors ?? new List<string>();
			Warnings = warnings ?? new List<string>();
		}
	}
}
