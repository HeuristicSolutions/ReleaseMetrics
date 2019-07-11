using Microsoft.EntityFrameworkCore.Migrations;

namespace ReleaseMetrics.Migrations {

    public partial class AddMetricViews : Migration {

        protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.Sql(@"
				create view vReleasedWorkItemSummary as
					select	w.StoryNumber
					,		w.Type
					,		w.Title
					,		w.ReleaseNumber
					,		w.Status
					,		w.EpicWorkItemId
					,		w.StoryPoints
					,		w.BillToClient
					,		(
								select	isnull(sum(tea.DurationMinutes), 0)
								from	dbo.TimeEntryWorkItemAllocations tea
										inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
								where	tea.WorkItemId = w.StoryNumber
							) as TotalMinutes
					,		(
								select	isnull(sum(tea.DurationMinutes), 0)
								from	dbo.TimeEntryWorkItemAllocations tea
										inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
								where	tea.WorkItemId = w.StoryNumber
										and te.Discipline = 'Dev'
							) as DevMinutes
					,		(
								select	isnull(sum(tea.DurationMinutes), 0)
								from	dbo.TimeEntryWorkItemAllocations tea
										inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
								where	tea.WorkItemId = w.StoryNumber
										and te.Discipline = 'QA'
							) as QaMinutes
					,		(
								select	isnull(sum(tea.DurationMinutes), 0)
								from	dbo.TimeEntryWorkItemAllocations tea
										inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
								where	tea.WorkItemId = w.StoryNumber
										and te.Discipline = 'UIUX'
							) as UiUxMinutes
					,		(
								select	isnull(sum(tea.DurationMinutes), 0)
								from	dbo.TimeEntryWorkItemAllocations tea
										inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
								where	tea.WorkItemId = w.StoryNumber
										and te.Discipline = 'Other'
							) as OtherMinutes
					from	dbo.WorkItems w
					where	w.Status = 'Shipped'
							and w.Type not in ('ArchitecturalIssue', 'Epic');
				go

				create view vReleaseMetrics as 
					with	
						ReleaseSummary as (
							select	r.ReleaseNumber
							,		r.Notes
							,		r.StartDate
							,		r.EndDate
							,		DateDiff(week, r.StartDate, r.EndDate) as WeeksInRelease

							,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChorePoints
							,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChoreCount
							,		( select isnull(sum(wi.TotalMinutes), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChoreMinutes

							,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeaturePoints
							,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeatureCount
							,		( select isnull(sum(wi.TotalMinutes), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeatureMinutes

							,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency' and wi.StoryPoints > 0) as ContingencyPoints
							,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency' and wi.StoryPoints > 0) as ContingencyCount
							,		( select isnull(sum(wi.TotalMinutes), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency') as ContingencyMinutes
		
							,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectPoints
							,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectCount
							,		( select isnull(sum(wi.TotalMinutes), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectMinutes

							,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectPoints
							,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectCount
							,		( select isnull(sum(wi.TotalMinutes), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectMinutes

							,		( select isnull(sum(te.DurationMinutesOverride), 0) from dbo.TimeEntries te where te.ReleaseNumber = r.ReleaseNumber) as TotalBilledMinutes

							from	dbo.Releases r
						)

					select	rs.ReleaseNumber
					,		rs.Notes
					,		rs.StartDate
					,		rs.EndDate
					,		rs.WeeksInRelease

					,		rs.ChoreCount
					,		rs.ChorePoints
					,		rs.ChoreMinutes

					,		rs.FeatureCount
					,		rs.FeaturePoints
					,		rs.FeatureMinutes

					,		rs.ContingencyCount
					,		rs.ContingencyPoints
					,		rs.ContingencyMinutes

					,		rs.LegacyDefectCount
					,		rs.LegacyDefectPoints
					,		rs.LegacyDefectMinutes

					,		rs.NewDefectCount
					,		rs.NewDefectPoints
					,		rs.NewDefectMinutes

					,		rs.TotalBilledMinutes
					,		(rs.ChoreCount + rs.FeatureCount) as ShippedFeatureAndChoreCount
					,		(rs.ChorePoints + rs.FeaturePoints) as ShippedFeatureAndChorePoints
					,		rs.ContingencyPoints as UnusedContingencyPoints
					,		((rs.ChoreMinutes + rs.FeatureMinutes + rs.NewDefectMinutes) / 60) / (rs.ChoreCount + rs.FeatureCount) as AvgHoursPerFeatureAndChorePoint
					,		(rs.TotalBilledMinutes / 60.0) / (rs.ChoreCount + rs.FeatureCount) as FullyLoadedAvgHoursPerPoint

					from	ReleaseSummary rs
			");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"
				drop view vReleasedWorkItemSummary
				go

				drop view vReleaseMetrics
				go
			");
		}
	}
}
