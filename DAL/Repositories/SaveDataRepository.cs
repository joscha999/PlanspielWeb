using DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL.Repositories {
    public class SaveDataRepository {
        private readonly Database Database;

        public SaveDataRepository(Database database) {
            Database = database;
        }

        private void EnsureOpen() {
            if (Database == null)
                throw new ArgumentNullException(nameof(Database));

            if (Database.Connection.State != ConnectionState.Open)
                Database.Connection.Open();
        }

        public SaveData GetById(int id) {
            EnsureOpen();

            return Database.Connection.Query<SaveData>("SELECT * FROM SaveData WHERE Id = @id", new { id }).FirstOrDefault();
        }

        public void AddOrIgnore(SaveData data) {
            EnsureOpen();

            Database.Connection.Execute(@"INSERT OR IGNORE INTO
SaveData(Id, SteamID, TimeStamp, Profit, CompanyValue, DemandSatisfaction, MachineUptime, AbleToPayLoansBack, AveragePollution)
Values(@id, @steamId, @timeStamp, @profit, @companyValue, @demand, @machineUptime, @loan, @pollution);",
                new {
                    id = data.Id,
                    steamId = data.SteamID,
                    timeStamp = data.TimeStamp,
                    profit = data.Profit,
                    companyValue = data.CompanyValue,
                    demand = data.DemandSatisfaction,
                    machineUptime = data.MachineUptime,
                    loan = data.AbleToPayLoansBack,
                    pollution = data.AveragePollution
                });
        }

        public void Update(SaveData data) {
            EnsureOpen();

            Database.Connection.Execute(@"UPDATE SaveData SET
Id = @id, SteamID = @steamId, TimeStamp = @timeStamp, Profit = @profit, CompanyValue = @companyValue,
DemandSatisfaction = @demand, MachineUptime = @machineUptime, AbleToPayLoansBack = @load, AveragePollution = @pollution
WHERE Id = @id",
                new {
                    id = data.Id,
                    steamId = data.SteamID,
                    timeStamp = data.TimeStamp,
                    profit = data.Profit,
                    companyValue = data.CompanyValue,
                    demand = data.DemandSatisfaction,
                    machineUptime = data.MachineUptime,
                    loan = data.AbleToPayLoansBack,
                    pollution = data.AveragePollution
                });
        }

        public void Delete(int? id) {
            if (id == null)
                return;

            EnsureOpen();

            Database.Connection.Execute("DELETE FROM SaveData WHERE Id = @id;", new { id = id.Value });
        }

        //TODO: same method in SaveDataRepo and TeamRepo
        public IEnumerable<SaveData> GetAll() {
            EnsureOpen();

            return Database.Connection.Query<SaveData>("SELECT * FROM SaveData");
        }
    }
}