use ReleaseMetrics

create view vOverheadTimeByReleaseAndUser as 

select	te.ReleaseNumber, r.ReleaseNumberSortable, te.Discipline, te.Username, sum(te.durationminutesoverride) as TotalMinutes
from	TimeEntries te
		inner join Releases r on te.ReleaseNumber = r.ReleaseNumber
where	te.TaskTitleOverride like '%overhead%'
group	by te.ReleaseNumber, r.ReleaseNumberSortable, te.Discipline, te.UserName
--order	by r.ReleaseNumberSortable, te.Discipline, te.UserName