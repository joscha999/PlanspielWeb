using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Models {
	public class SaveDataListViewModel {
		public const int ItemsPerPage = 50;

		public IEnumerable<SaveData> Data { get; set; }

		public int CurrentStartID { get; set; }

		public bool CanGoNext { get; set; }
		public bool CanGoPrevious { get; set; }

		public int NextStartID => CurrentStartID + ItemsPerPage;
		public int PreviousStartID => CurrentStartID - ItemsPerPage;

		public int CalculationQueueCount { get; set; }
	}
}