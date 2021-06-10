using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Models {
    public class TeamDetailsViewModel {
        public Team Team { get; }
        public List<ChartItem> BalanceItems { get; } = new();
        public List<ChartItem> ProfitItems { get; } = new();
        public List<ChartItem> CompanyValueItems { get; } = new();
        public List<ChartItem> MachineUptimeItems { get; } = new();
        public List<ChartItem> PollutionItems { get; } = new();
        public List<ChartItem> ShareValueItems { get; } = new();
		public IEnumerable<ChartItem> AdminShareValueItems { get; set; }

		public TeamDetailsViewModel(Team team) {
            Team = team;
        }
    }
}