using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Controllers {
	public class AdminController : AppController {
		public override void OnActionExecuting(ActionExecutingContext context) {
			if (!IsAdmin()) {
				AddError("Deine Berichtigung reicht nicht aus um diese Seite anzusehen!");
				context.Result = new RedirectToActionResult("Index", "Home", null);
			}

			base.OnActionExecuting(context);
		}
	}
}