using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Dapper;
using System.Linq;

namespace DAL.Repositories {
    public class UserRepository {
        private readonly Database Database;
        private readonly PasswordHasher Hasher;

        public UserRepository(Database database, PasswordHasher hasher) {
            Database = database;
            Hasher = hasher;
        }

        private void EnsureOpen() {
            if (Database == null)
                throw new ArgumentNullException(nameof(Database));

            if (Database.Connection.State != ConnectionState.Open)
                Database.Connection.Open();
        }

        public User CheckLogin(string username, string password) {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = GetByUsername(username);
            if (user == null)
                return null;

            if (!Hasher.CheckHash(password, user.Password))
                return null;

            return user;
        }

        public IEnumerable<User> GetAll() {
            EnsureOpen();

            return Database.Connection.Query<User>("SELECT * FROM Users;");
        }

        public User GetById(int? id) {
            if (id == null)
                return null;

            EnsureOpen();

            return Database.Connection.Query<User>("SELECT * FROM Users WHERE id = @id;", new { id = id.Value }).FirstOrDefault();
        }

        public User GetByUsername(string username) {
            if (string.IsNullOrEmpty(username))
                return null;

            EnsureOpen();

            return Database.Connection.Query<User>("SELECT * FROM Users WHERE Username = @uname;", new { uname = username }).FirstOrDefault();
        }

        public void AddOrIgnore(User u) {
            EnsureOpen();

            string sql;
            if (string.IsNullOrEmpty(u.Password)) {
                sql = @"INSERT OR IGNORE INTO
Users(Id, Username, Admin, TeamId)
Values(@id, @name, @admin, @team);";
            } else {
                u.Password = Hasher.Hash(u.Password);
                sql = @"INSERT OR IGNORE INTO
Users(Id, Username, Password, Admin, TeamId)
Values(@id, @name, @pw, @admin, @team);";
            }

            Database.Connection.Execute(sql,
                new { id = u.Id, name = u.Username, pw = u.Password, admin = u.Admin, team = u.TeamId });
        }

        public void Update(User u) {
            EnsureOpen();

            string sql;
            if (string.IsNullOrEmpty(u.Password)) {
                sql = @"UPDATE Users SET
Username = @name, Admin = @admin, TeamId = @team
WHERE Id = @id;";
            } else {
                u.Password = Hasher.Hash(u.Password);
                sql = @"UPDATE Users SET
Username = @name, Password = @pw, Admin = @admin, TeamId = @team
WHERE Id = @id;";
            }

            Database.Connection.Execute(sql,
                new { id = u.Id, name = u.Username, pw = u.Password, admin = u.Admin, team = u.TeamId });
        }

        public void Delete(int? id) {
            if (id == null)
                return;

            EnsureOpen();

            Database.Connection.Execute("DELETE FROM Users WHERE Id = @id;", new { id = id.Value });
        }
    }
}