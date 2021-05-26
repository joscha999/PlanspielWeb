using System;

namespace Planspiel.Models {
    public class SaveDataModel {
        public long SteamID { get; set; }

        /// <summary>
        /// Game time at which this save data was created.
        /// </summary>
        public int UnixDays { get; set; }

        public double Profit { get; set; }

        public double CompanyValue { get; set; }

        //public double DemandSatisfaction { get; set; }

        /// <summary>
        /// Average machine uptime.
        /// </summary>
        public double MachineUptime { get; set; }

        public bool AbleToPayLoansBack { get; set; }

        public double AveragePollution { get; set; }

        public int BuildingCount { get; set; }

        public int UnlockedResearchCount { get; set; }

        public int RegionCount { get; set; }

        public double ShareValue { get; set; } = -1;
    }
}