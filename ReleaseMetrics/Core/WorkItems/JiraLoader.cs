using Heuristics.Library.Extensions;
using Newtonsoft.Json;
using ReleaseMetrics.Core.DataModel;
using ReleaseMetrics.Core.WorkItems.JiraApi;
using RestSharp;
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
		public async Task<List<WorkItem>> RefreshLocalJiraStoryCacheForReleaseAsync(string releaseNum) {
			var storiesInRelease = await ApiProxy.GetStoriesInReleaseAsync(releaseNum);
			var workItems = new List<WorkItem>();

			foreach (var story in storiesInRelease) {
				var epic = story.EpicStoryId.IsNotNullOrEmpty()
					? await ApiProxy.GetStoryAsync(story.EpicStoryId)
					: null;

				var newWorkItem = new WorkItem(releaseNum, story, epic);
				workItems.Add(newWorkItem);
			}

			// clear all existing work items for the release
			Database.WorkItems
				.Where(x => x.ReleaseNumber == releaseNum)
				.Delete();

			// add all the new ones
			workItems.ForEach(
				x => Database.WorkItems.Add(x)
			);
			Database.SaveChanges();

			return workItems;
		}
	}
}
