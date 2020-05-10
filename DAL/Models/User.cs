using DAL.Repositories;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Models {
    public class User {
        private static TeamRepository teamRepository;

        public int? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Admin { get; set; }
        public int? TeamId { get; set; }

        public Team Team => teamRepository.GetById(TeamId);

        public static void Setup(Database database, PasswordHasher pwHasher) {
            teamRepository = new TeamRepository(database);

            database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [Users] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[Username] NVARCHAR(64) NOT NULL,
[Password] NVARCHAR(128) NOT NULL,
[Admin] INTEGER NOT NULL,
[TeamId] INTEGER
);");

            var ur = new UserRepository(database);
            ur.AddOrIgnore(new User {
                Id = 0,
                Username = "root",
                Password = pwHasher.Hash("root"),
                Admin = true
            });
        }
    }
}