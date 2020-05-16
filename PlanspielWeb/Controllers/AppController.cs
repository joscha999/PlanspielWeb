using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PlanspielWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Controllers {
	public class AppController : Controller {
		public override void OnActionExecuting(ActionExecutingContext context) {
			if (TempData.TryGetValue("alerts", out var o) && o is List<AlertViewModel> alerts)
				ViewData["alerts"] = alerts;

			base.OnActionExecuting(context);
		}

		public void AddError(string msg) => AddAlert(AlerType.Danger, msg);

		public void AddAlert(AlerType type, string msg) {
			var alerts = TempData.Get<List<AlertViewModel>>("alerts") ?? new List<AlertViewModel>();

			alerts.Add(new AlertViewModel {
				Message = msg,
				Type = type
			});

			TempData.Put("alerts", alerts);
		}

		public bool IsAdmin() => HttpContext.Session.TryGet<User>("user", out var user) && user.Admin;

		public bool IsInTeam(int teamId) => HttpContext.Session.TryGet<User>("user", out var user) && user.TeamId == teamId;
	}
}