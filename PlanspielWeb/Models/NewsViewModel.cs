using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Models {
	public class NewsViewModel {
		public int PageID { get; set; }

		public int GridWidth { get; set; }
		public int GridHeight { get; set; }

		public Dictionary<string, News> News { get; } = new Dictionary<string, News>();

		public string GridLayout { get; set; }

		public bool CanGoNext { get; set; }
		public bool CanGoPrevious { get; set; }

		public int NextPageID => PageID + 1;
		public int PreviousPageID => PageID - 1;
	}
}