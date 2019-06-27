﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReleaseMetrics.Core.DataModel;

namespace ReleaseMetrics.Migrations
{
    [DbContext(typeof(MetricsDbContext))]
    [Migration("20180817155730_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ReleaseMetrics.Core.DataModel.Release", b =>
                {
                    b.Property<string>("ReleaseNumber");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Notes");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("ReleaseNumber");

                    b.ToTable("Releases");

                    b.HasData(
                        new { ReleaseNumber = "9.2.0", EndDate = new DateTime(2018, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), Notes = "Quick tunaround release specific to GSX. Added Pearson VUE integration. Limited regression test. Missed the original expected release date by 1 week for stabilization; contributing factors were weak up-front technical analysis that failed to identify complexity in the API calls and Mike/Brad's relative inexperience with the product and team.", StartDate = new DateTime(2018, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                        new { ReleaseNumber = "9.3.0", EndDate = new DateTime(2018, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), Notes = "Multi-client release. Primary focus was Vouchers for ABPANC. Also included the \"call external API\" behavior (ABPANC), Tenant-specific dashboards (GSX), tweaks for 3rd party payment details (DCOPLA), and minor R&D enhancements. Included a full regression test. Spanned LB Academy.", StartDate = new DateTime(2018, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                        new { ReleaseNumber = "9.4.0", EndDate = new DateTime(2018, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), Notes = "Primary focus was prevention of duplicate SSNs for DCOPLA. Also included some R&D improvements to the Automations system and the ability to bulk load Intrinsic Attributes in the Workflow Attribute Retrieval Service (part of performance improvement long-term plan). Released as scheduled following a full regression test, but development team was about two weeks ahead of schedule and started the 9.5 features early.", StartDate = new DateTime(2018, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                    );
                });

            modelBuilder.Entity("ReleaseMetrics.Core.DataModel.TimeEntry", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Billable");

                    b.Property<DateTime>("DatePerformed");

                    b.Property<int>("DurationMinutes");

                    b.Property<bool>("Ignore");

                    b.Property<DateTime>("LocallyCreatedAt");

                    b.Property<DateTime>("LocallyUpdatedAt");

                    b.Property<string>("Notes");

                    b.Property<string>("ProjectId");

                    b.Property<string>("ProjectTitle");

                    b.Property<string>("ReleaseNumber");

                    b.Property<DateTime>("SourceRecordCreatedAt");

                    b.Property<DateTime>("SourceRecordUpdatedAt");

                    b.Property<string>("TaskId");

                    b.Property<string>("TaskTitle");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("ReleaseNumber");

                    b.ToTable("TimeEntries");
                });

            modelBuilder.Entity("ReleaseMetrics.Core.DataModel.TimeEntryWorkItemAllocation", b =>
                {
                    b.Property<string>("TimeEntryId");

                    b.Property<string>("WorkItemId");

                    b.Property<decimal>("DurationMinutes")
                        .HasColumnType("decimal(6,3)");

                    b.HasKey("TimeEntryId", "WorkItemId");

                    b.HasIndex("WorkItemId");

                    b.ToTable("TimeEntryWorkItemAllocations");
                });

            modelBuilder.Entity("ReleaseMetrics.Core.DataModel.WorkItem", b =>
                {
                    b.Property<string>("StoryNumber")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BillToClient")
                        .IsRequired();

                    b.Property<string>("EpicName");

                    b.Property<string>("EpicWorkItemId");

                    b.Property<string>("ReleaseNumber")
                        .IsRequired();

                    b.Property<int>("StoryPoints");

                    b.Property<int?>("StoryPointsOriginal");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.HasKey("StoryNumber");

                    b.HasIndex("ReleaseNumber");

                    b.HasIndex("StoryNumber");

                    b.ToTable("WorkItems");
                });

            modelBuilder.Entity("ReleaseMetrics.Core.DataModel.TimeEntry", b =>
                {
                    b.HasOne("ReleaseMetrics.Core.DataModel.Release", "Release")
                        .WithMany("TimeEntries")
                        .HasForeignKey("ReleaseNumber");
                });

            modelBuilder.Entity("ReleaseMetrics.Core.DataModel.TimeEntryWorkItemAllocation", b =>
                {
                    b.HasOne("ReleaseMetrics.Core.DataModel.WorkItem", "WorkItem")
                        .WithMany("TimeEntries")
                        .HasForeignKey("TimeEntryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ReleaseMetrics.Core.DataModel.TimeEntry", "TimeEntry")
                        .WithMany("WorkItems")
                        .HasForeignKey("WorkItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ReleaseMetrics.Core.DataModel.WorkItem", b =>
                {
                    b.HasOne("ReleaseMetrics.Core.DataModel.Release", "Release")
                        .WithMany("WorkItems")
                        .HasForeignKey("ReleaseNumber")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}