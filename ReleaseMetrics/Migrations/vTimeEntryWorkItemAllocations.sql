	-- NOTE: This has started to diverge from the initial version. Haven't totally figured out the best way to version these things in EF Core.
	-- For now, I'm manually updating my local DB copy and keeping this up to date.
	

ALTER view [dbo].[vTimeEntryWorkItemAllocations] as 
	with	cteTimeEntryWorkAllocations as (

			select	te.DatePerformed,
					CONVERT(varchar(4), DatePart(yyyy, te.DatePerformed)) + '-' + RIGHT('00' + CAST(DatePart(wk, te.DatePerformed) as nvarchar(2)), 2) as WeekNum,
					te.NotesOverride,
					te.Discipline,
					tewia.WorkItemId, 
					wi.Title,
					te.TaskTitleOverride,
					tewia.DurationMinutes
			from	TimeEntryWorkItemAllocations tewia
					inner join TimeEntries te on tewia.TimeEntryId = te.Id
					inner join WorkItems wi on tewia.WorkItemId = wi.StoryNumber
	)

	select	WeekNum,
			WorkItemId,
			Title,
			TaskTitleOverride,
			Discipline,
			DurationMinutes
	from	cteTimeEntryWorkAllocations

GO


