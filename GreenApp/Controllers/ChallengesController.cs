using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Model;
using GreenApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GreenApp.Controllers
{
    public class ChallengesController : BaseController
    {
        private readonly UserManager<Guest> _userManager;

        public ChallengesController(IGreenService greenService, ApplicationState applicationState, UserManager<Guest> userManager)
            : base(greenService, applicationState)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // védelem XSRF támadás ellen
        public async Task<IActionResult> Add(ChallangeViewModel challange)
        {
            if (challange == null)
                return RedirectToAction("Index", "Home");

            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!await _greenService.SaveChallengeAsync(guest.UserName, challange))
            {
                ModelState.AddModelError("", "A foglalás rögzítése sikertelen, kérem próbálja újra!");
                return View("Index");
            }

            ViewBag.Message = "A foglalását sikeresen rögzítettük!";
            return View("Result");
        }
    }
}
