using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.DataModel.EfCoreExtensions
{
	/// <summary>
	/// Set a default value defined on the sql server
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public class SqlDefaultValueAttribute : Attribute
	{
		public string DefaultValue { get; set; }

		public SqlDefaultValueAttribute(string value) {
			DefaultValue = value;
		}
	}

	public static class SqlDefaultValueAttributeConvention {

		public static void Apply(ModelBuilder builder)
		{
			ConventionBehaviors.SetSqlValueForPropertiesWithAttribute<SqlDefaultValueAttribute>(builder, x => x.DefaultValue);
		}
	}
}
