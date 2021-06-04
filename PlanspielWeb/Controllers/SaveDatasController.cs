using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.Repositories;
using PlanspielWeb.Attributes;
using Planspiel.Models;
using PlanspielWeb.Models;

namespace PlanspielWeb.Controllers {
    [AdminOnly]
    public class SaveDatasController : AppController {
        private readonly SaveDataRepository saveData;

        public SaveDatasController(SaveDataRepository sd) {
            saveData = sd;
        }

		public IActionResult Index(int currStartID = int.MinValue) {
			var max = saveData.GetMaxID();
			var min = saveData.GetMinID();

			if (currStartID == int.MinValue)
				currStartID = max;
			if (currStartID > max)
				currStartID = max;
			if (currStartID < min)
				currStartID = min;

			var vm = new SaveDataListViewModel {
				CanGoNext = currStartID < max,
				CanGoPrevious = currStartID - SaveDataListViewModel.ItemsPerPage > min,
				CurrentStartID = currStartID,
				Data = saveData.GetPagedFrom(currStartID, SaveDataListViewModel.ItemsPerPage)
			};

			return View(vm);
		}

		public IActionResult Details(int? id) {
            if (id == null)
                return NotFound();

            var data = saveData.GetById(id.Value);

            if (data == null)
                return NotFound();

            return View(data);
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,SteamID,TimeStamp,Profit,CompanyValue,DemandSatisfaction," +
            "MachineUptime,AbleToPayLoansBack,AveragePollution,BuildingCount,UnlockedResearchCount,RegionCount," +
            "Day,Month,Year")] SaveDataCreate data) {
            if (ModelState.IsValid) {
                saveData.AddOrIgnore(SaveData.FromCreate(data));
                return RedirectToAction(nameof(Index));
            }

            return View(saveData);
        }

        public IActionResult Edit(int? id) {
            if (id == null)
                return NotFound();

            var data = saveData.GetById(id.Value);
            if (data == null)
                return NotFound();

            return View(SaveDataCreate.FromSaveData(data));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, [Bind("Id,SteamID,TimeStamp,Profit,CompanyValue,DemandSatisfaction," +
            "MachineUptime,AbleToPayLoansBack,AveragePollution,BuildingCount,UnlockedResearchCount,RegionCount," +
            "Day,Month,Year,ShareValue")] SaveDataCreate data) {
            if (id != data.Id)
                return NotFound();

            if (ModelState.IsValid) {
                saveData.Update(SaveData.FromCreate(data));
                return RedirectToAction(nameof(Index));
            }
            return View(data);
        }

        public IActionResult Delete(int? id) {
            if (id == null)
                return NotFound();

            var data = saveData.GetById(id.Value);
            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id) {
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