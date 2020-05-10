using DAL.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAL {
    public class Database {
        public static Database Instance;

        public SqliteConnection Connection { get; }
        public PasswordHasher PasswordHasher { get; }

        public Database(string dbPath) {
            if (Instance != null)
                throw new InvalidOperationException("Cannot create two Database instances!");

            Instance = this;

            PasswordHasher = new PasswordHasher();

            Connection = new SqliteConnection($"Data Source={dbPath};");
            Connection.Open();

            //setup tables
            User.Setup(this, PasswordHasher);
            Team.Setup(this);
            SaveData.Setup(this);
        }
    }
}