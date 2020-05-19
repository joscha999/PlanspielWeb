using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Models {
	public class ShareValuesViewModel {
		public string TeamName { get; set; }
		public double ShareValue { get; set; }
		public double AvgShareValue { get; set; }
		public string TimeStamp { get; set; }
	}
}