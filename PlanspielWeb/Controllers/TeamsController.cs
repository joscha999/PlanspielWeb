using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories;

namespace PlanspielWeb.Controllers
{
    public class TeamsController : AppController
    {
        private readonly TeamRepository teams;

        public TeamsController(TeamRepository teamRepository)
        {
            teams = teamRepository;
        }

        // GET: Teams
        public IActionResult Index()
        {
            if (!IsAdmin()) {
                AddError("Deine Berichtigung reicht nicht aus um diese Seite anzusehen!");
                return RedirectToAction("Index", "Home");
            }

            return View(teams.GetAll());
        }

        // GET: Teams/Details/5
        public IActionResult Details(int? id) {
            if (id == null)
            {
                return NotFound();
            }

            var team = teams.GetById(id);
            if (team == null)
            {
                return NotFound();
            }

            if (IsAdmin() || IsInTeam(id.Value))
                return View(team);

            AddError("Deine Berichtigung reicht nicht aus um diese Seite anzusehen!");
            return RedirectToAction("Index", "Home");
        }

        // GET: Teams/Create
        public IActionResult Create() {
            if (!IsAdmin()) {
                AddError("Deine Berichtigung reicht nicht aus um diese Seite anzusehen!");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,SteamID")] Team team) {
            if (!IsAdmin()) {
                AddError("Deine Berichtigung reicht nicht aus um diese Seite anzusehen!");
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                teams.AddOrIgnore(team);
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Edit/5
        public IActionResult Edit(int? id) {
            if (!IsAdmin()) {
                AddError("Deine Berichtigung reicht nicht aus um diese Seite anzusehen!");
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var team = teams.GetById(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, [Bind("Id,Name,SteamID")] Team team) {
            if (!IsAdmin()) {
                AddError("Deine Berichtigung reicht nicht aus um diese Seite anzusehen!");
                return RedirectToAction("Index", "Home");
            }

            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                teams.Update(team);
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Delete/5
        public IActionResult Delete(int? id) {
            if (!IsAdmin()) {
                AddError("Deine Berichtigung reicht nicht aus um diese Seite anzusehen!");
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var team = teams.GetById(id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id) {
            if (!IsAdmin()) {
                AddError("Deine Berichtigung reicht nicht aus um diese Seite anzusehen!");
                return RedirectToAction("Index", "Home");
            }

            teams.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
