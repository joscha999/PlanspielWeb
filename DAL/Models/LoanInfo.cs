using Dapper;
using Planspiel.Models;
using System;

namespace DAL.Models {
	public class LoanInfo {
		public int? ID { get; set; }
		public float LoanAmount { get; set; }
		public float LoanInterest { get; set; }

		public LoanInfoModel ToModel() => new LoanInfoModel {
			LoanAmount = LoanAmount,
			LoanInterest = LoanInterest
		};

		public static void Setup(Database database) {
			database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [LoanInfo] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[SaveDataId] INTEGER NOT NULL,
[LoanAmount] REAL NOT NULL,
[LoanInterest] REAL NOT NULL
);");
		}
	}
}