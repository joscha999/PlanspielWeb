using DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL.Repositories {
	public class NewsRepository {
		private readonly Database Database;

		public NewsRepository(Database database) {
			Database = database;
		}

		private void EnsureOpen() {
			if (Database == null)
				throw new ArgumentNullException(nameof(Database));

			if (Database.Connection.State != ConnectionState.Open)
				Database.Connection.Open();
		}

		public IEnumerable<News> GetAllForPage(int pageID) {
			EnsureOpen();

			return Database.Connection.Query<News>("SELECT * FROM News WHERE PageID = @pageID;", new { pageID });
		}

		public int GetLatestPageID() {
			EnsureOpen();

			var result = Database.Connection.Query<News>("SELECT * FROM News ORDER BY PageID DESC LIMIT 1");
			if (!result.Any())
				return 0;

			return result.First().PageID;
		}

		public bool DoesPageExist(int pageID) {
			if (pageID < 0)
				return false;

			EnsureOpen();

			return Database.Connection.ExecuteScalar<bool>(
				"SELECT Count(1) FROM News WHERE PageID = @pageID", new { pageID });
		}

		public News GetById(int? id) {
			if (id == null)
				return null;

			EnsureOpen();

			return Database.Connection.Query<News>("SELECT * FROM News WHERE Id = @id;",
				new { id }).FirstOrDefault();
		}

		public void AddOrIgnore(News n) {
			EnsureOpen();

			Database.Connection.Execute(@"INSERT OR IGNORE INTO
News(Id, Title, Content, RealDateTime, PictureName, PageID, Visible, X, Y, Width, Height)
Values(@id, @title, @content, @realDateTime, @pictureName, @pageID, @visible, @x, @y, @width, @height);",
				new {
					id = n.Id,
					title = n.Title,
					content = n.Content,
					realDateTime = n.RealDateTime,
					pictureName = n.PictureName,
					pageID = n.PageID,
					visible = n.Visible,
					x = n.X,
					y = n.Y,
					width = n.Width,
					height = n.Height
				});
		}

		public void Update(News n) {
			EnsureOpen();

			Database.Connection.Execute(@"UPDATE News SET
Title = @title, Content = @content, RealDateTime = @realDateTime, PictureName = @pictureName,
PageID = @pageID, Visible = @visible, X = @x, Y = @y, Width = @width, Height = @height
WHERE Id = @id",
				new {
					id = n.Id,
					title = n.Title,
					content = n.Content,
					realDateTime = n.RealDateTime,
					pictureName = n.PictureName,
					pageID = n.PageID,
					visible = n.Visible,
					x = n.X,
					y = n.Y,
					width = n.Width,
					height = n.Height
				});
		}

		public void Delete(int? id) {
			if (id == null)
				return;

			EnsureOpen();

			Database.Connection.Execute("DELETE FROM News WHERE Id = @id;", new { id = id.Value });
		}
	}
}