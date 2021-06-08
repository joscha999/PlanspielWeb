using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PlanspielWeb
{
    public static class Program
    {
		//https://www.c-sharpcorner.com/article/creating-charts-with-asp-net-core/

		public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
					var urls = new List<string>();
					urls.Add("http://roi.jgdev.de:5001/");

#if DEBUG
					urls.Add("http://localhost:5001/");
#endif

					webBuilder.UseUrls(urls.ToArray());
                });
    }
}
