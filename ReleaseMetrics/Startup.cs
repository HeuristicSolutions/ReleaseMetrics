using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReleaseMetrics.Core.Configuration;
using ReleaseMetrics.Core.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Converters;

namespace ReleaseMetrics {

	public class Startup {

		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.Configure<CookiePolicyOptions>(options => {
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services
				.AddMvc()
					.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
					.AddJsonOptions(opt => {
						opt.SerializerSettings.Converters.Add(new StringEnumConverter());
						opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
					});

			var connString = Configuration.GetConnectionString("SQLConnection");
			services.AddDbContext<MetricsDbContext>(
				options => options
					.UseLazyLoadingProxies()
					.UseSqlServer(connString)
					.EnableSensitiveDataLogging()	// TODO: remove in PROD
			);

			var appSettings = new AppSettings();
			Configuration.Bind("AppSettings", appSettings);
			services.AddSingleton(appSettings);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}
			else {
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseMvc();
			
			// HACK: Temporarily expose node_modules via "/vendor/" for development. TODO: Replace this w/ a Gulp implementation
			// that copies just the necessary files into wwwroot
			if (env.IsDevelopment()) {
				app.UseStaticFiles(new StaticFileOptions() {
					FileProvider = new PhysicalFileProvider(
					  Path.Combine(Directory.GetCurrentDirectory(), @"node_modules")),
					RequestPath = new PathString("/vendor")
				});
			}
		}
	}
}
