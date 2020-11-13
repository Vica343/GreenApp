using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Model;
using GreenApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GreenApp.Controllers
{
    public class CuponsController : BaseController
    {
        private readonly UserManager<Guest> _userManager;

        public CuponsController(IGreenService greenService, ApplicationState applicationState, UserManager<Guest> userManager)
            : base(greenService, applicationState)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View("AddCupon");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // védelem XSRF támadás ellen
        public async Task<IActionResult> Add(CuponViewModel cupon)
        {
            if (cupon == null)
                return RedirectToAction("Index", "Home");

            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!await _greenService.SaveCuponAsync(guest.UserName, cupon))
            {
                ModelState.AddModelError("", "A foglalás rögzítése sikertelen, kérem próbálja újra!");
                return View("Index");
            }

            ViewBag.Message = "A foglalását sikeresen rögzítettük!";
          
            return View("Result");
        }


    }
}