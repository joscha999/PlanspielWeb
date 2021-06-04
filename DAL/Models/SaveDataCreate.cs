using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models {
    public class SaveDataCreate {
        public int? Id { get; set; }
        public long SteamID { get; set; }
        public double Profit { get; set; }
        public double CompanyValue { get; set; }
        public double DemandSatisfaction { get; set; }
        public double MachineUptime { get; set; }
        public bool AbleToPayLoansBack { get; set; }
        public double AveragePollution { get; set; }
        public int BuildingCount { get; set; }
        public int UnlockedResearchCount { get; set; }
        public int RegionCount { get; set; }
        public double ShareValue { get; set; }

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public static SaveDataCreate FromSaveData(SaveData sd) {
            return new SaveDataCreate {
                Id = sd.Id,
                SteamID = sd.SteamID,
                Profit = sd.Profit,
                CompanyValue = sd.CompanyValue,
                MachineUptime = sd.MachineUptime,
                AveragePollution = sd.AveragePollution,
                BuildingCount = sd.BuildingCount,
                UnlockedResearchCount = sd.UnlockedResearchCount,
                RegionCount = sd.RegionCount,
                Day = sd.Date.Day,
                Month = sd.Date.Month,
                Year = sd.Date.Year,
                ShareValue = sd.ShareValue
            };
        }
    }
}