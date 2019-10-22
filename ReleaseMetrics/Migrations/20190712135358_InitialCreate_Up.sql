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
						and Ignore = 0
			) as TimeEntryCount
	from	dbo.Releases r
go

-- vShippedWorkItemSummary
if exists(select 1 from sys.views where name='vShippedWorkItemSummary' and type='v')
drop view vShippedWorkItemSummary;
go
create view vShippedWorkItemSummary as
	with ShippedStories as (
		select	w.StoryNumber
		,		w.Type
		,		w.Title
		,		w.ReleaseNumber
		,		w.Status
		,		w.EpicWorkItemId
		,		w.StoryPoints
		,		w.BillToClient
		,		case w.StoryPoints
					when 0 then 0
					when 1 then 2
					when 2 then 4
					when 4 then 8
					when 8 then 12
					else 0
				end as ExpectedHours_Min
		,		case w.StoryPoints
					when 0 then 3
					when 1 then 10
					when 2 then 20
					when 4 then 40
					when 8 then 80
					else 100
				end as ExpectedHours_Max
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Ignore = 0
				) as TotalHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'Dev'
							and te.Ignore = 0
				) as DevHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'QA'
							and te.Ignore = 0
				) as QaHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'UIUX'
							and te.Ignore = 0
				) as UiUxHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'Other'
							and te.Ignore = 0
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
	,		case
				when s.Type in ('Feature', 'Chore') and (s.TotalHours = 0) and (StoryPoints > 0) then 'No time billed to non-zero story'
				when s.Type in ('Feature', 'Chore') and (s.TotalHours < ExpectedHours_Min) then 'Billed hours LOWER than standard range'
				when s.Type in ('Feature', 'Chore') and (s.TotalHours > ExpectedHours_Max) then 'Billed hours GREATER than standard range'
				when s.Type in ('Contingency') and (s.TotalHours > 0) then 'Time billed directly to a Contingency case'
				else null
			end as BillingWarningMessage
	,		case when s.StoryPoints = 0 then s.TotalHours else s.TotalHours / s.StoryPoints end as HoursPerPoint
	,		s.DevHours
	,		s.QaHours
	,		s.UiUxHours
	,		s.OtherHours
	from	ShippedStories s
go

-- vUnshippedWorkItemSummary
if exists(select 1 from sys.views where name='vUnshippedWorkItemSummary' and type='v')
drop view vUnshippedWorkItemSummary;
go
create view vUnshippedWorkItemSummary as
	with UnshippedStories as (
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
							and te.Ignore = 0
				) as TotalHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'Dev'
							and te.Ignore = 0
				) as DevHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'QA'
							and te.Ignore = 0
				) as QaHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'UIUX'
							and te.Ignore = 0
				) as UiUxHours
		,		(
					select	isnull(sum(tea.DurationMinutes), 0) / 60.0
					from	dbo.TimeEntryWorkItemAllocations tea
							inner join dbo.TimeEntries te on tea.TimeEntryId = te.Id
					where	tea.WorkItemId = w.StoryNumber
							and te.Discipline = 'Other'
							and te.Ignore = 0
				) as OtherHours
		from	dbo.WorkItems w
		where	w.Status <> 'Shipped'
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
	,		s.DevHours
	,		s.QaHours
	,		s.UiUxHours
	,		s.OtherHours
	from	UnshippedStories s
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

			,		( select isnull(sum(wi.StoryPoints), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChorePoints
			,		( select count(*) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChoreCount
			,		( select isnull(sum(wi.TotalHours), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChoreHours

			,		( select isnull(sum(wi.StoryPoints), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeaturePoints
			,		( select count(*) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeatureCount
			,		( select isnull(sum(wi.TotalHours), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeatureHours

			,		( select isnull(sum(wi.StoryPoints), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectPoints
			,		( select count(*) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectCount
			,		( select isnull(sum(wi.TotalHours), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectHours
		
			,		( select isnull(sum(wi.StoryPoints), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectPoints
			,		( select count(*) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectCount
			,		( select isnull(sum(wi.TotalHours), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectHours

			,		( select isnull(sum(wi.StoryPoints), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency' and wi.StoryPoints > 0) as UnusedContingencyPoints
			,		( select isnull(sum(wi.TotalHours), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency') as ContingencyHours

			,		( select isnull(sum(wi.TotalHours), 0) from vUnshippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber) as UnshippedHours
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

	,		rs.NewDefectCount
	,		rs.NewDefectPoints
	,		rs.NewDefectHours

	,		rs.LegacyDefectCount
	,		rs.LegacyDefectPoints
	,		rs.LegacyDefectHours

	,		rs.UnusedContingencyPoints
	,		rs.ContingencyHours

	,		(rs.ChoreCount + rs.FeatureCount) as ShippedFeatureAndChoreCount
	,		(rs.ChorePoints + rs.FeaturePoints) as ShippedFeatureAndChorePoints
	,		(rs.ChoreHours + rs.FeatureHours + rs.NewDefectHours) as ShippedFeatureAndChoreHours

	,		rs.UnshippedHours
	,		rs.TotalBilledHours
	,		case 
				when (rs.ChoreCount = 0 and rs.FeatureCount = 0) then 0 
				else (rs.ChoreHours + rs.FeatureHours + rs.NewDefectHours) / (rs.ChorePoints + rs.FeaturePoints)
			end as AvgHoursPerFeatureAndChorePoint
	,		case 
				when (rs.LegacyDefectCount = 0) then 0 
				else (rs.LegacyDefectHours / rs.LegacyDefectCount)
			end as AvgHoursPerLegacyDefect
	,		case 
				when (rs.ChoreCount = 0 and rs.FeatureCount = 0) then 0
				else rs.TotalBilledHours / (rs.ChoreCount + rs.FeatureCount)
			end as FullyLoadedAvgHoursPerPoint
	from	ReleaseSummary rs;
go

-- vTimeEntriesWithWorkAllocationIssues
if exists(select 1 from sys.views where name='vTimeEntriesWithWorkAllocationIssues' and type='v')
drop view vTimeEntriesWithWorkAllocationIssues;
go
create view vTimeEntriesWithWorkAllocationIssues as 
	with	
		TimeEntrySummary as (
			select	te.id
			,		te.ReleaseNumber
			,		te.DatePerformed
			,		te.UserName
			,		te.Discipline
			,		te.ProjectTitleOverride
			,		te.TaskTitleOverride
			,		te.NotesOverride
			,		te.DurationMinutesOverride
			,		sum(IsNull(wa.DurationMinutes, 0)) as AllocatedMinutes
			from	dbo.TimeEntries te
					left outer join dbo.TimeEntryWorkItemAllocations wa on te.Id = wa.TimeEntryId
			where	te.Ignore = 0
			group	by te.Id
			,		te.ReleaseNumber
			,		te.DatePerformed
			,		te.UserName
			,		te.Discipline
			,		te.ProjectTitleOverride
			,		te.TaskTitleOverride
			,		te.NotesOverride
			,		te.DurationMinutesOverride
		)

	select	*
	from	TimeEntrySummary
	where	AllocatedMinutes != DurationMinutesOverride
			and Discipline != 'Other'
go