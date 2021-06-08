using System;
using DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DAL.Repositories;
using DAL.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Planspiel.Models;

namespace PlanspielWeb {
	public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson();

            var db = new Database("./db.sqlite");
            var sdr = new SaveDataRepository(db);
            var tr = new TeamRepository(db);
            var ur = new UserRepository(db, db.PasswordHasher);
            var sr = new ShareRepository(sdr);
			var nr = new NewsRepository(db);
			var tar = new TaskRepository(db);

            services.AddSingleton(db);
            services.AddSingleton(sdr);
            services.AddSingleton(tr);
            services.AddSingleton(ur);
            services.AddSingleton(sr);
			services.AddSingleton(nr);
			services.AddSingleton(tar);

            services.AddDistributedMemoryCache();
            services.AddSession(options => options.IdleTimeout = TimeSpan.FromDays(7));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            } else {
                app.UseForwardedHeaders(new ForwardedHeadersOptions {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseSession();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void AddTestData(TeamRepository tr, UserRepository ur, SaveDataRepository sdr) {
            for (int i = 0; i < 2; i++) {
                tr.AddOrIgnore(new Team { Name = "Team" + i, SteamID = i });
            }

            for (int i = 0; i < 4; i++) {
                ur.AddOrIgnore(new User {
                    Username = "User" + i,
                    Password = "pw" + i,
                    TeamId = (i % 2) + 1
                });
            }

            for (int i = 0; i < 6; i++) {
                sdr.AddOrIgnore(new SaveData {
                    SteamID = i % 2,
                    Date = new Date((i * 5 % 30) + 1, (i * i % 12) + 1, (i % 2) + 1),
                    Profit = i * 100,
                    CompanyValue = i * 200,
                    //DemandSatisfaction = i * 0.1d,
                    MachineUptime = i * 0.15d,
                    //AbleToPayLoansBack = (i % 2) == 0,
                    AveragePollution = i * 0.05,
                    BuildingCount = i * 5,
                    RegionCount = i % 2,
                    UnlockedResearchCount = (i * 10)
                });
            }
        }

        private void AddTestDataJoscha(TeamRepository tr, UserRepository ur) {
            tr.AddOrIgnore(new Team { Name = "Joschas Team", SteamID = 76561198043632379 });
            ur.AddOrIgnore(new User {
                Username = "joscha",
                Password = "pw",
                TeamId = 1
            });
        }
    }
}