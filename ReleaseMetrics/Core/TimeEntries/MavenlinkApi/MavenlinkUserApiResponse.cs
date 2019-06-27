using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.TimeEntries.MavenlinkApi {

	/*

	Sample of the ML user API payload

	{
	   "count": 1,
	   "results":    [
				{
			 "key": "time_entries",
			 "id": "10077365"
		  }
	   ],
		"users": {"10077365":    {
			  "full_name": "Kees Humes",
			  "photo_path": "https://app.mavenlink.com/images/default_profile_photo/default.png",
			  "email_address": "khumes@heuristics.net",
			  "headline": "Summer Intern",
			  "generic": false,
			  "disabled": false,
			  "update_whitelist":       [
				 "full_name",
				 "headline",
				 "email_address",
				 "external_reference"
			  ],
			  "account_id": "4986335",
			  "id": "10077365"
		   }
		},
		"meta":    {
		  "count": 1,
		  "page_count": 1,
		  "page_number": 1,
		  "page_size": 20
	   }
	}
	*/
	public class MavenlinkUserApiResponse {
		public class ResultsNode {
			[JsonProperty("key")]
			public string Key { get; set; }

			[JsonProperty("id")]
			public string Id { get; set; }
		}

		public class UserData {
			[JsonProperty("id")]
			public string Id { get; set; }

			[JsonProperty("full_name")]
			public string Name { get; set; }
		}

		// map the root of the JSON doc
		[JsonProperty("count")]
		public int Count { get; set; }

		[JsonProperty("results")]
		public ResultsNode[] ResultsIndex { get; set; }

		[JsonProperty("users")]
		public Dictionary<string, UserData> Users { get; set; }
	}
}
