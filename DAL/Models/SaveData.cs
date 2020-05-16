using DAL.Repositories;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models {
    public class SaveData {
        public int? Id { get; set; }
        public long SteamID { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Profit { get; set; }
        public double CompanyValue { get; set; }
        public double DemandSatisfaction { get; set; }
        public double MachineUptime { get; set; }
        public bool AbleToPayLoansBack { get; set; }
        public double AveragePollution { get; set; }

        [JsonIgnore]
        public double ShareValue => ShareRepository.CalculateSharePrice(this);

        //TODO: really dirty fix, DateTime needs to be replaced, there's no 30th of feb.
        [JsonIgnore]
        public string IngameDate => TimeStamp.ToString("MMM") + " 30, Y" + TimeStamp.Year;

        public static void Setup(Database database) {
            database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [SaveData] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[SteamID] INTEGER NOT NULL,
[TimeStamp] TEXT NOT NULL,
[Profit] REAL NOT NULL,
[CompanyValue] REAL NOT NULL,
[DemandSatisfaction] REAL NOT NULL,
[MachineUptime] REAL NOT NULL,
[AbleToPayLoansBack] INTEGER NOT NULL,
[AveragePollution] REAL NOT NULL
);");
        }
    }
}