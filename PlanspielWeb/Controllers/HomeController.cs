﻿using System.Collections.Generic;
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

        public HomeController(TeamRepository teamRepository) {
            teams = teamRepository;
        }

        public IActionResult Index() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        [AdminOnly]
        public IActionResult ShareValues() {
            var shareValues = new List<ShareValuesViewModel>();

            foreach (var team in teams.GetAll()) {
                if (team.Data == null || !team.Data.Any())
                    continue;

                var lastData = team.Data.LastOrDefault();
                shareValues.Add(new ShareValuesViewModel {
                    TeamName = team.Name,
                    ShareValue = lastData?.ShareValue ?? 0,
                    AvgShareValue = team.Data.Average(d => d.ShareValue),
                    TimeStamp = lastData?.IngameDate
                });
            }

            return View(shareValues.OrderByDescending(s => s.AvgShareValue));
        }
    }
}