using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanspielWeb.Attributes;
using PlanspielWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Controllers {
	[AdminOnly]
	public class TasksController : AppController {
		private readonly TaskRepository tasks;

		public TasksController(TaskRepository tr) {
			tasks = tr;
		}

		public IActionResult Index() => View(tasks.GetAll(-2, true));

		public IActionResult Details(int? id) {
			if (id == null)
				return NotFound();

			var data = tasks.GetById(id.Value, true);
			if (data == null)
				return NotFound();

			return View(data);
		}

		public IActionResult CreateTask() => View(new AssignmentTask { UTCStart = DateTime.UtcNow });

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CreateTask([Bind("ID, Name, UTCStart, Actions, Enabled")] AssignmentTask task) {
			if (ModelState.IsValid) {
				tasks.AddOrIgnore(task);
				return RedirectToAction(nameof(Index));
			}

			return View(task);
		}

		public IActionResult CreateAction(int taskID) => View(new AssignmentAction {
			TaskID = taskID
		});

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CreateAction([FromForm]
			[Bind("TaskID, GroupSteamID, SecondsFromStart, Type, Value")]
			AssignmentAction action) {
			if (ModelState.IsValid) {
				tasks.AddOrIgnore(action);
				return RedirectToAction(nameof(Details), new { id = action.TaskID });
			}

			return View();
		}

		public IActionResult EditTask(int? id) {
			if (id == null)
				return NotFound();

			var data = tasks.GetById(id.Value);
			if (data == null)
				return NotFound();

			return View(data);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditTask([Bind("ID, Name, UTCStart, Actions, Enabled")] AssignmentTask task) {
			if (ModelState.IsValid) {
				tasks.Update(task);
				return RedirectToAction(nameof(Index));
			}

			return View(task);
		}

		public IActionResult EditAction(int? id) {
			if (id == null)
				return NotFound();

			var data = tasks.GetActionById(id.Value);
			if (data == null)
				return NotFound();

			return View(data);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditAction([FromForm][Bind("ID, TaskID, GroupSteamID, " +
			"SecondsFromStart, Type, Value")] AssignmentAction action) {
			if (ModelState.IsValid) {
				tasks.Update(action);
				return RedirectToAction(nameof(Details), new { id = action.TaskID });
			}

			return View(action);
		}

		public IActionResult DeleteTask(int? id) {
			if (id == null)
				return NotFound();

			var data = tasks.GetById(id.Value);
			if (data == null)
				return NotFound();

			return View(data);
		}

		public IActionResult DeleteAction(int? id) {
			if (id == null)
				return NotFound();

			var data = tasks.GetActionById(id.Value);
			if (data == null)
				return NotFound();

			return View(data);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteTaskConfirmed(int? id) {
			if (id == null)
				return NotFound();

			var data = tasks.GetById(id.Value);
			if (data == null)
				return NotFound();

			tasks.DeleteTask(data);
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteActionConfirmed(int? id, int? taskid) {
			tasks.DeleteAction(id);

			if (taskid != null)
				return RedirectToAction(nameof(Details), new { id = taskid });

			return RedirectToAction(nameof(Index));
		}
	}
}
