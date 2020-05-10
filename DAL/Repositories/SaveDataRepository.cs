using DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
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

        public void AddOrIgnore(SaveData data) {
            EnsureOpen();

            Database.Connection.Execute(@"INSERT OR IGNORE INTO
SaveData(Id, SteamID, TimeStamp, Profit, CompanyValue, DemandSatisfcation, MachineUptime, AbleToPayLoansBack, AveragePollution)
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
    }
}