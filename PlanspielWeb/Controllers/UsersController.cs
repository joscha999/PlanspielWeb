using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.Repositories;
using PlanspielWeb.Attributes;

namespace PlanspielWeb.Controllers {
    [AdminOnly]
    public class UsersController : AppController {
        private readonly UserRepository users;

        public UsersController(UserRepository userRepository) {
            users = userRepository;
        }

        public IActionResult Index() => View(users.GetAll());

        public IActionResult Details(int? id) {
            if (id == null)
                return NotFound();

            var user = users.GetById(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Username,Password,Admin,TeamId")] User user) {
            if (ModelState.IsValid) {
                users.AddOrIgnore(user);
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        public IActionResult Edit(int? id) {
            if (id == null)
                return NotFound();

            var user = users.GetById(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, [Bind("Id,Username,Password,Admin,TeamId")] User user) {
            if (id != user.Id)
                return NotFound();

            if (ModelState.IsValid) {
                users.Update(user);
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        public IActionResult Delete(int? id) {
            if (id == null)
                return NotFound();

            var user = users.GetById(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id) {
            users.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}