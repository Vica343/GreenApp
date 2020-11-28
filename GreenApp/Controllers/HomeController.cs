using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GreenApp.Models;
using Microsoft.AspNetCore.Identity;
using GreenApp.Model;

namespace GreenApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly UserManager<Guest> _userManager;
        public HomeController(IGreenService greenService, ApplicationState applicationState, UserManager<Guest> userManager)
            : base(greenService, applicationState)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var username = String.IsNullOrEmpty(User.Identity.Name) ? null : User.Identity.Name;
            if (username == null)
            {
                return View();
            }
            else
            {
                var guest = await _userManager.FindByNameAsync(User.Identity.Name);
                var roles = await _userManager.GetRolesAsync(guest);
                if (roles.Contains("companyAdmin"))
                {
                    return RedirectToAction("OwnCampaigns", "Challenges");
                } 
                else if (roles.Contains("superAdmin"))
                {
                    return RedirectToAction("Index", "Challenges");
                }
                return View();
            }
            
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
