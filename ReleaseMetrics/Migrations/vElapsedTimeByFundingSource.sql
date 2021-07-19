	-- NOTE: This has started to diverge from the initial version. Haven't totally figured out the best way to version these things in EF Core.
	-- For now, I'm manually updating my local DB copy and keeping this up to date.
	

ALTER view [dbo].[vElapsedTimeByFundingSource] as 

	with	cteElapsedTime (ReleaseNumber, ReleaseNumberSortable, EndDate, BillTo, TotalMinutes)
	as		(
				select	r.ReleaseNumber,
						r.ReleaseNumberSortable,
						r.EndDate, 
						CASE 
							WHEN ProjectTitleOverride = 'HS: LB R&D [Internal]' and TaskTitleOverride like '%defect%' then 'Defect'
							WHEN ProjectTitleOverride = 'HS: LB R&D [Internal]' then 'R&D Innovation'
							ELSE 'Client Innovation' END as BillTo,
						DurationMinutesOverride
				from	TimeEntries
						inner join vReleaseSummaries r on TimeEntries.ReleaseNumber = r.ReleaseNumber
	)

	select	ReleaseNumber, ReleaseNumberSortable, BillTo, sum(TotalMinutes) / 60 as TotalHours, EndDate
	from	cteElapsedTime
	group	by ReleaseNumber, ReleaseNumberSortable, EndDate, BillTo
GO


