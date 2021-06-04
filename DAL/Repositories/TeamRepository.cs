using DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL.Repositories {
    public class TeamRepository {
        private readonly Database Database;

        public TeamRepository(Database database) {
            Database = database;
        }

        private void EnsureOpen() {
            if (Database == null)
                throw new ArgumentNullException(nameof(Database));

            if (Database.Connection.State != ConnectionState.Open)
                Database.Connection.Open();
        }

        public IEnumerable<Team> GetAll() {
            EnsureOpen();

            return Database.Connection.Query<Team>("SELECT * FROM Teams;");
        }

        public Team GetById(int? id) {
            if (id == null)
                return null;

            EnsureOpen();

            return Database.Connection.Query<Team>("SELECT * FROM Teams WHERE Id = @id;", new { id }).FirstOrDefault();
        }

        public IEnumerable<User> GetMembersForId(int? id) {
            if (id == null)
                return new List<User>();

            EnsureOpen();

            return Database.Connection.Query<User>("SELECT * FROM Users WHERE TeamId = @id;", new { id });
        }

        public void AddOrIgnore(Team t) {
            EnsureOpen();

			Database.Connection.Execute(@"INSERT OR IGNORE INTO
Teams(Id, Name, SteamID)
Values(@id, @name, @sid);",
				new {
					id = t.Id,
					name = t.Name,
					sid = t.SteamID
				});
        }

        public void Update(Team t) {
            EnsureOpen();

			Database.Connection.Execute(@"UPDATE Teams SET
Name = @name, SteamId = @sid
WHERE Id = @id",
				new {
					id = t.Id,
					name = t.Name,
					sid = t.SteamID
				});
        }

        public void Delete(int? id) {
            if (id == null)
                return;

            EnsureOpen();

            Database.Connection.Execute("DELETE FROM Teams WHERE Id = @id;", new { id = id.Value });
        }
    }
}