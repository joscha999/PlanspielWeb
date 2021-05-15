using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class TaskAPIController : Controller {
		private readonly TaskRepository tasks;

		public TaskAPIController(TaskRepository tr) {
			tasks = tr;
		}

		/// <summary>
		/// Gets all tasks for and filters out actions that aren't supposed to exist for the specified group.
		/// </summary>
		[HttpGet]
		public JsonResult Get(int groupSteamID) {
			return Json(tasks.GetAll(groupSteamID));
		}
	}
}