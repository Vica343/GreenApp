using GreenApp.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Boolean> SaveImageAsync(String userName, ImageUploadViewModel image)
        {
            Guest guest = await _userManager.FindByNameAsync(userName);
            _context.Images.Add(new Image
            {
                Name = image.Name,
                CreatedOn = image.CreatedOn,
                UploadedBy = image.UploadedBy,
                FileType = image.FileType,
                Extension = image.Extension,
                Data = image.Data
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

        public Image GetImage(Int32? imageId)
        {
            if (imageId == null)
                return null;

            return _context.Images
                .FirstOrDefault(image => image.Id == imageId);
        }

        public async Task<Image> DownLoadFileAsync(Int32? id)
        {
            var file = await _context.Images.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
            return file;
        }

    }
}
