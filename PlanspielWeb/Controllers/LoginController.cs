using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using PlanspielWeb.Models;

namespace PlanspielWeb.Controllers
{
    public class LoginController : AppController
    {
        private readonly UserRepository users;

        public LoginController(UserRepository userRepository) {
            users = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginViewModel login) {
            var user = users.CheckLogin(login.Username, login.Password);
            if (user == null) {
                AddError("Nutzername oder Passwort falsch!");
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.Put("user", user);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout() {
            HttpContext.Session.Remove("user");
            return RedirectToAction("Index", "Home");
        }
    }
}