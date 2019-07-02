using Heuristics.Library.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.WorkItems.JiraApi;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace ReleaseMetrics.Core.WorkItems {

	/// <summary>
	/// Handles ETL tasks dealing with Jira. Jira story data is cached in the local database to serve as a historical record and data cache.
	/// 
	/// Unlike the Mavenlink cache, locally cached Jira stories should be READONLY. We support local modification of time data
	/// because modifying it in the source system post-invoicing is difficult. We don't have those issues w/ Jira, so if a problem
	/// is found with a Jira story it should be fixed in Jira and then re-loaded locally.
	/// </summary>
	public class JiraLoader {

		private MetricsDbContext Database { get; set; }
		private JiraApiProxy ApiProxy { get; set; }

		public JiraLoader(MetricsDbContext db, JiraApiProxy apiProxy) {
			this.Database = db;
			this.ApiProxy = apiProxy;
		}

		/// <summary>
		/// Given a release number, refreshes the local database of work items for that release from Jira. Returns
		/// the new set of stories.
		/// 
		/// All existing local work items tied to the release are removed and replaced; unlike time data, which
		/// can be hard to modify once a timesheet is approved, we do not expect to make local modifications to the 
		/// story data.
		/// </summary>
		public async Task<(List<WorkItem> workItems, List<ResultMsg> messages)> RefreshLocalJiraStoryCacheForReleaseAsync(string releaseNum) {
			var storiesInRelease = await ApiProxy.GetStoriesInReleaseAsync(releaseNum);
			var messages = new List<ResultMsg>();
			var workItems = new List<WorkItem>();

			foreach (var story in storiesInRelease) {
				// TODO: To start tracking metrics for patch releases, will need to make this smarter 
				var effectiveMajorRelease = GetPrimaryMajorRelease(story.FixVersions);

				if (effectiveMajorRelease.IsNullOrEmpty()) {
					messages.Add(
						new ResultMsg($"Found 0 major release versions for {story.Id}, with fix versions '{story.FixVersions.Join(", ")}'", MessageType.Error)
					);
					continue;
				}
				else if (effectiveMajorRelease != releaseNum) {
					messages.Add(
						new ResultMsg($"{story.Id} is mapped to {releaseNum}, but its primary major release is {effectiveMajorRelease}. Jira stories should only be tracked to a single major release version.", MessageType.Warning)
					);
					continue;
				}

				try {
					var epic = story.EpicStoryId.IsNotNullOrEmpty()
						? await ApiProxy.GetStoryAsync(story.EpicStoryId)
						: null;

					var newWorkItem = new WorkItem(releaseNum, story, epic);
					workItems.Add(newWorkItem);
				}
				catch (Exception ex) {
					messages.Add(
						new ResultMsg($"Error processing {releaseNum}: {story.Id}: {ex.Message}", MessageType.Error)
					);
				}
			}

			// clear all existing work items for the release
			Database.WorkItems
				.Where(x => x.ReleaseNumber == releaseNum)
				.Delete();

			// add all the new ones
			try {
				workItems.ForEach(
					x => Database.WorkItems.Add(x)
				);
				Database.SaveChanges();
			}
			catch (DbUpdateException ex) {
				messages.Add(
					new ResultMsg($"Error processing {releaseNum}: While saving changes: {ex.InnerException.Message}", MessageType.Error)
				);
			}
			catch (Exception ex) {
				messages.Add(
					new ResultMsg($"Error processing {releaseNum}: While saving changes: {ex.Message}", MessageType.Error)
				);
			}

			return (workItems, messages);
		}

		/// <summary>
		/// stories should be tagged to at most 1 major release (x.y.0). If a story is tagged to multiple, 
		/// we assign it to the EARLIEST version under the assumption that anything in x.y.0 is also in x.(y+1).0
		/// </summary>
		private string GetPrimaryMajorRelease(List<string> fixVersions) {
			var candidateMajorReleases = fixVersions.Where(v => v.EndsWith(".0")).ToList();

			if (candidateMajorReleases.Count == 0) {
				return null;
			}

			else if (candidateMajorReleases.Count == 1) {
				return candidateMajorReleases[0];
			}

			else {
				var minMarketingReleaseNum = Int32.MaxValue;
				var minMajorReleaseNum = Int32.MaxValue;

				foreach (var releaseNum in candidateMajorReleases) {
					try {
						var versionComponents = releaseNum.Split(".");
						var marketingRelease = versionComponents[0].ToInt32();
						var majorRelease = versionComponents[1].ToInt32();

						// 3.1.0 is earlier than 4.9.0
						if (marketingRelease < minMarketingReleaseNum) {
							minMarketingReleaseNum = marketingRelease;
							minMajorReleaseNum = majorRelease;
						}
						// 3.1.0 is earlier than 3.2.0
						else if (marketingRelease == minMarketingReleaseNum && majorRelease < minMajorReleaseNum) {
							minMajorReleaseNum = majorRelease;
						}
					}
					catch (Exception ex) {
						throw new Exception($"Error parsing '{releaseNum}': " + ex.Message, ex);
					}
				}

				return $"{minMarketingReleaseNum}.{minMajorReleaseNum}.0";
			}
		}
	}
}
