using DAL.DapperTypeHandlers;
using DAL.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace DAL {
    public class Database {
        public static Database Instance;

		public SqliteConnection Connection => Connections.Value;

		public PasswordHasher PasswordHasher { get; }

		private ThreadLocal<SqliteConnection> Connections;

		public Database(string dbPath) {
            if (Instance != null)
                throw new InvalidOperationException("Cannot create two Database instances!");

            Instance = this;

            PasswordHasher = new PasswordHasher();

			Connections = new ThreadLocal<SqliteConnection>(() => {
				var c = new SqliteConnection($"Data Source={dbPath};");
				c.Open();
				return c;
			});

			SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());

            //setup tables
            User.Setup(this, PasswordHasher);
            Team.Setup(this);
            SaveData.Setup(this);
			News.Setup(this);
			AssignmentAction.Setup(this);
			AssignmentTask.Setup(this);
			ProductDemandInfo.Setup(this);
			LoanInfo.Setup(this);
        }
    }
}