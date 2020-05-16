using DAL.Repositories;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models {
    public class Team {
        private static TeamRepository teamRepository;

        public int? Id { get; set; }
        public string Name { get; set; }
        public long SteamID { get; set; }

        [JsonIgnore]
        public IEnumerable<User> Members => teamRepository.GetMembersForId(Id);

        [JsonIgnore]
        public IEnumerable<SaveData> Data => teamRepository.GetSaveDataForTeam(SteamID);

        public static void Setup(Database database) {
            teamRepository = new TeamRepository(database);

            database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [Teams] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[Name] NVARCHAR(64) NOT NULL,
[SteamID] INTEGER NOT NULL
);");
        }
    }
}