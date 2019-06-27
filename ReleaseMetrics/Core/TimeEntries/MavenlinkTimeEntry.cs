using System;
using Newtonsoft.Json;
using ReleaseMetrics.Core.TimeEntries.MavenlinkApi;

namespace ReleaseMetrics.Core.TimeEntries {

	/// <summary>
	/// Flattened DTO representing a Mavenlink time entry.
	/// </summary>
	public class MavenlinkTimeEntry {

		public string MavenlinkTimeId { get; set; }
		public string ProjectId { get; set; }
		public string ProjectTitle { get; set; }
		public string TaskId { get; set; }
		public string TaskTitle { get; set; }
		public string UserName { get; set; }
		public DateTime DatePerformed { get; set; }
		public int DurationMinutes { get; set; }
		public string Notes { get; set; }
		public bool Billable { get; set; }

		public DateTime MavenlinkCreatedAt { get; set; }
		public DateTime MavenlinkUpdatedAt { get; set; }

		public MavenlinkTimeEntry() { }

		public MavenlinkTimeEntry(
			MavenlinkTimesheetApiResponse.TimesheetData entry, 
			MavenlinkUserApiResponse.UserData user, 
			MavenlinkWorkspaceApiResponse.Project project, 
			MavenlinkTaskSearchApiResponse.StoryData task) {

			MavenlinkTimeId = entry.Id;
			ProjectId = entry.ProjectId;
			ProjectTitle = project.Title;
			TaskId = entry.TaskId;
			TaskTitle = task.Title;
			UserName = user.Name;
			DatePerformed = entry.DatePerformed;
			DurationMinutes = entry.DurationMinutes;
			Notes = entry.Notes;
			Billable = entry.Billable;

			MavenlinkCreatedAt = entry.CreatedAt;
			MavenlinkUpdatedAt = entry.UpdatedAt;
		}
	}
}
