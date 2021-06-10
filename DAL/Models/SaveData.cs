using DAL.Repositories;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Planspiel.Models;
using ShareCalculationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Models {
    public class SaveData {
        public int? Id { get; set; }
        public long SteamID { get; set; }
		public int UnixDays { get; set; }
		public double Profit { get; set; }
        public double CompanyValue { get; set; }

		[Write(false)]
		public List<ProductDemandInfo> DemandSatisfaction { get; set; }
		public double MachineUptime { get; set; }

		[Write(false)]
		public List<LoanInfo> LoansList { get; set; }
		public double AveragePollution { get; set; }
        public int BuildingCount { get; set; }
        public int UnlockedResearchCount { get; set; }
        public int RegionCount { get; set; }
		public double Balance { get; set; }

		internal double _shareValue = -1;
        public double ShareValue {
            get {
                if (_shareValue == -1)
                    ShareRepository.Instance.RequestCalculation(this);

                return _shareValue;
            }
            set => _shareValue = value;
        }

		[Write(false)]
		[JsonIgnore]
		public Date Date {
			get => new Date(UnixDays + 481);
			set => UnixDays = value.UnixDays - 481;
		}

		[Write(false)]
        [JsonIgnore]
        public string IngameDate => Date.ToString();

        public static SaveData FromCreate(SaveDataCreate sd) {
            return new SaveData {
                Id = sd.Id,
                SteamID = sd.SteamID,
                Profit = sd.Profit,
                CompanyValue = sd.CompanyValue,
                MachineUptime = sd.MachineUptime,
                AveragePollution = sd.AveragePollution,
                BuildingCount = sd.BuildingCount,
                UnlockedResearchCount = sd.UnlockedResearchCount,
                RegionCount = sd.RegionCount,
                Date = new Date(sd.Day, sd.Month, sd.Year),
                ShareValue = sd.ShareValue
            };
        }

        public SaveDataModel ToModel() {
			var sdm = new SaveDataModel {
				AveragePollution = AveragePollution,
				BuildingCount = BuildingCount,
				CompanyValue = CompanyValue,
				UnixDays = UnixDays,
                MachineUptime = MachineUptime,
                Profit = Profit,
                RegionCount = RegionCount,
                ShareValue = _shareValue,
                SteamID = SteamID,
                UnlockedResearchCount = UnlockedResearchCount,
				Balance = Balance
            };

			sdm.DemandSatisfaction = DemandSatisfaction.ConvertAll(ds => ds.ToModel());
			sdm.LoansList = LoansList.ConvertAll(l => l.ToModel());

			return sdm;
        }

        public static void Setup(Database database) {
            database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [SaveData] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[SteamID] INTEGER NOT NULL,
[UnixDays] INTEGER NOT NULL,
[Profit] REAL NOT NULL,
[CompanyValue] REAL NOT NULL,
[MachineUptime] REAL NOT NULL,
[AveragePollution] REAL NOT NULL,
[BuildingCount] INTEGER NOT NULL,
[UnlockedResearchCount] INTEGER NOT NULL,
[RegionCount] INTEGER NOT NULL,
[Balance] REAL NOT NULL,
[ShareValue] INTEGER NOT NULL
);");
        }
    }
}