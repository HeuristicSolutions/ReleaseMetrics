-- NOTE: This has started to diverge from the initial version. Haven't totally figured out the best way to version these things in EF Core.
-- For now, I'm manually updating my local DB copy and keeping this up to date.


-- This only counts time to work items that are tagged to the release, but were not shipped.
-- It does NOT include time billed to a work item that is NOT tagged to the release (e.g. something
-- that got pushed out to a different release, or put back in the backlog after doing a spike)
--
-- SEE ALSO: vTimeEntriesForWorkItemsNotInRelease, which covers the items NOT tagged to the release
ALTER view [dbo].[vUnshippedWorkItemSummary] as
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
				or w.Type in ('ArchitecturalIssue', 'Epic')
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
GO


