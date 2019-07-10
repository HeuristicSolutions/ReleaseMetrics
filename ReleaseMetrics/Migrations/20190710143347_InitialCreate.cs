using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReleaseMetrics.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Releases",
                columns: table => new
                {
                    ReleaseNumber = table.Column<string>(maxLength: 25, nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.ReleaseNumber);
                });

            migrationBuilder.CreateTable(
                name: "TimeEntries",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    ReleaseNumber = table.Column<string>(maxLength: 25, nullable: true),
                    Discipline = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    ProjectIdOrig = table.Column<string>(maxLength: 50, nullable: true),
                    ProjectIdOverride = table.Column<string>(maxLength: 50, nullable: true),
                    ProjectTitleOrig = table.Column<string>(maxLength: 250, nullable: true),
                    ProjectTitleOverride = table.Column<string>(maxLength: 250, nullable: true),
                    TaskIdOrig = table.Column<string>(maxLength: 50, nullable: true),
                    TaskIdOverride = table.Column<string>(maxLength: 50, nullable: true),
                    TaskTitleOrig = table.Column<string>(maxLength: 250, nullable: true),
                    TaskTitleOverride = table.Column<string>(maxLength: 250, nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    DatePerformed = table.Column<DateTime>(nullable: false),
                    NotesOrig = table.Column<string>(nullable: true),
                    NotesOverride = table.Column<string>(nullable: true),
                    Billable = table.Column<bool>(nullable: false),
                    SourceRecordCreatedAt = table.Column<DateTime>(nullable: false),
                    SourceRecordUpdatedAt = table.Column<DateTime>(nullable: false),
                    LocallyCreatedAt = table.Column<DateTime>(nullable: false),
                    LocallyUpdatedAt = table.Column<DateTime>(nullable: false),
                    DurationMinutesOrig = table.Column<int>(nullable: false),
                    DurationMinutesOverride = table.Column<int>(nullable: false),
                    Ignore = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeEntries_Releases_ReleaseNumber",
                        column: x => x.ReleaseNumber,
                        principalTable: "Releases",
                        principalColumn: "ReleaseNumber",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkItems",
                columns: table => new
                {
                    StoryNumber = table.Column<string>(maxLength: 50, nullable: false),
                    ReleaseNumber = table.Column<string>(maxLength: 25, nullable: false),
                    EpicWorkItemId = table.Column<string>(maxLength: 50, nullable: true),
                    EpicName = table.Column<string>(maxLength: 250, nullable: true),
                    Title = table.Column<string>(maxLength: 250, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    StoryPointsOriginal = table.Column<int>(nullable: true),
                    StoryPoints = table.Column<int>(nullable: false),
                    BillToClient = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItems", x => x.StoryNumber);
                    table.ForeignKey(
                        name: "FK_WorkItems_Releases_ReleaseNumber",
                        column: x => x.ReleaseNumber,
                        principalTable: "Releases",
                        principalColumn: "ReleaseNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeEntryWorkItemAllocations",
                columns: table => new
                {
                    TimeEntryId = table.Column<string>(maxLength: 50, nullable: false),
                    WorkItemId = table.Column<string>(maxLength: 50, nullable: false),
                    DurationMinutes = table.Column<decimal>(type: "decimal(6,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeEntryWorkItemAllocations", x => new { x.TimeEntryId, x.WorkItemId });
                    table.ForeignKey(
                        name: "FK_TimeEntryWorkItemAllocations_TimeEntries_TimeEntryId",
                        column: x => x.TimeEntryId,
                        principalTable: "TimeEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeEntryWorkItemAllocations_WorkItems_WorkItemId",
                        column: x => x.WorkItemId,
                        principalTable: "WorkItems",
                        principalColumn: "StoryNumber",
                        onDelete: ReferentialAction.Cascade);
                });

			migrationBuilder.InsertData(
				table: "Releases",
				columns: new[] { "ReleaseNumber", "EndDate", "Notes", "StartDate" },
				values: new object[] { "9.2.0", new DateTime(2018, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quick tunaround release specific to GSX. Added Pearson VUE integration. Limited regression test. Missed the original expected release date by 1 week for stabilization; contributing factors were weak up-front technical analysis that failed to identify complexity in the API calls and Mike/Brad's relative inexperience with the product and team.", new DateTime(2018, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });

			migrationBuilder.InsertData(
				table: "Releases",
				columns: new[] { "ReleaseNumber", "EndDate", "Notes", "StartDate" },
				values: new object[] { "9.3.0", new DateTime(2018, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Multi-client release. Primary focus was Vouchers for ABPANC. Also included the \"call external API\" behavior (ABPANC), Tenant-specific dashboards (GSX), tweaks for 3rd party payment details (DCOPLA), and minor R&D enhancements. Included a full regression test. Spanned LB Academy.", new DateTime(2018, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) });

			migrationBuilder.InsertData(
				table: "Releases",
				columns: new[] { "ReleaseNumber", "EndDate", "Notes", "StartDate" },
				values: new object[] { "9.4.0", new DateTime(2018, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Primary focus was prevention of duplicate SSNs for DCOPLA. Also included some R&D improvements to the Automations system and the ability to bulk load Intrinsic Attributes in the Workflow Attribute Retrieval Service (part of performance improvement long-term plan). Released as scheduled following a full regression test, but development team was about two weeks ahead of schedule and started the 9.5 features early.", new DateTime(2018, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) });

			migrationBuilder.InsertData(
				table: "Releases",
				columns: new[] { "ReleaseNumber", "EndDate", "Notes", "StartDate" },
				values: new object[] { "9.5.0", new DateTime(2018, 9, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cert Printing, Activity Copy features for NCBATE, USAePay, Lock accounts after N days", new DateTime(2018, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) });

			migrationBuilder.InsertData(
				table: "Releases",
				columns: new[] { "ReleaseNumber", "EndDate", "Notes", "StartDate" },
				values: new object[] { "9.6.0", new DateTime(2018, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Timed Assements for ABO", new DateTime(2018, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

			migrationBuilder.InsertData(
				table: "Releases",
				columns: new[] { "ReleaseNumber", "EndDate", "Notes", "StartDate" },
				values: new object[] { "9.7.0", new DateTime(2019, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Learning Plan Instance data type, for ASME", new DateTime(2018, 11, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) });

			migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_Id",
                table: "TimeEntries",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_ReleaseNumber",
                table: "TimeEntries",
                column: "ReleaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntryWorkItemAllocations_WorkItemId",
                table: "TimeEntryWorkItemAllocations",
                column: "WorkItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_ReleaseNumber",
                table: "WorkItems",
                column: "ReleaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_StoryNumber",
                table: "WorkItems",
                column: "StoryNumber");
			
			migrationBuilder.Sql(@"
				create view vReleaseSummaries as
					select	r.ReleaseNumber,
							r.StartDate,
							r.EndDate,
							r.Notes,
							(
								select	IsNull(sum(wi.StoryPoints), 0)
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
			");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"
				drop view vReleaseSummaries
			");

			migrationBuilder.DropTable(
                name: "TimeEntryWorkItemAllocations");

            migrationBuilder.DropTable(
                name: "TimeEntries");

            migrationBuilder.DropTable(
                name: "WorkItems");

            migrationBuilder.DropTable(
                name: "Releases");
        }
    }
}
