using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Models {
	public class AssignmentActionViewModel {
		public int? ID { get; set; }
		public int TaskID { get; set; }
		public int GroupSteamID { get; set; } = -1;
		public int SecondsFromStart { get; set; }
		public int AType { get; set; }
		public string Payload { get; set; }

		public bool CanChange
			=> (AssignmentActionType)AType != AssignmentActionType.TaskStart
			&& (AssignmentActionType)AType != AssignmentActionType.TaskEnd;

		public AssignmentAction ToDAL() => new() {
			ID = ID,
			TaskID = TaskID,
			GroupSteamID = GroupSteamID,
			SecondsFromStart = SecondsFromStart,
			Type = (AssignmentActionType)AType,
			Value = Payload
		};
	}
}