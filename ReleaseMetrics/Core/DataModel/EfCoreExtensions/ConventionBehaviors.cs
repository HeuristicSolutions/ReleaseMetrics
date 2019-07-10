using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core.DataModel.EfCoreExtensions
{
	public static class ConventionBehaviors {

		public static void SetTypeForPropertiesWithAttribute<TAttribute>(ModelBuilder builder, Func<TAttribute, string> lambda) where TAttribute : class {
			SetPropertyValue<TAttribute>(builder).ForEach((x) =>
			{
				x.Item1.Relational().ColumnType = lambda(x.Item2);
			});
		}

		public static void SetSqlValueForPropertiesWithAttribute<TAttribute>(ModelBuilder builder, Func<TAttribute, string> lambda) where TAttribute : class {
			SetPropertyValue<TAttribute>(builder).ForEach((x) =>
			{
				x.Item1.Relational().DefaultValueSql = lambda(x.Item2);
			});
		}

		private static List<Tuple<IMutableProperty, TAttribute>> SetPropertyValue<TAttribute>(ModelBuilder builder) where TAttribute : class {
			var propsToModify = new List<Tuple<IMutableProperty, TAttribute>>();

			foreach (var entity in builder.Model.GetEntityTypes()) {
				var properties = entity.GetProperties();

				foreach (var property in properties) {
					var attribute = property.PropertyInfo
						.GetCustomAttributes(typeof(TAttribute), false)
						.FirstOrDefault() as TAttribute;

					if (attribute != null) {
						propsToModify.Add(new Tuple<IMutableProperty, TAttribute>(property, attribute));
					}
				}
			}

			return propsToModify;
		}
	}
}
