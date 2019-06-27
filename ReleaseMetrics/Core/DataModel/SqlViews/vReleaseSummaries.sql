create view vReleaseSummaries as
	select	r.ReleaseNumber,
			r.StartDate,
			r.EndDate,
			r.Notes,
			(
				select	sum(wi.StoryPoints)
				from	dbo.WorkItems wi
				where	wi.ReleaseNumber = r.ReleaseNumber
			) as TotalPoints,
			(
				select	count(*)
				from	dbo.WorkItems wi
				where	wi.ReleaseNumber = r.ReleaseNumber
			) as WorkItemCount,
			(
				select	count(*)
				from	dbo.TimeEntries te
				where	te.ReleaseNumber = r.ReleaseNumber
			) as TimeEntryCount
	from	dbo.Releases r
go