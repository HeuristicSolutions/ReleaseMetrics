-- vReleaseSummaries
if exists(select 1 from sys.views where name='vReleaseSummaries' and type='v')
drop view vReleaseSummaries;
go

create view vReleaseSummaries as
	select	r.ReleaseNumber,
			r.StartDate,
			r.EndDate,
			r.Notes,
			(
				select	sum(wi.StoryPoints)
				from	dbo.WorkItems wi
				where	wi.ReleaseNumber = r.ReleaseNumber
						and wi.Type in ('Chore', 'Feature')
						and wi.Status = 'Shipped'
			) as FeatureAndChorePoints,
			(
				select	count(*)
				from	dbo.WorkItems wi
				where	wi.ReleaseNumber = r.ReleaseNumber
						and wi.Type in ('Chore', 'Contingency', 'Feature')
						and wi.Status = 'Shipped'
			) as FeatureAndChoreCount,
			(
				select	count(*)
				from	dbo.TimeEntries te
				where	te.ReleaseNumber = r.ReleaseNumber
			) as TimeEntryCount
	from	dbo.Releases r
go

-- vReleasedWorkItemSummary
if exists(select 1 from sys.views where name='vReleasedWorkItemSummary' and type='v')
drop view vReleasedWorkItemSummary;
go
create view vReleasedWorkItemSummary as
	with ShippedStories as (
		select	w.StoryNumber
		,		w.Type
		,		w.Title
		,		w.ReleaseNumber
		,		w.Status
		,		w.EpicWorkItemId
		,		w.StoryPoints
		,		w.BillToClient
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
				) as TotalHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'Dev'
				) as DevHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'QA'
				) as QaHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'UIUX'
				) as UiUxHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'Other'
				) as OtherHours
		from	dbo.WorkItems w
		where	w.Status = 'Shipped'
				and w.Type not in ('ArchitecturalIssue', 'Epic')
	)

	select	s.StoryNumber
	,		s.Type
	,		s.Title
	,		s.ReleaseNumber
	,		s.Status
	,		s.EpicWorkItemId
	,		s.StoryPoints
	,		s.BillToClient
	,		s.TotalHours
	,		case when s.StoryPoints = 0 then s.TotalHours else s.TotalHours / s.StoryPoints end as HoursPerPoint
	,		s.DevHours
	,		s.QaHours
	,		s.UiUxHours
	,		s.OtherHours
	from	ShippedStories s
go


-- vReleaseMetrics
if exists(select 1 from sys.views where name='vReleaseMetrics' and type='v')
drop view vReleaseMetrics;
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
			,		( select isnull(sum(wi.TotalHours), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChoreHours

			,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeaturePoints
			,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeatureCount
			,		( select isnull(sum(wi.TotalHours), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeatureHours

			,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency' and wi.StoryPoints > 0) as ContingencyPoints
			,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency' and wi.StoryPoints > 0) as ContingencyCount
			,		( select isnull(sum(wi.TotalHours), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency') as ContingencyHours
		
			,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectPoints
			,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectCount
			,		( select isnull(sum(wi.TotalHours), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectHours

			,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectPoints
			,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectCount
			,		( select isnull(sum(wi.TotalHours), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectHours

			,		( select (isnull(sum(te.DurationMinutesOverride), 0.0) / 60.0) from dbo.TimeEntries te where te.ReleaseNumber = r.ReleaseNumber) as TotalBilledHours

			from	dbo.Releases r
		)

	select	rs.ReleaseNumber
	,		rs.Notes
	,		rs.StartDate
	,		rs.EndDate
	,		rs.WeeksInRelease

	,		rs.ChoreCount
	,		rs.ChorePoints
	,		rs.ChoreHours

	,		rs.FeatureCount
	,		rs.FeaturePoints
	,		rs.FeatureHours

	,		rs.ContingencyCount
	,		rs.ContingencyPoints
	,		rs.ContingencyHours

	,		rs.LegacyDefectCount
	,		rs.LegacyDefectPoints
	,		rs.LegacyDefectHours

	,		rs.NewDefectCount
	,		rs.NewDefectPoints
	,		rs.NewDefectHours

	,		rs.TotalBilledHours
	,		(rs.ChoreCount + rs.FeatureCount) as ShippedFeatureAndChoreCount
	,		(rs.ChorePoints + rs.FeaturePoints) as ShippedFeatureAndChorePoints
	,		rs.ContingencyPoints as UnusedContingencyPoints
	,		case 
				when (rs.ChoreCount = 0 and rs.FeatureCount = 0) then 0 
				else (rs.ChoreHours + rs.FeatureHours + rs.NewDefectHours) / (rs.ChoreCount + rs.FeatureCount)
			end as AvgHoursPerFeatureAndChorePoint
	,		case 
				when (rs.ChoreCount = 0 and rs.FeatureCount = 0) then 0
				else rs.TotalBilledHours / (rs.ChoreCount + rs.FeatureCount)
			end as FullyLoadedAvgHoursPerPoint

	from	ReleaseSummary rs
