using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.TimeEntries.MavenlinkApi {

	/*
	Mavenlink Task Search API payload example
	{
	   "count": 1,
	   "results":    [
		  {
			 "key": "stories",
			 "id": "310106435"
		  }
	   ],
	   "stories":    {
		  "310106435":       {
			 "title": "9.3: Planned",
			 "description": null,
			 "updated_at": "2018-07-24T19:10:30-07:00",
			 "assignment_timestamped_at": "2018-07-24T19:10:30-07:00",
			 "created_at": "2018-05-31T04:19:04-07:00",
			 "due_date": null,
			 "start_date": null,
			 "story_type": "task",
			 "state": "completed",
			 "position": 2,
			 "archived": false,
			 "deleted_at": null,
			 "sub_story_count": 0,
			 "percentage_complete": 100,
			 "priority": "normal",
			 "has_proofing_access": false,
			 "ancestor_ids": ["310106325"],
			 "subtree_depth": 0,
			 "time_trackable": true,
			 "time_estimate_in_minutes": null,
			 "logged_billable_time_in_minutes": 5537,
			 "logged_nonbillable_time_in_minutes": 0,
			 "sub_stories_time_estimate_in_minutes": null,
			 "sub_stories_billable_time_in_minutes": null,
			 "weight": null,
			 "budget_estimate_in_cents": null,
			 "budget_used_in_cents": 1659675,
			 "uninvoiced_balance_in_cents": 1659675,
			 "invoiced_balance_in_cents": 0,
			 "sub_stories_budget_estimate_in_cents": null,
			 "sub_stories_budget_used_in_cents": null,
			 "fixed_fee": true,
			 "billable": true,
			 "workspace_id": "19842585",
			 "creator_id": "9572755",
			 "parent_id": "310106325",
			 "root_id": "310106325",
			 "id": "310106435"
		  }
	   },
	   "meta":    {
		  "count": 1,
		  "page_count": 1,
		  "page_number": 1,
		  "page_size": 20
	   },
	  "workspaces":    {
		  "19092375":       {
			 "title": "DCOPLA: Final Implementation Plan",
			 "account_id": 4986335,
			 "archived": false,
			 "description": "Implementation Guide: \nWorkflow Diagrams: https://tinyurl.com/y7acz8dj",
			 "due_date": "2018-06-30",
			 "effective_due_date": "2018-06-30",
			 "start_date": "2018-01-12",
			 "budgeted": true,
			 "change_orders_enabled": false,
			 "updated_at": "2018-07-24T19:13:52-07:00",
			 "created_at": "2018-02-01T06:42:28-08:00",
			 "consultant_role_name": "Implementation Analyst",
			 "client_role_name": "Clients",
			 "percentage_complete": 89,
			 "access_level": "open",
			 "exclude_archived_stories_percent_complete": false,
			 "can_create_line_items": true,
			 "default_rate": "250.00",
			 "currency": "USD",
			 "currency_symbol": "$",
			 "currency_base_unit": 100,
			 "can_invite": true,
			 "has_budget_access": true,
			 "tasks_default_non_billable": false,
			 "rate_card_id": 368415,
			 "workspace_invoice_preference_id": 3167415,
			 "posts_require_privacy_decision": false,
			 "require_time_approvals": true,
			 "require_expense_approvals": true,
			 "has_active_timesheet_submissions": true,
			 "has_active_expense_report_submissions": false,
			 "status":          {
				"color": "green",
				"key": 310,
				"message": "In Progress"
			 },
			 "update_whitelist":          [
				"title",
				"budgeted",
				"start_date",
				"due_date",
				"description",
				"currency",
				"access_level",
				"consultant_role_name",
				"client_role_name",
				"rate_card_id",
				"exclude_archived_stories_percent_complete",
				"tasks_default_non_billable",
				"stories_are_fixed_fee_by_default",
				"change_orders_enabled",
				"expenses_in_burn_rate",
				"posts_require_privacy_decision",
				"status_key",
				"invoice_preference",
				"approver_id",
				"approver_ids",
				"workspace_group_ids",
				"project_template_start_date",
				"project_template_assignment_mappings",
				"project_template_weekends_as_workdays",
				"project_template_create_unnamed_resources",
				"project_tracker_template_id",
				"target_margin",
				"external_reference",
				"account_color_id",
				"archived",
				"price",
				"project_template_before_story_id"
			 ],
			 "account_features":          {
				"time_trackable": false,
				"has_time_entry_role_picker": true,
				"project_side_panel": true
			 },
			 "permissions":          {
				"can_upload_files": true,
				"can_private_message": true,
				"can_join": false,
				"is_participant": true,
				"access_level": "admin",
				"team_lead": false,
				"user_is_client": false,
				"can_change_price": true,
				"can_change_story_billable": true,
				"can_post": true,
				"can_edit": true,
				"restricted": false,
				"can_see_financials": true
			 },
			 "over_budget": false,
			 "expenses_in_burn_rate": true,
			 "total_expenses_in_cents": 0,
			 "price_in_cents": null,
			 "price": "TBD",
			 "percent_of_budget_used": 0,
			 "budget_used": "$257,615",
			 "budget_used_in_cents": 25761525,
			 "budget_remaining": null,
			 "target_margin": null,
			 "stories_are_fixed_fee_by_default": false,
			 "creator_id": "9565985",
			 "primary_maven_id": "9572735",
			 "id": "19092375"
		  }
	   }   
	}
	*/
	public class MavenlinkTaskSearchApiResponse {
		public class ResultsNode {
			[JsonProperty("key")]
			public string Key { get; set; }

			[JsonProperty("id")]
			public string Id { get; set; }
		}

		public class StoryData {
			[JsonProperty("id")]
			public string Id { get; set; }

			[JsonProperty("title")]
			public string Title { get; set; }

			[JsonProperty("description")]
			public string Description { get; set; }

			[JsonProperty("workspace_id")]
			public string ProjectId { get; set; }

			[JsonProperty("parent_id")]
			public string ParentId { get; set; }

			[JsonProperty("root_id")]
			public string RootId { get; set; }
		}

		public class WorkspaceData {
			[JsonProperty("id")]
			public string Id { get; set; }

			[JsonProperty("title")]
			public string Title { get; set; }

			[JsonProperty("description")]
			public string Description { get; set; }

			[JsonProperty("archived")]
			public bool IsArchived { get; set; }
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

		[JsonProperty("stories")]
		public Dictionary<string, StoryData> Stories { get; set; }

		[JsonProperty("workspaces")]
		public Dictionary<string, WorkspaceData> Projects { get; set; }

		[JsonProperty("meta")]
		public MetaNode Meta { get; set; }
	}
}
