using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanspielWeb.Attributes;
using PlanspielWeb.Models;

namespace PlanspielWeb.Controllers {
    public class HomeController : AppController {
        private readonly TeamRepository teams;
		private readonly SaveDataRepository saveData;

        public HomeController(TeamRepository teamRepository, SaveDataRepository saveDataRepository) {
            teams = teamRepository;
			saveData = saveDataRepository;
        }

        public IActionResult Index() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        public IActionResult ShareValues() {
            var shareValues = new List<ShareValuesViewModel>();

            foreach (var team in teams.GetAll()) {
				var teamData = saveData.GetForTeam(team.SteamID, int.MaxValue);
                if (teamData == null || !teamData.Any())
                    continue;

                var lastData = teamData.FirstOrDefault();
                shareValues.Add(new ShareValuesViewModel {
                    TeamName = team.Name,
                    ShareValue = lastData?.ShareValue ?? 0,
                    AvgShareValue = teamData.Average(d => d.ShareValue),
                    TimeStamp = lastData?.IngameDate
                });
            }

            return View(shareValues.OrderByDescending(s => s.AvgShareValue));
        }
    }
}