using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using PlanspielWeb.Attributes;
using PlanspielWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Controllers {
	public class NewsController : AppController {
		private const int NewsPageWidth = 6;
		private const int NewsPageHeight = 3;

		private readonly NewsRepository newsRepo;

		public NewsController(NewsRepository newsRepository) {
			newsRepo = newsRepository;
		}

		[Route("/News/{id:int?}")]
		public IActionResult Index(int? id) {
			if (id == null)
				id = newsRepo.GetLatestPageID(IsAdmin());

			var news = newsRepo.GetAllForPage(id.Value);
			if (!news.Any() && !IsAdmin())
				return NotFound();

			//filter out non-visible articles
			if (!IsAdmin())
				news = news.Where(n => n.Visible);

			var vm = new NewsViewModel {
				GridWidth = NewsPageWidth,
				GridHeight = NewsPageHeight,
				CanGoNext = newsRepo.DoesPageExist(id.Value + 1),
				CanGoPrevious = newsRepo.DoesPageExist(id.Value - 1),
				PageID = id.Value
			};

			if (!news.Any()) {
				vm.GridLayout = "";
				return View(vm);
			}

			var areaTemplate = "";

			var area = new News[NewsPageWidth, NewsPageHeight];
			foreach (var n in news) {
				for (int x = 0; x < n.Width; x++) {
					for (int y = 0; y < n.Height; y++) {
						area[x + n.X, y + n.Y] = n;
					}
				}

				vm.News.Add("a" + n.Id, n);
			}

			for (int y = 0; y < NewsPageHeight; y++) {
				var row = "";

				for (int x = 0; x < NewsPageWidth; x++) {
					row += "a" + (area[x, y] == null ? $"NA{x}{y}" : area[x, y].Id);

					if (x < NewsPageWidth - 1)
						row += " ";
				}

				areaTemplate += "'" + row + "'";
			}

			vm.GridLayout = areaTemplate;
			return View(vm);
		}

		[AdminOnly]
		public IActionResult Edit() {
			return View();
		}

		[AdminOnly]
		public IActionResult Details(int? id) {
			if (id == null)
				return NotFound();

			var news = newsRepo.GetById(id);
			if (news == null)
				return NotFound();

			return View(news);
		}

		[AdminOnly]
		public IActionResult Create() => View(new News());

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AdminOnly]
		public IActionResult Create([Bind("Id, Title, Content, RealDateTime, " +
			"PictureName, PageID, Visible, X, Y, Width, Height")] News news) {
			if (ModelState.IsValid) {
				newsRepo.AddOrIgnore(news);
				return RedirectToAction(nameof(Index));
			}

			return View(news);
		}

		[HttpGet]
		[AdminOnly]
		public IActionResult Edit(int? id) {
			if (id == null)
				return NotFound();

			var news = newsRepo.GetById(id);
			if (news == null)
				return NotFound();

			return View(news);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AdminOnly]
		public IActionResult Edit(int? id, [Bind("Id, Title, Content, RealDateTime, " +
			"PictureName, PageID, X, Y, Visible, Width, Height")] News news) {
			if (id != news.Id)
				return NotFound();

			if (ModelState.IsValid) {
				newsRepo.Update(news);
				return RedirectToAction(nameof(Index));
			}

			return View(news);
		}

		[AdminOnly]
		public IActionResult Delete(int? id) {
			if (id == null)
				return NotFound();

			var news = newsRepo.GetById(id);
			if (news == null)
				return NotFound();

			return View(news);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[AdminOnly]
		public IActionResult DeleteConfirmed(int? id) {
			newsRepo.Delete(id);
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		[AdminOnly]
		public IActionResult SetAllVisible(int? id) {
			if (id == null)
				return RedirectToAction(nameof(Index));

			foreach (var n in newsRepo.GetAllForPage(id.Value)) {
				if (n.Visible)
					continue;

				n.Visible = true;
				newsRepo.Update(n);
			}

			return RedirectToAction(nameof(Index), id);
		}
	}
}