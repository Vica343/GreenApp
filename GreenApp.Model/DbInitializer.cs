using GreenApp.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Data;

namespace GreenApp.Models
{
    public static class DbInitializer
    {
        private static GreenAppContext _context;
		private static UserManager<Guest> _userManager;
		private static RoleManager<IdentityRole<int>> _roleManager;

		public static void Initialize(IServiceProvider serviceProvider)
        {
            _context = serviceProvider.GetRequiredService<GreenAppContext>();
			_userManager = serviceProvider.GetRequiredService<UserManager<Guest>>();
			_roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

			_context.Database.Migrate();

			if (!_context.Challenges.Any())
			{
				SeedChallenges();
			}

			if (!_context.Users.Any())
			{
				SeedUsers();
			}	
		}
		private static void SeedUsers()
		{
			var adminUser = new Guest
			{
				UserName = "admin",
				FirstName = "Adminisztrátor",
				LastName = "Admin",
				Email = "admin@example.com",
				PhoneNumber = "+36123456789",
				Company = "Company"
			};
			var adminPassword = "Almafa123";
			var adminRole = new IdentityRole<int>("superAdmin");
			var companyAdminRole = new IdentityRole<int>("companyAdmin");
			var appUserRole = new IdentityRole<int>("appUser");

			var result1 = _userManager.CreateAsync(adminUser, adminPassword).Result;
			var result2 = _roleManager.CreateAsync(adminRole).Result;
			var result3 = _roleManager.CreateAsync(companyAdminRole).Result;
			var result4 = _roleManager.CreateAsync(appUserRole).Result;
			var result5 = _userManager.AddToRoleAsync(adminUser, adminRole.Name).Result;
		}

		private static void SeedChallenges()
		{
			var challenges = new Challenge[]
			{
				new Challenge
				{
					Name = "Szemétszedés",
					Description = "Szedj össze egy zsák szemetet",
					StartDate = new DateTime(2020,11,20),
					EndDate = new DateTime(2021,11,20),
					Type = ChallengeType.QRCode,
					Reward = RewardType.Cupon,
					Status = StatusType.Accepted
				},
				new Challenge
				{
					Name = "Szemétszedés2",
					Description = "Szedj össze egy zsák szemetet",
					StartDate = new DateTime(2020,10,20),
					EndDate = new DateTime(2021,10,20),
					Type = ChallengeType.Upload,
					Reward = RewardType.Cupon,
					Status = StatusType.Accepted
				},
				new Challenge
				{
					Name = "Szemétszedés3",
					Description = "Szedj össze egy zsák szemetet",
					StartDate = new DateTime(2020,12,20),
					EndDate = new DateTime(2021,12,20),
					Type = ChallengeType.Upload,
					Reward = RewardType.Cupon,
					Status = StatusType.Accepted
				},
				new Challenge
				{
					Name = "Szemétszedés4",
					Description = "Szedj össze egy zsák szemetet",
					StartDate = new DateTime(2020,11,30),
					EndDate = new DateTime(2021,11,30),
					Type = ChallengeType.Upload,
					Reward = RewardType.Cupon,
					Status = StatusType.Accepted
				},
			};

			foreach (Challenge c in challenges)
			{
				_context.Challenges.Add(c);
			}

			_context.SaveChanges();
		}

	}
}
