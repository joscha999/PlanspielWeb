using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Models {
	public class AlertViewModel {
		public string Message { get; set; }
		public AlerType Type { get; set; }
		public string CssClass => "alert-" + Type.ToString().ToLower();
	}

	public enum AlerType {
		Primary,
		Secondary,
		Success,
		Danger,
		Warning,
		Info,
		Light,
		Dark
	}
}