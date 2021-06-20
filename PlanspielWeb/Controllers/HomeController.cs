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
		private readonly ShareRepository shares;

		//5.6 share value => 2 million bonus
		private readonly Dictionary<long, double> TeamShareAdjustments = new() {
			{ 76561199057017731, 5.6 },
			{ 76561199058089453, 5.6 },
			{ 76561199058175194, 11.2 },
			{ 76561199058091566, 11.2 },
			{ 76561199058068874, 5.6 },
			{ 76561199058137726, 11.2 },
			{ 76561199058220443, 11.2 },
			{ 76561199058009606, 11.2 },
			{ 76561199058042301, 11.2 },
			{ 76561198063402883, 5.6 }
		};

        public HomeController(TeamRepository teamRepository, SaveDataRepository saveDataRepository,
			ShareRepository shareRepository) {
            teams = teamRepository;
			saveData = saveDataRepository;
			shares = shareRepository;
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
				var shareValue = shares.GetCached(lastData);

				if (TeamShareAdjustments.TryGetValue(team.SteamID, out var adjustment))
					shareValue += adjustment;

                shareValues.Add(new ShareValuesViewModel {
                    TeamName = team.Name,
                    ShareValue = shareValue,
                    Balance = lastData?.Balance ?? 0,
                    TimeStamp = lastData?.IngameDate
                });
            }

            return View(shareValues.OrderByDescending(s => s.ShareValue));
        }
    }
}