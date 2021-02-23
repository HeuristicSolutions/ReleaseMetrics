	-- NOTE: This has started to diverge from the initial version. Haven't totally figured out the best way to version these things in EF Core.
	-- For now, I'm manually updating my local DB copy and keeping this up to date.
	

ALTER view [dbo].[vFundingSourceByRelease] as 

	with	cteFundingSources (ReleaseNumber, EndDate, BillTo, StoryPoints, TotalHours)
	as		(
				select	r.ReleaseNumber, 
						r.EndDate, 
						CASE (BillToClient) WHEN 'R&D' THEN 'R&D' else 'Client' END as BillTo,
						StoryPoints,
						TotalHours
				from	vShippedWorkItemSummary
						inner join vReleaseSummaries r on vShippedWorkItemSummary.ReleaseNumber = r.ReleaseNumber
	)

	select	ReleaseNumber, BillTo, sum(StoryPoints) as TotalPoints, sum(TotalHours) as TotalHours, EndDate
	from	cteFundingSources
	group	by ReleaseNumber, EndDate, BillTo

GO


