using DAL.Repositories;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Planspiel.Models;
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

        public string SaveDataDate {
            get => Date.ToDB();
            set => Date = Date.FromDB(value);
        }

        [Write(false)]
        [JsonIgnore]
        public Date Date { get; set; }

        [JsonIgnore]
        public double ShareValue => ShareRepository.CalculateSharePrice(this);

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
                Date = new Date(sd.Day, sd.Month, sd.Year)
            };
        }

        public static void Setup(Database database) {
            database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [SaveData] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[SteamID] INTEGER NOT NULL,
[SaveDataDate] TEXT NOT NULL,
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