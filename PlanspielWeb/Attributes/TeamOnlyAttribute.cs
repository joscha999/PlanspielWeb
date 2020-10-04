using Microsoft.AspNetCore.Mvc.Filters;
using PlanspielWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Attributes {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class TeamOnlyAttribute : Attribute {
        public bool CanEnter(AppController controller, ActionExecutingContext context) {
            //if no id or id not an int => return true, the action has to take care of it
            if (!context.ActionArguments.TryGetValue("id", out object pid) || !(pid is int id))
                return true;

            return controller.IsInTeamOrAdmin(id);
        }
    }
}