using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.Helpers {

	public static class Extensions {

		public static string ToIso8601(this DateTime d) {
			return d.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string ToJsonIndentedFormat(this string json) {
			var parsedJson = JToken.Parse(json);
			return parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);
		}
	}
}
