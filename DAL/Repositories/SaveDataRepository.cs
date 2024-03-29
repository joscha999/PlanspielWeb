﻿using DAL.Models;
using Dapper;
using Planspiel.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL.Repositories {
    public class SaveDataRepository {
        private readonly Database Database;

		private string BaseQuery = @"SELECT 
Id, SteamID, UnixDays, CAST(Profit AS REAL) AS Profit, CAST(CompanyValue AS REAL) AS CompanyValue, 
CAST(MachineUptime AS REAL) AS MachineUptime, CAST(AveragePollution AS REAL) AS AveragePollution, 
BuildingCount, UnlockedResearchCount, RegionCount, CAST(Balance AS REAL) AS Balance, 
CAST(ShareValue AS REAL) AS ShareValue
FROM SaveData ";

        public SaveDataRepository(Database database) {
            Database = database;
        }

        private void EnsureOpen() {
            if (Database == null)
                throw new ArgumentNullException(nameof(Database));

            if (Database.Connection.State != ConnectionState.Open)
                Database.Connection.Open();
        }

		private void QuerrySup(SaveData sd) {
			sd.DemandSatisfaction = Database.Connection.Query<ProductDemandInfo>(
				"SELECT * FROM ProductDemandInfo WHERE SaveDataId = @sdid", new { sdid = sd.Id }).ToList();
			sd.LoansList = Database.Connection.Query<LoanInfo>("SELECT * FROM LoanInfo WHERE SaveDataId = @sdid",
				new { sdid = sd.Id }).ToList();
		}

		public IEnumerable<SaveData> GetForTeam(long steamID, int days = 30, bool desc = true) {
			EnsureOpen();

			foreach (var sd in Database.Connection.Query<SaveData>(
				BaseQuery + $"WHERE SteamID = @sid ORDER BY UnixDays {(desc ? "DESC" : "ASC")} LIMIT @days",
				new { sid = steamID, days })) {
				QuerrySup(sd);
				yield return sd;
			}
		}

		public SaveData GetById(int id) {
            EnsureOpen();

            var sd = Database.Connection.Query<SaveData>(BaseQuery + "WHERE Id = @id", new { id }).FirstOrDefault();
			QuerrySup(sd);
			return sd;
        }

		public bool DayDataExists(long steamID, int unixDay) {
			EnsureOpen();

			return Database.Connection.ExecuteScalar<int>(
				"SELECT COUNT(Id) FROM SaveData WHERE SteamID = @steamID AND UnixDays = @unixDay",
				new { steamID, unixDay }) != 0;
		}

        public IEnumerable<SaveData> GetPeriod(int end, int start, long steamID) {
            EnsureOpen();

            foreach (var sd in Database.Connection.Query<SaveData>(
				BaseQuery + "WHERE UnixDays >= @start AND UnixDays < @end AND SteamID = @steamID ORDER BY UnixDays DESC",
                new { end, start, steamID })) {
				QuerrySup(sd);
				yield return sd;
			}
        }

		public SaveData GetForDate(int unixDay) {
			EnsureOpen();

			var sd = Database.Connection.QueryFirstOrDefault<SaveData>(
				BaseQuery + "WHERE UnixDays = @day", new { day = unixDay });
			if (sd != null)
				QuerrySup(sd);
			return sd;
		}

		public SaveData GetPrior(int unixDay, long steamID) {
			EnsureOpen();

			var sd = Database.Connection.QueryFirstOrDefault<SaveData>(
				BaseQuery + "WHERE UnixDays < @day AND SteamID = @steamID ORDER BY UnixDays DESC LIMIT 1",
				new { day = unixDay, steamID });
			if (sd != null)
				QuerrySup(sd);
			return sd;
		}

		public IEnumerable<SaveData> GetPagedFrom(int unixDayMax, int count) {
			EnsureOpen();

			foreach (var sd in Database.Connection.Query<SaveData>(
				BaseQuery + "WHERE Id <= @max ORDER BY Id DESC LIMIT @amount",
				new { max = unixDayMax, amount = count })) {
				QuerrySup(sd);
				yield return sd;
			}
		}

		public int GetMinID() {
			EnsureOpen();

			return Database.Connection.ExecuteScalar<int>("SELECT MIN(Id) FROM SaveData");
		}

		public int GetMaxID() {
			EnsureOpen();

			return Database.Connection.ExecuteScalar<int>("SELECT MAX(Id) FROM SaveData");
		}

        public void AddOrIgnore(SaveData data) {
            EnsureOpen();

            Database.Connection.Execute(@"INSERT OR IGNORE INTO
SaveData(Id, SteamID, UnixDays, Profit, CompanyValue, MachineUptime, 
AveragePollution, BuildingCount, UnlockedResearchCount, RegionCount, Balance, ShareValue)
Values(@id, @steamId, @unixDays, @profit, @companyValue, @machineUptime, 
@pollution, @buildingCount, @unlockedResearch, @regionCount, @balance, @shareValue);",
                new {
                    id = data.Id,
                    steamId = data.SteamID,
					unixDays = data.UnixDays,
                    profit = data.Profit,
                    companyValue = data.CompanyValue,
                    machineUptime = data.MachineUptime,
                    pollution = data.AveragePollution,
                    buildingCount = data.BuildingCount,
                    unlockedResearch = data.UnlockedResearchCount,
                    regionCount = data.RegionCount,
					balance = data.Balance,
					shareValue = data.ShareValue
                });

			data.Id = Database.Connection.QueryFirst<int>("SELECT last_insert_rowid()");

			foreach (var pd in data.DemandSatisfaction) {
				Database.Connection.Execute(@"INSERT OR IGNORE INTO
ProductDemandInfo(Id, SaveDataId, ProductName, Settlement, Shop, Demand, Sales)
Values(@id, @sdid, @productName, @settlement, @shop, @demand, @sales)",
					new {
						id = pd.ID,
						sdid = data.Id,
						productName = pd.ProductName,
						settlement = pd.Settlement,
						shop = pd.Shop,
						demand = pd.Demand,
						sales = pd.Sales
					});
			}

			foreach (var li in data.LoansList) {
				Database.Connection.Execute(@"INSERT OR IGNORE INTO
LoanInfo(Id, SaveDataId, LoanAmount, LoanInterest)
Values(@id, @sdid, @loanAmount, @loanInterest)",
					new {
						id = li.ID,
						sdid = data.Id,
						loanAmount = li.LoanAmount,
						loanInterest = li.LoanInterest
					});
			}
        }

        public void Update(SaveData data) {
            EnsureOpen();

            Database.Connection.Execute(@"UPDATE SaveData SET 
Id = @id, SteamID = @steamId, UnixDays = @timeStamp, Profit = @profit, CompanyValue = @companyValue, 
MachineUptime = @machineUptime, AveragePollution = @pollution, BuildingCount = @buildingCount, 
UnlockedResearchCount = @researchCount, RegionCount = @regionCount, Balance = @balance, ShareValue = @shareValue 
WHERE Id = @id",
                new {
                    id = data.Id,
                    steamId = data.SteamID,
                    timeStamp = data.UnixDays,
                    profit = data.Profit,
                    companyValue = data.CompanyValue,
                    machineUptime = data.MachineUptime,
                    pollution = data.AveragePollution,
                    buildingCount = data.BuildingCount,
                    researchCount = data.UnlockedResearchCount,
                    regionCount = data.RegionCount,
					balance = data.Balance,
					shareValue = data.ShareValue
				});

			foreach (var pd in data.DemandSatisfaction) {
				Database.Connection.Execute(@"UPDATE ProductDemandInfo SET
Id = @id, SaveDataId = @sdid, ProductName = @productName, Settlement = @settlement,
Shop = @shop, Demand = @demand, Sales = @sales WHERE Id = @id",
					new {
						id = pd.ID,
						sdid = data.Id,
						productName = pd.ProductName,
						settlement = pd.Settlement,
						shop = pd.Shop,
						demand = pd.Demand,
						sales = pd.Sales
					});
			}

			foreach (var li in data.LoansList) {
				Database.Connection.Execute(@"UPDATE LoanInfo SET
Id = @id, SaveDataId = @sdid, LoanAmount = @loanAmount, LoanInterest = @loanInterest WHERE Id = @id",
					new {
						id = li.ID,
						sdid = data.Id,
						loanAmount = li.LoanAmount,
						loanInterest = li.LoanInterest
					});
			}
		}

        public void Delete(int? id) {
            if (id == null)
                return;

            EnsureOpen();

            Database.Connection.Execute("DELETE FROM SaveData WHERE Id = @id;", new { id = id.Value });
			Database.Connection.Execute("DELETE FROM ProductDemandInfo WHERE SaveDataId = @id;", new { id = id.Value });
			Database.Connection.Execute("DELETE FROM LoanInfo WHERE SaveDataId = @id;", new { id = id.Value });
		}

		public void ClearShareValues() {
			EnsureOpen();

			Database.Connection.Execute("UPDATE SaveData SET ShareValue = -1");
		}
    }
}