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
    public class AccountController : BaseController
    {
		private readonly UserManager<Guest> _userManager;		
		private readonly SignInManager<Guest> _signInManager;
		private static RoleManager<IdentityRole<int>> _roleManager;

		public AccountController(IGreenService greenService, ApplicationState applicationState,
			UserManager<Guest> userManager, SignInManager<Guest> signInManager, RoleManager<IdentityRole<int>> roleManager)
			: base(greenService, applicationState)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		public IActionResult Index()
		{
			return RedirectToAction("Login");
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View("Login");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel user)
		{
			if (!ModelState.IsValid)
				return View("Login", user);

			var result1 = await _userManager.FindByNameAsync(user.UserName);
			if (result1 == null)
			{
				ModelState.AddModelError("", "Hibás felhasználónév, vagy jelszó.");
				return View("Login", user);
			}

			if (result1.Status == Data.StatusType.Declined)
			{
				TempData["ErrorMessage"] = "Ez a fiók le lett tiltva.";
				return View("Result");
			} 
			else if (result1.Status == Data.StatusType.Pending)
			{
				TempData["Info"] = "Ez a fiók még jóváhagyásra vár.";
				return View("Result");
			}

			var userRoles = await _userManager.GetRolesAsync(result1);
			if (!userRoles.Contains("companyAdmin") && !userRoles.Contains("superAdmin"))
			{
				ModelState.AddModelError("", "Csak admin jelentkezhet be.");
				return View("Login", user);
			}			
			
			var result2 = await _signInManager.PasswordSignInAsync(user.UserName, user.UserPassword, user.RememberLogin, false);
			if (!result2.Succeeded)
			{				
				ModelState.AddModelError("", "Hibás felhasználónév, vagy jelszó.");
				return View("Login", user);
			}

			_applicationState.UserCount++;
			if (userRoles.Contains("companyAdmin"))
			{
				return RedirectToAction("OwnCampaigns", "Challenges");
			}
			return RedirectToAction("Index", "Challenges");
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View("Register");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegistrationViewModel user)
		{
			if (!ModelState.IsValid)
				return View("Register", user);

			Guest guest = new Guest
			{
				UserName = user.UserName,
				Email = user.GuestEmail,
				FirstName = user.GuestFirstName,
				LastName = user.GuestLastName,
				Company = user.GuestCompany,
				PhoneNumber = user.GuestPhoneNumber,
				Status = Data.StatusType.Pending
			};
			var result = await _userManager.CreateAsync(guest, user.UserPassword);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError("", error.Description);
				return View("Register", user);
			}
			var companyAdminRole = await _roleManager.FindByNameAsync("companyAdmin");
			var result3 = _userManager.AddToRoleAsync(guest, companyAdminRole.Name).Result;
			if (!result3.Succeeded)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError("", error.Description);
				return View("Register", user);
			}

			TempData["Info"] = "Ez a fiók még jóváhagyásra vár.";
			return View("Result");
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			_applicationState.UserCount--;
			return RedirectToAction("Index", "Home");
		}


		[Authorize(Roles = "superAdmin")]
		[HttpGet]
		public async Task<IActionResult> ListCompanyAdmins()
		{

			var users = await _greenService.CompanyAdminsAsync();
			users.ToList();

			return View("CompanyAdmins", users);
		}

		[Authorize(Roles = "superAdmin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Enable(Int32? id)
		{
			if(! await _greenService.EnableUserAsync(id))
			{
				TempData["ErrorMessage"] = "A felhasználó engedélyezése sikertelen, próbálja újra!";
			}

			TempData["Success"] = "A felhasználó sikeresen engedélyezve!";
			return RedirectToAction("ListCompanyAdmins", "Account");
		}

		[Authorize(Roles = "superAdmin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Disable(Int32? id)
		{
			if (!await _greenService.DisableUserAsync(id))
			{
				TempData["ErrorMessage"] = "A felhasználó letiltása sikertelen, próbálja újra!";
			}

			TempData["Success"] = "A felhasználó sikeresen letiltva!";
			return RedirectToAction("ListCompanyAdmins", "Account");
		}


	



	}
}
