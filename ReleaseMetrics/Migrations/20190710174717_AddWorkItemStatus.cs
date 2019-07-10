using Microsoft.EntityFrameworkCore.Migrations;

namespace ReleaseMetrics.Migrations
{
    public partial class AddWorkItemStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "WorkItems",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "WorkItemId",
                table: "TimeEntryWorkItemAllocations",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "TimeEntryId",
                table: "TimeEntryWorkItemAllocations",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250);

			migrationBuilder.Sql(@"
				drop view vReleaseSummaries;
				go

				create view vReleaseSummaries as
					select	r.ReleaseNumber,
							r.StartDate,
							r.EndDate,
							r.Notes,
							(
								select	isnull(sum(wi.StoryPoints), 0)
								from	dbo.WorkItems wi
								where	wi.ReleaseNumber = r.ReleaseNumber
										and wi.Type in ('Chore', 'Contingency', 'Feature')
										and wi.Status = 'Shipped'
							) as FeatureAndChorePoints,
							(
								select	isnull(count(*), 0)
								from	dbo.WorkItems wi
								where	wi.ReleaseNumber = r.ReleaseNumber
										and wi.Type in ('Chore', 'Contingency', 'Feature')
										and wi.Status = 'Shipped'
							) as FeatureAndChoreCount,
							(
								select	isnull(count(*), 0)
								from	dbo.TimeEntries te
								where	te.ReleaseNumber = r.ReleaseNumber
							) as TimeEntryCount
					from	dbo.Releases r
			");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "WorkItems");

            migrationBuilder.AlterColumn<string>(
                name: "WorkItemId",
                table: "TimeEntryWorkItemAllocations",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "TimeEntryId",
                table: "TimeEntryWorkItemAllocations",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }
    }
}
