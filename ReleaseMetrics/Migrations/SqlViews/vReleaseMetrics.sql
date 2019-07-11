				drop view vReleaseMetrics
				go

				create view vReleaseMetrics as 
					with	
						ReleaseSummary as (
							select	r.ReleaseNumber
							,		r.Notes
							,		r.StartDate
							,		r.EndDate
							,		DateDiff(week, r.StartDate, r.EndDate) as WeeksInRelease

							,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChorePoints
							,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChoreCount
							,		( select isnull(sum(wi.TotalMinutes), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Chore') as ChoreMinutes

							,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeaturePoints
							,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeatureCount
							,		( select isnull(sum(wi.TotalMinutes), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Feature') as FeatureMinutes

							,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency' and wi.StoryPoints > 0) as ContingencyPoints
							,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency' and wi.StoryPoints > 0) as ContingencyCount
							,		( select isnull(sum(wi.TotalMinutes), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'Contingency') as ContingencyMinutes
		
							,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectPoints
							,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectCount
							,		( select isnull(sum(wi.TotalMinutes), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'LegacyDefect') as LegacyDefectMinutes

							,		( select isnull(sum(wi.StoryPoints), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectPoints
							,		( select count(*) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectCount
							,		( select isnull(sum(wi.TotalMinutes), 0) from vReleasedWorkItemSummary wi where wi.ReleaseNumber = r.ReleaseNumber and wi.Type = 'NewDefect') as NewDefectMinutes

							,		( select isnull(sum(te.DurationMinutesOverride), 0) from dbo.TimeEntries te where te.ReleaseNumber = r.ReleaseNumber) as TotalBilledMinutes

							from	dbo.Releases r
						)

					select	rs.ReleaseNumber
					,		rs.Notes
					,		rs.StartDate
					,		rs.EndDate
					,		rs.WeeksInRelease

					,		rs.ChoreCount
					,		rs.ChorePoints
					,		rs.ChoreMinutes

					,		rs.FeatureCount
					,		rs.FeaturePoints
					,		rs.FeatureMinutes

					,		rs.ContingencyCount
					,		rs.ContingencyPoints
					,		rs.ContingencyMinutes

					,		rs.LegacyDefectCount
					,		rs.LegacyDefectPoints
					,		rs.LegacyDefectMinutes

					,		rs.NewDefectCount
					,		rs.NewDefectPoints
					,		rs.NewDefectMinutes

					,		rs.TotalBilledMinutes
					,		(rs.ChoreCount + rs.FeatureCount) as ShippedFeatureAndChoreCount
					,		(rs.ChorePoints + rs.FeaturePoints) as ShippedFeatureAndChorePoints
					,		rs.ContingencyPoints as UnusedContingencyPoints
					,		((rs.ChoreMinutes + rs.FeatureMinutes + rs.NewDefectMinutes) / 60) / (rs.ChoreCount + rs.FeatureCount) as AvgHoursPerFeatureAndChorePoint
					,		(rs.TotalBilledMinutes / 60.0) / (rs.ChoreCount + rs.FeatureCount) as FullyLoadedAvgHoursPerPoint

					from	ReleaseSummary rs
