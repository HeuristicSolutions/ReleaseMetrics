using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Heuristics.Library.Extensions;
using ReleaseMetrics.Core.Helpers;
using ReleaseMetrics.Core.Releases;
using System.Data.SqlClient;
using ReleaseMetrics.Core.TimeEntries;

namespace ReleaseMetrics.Core.DataModel {

	public class MetricsDbContext : DbContext {
		public MetricsDbContext(DbContextOptions<MetricsDbContext> options)
			: base(options) { }

		public DbSet<Release> Releases { get; set; }
		public DbSet<WorkItem> WorkItems { get; set; }
		public DbSet<TimeEntry> TimeEntries { get; set; }
		public DbSet<TimeEntryWorkItemAllocation> TimeEntryWorkItemAllocations { get; set; }
		public DbQuery<ReleaseSummary> ReleaseSummaries { get; set; }

		protected override void OnModelCreating(ModelBuilder builder) {
			builder.Entity<TimeEntry>()
				.HasMany(x => x.WorkItems)
				.WithOne(w => w.TimeEntry)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<TimeEntry>()
				.HasIndex(x => x.Id).IsUnique(true);

			builder.Entity<WorkItem>()
				.HasIndex(x => x.StoryNumber);

			builder.Entity<TimeEntryWorkItemAllocation>()
				.HasKey(x => new { x.TimeEntryId, x.WorkItemId });

			// many to many join table
			builder.Entity<TimeEntryWorkItemAllocation>()
				.HasOne(wa => wa.TimeEntry)
				.WithMany(te => te.WorkItems)
				.HasForeignKey(wa => wa.TimeEntryId);
			builder.Entity<TimeEntryWorkItemAllocation>()
				.HasOne(wa => wa.WorkItem)
				.WithMany(wi => wi.TimeEntries)
				.HasForeignKey(wa => wa.WorkItemId);

			// summary views
			builder
				.Query<ReleaseSummary>().ToView("vReleaseSummaries");

			// initial data load
			builder.Entity<Release>().HasData(
				new Release {
					ReleaseNumber = "9.2.0",
					StartDate = new DateTime(2018, 3, 5),
					EndDate = new DateTime(2018, 4, 10),
					Notes = "Quick tunaround release specific to GSX. Added Pearson VUE integration. Limited regression test. Missed the original expected release date by 1 week for stabilization; contributing factors were weak up-front technical analysis that failed to identify complexity in the API calls and Mike/Brad's relative inexperience with the product and team."
				},
				new Release {
					ReleaseNumber = "9.3.0",
					StartDate = new DateTime(2018, 4, 9),
					EndDate = new DateTime(2018, 6, 1),
					Notes = "Multi-client release. Primary focus was Vouchers for ABPANC. Also included the \"call external API\" behavior (ABPANC), Tenant-specific dashboards (GSX), tweaks for 3rd party payment details (DCOPLA), and minor R&D enhancements. Included a full regression test. Spanned LB Academy."
				},
				new Release {
					ReleaseNumber = "9.4.0",
					StartDate = new DateTime(2018, 6, 4),
					EndDate = new DateTime(2018, 8, 3),
					Notes = "Primary focus was prevention of duplicate SSNs for DCOPLA. Also included some R&D improvements to the Automations system and the ability to bulk load Intrinsic Attributes in the Workflow Attribute Retrieval Service (part of performance improvement long-term plan). Released as scheduled following a full regression test, but development team was about two weeks ahead of schedule and started the 9.5 features early."
				}
			);
		}

		#region Stuff that should probably be moved to a repository or service class...

		public Release GetReleaseByNumber(string releaseNum) {
			var release = this.Releases
				.Where(r => r.ReleaseNumber == releaseNum.Trim())
				.FirstOrThrow($"Release '{releaseNum}' not found");

			return release;
		}

		public ReleaseSummary GetReleaseSummaryByNumber(string releaseNum) {
			return ReleaseSummaries
				.Where(rel => rel.ReleaseNumber == releaseNum)
				.FirstOrDefault();
		}

		public TimeEntry GetTimeEntry(string timeEntryId) {
			if (timeEntryId.IsNullOrEmpty())
				throw new ArgumentException("'timeEntryId' cannot be null or empty");

			var entry = this.TimeEntries
				.Where(x => x.Id == timeEntryId)
				.FirstOrDefault();

			return entry;
		}

		public List<TimeEntry> GetTimeEntriesForRelease(string releaseNum) {
			// if the release ends in ".0", then the task titles will just use "x.y" and not "x.y.z"
			if (releaseNum.EndsWith(".0")) {
				releaseNum = releaseNum.RemoveTrailing(".0");
			}

			var timeEntries = this.TimeEntries
				.Where(x => x.TaskTitleOrig.Contains(releaseNum))
				.OrderBy(x => x.ProjectTitleOverride)
				.ThenBy(x => x.TaskTitleOverride)
				.ThenBy(x => x.UserName)
				.ThenBy(x => x.DatePerformed)
				.ToList();

			return timeEntries;
		}

		public WorkItem GetWorkItem(string id) {
			var workItem = this.WorkItems
				.Where(w => w.StoryNumber == id.Trim())
				.FirstOrDefault();

			if (workItem == null) {
				throw new NotFoundException($"Work Item '{id}' not found");
			}

			return workItem;
		}

		public TimeEntry RevertToOriginal(string timeEntryId) {
			var commandText = @"
				UPDATE	TimeEntries 
				SET		ProjectIdOverride = ProjectIdOrig,
						ProjectTitleOverride = ProjectTitleOrig,
						TaskIdOverride = TaskIdOrig,
						TaskTitleOverride = TaskTitleOrig,
						DurationMinutesOverride = DurationMinutesOrig,
						NotesOverride = NotesOrig,
						LocallyUpdatedAt = LocallyCreatedAt
				WHERE	Id = @Id";
			var idParam = new SqlParameter("@Id", timeEntryId);
			this.Database.ExecuteSqlCommand(commandText, idParam);

			return GetTimeEntry(timeEntryId);
		}

		public TimeEntry SetTimeEntryIgnoredFlag(string timeEntryId, bool ignore) {
			var commandText = @"
				UPDATE	TimeEntries 
				SET		Ignore = @Ignore
				WHERE	Id = @Id";
			var ignoreParam = new SqlParameter("@Ignore", ignore);
			var idParam = new SqlParameter("@Id", timeEntryId);
			this.Database.ExecuteSqlCommand(commandText, ignoreParam, idParam);

			return GetTimeEntry(timeEntryId);
		}

		public TimeEntry UpdateTimeEntry(EditTimeEntryModel data) {
			var commandText = @"
				UPDATE	TimeEntries 
				SET		TaskTitleOverride = @Title,
						NotesOverride = @Notes,
						DurationMinutesOverride = @Duration,
						LocallyUpdatedAt = getDate()
				WHERE	Id = @Id";
			var taskParam = new SqlParameter("@Title", data.TaskTitle);
			var notesParam = new SqlParameter("@Notes", data.Notes);
			var durationParam = new SqlParameter("@Duration", data.Duration);
			var idParam = new SqlParameter("@Id", data.TimeEntryId);
			this.Database.ExecuteSqlCommand(commandText, taskParam, notesParam, durationParam, idParam);

			return GetTimeEntry(data.TimeEntryId);
		}

		#endregion
	}
}