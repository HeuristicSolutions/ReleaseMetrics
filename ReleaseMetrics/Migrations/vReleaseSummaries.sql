	-- NOTE: This has started to diverge from the initial version. Haven't totally figured out the best way to version these things in EF Core.
	-- For now, I'm manually updating my local DB copy and keeping this up to date.
	

ALTER view [dbo].[vReleaseSummaries] as
	select	r.ReleaseNumber,
			r.ReleaseNumberSortable,
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
GO

