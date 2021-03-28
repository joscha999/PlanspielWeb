﻿using DAL.Models;
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

		public IEnumerable<News> GetAll() {
			EnsureOpen();

			return Database.Connection.Query<News>("SELECT * FROM News;");
		}

		public IEnumerable<News> GetAllTimeOrdered() => GetAll().OrderByDescending(n => n.RealDateTime);

		public News GetById(int? id) {
			if (id == null)
				return null;

			EnsureOpen();

			return Database.Connection.Query<News>("SELECT * FROM News WHERE Id = @id;", new { id }).FirstOrDefault();
		}

		public void AddOrIgnore(News n) {
			EnsureOpen();

			Database.Connection.Execute(@"INSERT OR IGNORE INTO
News(Id, Title, Content, RealDateTime, PictureName, PageID, X, Y, Width, Height)
Values(@id, @title, @content, @realDateTime, @pictureName, @pageID, @x, @y, @width, @height);",
				new {
					id = n.Id,
					title = n.Title,
					content = n.Content,
					realDateTime = n.RealDateTime,
					pictureName = n.PictureName,
					pageID = n.PageID,
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
PageID = @pageID, X = @x, Y = @y, Width = @width, Height = @height
WHERE Id = @id",
				new {
					id = n.Id,
					title = n.Title,
					content = n.Content,
					realDateTime = n.RealDateTime,
					pictureName = n.PictureName,
					pageID = n.PageID,
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