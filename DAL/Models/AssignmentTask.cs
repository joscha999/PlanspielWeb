using DAL.Repositories;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models {
	public class AssignmentTask {
		public int? ID { get; set; }

		public string Name { get; set; }

		public DateTimeOffset UTCStart { get; set; }

		/// <summary>
		/// List of Actions on this Task, not stored in DB explicitly but always queried!
		/// </summary>
		[Write(false)]
		public IEnumerable<AssignmentAction> Actions { get; set; }

		public static void Setup(Database database) {
			database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [AssignmentTasks] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[Name] NVARCHAR(128) NOT NULL,
[UTCStart] INTEGER
);");
		}
	}
}