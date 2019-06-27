using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.Configuration
{
    public class AppSettings {
		public JiraApiSettings JiraApiSettings { get; set; }
		public MavenlinkApiSettings MavenlinkApiSettings { get; set; }
	}
}
