	-- NOTE: This has started to diverge from the initial version. Haven't totally figured out the best way to version these things in EF Core.
	-- For now, I'm manually updating my local DB copy and keeping this up to date.
	

ALTER view [dbo].[vReleaseMetrics] as 
	with	
		ReleaseSummary as (
			select	r.ReleaseNumber
			,		r.Notes
			,		r.StartDate
			,		r.EndDate
			,		DateDiff(week, r.StartDate, r.EndDate) as WeeksInRelease

			,		( select isnull(sum(wi.StoryPoints), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChorePoints
			,		( select count(*) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChoreCount
			,		( select isnull(sum(wi.TotalHours), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChoreHours

			,		( select isnull(sum(wi.StoryPoints), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type IN ('Feature', 'UITest')) as FeaturePoints
			,		( select count(*) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type IN ('Feature', 'UITest')) as FeatureCount
			,		( select isnull(sum(wi.TotalHours), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type IN ('Feature', 'UITest')) as FeatureHours

			,		( select isnull(sum(wi.StoryPoints), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectPoints
			,		( select count(*) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectCount
			,		( select isnull(sum(wi.TotalHours), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectHours
		
			,		( select isnull(sum(wi.StoryPoints), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectPoints
			,		( select count(*) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectCount
			,		( select isnull(sum(wi.TotalHours), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectHours

			,		( select isnull(sum(wi.StoryPoints), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency' and wi.StoryPoints > 0) as UnusedContingencyPoints
			,		( select isnull(sum(wi.TotalHours), 0) from vShippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency') as ContingencyHours

			,		( select isnull(sum(wi.TotalHours), 0) from vUnshippedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber) as UnshippedHours
			,		( select isnull(sum(te.DurationMinutesOverride - te.AllocatedMinutes) / 60, 0) from vTimeEntriesWithWorkAllocationIssues te where te.ReleaseNumber = r.ReleaseNumber) as DevTeamTotalOverheadHours
			,		( select (isnull(sum(te.DurationMinutesOverride), 0.0) / 60.0) from dbo.TimeEntries te where te.ReleaseNumber = r.ReleaseNumber) as TotalBilledHours
			from	dbo.Releases r
		)

	select	rs.ReleaseNumber
	,		rs.Notes
	,		rs.StartDate
	,		rs.EndDate
	,		rs.WeeksInRelease

	,		rs.ChoreCount
	,		rs.ChorePoints
	,		rs.ChoreHours

	,		rs.FeatureCount
	,		rs.FeaturePoints
	,		rs.FeatureHours

	,		rs.NewDefectCount
	,		rs.NewDefectPoints
	,		rs.NewDefectHours

	,		rs.LegacyDefectCount
	,		rs.LegacyDefectPoints
	,		rs.LegacyDefectHours

	,		rs.UnusedContingencyPoints
	,		rs.ContingencyHours

	,		(rs.ChoreCount + rs.FeatureCount) as ShippedFeatureAndChoreCount
	,		(rs.ChorePoints + rs.FeaturePoints) as ShippedFeatureAndChorePoints
	,		(rs.ChoreHours + rs.FeatureHours + rs.NewDefectHours) as ShippedFeatureAndChoreHours

	,		rs.UnshippedHours
	,		rs.DevTeamTotalOverheadHours
	,		rs.TotalBilledHours
	,		case 
				when (rs.ChoreCount = 0 and rs.FeatureCount = 0) then 0 
				else (rs.ChoreHours + rs.FeatureHours + rs.NewDefectHours) / (rs.ChorePoints + rs.FeaturePoints)
			end as AvgHoursPerFeatureAndChorePoint
	,		case 
				when (rs.LegacyDefectCount = 0) then 0 
				else (rs.LegacyDefectHours / rs.LegacyDefectCount)
			end as AvgHoursPerLegacyDefect
	,		case 
				when (rs.ChoreCount = 0 and rs.FeatureCount = 0) then 0 
				else (rs.DevTeamTotalOverheadHours) / (rs.ChorePoints + rs.FeaturePoints) 
			end as AvgDevTeamOverheadPerFeatureAndChorePoint
	,		case 
				when (rs.ChoreCount = 0 and rs.FeatureCount = 0) then 0
				else rs.TotalBilledHours / (rs.ChoreCount + rs.FeatureCount)
			end as FullyLoadedAvgHoursPerPoint
	from	ReleaseSummary rs;
GO


