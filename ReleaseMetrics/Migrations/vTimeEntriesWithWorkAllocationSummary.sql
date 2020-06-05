	-- NOTE: This has started to diverge from the initial version. Haven't totally figured out the best way to version these things in EF Core.
	-- For now, I'm manually updating my local DB copy and keeping this up to date.
	

ALTER view [dbo].[vTimeEntriesWithWorkAllocationSummary] as 
	with	
		TimeEntrySummary as (
			select	te.id
			,		te.ReleaseNumber
			,		te.DatePerformed
			,		CONVERT(varchar(4), DatePart(yyyy, te.DatePerformed)) + '-' + RIGHT('00' + CAST(DatePart(wk, te.DatePerformed) as nvarchar(2)), 2) as WeekNum
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
GO


