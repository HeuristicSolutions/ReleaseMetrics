using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.TimeEntries.MavenlinkApi {

	/*

	Sample of the ML timesheet API payload

	{
	   "count": 1,
	   "results":    [
				{
			 "key": "time_entries",
			 "id": "562670875"
		  }
	   ],
	   "time_entries":    {
		  "562670875":       {
			 "created_at": "2018-03-30T14:31:54-07:00",
			 "updated_at": "2018-04-02T07:45:29-07:00",
			 "date_performed": "2018-03-30",
			 "time_in_minutes": 120,
			 "billable": true,
			 "notes": "Import the reason codes from Pulse and update the public member search for each board.",
			 "rate_in_cents": 16500,
			 "cost_rate_in_cents": 10000,
			 "currency": "USD",
			 "currency_symbol": "$",
			 "currency_base_unit": 100,
			 "user_can_edit": false,
			 "approved": true,
			 "taxable": false,
			 "is_invoiced": true,
			 "story_id": "274141475",
			 "workspace_id": "19092375",
			 "user_id": "9572715",
			 "active_submission_id": "45052605",
			 "id": "562670875"
		  }
	   },
	   "meta":    {
		  "count": 1,
		  "page_count": 1,
		  "page_number": 1,
		  "page_size": 200
	   }

	*/
	public class MavenlinkTimesheetApiResponse {
		public class ResultsNode {
			[JsonProperty("key")]
			public string Key { get; set; }

			[JsonProperty("id")]
			public string Id { get; set; }
		}

		public class TimesheetData {
			[JsonProperty("id")]
			public string Id { get; set; }

			[JsonProperty("created_at")]
			public DateTime CreatedAt { get; set; }

			[JsonProperty("updated_at")]
			public DateTime UpdatedAt { get; set; }

			[JsonProperty("date_performed")]
			public DateTime DatePerformed { get; set; }

			[JsonProperty("time_in_minutes")]
			public int DurationMinutes { get; set; }

			[JsonProperty("billable")]
			public bool Billable { get; set; }

			[JsonProperty("notes")]
			public string Notes { get; set; }

			[JsonProperty("workspace_id")]
			public string ProjectId { get; set; }

			[JsonProperty("story_id")]
			public string TaskId { get; set; }

			[JsonProperty("user_id")]
			public string UserId { get; set; }
		}

		public class MetaNode {
			[JsonProperty("count")]
			public int Count { get; set; }

			[JsonProperty("page_count")]
			public int PageCount { get; set; }

			[JsonProperty("page_number")]
			public int PageNumber { get; set; }

			[JsonProperty("page_size")]
			public int PageSize { get; set; }
		}

		// map the root of the JSON doc
		[JsonProperty("count")]
		public int Count { get; set; }

		[JsonProperty("results")]
		public ResultsNode[] ResultsIndex { get; set; }

		[JsonProperty("time_entries")]
		public Dictionary<string, TimesheetData> TimeEntries { get; set; }

		[JsonProperty("meta")]
		public MetaNode Meta { get; set; }
	}
}
