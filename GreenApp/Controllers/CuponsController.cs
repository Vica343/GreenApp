﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Model;
using GreenApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GreenApp.Controllers
{
    [Authorize(Roles = "companyAdmin")]
    public class CuponsController : BaseController
    {
        private readonly UserManager<Guest> _userManager;

        public CuponsController(IGreenService greenService, ApplicationState applicationState, UserManager<Guest> userManager)
            : base(greenService, applicationState)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);
            var cupons = _greenService.GetOwnCupons(guest.Id).ToList();

            if (cupons.Count == 0)
            {
                TempData["Info"] = "Jelenleg nincsenek saját kuponok.";
            }

            return View("Index", cupons);
        }

        [HttpGet]
        public IActionResult AddCupon(CuponViewModel cupon)
        {
            return View("AddCupon");
        }

        [Authorize(Roles = "companyAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string searchString)
        {
            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);
            var searchresult = _greenService.SearchCupon(guest.Id, searchString).ToList();

            if (searchresult.Count == 0)
            {
                TempData["Info"] = "Nincs ilyen kupon.";
            }

            return View("Index", searchresult);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Int32? cuponId)
        {
            var cupon = _greenService.GetCuponById(cuponId);
            var stream = new MemoryStream(cupon.Image);
            IFormFile file = new FormFile(stream, 0, cupon.Image.Length, "name", "fileName");

            CuponViewModel cuponview = new CuponViewModel
            {
                CuponName = cupon.Name,
                CuponValue = cupon.Value,
                CuponStartDate = cupon.StartDate,
                CuponEndDate = cupon.EndDate,
                CuponImage = file
            };

            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);    

            ViewBag.image = _greenService.GetCupon(cuponview).Id;

            return View("Edit", cuponview);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CuponViewModel cupon, Int32? id)
        {
            if (cupon == null)
                return RedirectToAction("Index", "Cupons");

            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);

            string inputText = Request.Scheme + "://" + Request.Host.Value + "/api/Challenges/QR/";

            if (!await _greenService.UpdateCuponAsync(guest.UserName, cupon, id))
            {
                TempData["ErrorMessage"] = "A kupon módosítása sikertelen, kérem próbálja újra!";
                return View("CompanyAdmin/AddChallenge");
            }

            TempData["Success"] = "A kupon sikeresen módosítva!";

            return RedirectToAction("Index", "Cupons");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Int32? id)
        {
            if (!await _greenService.DeleteCuponAsync(id))
            {
                TempData["ErrorMessage"] = "A kupon törlése sikertelen! Ellenőrizd, hogy nincs-e a kupon kihíváshoz rendelve!";
                return RedirectToAction("Index", "Cupons");
            }

            TempData["Success"] = "A kupon törlése sikeres!";

            return RedirectToAction("Index", "Cupons");
        }


        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Add(CuponViewModel cupon)
        {
            if (cupon == null)
                return RedirectToAction("Index", "Home");

            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!await _greenService.SaveCuponAsync(guest.UserName, cupon))
            {
                TempData["ErrorMessage"] = "A kupon létrehozása sikertelen, kérem próbálja újra!";
                return RedirectToAction("Index", "Cupons");
            }

            TempData["Success"] = "A kupon sikeresen hozzáadva!";

            return RedirectToAction("Index", "Cupons");
        }

        public ActionResult CuponImage(Int32? cuponid)
        {
            Byte[] imageContent = _greenService.GetCuponImage(cuponid);

            if (imageContent == null)
            {
                return Content("No file name provided");
            }

            return File(imageContent, "image/png");
        }


    }
}