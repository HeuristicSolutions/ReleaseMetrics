-- NOTE: This has started to diverge from the initial version. Haven't totally figured out the best way to version these things in EF Core.
-- For now, I'm manually updating my local DB copy and keeping this up to date.


ALTER view [dbo].[vTimeEntriesForWorkItemsNotInRelease] as 

	-- Summaries time entries for stories not tagged to the release (and therefore won't have a valid work item).
	-- SEE ALSO: vUnshippedWorkItemSummary, which shows the work items that ARE tagged to the release, but didn't
	-- actually ship. These two views catch two different sides of the same coin.
	select * from vTimeEntriesWithWorkAllocationIssues 
	where 
		NotesOverride like '%LB-%'						-- "Unshipped" means time billed to a story that didn't actually ship in the release
		and TaskTitleOverride not like '%Defect%'		-- We're counting triage as "defect", not "unshipped"
		and TaskTitleOverride not like '%Overhead%'		-- Time is often billed to an epic as overhead. That doesn't count as "unshipped"
		and AllocatedMinutes = 0						-- 0 allocated minutes mean there was no valid work item to bill to.
GO


