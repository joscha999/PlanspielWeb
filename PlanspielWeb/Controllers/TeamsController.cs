using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.Repositories;
using PlanspielWeb.Attributes;
using PlanspielWeb.Models;
using System.Collections.Generic;
using System.Linq;

namespace PlanspielWeb.Controllers {
    public class TeamsController : AppController {
        private readonly TeamRepository teams;
        private readonly SaveDataRepository saves;
		private readonly ShareRepository shares;

        public TeamsController(TeamRepository teamRepository, SaveDataRepository saveDataRepository,
			ShareRepository shareRepository) {
            teams = teamRepository;
            saves = saveDataRepository;
			shares = shareRepository;
        }

        [AdminOnly]
        public IActionResult Index() => View(teams.GetAll());

        [TeamOnly]
        public IActionResult Details(int? id) {
            if (id == null)
                return NotFound();

            var team = teams.GetById(id);
            if (team == null)
                return NotFound();

            var vm = new TeamDetailsViewModel(team);

            foreach (var sd in saves.GetForTeam(team.SteamID, 60).Reverse()) {
                vm.BalanceItems.Add(new ChartItem { Label = sd.Date.ToString(), Quantity = (float)sd.Balance });
                vm.PollutionItems.Add(new ChartItem { Label = sd.Date.ToString(), Quantity = (float)sd.AveragePollution });
                vm.ProfitItems.Add(new ChartItem { Label = sd.Date.ToString(), Quantity = (float)sd.Profit });
                vm.CompanyValueItems.Add(new ChartItem { Label = sd.Date.ToString(), Quantity = (float)sd.CompanyValue });
                vm.MachineUptimeItems.Add(new ChartItem { Label = sd.Date.ToString(), Quantity = (float)sd.MachineUptime });
                vm.ShareValueItems.Add(new ChartItem { Label = sd.Date.ToString(), Quantity = (float)sd.ShareValue });
            }

			if (IsAdmin()) {
				vm.AdminShareValueItems = shares.GetCachedForTeam(team.SteamID)
					.Select(sd => new ChartItem { Label = sd.UnixDays.ToString(),
						Quantity = (float)sd.ShareValue });
			}

			return View(vm);
        }

        [AdminOnly]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOnly]
        public IActionResult Create([Bind("Id,Name,SteamID")] Team team) {
            if (ModelState.IsValid) {
                teams.AddOrIgnore(team);
                return RedirectToAction(nameof(Index));
            }

            return View(team);
        }

        [AdminOnly]
        public IActionResult Edit(int? id) {
            if (id == null)
                return NotFound();

            var team = teams.GetById(id);
            if (team == null)
                return NotFound();

            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOnly]
        public IActionResult Edit(int? id, [Bind("Id,Name,SteamID")] Team team) {
            if (id != team.Id)
                return NotFound();

            if (ModelState.IsValid) {
                teams.Update(team);
                return RedirectToAction(nameof(Index));
            }

            return View(team);
        }

        [AdminOnly]
        public IActionResult Delete(int? id) {
            if (id == null)
                return NotFound();

            var team = teams.GetById(id);
            if (team == null)
                return NotFound();

            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AdminOnly]
        public IActionResult DeleteConfirmed(int? id) {
            teams.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}