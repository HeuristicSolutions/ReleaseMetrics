-- vReleaseSummaries
if exists(select 1 from sys.views where name='vReleaseSummaries' and type='v')
drop view vReleaseSummaries;
go

-- vReleasedWorkItemSummary
if exists(select 1 from sys.views where name='vReleasedWorkItemSummary' and type='v')
drop view vReleasedWorkItemSummary;
go

-- vReleaseMetrics
if exists(select 1 from sys.views where name='vReleaseMetrics' and type='v')
drop view vReleaseMetrics;
go
