using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.WorkItems.JiraApi {

	/*
	Jira API payload example
	{
		"expand": "schema,names",
		"startAt": 0,
		"maxResults": 50,
		"total": 33,
		"issues": [{
			"expand": "operations,versionedRepresentations,editmeta,changelog,renderedFields",
			"id": "12077",
			"self": "https://heuristicsolutions.atlassian.net/rest/api/2/issue/12077",
			"key": "LB-168",
			"fields": {
				"summary": "Prep for and cut the 9.2.0 release",
				"issuetype": {
					"self": "https://heuristicsolutions.atlassian.net/rest/api/2/issuetype/10002",
					"id": "10002",
					"description": "A task that needs to be done.",
					"iconUrl": "https://heuristicsolutions.atlassian.net/secure/viewavatar?size=xsmall&avatarId=10318&avatarType=issuetype",
					"name": "Chore",
					"subtask": false,
					"avatarId": 10318
				},
				"fixVersions": [
					{
						"self": "url",
						"id": "16001",
						"description": "desc",
						"name": "x.y.z",
						"archived": false,
						"released": true,
						"releaseDate": "2018-01-02"
					}
				],
				"customfield_10022": 4,
				"customfield_10018": "LB-epic-ID",
				"labels": [],
				"status": {
					"self": "https://heuristicsolutions.atlassian.net/rest/api/2/status/10500",
					"description": "Story was designed (at least at a high level) but was not selected for inclusion in a release.",
					"iconUrl": "https://heuristicsolutions.atlassian.net/images/icons/statuses/generic.png",
					"name": "Declined",
					"id": "10500",
					"statusCategory": {
					   "self": "https://heuristicsolutions.atlassian.net/rest/api/2/statuscategory/3",
					   "id": 3,
					   "key": "done",
					   "colorName": "green",
					   "name": "Done"
					}
				}
			}
		}]
	}
	*/
	public class JiraSearchApiResponse {
		public class JiraIssue {
			[JsonProperty("key")]
			public string StoryNumber { get; set; }

			[JsonProperty("fields")]
			public StoryDetails Details { get; set; }
		}

		public class StoryDetails {
			[JsonProperty("summary")]
			public string Summary { get; set; }

			[JsonProperty("issuetype")]
			public IssueType IssueType { get; set; }

			[JsonProperty("status")]
			public IssueStatus Status { get; set; }

			[JsonProperty("customfield_10022")]
			public decimal? StoryPoints { get; set; }

			[JsonProperty("customfield_10018")]
			public string EpicStoryId { get; set; }

			[JsonProperty("labels")]
			public List<string> Labels { get; set; }

			[JsonProperty("fixVersions")]
			public FixVersion[] FixVersions { get; set; }
		}

		public class IssueType {
			[JsonProperty("name")]
			public string Name { get; set; }
		}

		public class IssueStatus {
			[JsonProperty("name")]
			public string Name { get; set; }
		}

		public class FixVersion {
			[JsonProperty("id")]
			public string Id { get; set; }

			[JsonProperty("name")]
			public string ReleaseNumber { get; set; }
		}

		// map the root of the JSON doc
		[JsonProperty("issues")]
		public JiraIssue[] Issues { get; set; }

		[JsonProperty("maxResults")]
		public int MaxResults { get; set; }

		[JsonProperty("total")]
		public int TotalResults { get; set; }
	}
}
