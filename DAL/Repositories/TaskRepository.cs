using DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories {
	public class TaskRepository {
		private readonly Database Database;

		public TaskRepository(Database database) {
			Database = database;
		}

		private void EnsureOpen() {
			if (Database == null)
				throw new ArgumentNullException(nameof(Database));

			if (Database.Connection.State != ConnectionState.Open)
				Database.Connection.Open();
		}

		public AssignmentTask GetById(int id, bool queryAll = false) {
			EnsureOpen();

			var task = Database.Connection.Query<AssignmentTask>(
				"SELECT * FROM AssignmentTasks WHERE Id = @id", new { id }).FirstOrDefault();

			if (task == null)
				return task;

			if (task.ID != null)
				task.Actions = GetActionsForTask(task.ID.Value, queryAll ? -2 : -1);

			return task;
		}

		public AssignmentAction GetActionById(int id) {
			EnsureOpen();

			return Database.Connection.Query<AssignmentAction>(
				"SELECT * FROM AssignmentActions WHERE Id = @id", new { id }).FirstOrDefault();
		}

		public IEnumerable<AssignmentAction> GetActionsForTask(int taskId, long groupSteamID = -1) {
			EnsureOpen();

			//admin - query all
			if (groupSteamID == -2) {
				return Database.Connection.Query<AssignmentAction>(
					"SELECT * FROM AssignmentActions WHERE TaskID = @taskId", new { taskId });
			}

			return Database.Connection.Query<AssignmentAction>(
				"SELECT * FROM AssignmentActions WHERE TaskID = @taskId "
				+ "AND (GroupSteamID = -1 OR GroupSteamID = @groupSteamID)",
				new { taskId, groupSteamID });
		}

		public IEnumerable<AssignmentTask> GetAll(long groupSteamID, bool includeDisabled = false) {
			EnsureOpen();

			var sql = "SELECT * FROM AssignmentTasks";
			sql += includeDisabled ? "" : " WHERE Enabled = true";

			foreach (var task in Database.Connection.Query<AssignmentTask>(sql)) {
				if (task.ID == null)
					continue;

				task.Actions = GetActionsForTask(task.ID.Value, groupSteamID);
				yield return task;
			}
		}

		public void AddOrIgnore(AssignmentTask task) {
			EnsureOpen();

			Database.Connection.Execute(@"INSERT OR IGNORE INTO
AssignmentTasks(Id, Name, UTCStart, Enabled)
Values(@id, @name, @utcStart, @enabled)",
				new {
					id = task.ID,
					name = task.Name,
					utcStart = task.UTCStart,
					enabled = task.Enabled
				});
		}

		public void AddOrIgnore(AssignmentAction action) {
			EnsureOpen();

			Database.Connection.Execute(@"INSERT OR IGNORE INTO 
AssignmentActions(Id, TaskID, GroupSteamID, SecondsFromStart, Type, Value)
Values(@id, @taskID, @groupSteamID, @secondsFromStart, @type, @value)",
				new {
					id = action.ID,
					taskID = action.TaskID,
					groupSteamID = action.GroupSteamID,
					secondsFromStart = action.SecondsFromStart,
					type = action.Type,
					value = action.Value
				});
		}

		public void Update(AssignmentTask task) {
			Database.Connection.Execute(@"UPDATE AssignmentTasks SET
Id = @id, Name = @name, UTCStart = @utcStart, Enabled = @enabled WHERE Id = @id",
				new {
					id = task.ID,
					name = task.Name,
					utcStart = task.UTCStart,
					enabled = task.Enabled
				});

			if (task.Actions != null) {
				foreach (var action in task.Actions)
					Update(action);
			}
		}

		public void Update(AssignmentAction action) {
			EnsureOpen();

			Database.Connection.Execute(@"UPDATE AssignmentActions SET
Id = @id, TaskID = @taskID, GroupSteamID = @groupSteamID, SecondsFromStart = @secondsFromStart,
Type = @type, Value = @value WHERE Id = @id",
				new {
					id = action.ID,
					taskID = action.TaskID,
					groupSteamID = action.GroupSteamID,
					secondsFromStart = action.SecondsFromStart,
					type = action.Type,
					value = action.Value
				});
		}

		public void DeleteTask(AssignmentTask task) {
			if (task == null)
				return;

			EnsureOpen();

			Database.Connection.Execute("DELETE FROM AssignmentTasks WHERE Id = @id", new { id = task.ID });

			foreach (var action in task.Actions)
				DeleteAction(action.ID);
		}

		public void DeleteAction(int? id) {
			if (id == null)
				return;

			EnsureOpen();

			Database.Connection.Execute("DELETE FROM AssignmentActions WHERE Id = @id", new { id = id.Value });
		}
	}
}
