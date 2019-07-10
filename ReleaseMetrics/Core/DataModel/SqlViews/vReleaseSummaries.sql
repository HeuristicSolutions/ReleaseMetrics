------------------------ Migration 1 ------------------------
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


------------------------ Migration 2 ------------------------
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
			) as TimeEntryCount
	from	dbo.Releases r
go
