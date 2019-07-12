using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ReleaseMetrics.Migrations {
	public static class MigrationHelper {

		public static void ExecuteCustomSql(MigrationBuilder migrationBuilder, string filename) {
			var asm = Assembly.GetExecutingAssembly();
			var resource = typeof(MigrationHelper).Namespace + "." + filename;
			using (var stream = asm.GetManifestResourceStream(resource)) {
				if (stream != null) {
					var reader = new StreamReader(stream);
					var fileText = reader.ReadToEnd();
					migrationBuilder.Sql(fileText);
				}
				else {
					throw new Exception($"Could not find embedded resource for '{filename}' using resource '{resource}'");
				}
			}
		}
	}
}
