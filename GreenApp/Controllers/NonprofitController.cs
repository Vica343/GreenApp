using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Model;
using GreenApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GreenApp.Controllers
{
    [Authorize(Roles = "superAdmin")]
    public class NonprofitController : BaseController
    {
        private readonly UserManager<Guest> _userManager;

        public NonprofitController(IGreenService greenService, ApplicationState applicationState, UserManager<Guest> userManager)
            : base(greenService, applicationState)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var nonprofits = _greenService.Nonprofits.ToList();
            if (nonprofits.Count == 0)
            {
                TempData["Info"] = "Nincs ilyen nonprofit.";
            }

            return View("index", nonprofits);
        }

        [HttpGet]
        public IActionResult AddNonprofit(NonprofitViewModel nonprofit)
        {
            return View("AddNonprofit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(string SearchString)
        {
            var nonprofits =  _greenService.SearchNonprofit(SearchString).ToList();
            if (nonprofits.Count == 0)
            {
                TempData["Info"] = "Nincs ilyen nonprofit.";
            }
            return View("Index", nonprofits);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(NonprofitViewModel nonprofit)
        {
            if (nonprofit == null)
                return RedirectToAction("Index", "Home");

            if (!await _greenService.SaveNonprofitAsync(nonprofit))
            {
                TempData["ErrorMessage"] = "A nonprofit hozzáadása sikertelen, kérem próbálja újra!";
                return RedirectToAction("Index", "Nonprofit");
            }

            TempData["Success"] = "A nonprofit sikeresen hozzáadva!";

            return RedirectToAction("Index", "Nonprofit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Int32? id)
        {
            if (!await _greenService.DeleteNonprofitAsync(id))
            {
                TempData["ErrorMessage"] = "A nonprofit törlése sikertelen, kérem próbálja újra!";
                return RedirectToAction("Index", "Nonprofit");
            }

            TempData["Success"] = "A nonprofit törlése sikeres!";

            return RedirectToAction("Index", "Nonprofit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(Int32? id)
        {
            if (!await _greenService.DisableNonprofitAsync(id))
            {
                TempData["ErrorMessage"] = "A nonprofit letiltása sikertelen, próbálja újra!";
                return RedirectToAction("Index", "Nonprofit");
            }

            TempData["Success"] = "A nonprofit sikeresen letiltva!";

            return RedirectToAction("Index", "Nonprofit");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enable(Int32? id)
        {
            if (!await _greenService.EnableNonprofitAsync(id))
            {
                TempData["ErrorMessage"] = "A nonprofit engedélyezése sikertelen, próbálja újra!";
                return RedirectToAction("Index", "Nonprofit");
            }

            TempData["Success"] = "A nonprofit sikeresen engedélyezve!";

            return RedirectToAction("Index", "Nonprofit");
        }


        public ActionResult NonprofitImage(Int32? id)
        {
            Byte[] imageContent = _greenService.GetNonprofitImage(id);

            if (imageContent == null)
            {
                return Content("No file name provided");
            }

            return File(imageContent, "image/png");
        }

    }
}