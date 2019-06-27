using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.Helpers {

	public static class SqlServerModelBuilderExtensions {

		public static PropertyBuilder<decimal?> HasPrecision(this PropertyBuilder<decimal?> builder, int precision, int scale) {
			return builder.HasColumnType($"decimal({precision},{scale})");
		}

		public static PropertyBuilder<decimal> HasPrecision(this PropertyBuilder<decimal> builder, int precision, int scale) {
			return builder.HasColumnType($"decimal({precision},{scale})");
		}
	}
}
