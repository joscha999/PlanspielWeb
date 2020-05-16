using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlanspielWeb.Models;

namespace PlanspielWeb.Controllers {
    public class HomeController : AppController {
        private readonly TeamRepository teams;

        public HomeController(TeamRepository teamRepository) {
            teams = teamRepository;
        }

        public IActionResult Index() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ShareValues() {
            if (!IsAdmin()) {
                AddError("Deine Berichtigung reicht nicht aus um diese Seite anzusehen!");
                return RedirectToAction("Index", "Home");
            }

            var shareValues = new List<ShareValuesViewModel>();

            foreach (var team in teams.GetAll()) {
                if (!team.Data.Any())
                    continue;

                shareValues.Add(new ShareValuesViewModel {
                    TeamName = team.Name,
                    ShareValue = team.Data.LastOrDefault()?.ShareValue ?? 0,
                    AvgShareValue = team.Data.Average(d => d.ShareValue)
                });
            }

            return View(shareValues.OrderBy(s => s.ShareValue));
        }
    }
}