using Dapper;
using Planspiel.Models;
using System;

namespace DAL.Models {
	public class ProductDemandInfo {
		public int? ID { get; set; }
		public string ProductName { get; set; }
		public string Settlement { get; set; }
		public string Shop { get; set; }
		public int Demand { get; set; }
		public int Sales { get; set; }

		public ProductDemandInfoModel ToModel() => new ProductDemandInfoModel {
			Demand = Demand,
			ProductName = ProductName,
			Sales =	Sales,
			Settlement = Settlement,
			Shop = Shop
		};

		public static void Setup(Database database) {
			database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [ProductDemandInfo] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[SaveDataId] INTEGER NOT NULL,
[ProductName] NVARCHAR(255) NOT NULL,
[Settlement] NVARCHAR(255) NOT NULL,
[Shop] NVARCHAR(255) NOT NULL,
[Demand] INTEGER NOT NULL,
[Sales] INTEGER NOT NULL
);");
		}
	}
}