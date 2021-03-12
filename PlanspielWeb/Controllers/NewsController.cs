using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using PlanspielWeb.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanspielWeb.Controllers {
	public class NewsController : AppController {
		private readonly NewsRepository newsRepo;

		public NewsController(NewsRepository newsRepository) {
			newsRepo = newsRepository;
		}

		//TODO: this may be too long, maybe set max or paginate
		public IActionResult Index() => View(newsRepo.GetAllTimeOrdered().Take(20));

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
		public IActionResult Create([Bind("Id, Title, Content, RealDateTime, PictureName")] News news) {
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
		public IActionResult Edit(int? id, [Bind("Id, Title, Content, RealDateTime, PictureName")] News news) {
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
	}
}