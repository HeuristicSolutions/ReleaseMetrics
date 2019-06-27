using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.TimeEntries.MavenlinkApi {

	/*

	Sample of the ML workspace API payload

	{
	   "count": 1,
	   "results":    [
				{
			 "key": "workspaces",
			 "id": "19072685"
		  }
	   ],
	   "workspaces":    {
		  "19072685":       {
			 "title": "DCOPLA: Support",
			 "account_id": 4986335,
			 "archived": false,
			 "description": "This project tracks DCOPLA tasks.\n\nImplementation Guide: https://tinyurl.com/yblxywh5\nWorkflow Diagrams: https://tinyurl.com/y7acz8dj",
			 "due_date": "2018-06-30",
			 "effective_due_date": "2018-06-30",
			 "start_date": "2018-01-01",
			 "budgeted": true,
			 "change_orders_enabled": false,
			 "updated_at": "2018-07-25T08:20:28-07:00",
			 "created_at": "2018-01-30T13:58:50-08:00",
			 "consultant_role_name": "Implementation Analyst",
			 "client_role_name": "Clients",
			 "percentage_complete": 91,
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
			 "workspace_invoice_preference_id": 3102395,
			 "posts_require_privacy_decision": false,
			 "require_time_approvals": true,
			 "require_expense_approvals": true,
			 "has_active_timesheet_submissions": true,
			 "has_active_expense_report_submissions": false,
			 "status":          {
				"color": "yellow",
				"key": 401,
				"message": "Active"
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
			 "budget_used": "$149,045",
			 "budget_used_in_cents": 14904575,
			 "budget_remaining": null,
			 "target_margin": 20,
			 "stories_are_fixed_fee_by_default": false,
			 "creator_id": "9565985",
			 "primary_maven_id": "9572735",
			 "id": "19072685"
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
	public class MavenlinkWorkspaceApiResponse {
		public class ResultsNode {
			[JsonProperty("key")]
			public string Key { get; set; }

			[JsonProperty("id")]
			public string Id { get; set; }
		}

		public class Project {
			[JsonProperty("id")]
			public string Id { get; set; }

			[JsonProperty("title")]
			public string Title { get; set; }
		}

		// map the root of the JSON doc
		[JsonProperty("count")]
		public int Count { get; set; }

		[JsonProperty("results")]
		public ResultsNode[] ResultsIndex { get; set; }

		[JsonProperty("workspaces")]
		public Dictionary<string, Project> Projects { get; set; }
	}
}
