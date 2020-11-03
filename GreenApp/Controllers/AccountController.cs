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

			var result = await _signInManager.PasswordSignInAsync(user.UserName, user.UserPassword, user.RememberLogin, false);
			if (!result.Succeeded)
			{				
				ModelState.AddModelError("", "Hibás felhasználónév, vagy jelszó.");
				return View("Login", user);
			}	
			
			_applicationState.UserCount++; 
			return RedirectToAction("Index", "Home"); 
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
				PhoneNumber = user.GuestPhoneNumber
			};
			var result = await _userManager.CreateAsync(guest, user.UserPassword);
			var companyAdminRole = new IdentityRole<int>("companyAdmin");
			var result2 = _roleManager.CreateAsync(companyAdminRole).Result;
			var result3 = _userManager.AddToRoleAsync(guest, companyAdminRole.Name).Result;
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError("", error.Description);
				return View("Register", user);
			}

			await _signInManager.SignInAsync(guest, false); 
			_applicationState.UserCount++; 
			return RedirectToAction("Index", "Home"); 
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			_applicationState.UserCount--;
			return RedirectToAction("Index", "Home");
		}
	}
}
