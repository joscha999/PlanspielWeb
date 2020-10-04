using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.Repositories;
using PlanspielWeb.Attributes;

namespace PlanspielWeb.Controllers {
    public class TeamsController : AppController {
        private readonly TeamRepository teams;

        public TeamsController(TeamRepository teamRepository) {
            teams = teamRepository;
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

            return View(team);
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