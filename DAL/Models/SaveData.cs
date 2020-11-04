using DAL.Repositories;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Planspiel.Models;
using ShareCalculationSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models {
    public class SaveData {
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

        private double _shareValue = -1;
        public double ShareValue {
            get {
                if (_shareValue == -1)
                    _shareValue = ShareRepository.Instance.Calculate(this);

                return _shareValue;
            }
            set => _shareValue = value;
        }

        internal double InternalShareValue => _shareValue;

        public int UnixDays {
            get => Date.UnixDays;
            set => Date = new Date(value);
        }

        [Write(false)]
        [JsonIgnore]
        public Date Date { get; set; }

        [JsonIgnore]
        public string IngameDate => Date.ToString();

        public static SaveData FromCreate(SaveDataCreate sd) {
            return new SaveData {
                Id = sd.Id,
                SteamID = sd.SteamID,
                Profit = sd.Profit,
                CompanyValue = sd.CompanyValue,
                DemandSatisfaction = sd.DemandSatisfaction,
                MachineUptime = sd.MachineUptime,
                AbleToPayLoansBack = sd.AbleToPayLoansBack,
                AveragePollution = sd.AveragePollution,
                BuildingCount = sd.BuildingCount,
                UnlockedResearchCount = sd.UnlockedResearchCount,
                RegionCount = sd.RegionCount,
                Date = new Date(sd.Day, sd.Month, sd.Year),
                ShareValue = sd.ShareValue
            };
        }

        public SaveDataModel ToModel() {
            return new SaveDataModel {
                AbleToPayLoansBack = AbleToPayLoansBack,
                AveragePollution = AveragePollution,
                BuildingCount = BuildingCount,
                CompanyValue = CompanyValue,
                Date = Date,
                DemandSatisfaction = DemandSatisfaction,
                MachineUptime = MachineUptime,
                Profit = Profit,
                RegionCount = RegionCount,
                ShareValue = _shareValue,
                SteamID = SteamID,
                UnlockedResearchCount = UnlockedResearchCount
            };
        }

        public static void Setup(Database database) {
            database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [SaveData] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[SteamID] INTEGER NOT NULL,
[UnixDays] INTEGER NOT NULL,
[Profit] REAL NOT NULL,
[CompanyValue] REAL NOT NULL,
[DemandSatisfaction] REAL NOT NULL,
[MachineUptime] REAL NOT NULL,
[AbleToPayLoansBack] INTEGER NOT NULL,
[AveragePollution] REAL NOT NULL,
[BuildingCount] INTEGER NOT NULL,
[UnlockedResearchCount] INTEGER NOT NULL,
[RegionCount] INTEGER NOT NULL
);");
        }
    }
}