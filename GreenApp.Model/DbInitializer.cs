using GreenApp.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Data;
using System.IO;

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

			if (!_context.Users.Any())
			{
				SeedUsers();
			}

			if (!_context.Challenges.Any())
			{
				SeedChallenges();
			}

			if (!_context.Cupons.Any())
			{
				SeedCupons();
			}

			if (!_context.Nonprofits.Any())
			{
				SeedNonprofit();
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

			var User1 = new Guest
			{
				UserName = "Bela1",
				FirstName = "Béla",
				LastName = "Kiss",
				Email = "kissbela@example.com",
				PhoneNumber = "+36123456789",
				Status = StatusType.Accepted,
				Company = "Running Kft."
			};
			var User1Password = "Bela1234";
			var result6 = _userManager.CreateAsync(User1, User1Password).Result;
			var result7 = _userManager.AddToRoleAsync(User1, companyAdminRole.Name).Result;

			var User2 = new Guest
			{
				UserName = "Anna1",
				FirstName = "Anna",
				LastName = "Nagy",
				Email = "nagyanna@example.com",
				Status = StatusType.Accepted,
				PhoneNumber = "+36123456789",
				Company = "Zöld jövő Zrt."
			};
			var User2Password = "Anna1234";
			var result8 = _userManager.CreateAsync(User2, User2Password).Result;
			var result9 = _userManager.AddToRoleAsync(User2, companyAdminRole.Name).Result;

			var User3 = new Guest
			{
				UserName = "Andris1",
				FirstName = "Andris",
				LastName = "Kovács",
				Email = "andris@example.com"
			};
			var User3Password = "Andris1234";
			var result10 = _userManager.CreateAsync(User3, User3Password).Result;
			var result11 = _userManager.AddToRoleAsync(User3, appUserRole.Name).Result;

		}

		private static void SeedChallenges()
		{
			var image1 = File.ReadAllBytes("Images/trash.jpg");

			var challenges = new Challenge[]
			{			
				new Challenge
				{
					Name = "Szemétszedés a parkban",
					Description = "Szedj össze egy zsák szemetet a parkban! Nincs annál rosszabb látvány, mint amikor a természedben sétálsz és minden teli van szeméttel. Gyűjts össze egy zsákkal és tölts fel róla fényképet!",
					StartDate = new DateTime(2020,12,20),
					EndDate = new DateTime(2021,01,20),
					Type = ChallengeType.Upload,
					Reward = RewardType.Cupon,
					RewardValue = 1,
					Disabled = false,
					CreatorId = 2,
					Image = image1
				}				
			};

			foreach (Challenge c in challenges)
			{
				_context.Challenges.Add(c);
			}

			_context.SaveChanges();
		}

		private static void SeedCupons()
		{
			var image1 = File.ReadAllBytes("Images/cupon1.jpg");

			var cupons = new Cupon[]
			{
				new Cupon
				{
					Name = "Kedvezmény a zoldjovo.hu webáruházba",
					StartDate = new DateTime(2020,12,20),
					EndDate = new DateTime(2021,05,20),
					CreatorId = 2,
					Value = "QWE3243WA3",
					Image = image1

				}
			};

			foreach (Cupon c in cupons)
			{
				_context.Cupons.Add(c);
			}

			_context.SaveChanges();
		}

		private static void SeedNonprofit()
		{
			var image1 = File.ReadAllBytes("Images/nonprofit1.jpg");

			var nonprofits = new Nonprofit[]
			{
				new Nonprofit
				{
					Name = "Seals Kft.",
					Image = image1,
					Disabled = false,
					CollectedMoney = 0

				}
			};

			foreach (Nonprofit n in nonprofits)
			{
				_context.Nonprofits.Add(n);
			}

			_context.SaveChanges();
		}
	}
}
