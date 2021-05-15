using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models {
	public class AssignmentAction {
		public int? ID { get; set; }

		[JsonIgnore]
		public int TaskID { get; set; }

		/// <summary>
		/// The Group this Action should apply to, -1 for all, default = -1.
		/// </summary>
		public int GroupSteamID { get; set; } = -1;

		public int SecondsFromStart { get; set; }

		public AssignmentActionType Type { get; set; }

		public string Value { get; set; }

		public static void Setup(Database database) {
			database.Connection.Execute(@"CREATE TABLE IF NOT EXISTS [AssignmentActions] (
[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[TaskID] INTEGER NOT NULL,
[GroupSteamID] INTEGER NOT NULL,
[SecondsFromStart] INTEGER NOT NULL,
[Type] INTEGER NOT NULL,
[Value] NVARCHAR(256)
);");
		}
	}

	//NOTE: Never change these values while tasks are planned/running
	public enum AssignmentActionType {
		None = 0,

		//Control 1..99
		TaskStart = 1,
		TaskEnd = 2,
		Pause = 3,
		Unpause = 4,
		CreateSave = 5,
		DisplayMessage = 6,

		//Event Triggers 1000..
		IncreasedResearchSpeed = 1000,
		PollutionFine = 1001,
		Grant = 1002,
		Fine = 1003,
		Upkeep = 1004,
		TrainShipNetworkSpeedIncrease = 1005,
		TrainShipDispatchCost = 1006,
		Demand = 1007, //Value is int modifier
		BuildingCostIncrease = 1008,
	}
}