using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Dapper;

namespace DAL.Repositories {
    public class UserRepository {
        private readonly Database Database;

        public UserRepository(Database database) {
            Database = database;
        }

        private void EnsureOpen() {
            if (Database == null)
                throw new ArgumentNullException(nameof(Database));

            if (Database.Connection.State != ConnectionState.Open)
                Database.Connection.Open();
        }

        public IEnumerable<User> GetAll() {
            EnsureOpen();

            return Database.Connection.Query<User>("SELECT * FROM Users;");
        }

        public void AddOrIgnore(User u) {
            EnsureOpen();

            Database.Connection.Execute(@"INSERT OR IGNORE INTO
Users(Id, Username, Password, Admin, TeamId)
Values(@id, @name, @pw, @admin, @team);",
                new { id = u.Id, name = u.Username, pw = u.Password, admin = u.Admin, team = u.TeamId });
        }
    }
}