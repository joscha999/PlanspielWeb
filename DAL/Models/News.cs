using DAL.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models {
	public class News {
		private static NewsRepository newsRepository;

		public int? Id { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public DateTime RealDateTime { get; set; } = DateTime.Now;

		public string PictureName { get; set; }

		public int PageID { get; set; }

		public int X { get; set; }
		public int Y { get; set; }

		public int Width { get; set; }
		public int Height { get; set; }

		public static void Setup(Database database) {
			newsRepository = new NewsRepository(database);

			database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [News] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[Title] NVARCHAR(255) NOT NULL,
[Content] NVARCHAR(2048) NOT NULL,
[RealDateTime] NVARCHAR(64) NOT NULL,
[PictureName] NVARCHAR(255),
[PageID] INTEGER NOT NULL,
[X] INTEGER NOT NULL,
[Y] INTEGER NOT NULL,
[Width] INTEGER NOT NULL,
[Height] INTEGER NOT NULL
);");
		}
	}
}