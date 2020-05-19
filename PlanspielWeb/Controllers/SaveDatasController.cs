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
    public class SaveDatasController : AdminController
    {
        private readonly SaveDataRepository saveData;

        public SaveDatasController(SaveDataRepository sd)
        {
            saveData = sd;
        }

        // GET: SaveDatas
        public IActionResult Index()
        {
            return View(saveData.GetAll());
        }

        // GET: SaveDatas/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var data = saveData.GetById(id.Value);

            if (data == null)
                return NotFound();

            return View(data);
        }

        // GET: SaveDatas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SaveDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,SteamID,TimeStamp,Profit,CompanyValue,DemandSatisfaction,MachineUptime,AbleToPayLoansBack,AveragePollution")] SaveData data)
        {
            if (ModelState.IsValid)
            {
                saveData.AddOrIgnore(data);
                return RedirectToAction(nameof(Index));
            }

            return View(saveData);
        }

        // GET: SaveDatas/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var data = saveData.GetById(id.Value);
            if (data == null)
                return NotFound();

            return View(data);
        }

        // POST: SaveDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, [Bind("Id,SteamID,TimeStamp,Profit,CompanyValue,DemandSatisfaction,MachineUptime,AbleToPayLoansBack,AveragePollution")] SaveData data)
        {
            if (id != data.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                saveData.Update(data);
                return RedirectToAction(nameof(Index));
            }
            return View(data);
        }

        // GET: SaveDatas/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var data = saveData.GetById(id.Value);
            if (data == null)
                return NotFound();

            return View(data);
        }

        // POST: SaveDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            saveData.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteFromTeam(int? id, int? teamId) {
            if (id == null)
                return NotFound();

            saveData.Delete(id.Value);

            if (teamId == null)
                return View(nameof(Index));

            return RedirectToAction("Details", "Teams", new { id = teamId });
        }
    }
}
