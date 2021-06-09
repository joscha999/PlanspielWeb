using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Models {
    public class TeamDetailsViewModel {
        public Team Team { get; set; }
        public List<ChartItem> BalanceItems { get; set; }

        //other chart lists
    }
}
