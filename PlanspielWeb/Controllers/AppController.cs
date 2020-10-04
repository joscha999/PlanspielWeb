using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using PlanspielWeb.Attributes;
using PlanspielWeb.Models;
using System;
using System.Collections.Generic;

namespace PlanspielWeb.Controllers {
    public class AppController : Controller {
		public override void OnActionExecuting(ActionExecutingContext context) {
			if (TempData.TryGetValue("alerts", out var o) && o is List<AlertViewModel> alerts)
				ViewData["alerts"] = alerts;

			var attributes = new List<object>();

			//get attributes of action
            if (context.ActionDescriptor is ControllerActionDescriptor cad)
				attributes.AddRange(cad.MethodInfo.GetCustomAttributes(true));

			//get attributes of controller
			attributes.AddRange(Attribute.GetCustomAttributes(context.Controller.GetType(), true));

			foreach (var attr in attributes) {
				if (attr is TeamOnlyAttribute toa && !toa.CanEnter(this, context)) {
					AddError("Deine Berechtigung reicht nicht aus um diese Seite anzusehen!");
					context.Result = new RedirectToActionResult("Index", "Home", null);
					return;
				}

				if (attr is AdminOnlyAttribute && !IsAdmin()) {
					AddError("Deine Berechtigung reicht nicht aus um diese Seite anzusehen!");
					context.Result = new RedirectToActionResult("Index", "Home", null);
					return;
				}
            }

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

		//NOTE: this is a security risk, a logged in user that is downgraded from admin to non-admin could still use admin features
		public bool IsAdmin() => HttpContext.Session.TryGet<User>("user", out var user) && user.Admin;

		//NOTE: this is a security risk, a logged in user whos team id is changed does not get the new team id
		public bool IsInTeam(int teamId) => HttpContext.Session.TryGet<User>("user", out var user) && user.TeamId == teamId;

		public bool IsInTeamOrAdmin(int teamid) => IsAdmin() || IsInTeam(teamid);
	}
}