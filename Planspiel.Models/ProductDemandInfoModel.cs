using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planspiel.Models {
	public class ProductDemandInfoModel {
		public string ProductName { get; set; }
		public string Settlement { get; set; }
		public string Shop { get; set; }
		public int Demand { get; set; }
		public int Sales { get; set; }
	}
}