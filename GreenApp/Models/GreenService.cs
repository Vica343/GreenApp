using GreenApp.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Models
{
    public class GreenService : IGreenService
    {
        private readonly GreenAppContext _context;
        private readonly UserManager<Guest> _userManager;

        public GreenService(GreenAppContext context, UserManager<Guest> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IEnumerable<Challenge> Challenges => _context.Challenges;

        public async Task<Boolean> SaveChallengeAsync(String userName, ChallangeViewModel challenge)
        {
            if (!Validator.TryValidateObject(challenge, new ValidationContext(challenge, null, null), null))
                return false;

            Guest guest = await _userManager.FindByNameAsync(userName);

            _context.Challenges.Add(new Challenge
            {
                CreatorId = guest.Id,
                Name = challenge.ChallangeName,
                Description = challenge.ChallangDescription,
                StartDate = challenge.ChallangStartDate,
                EndDate = challenge.ChallangEndDate,
                Type = challenge.ChallengeSelectedType,
                Reward = challenge.ChallengeReward,
                DataImage = challenge.ChallengeImage,
                Status = Data.StatusType.Pending             
            });

            try 
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }
    }
}
