drop view vReleasedWorkItemSummary
go

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
			and w.Type not in ('ArchitecturalIssue', 'Epic')
