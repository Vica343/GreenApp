using GreenApp.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;

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

        public async Task<Boolean> SaveChallengeAsync(String userName, ChallengeViewModel challenge)
        {
            if (!Validator.TryValidateObject(challenge, new ValidationContext(challenge, null, null), null))
                return false;

            Guest guest = await _userManager.FindByNameAsync(userName);

            byte[] bytes = null;
            using (var memoryStream = new MemoryStream())
            {
                await challenge.ChallengeImage.CopyToAsync(memoryStream);
                bytes = memoryStream.ToArray();

                // do what you want with the bytes
            }

            _context.Challenges.Add(new Challenge
            {
                CreatorId = guest.Id,
                Name = challenge.ChallengeName,
                Description = challenge.ChallengeDescription,
                StartDate = challenge.ChallengeStartDate,
                EndDate = challenge.ChallengeEndDate,
                Type = challenge.ChallengeSelectedType,
                Reward = challenge.ChallengeReward,               
                Image = bytes,
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

        public Byte[] GetChallangeImage(Int32? challangeId)
        {
            if (challangeId == null)
                return null;

            // lekérjük az épület első tárolt képjét (kicsiben)
            Challenge c =  _context.Challenges
                .Where(image => image.Id == challangeId)
                .FirstOrDefault();
            return c.Image;
        }
    }
}
