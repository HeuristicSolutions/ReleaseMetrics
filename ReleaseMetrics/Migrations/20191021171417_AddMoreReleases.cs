using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReleaseMetrics.Migrations
{
    public partial class AddMoreReleases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

			migrationBuilder.InsertData(
				table: "Releases",
				columns: new[] { "ReleaseNumber", "EndDate", "Notes", "StartDate" },
				values: new object[] { "9.8.0", new DateTime(2019, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "NBCE Phase 1: Initial Offerings stub", new DateTime(2019, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) });

			migrationBuilder.InsertData(
				table: "Releases",
				columns: new[] { "ReleaseNumber", "EndDate", "Notes", "StartDate" },
				values: new object[] { "9.9.0", new DateTime(2019, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Automations performance improvements, mixed bag of other stuff", new DateTime(2019, 5, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) });

			migrationBuilder.InsertData(
				table: "Releases",
				columns: new[] { "ReleaseNumber", "EndDate", "Notes", "StartDate" },
				values: new object[] { "9.10.0", new DateTime(2019, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "NBCE Phase 2 (Offerings), NGLP release candidate, 508 Compliance", new DateTime(2019, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) });
		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
