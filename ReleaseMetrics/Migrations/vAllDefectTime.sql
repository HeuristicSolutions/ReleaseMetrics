	-- NOTE: This has started to diverge from the initial version. Haven't totally figured out the best way to version these things in EF Core.
	-- For now, I'm manually updating my local DB copy and keeping this up to date.
	

ALTER view [dbo].[vAllDefectTime] as 
	with	cteAllDefectTime as (
		select	WeekNum
		,		WorkItemId as StoryNumber
		,		Title
		,		TaskTitleOverride
		,		Discipline
		,		DurationMinutes
		from	vTimeEntryWorkItemAllocations
		where	TaskTitleOverride like '%defect%'

		union

		select	WeekNum
		,		'<none>' as StoryNumber
		,		'' as Title
		,		TaskTitleOverride
		,		Discipline
		,		DurationMinutesOverride
		from	vTimeEntriesWithWorkAllocationSummary
		where	AllocatedMinutes = 0
				and TaskTitleOverride like '%defect%'
	)

	select	WeekNum
	,		StoryNumber
	,		Title as StoryTitle
	,		TaskTitleOverride as TaskTitle
	,		Discipline
	,		DurationMinutes
	from	cteAllDefectTime
GO


