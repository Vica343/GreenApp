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
using GreenApp.Data;

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
        public IEnumerable<Cupon> Cupons => _context.Cupons;

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
                QRCode = challenge.ChallengeQr,
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

        public async Task<Boolean> SaveCuponAsync(String userName, CuponViewModel cupon)
        {
            if (!Validator.TryValidateObject(cupon, new ValidationContext(cupon, null, null), null))
                return false;

            Guest guest = await _userManager.FindByNameAsync(userName);

            byte[] bytes = null;
            using (var memoryStream = new MemoryStream())
            {
                await cupon.CuponImage.CopyToAsync(memoryStream);
                bytes = memoryStream.ToArray();
            }

            _context.Cupons.Add(new Cupon
            {
                CreatorId = guest.Id,
                Name = cupon.CuponName,
                StartDate = cupon.CuponStartDate,
                EndDate = cupon.CuponEndDate,
                Image = bytes
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

        public async Task<byte[]> SaveQRAsync(Int32? id)
        {
            var challenge = await _context.Challenges.Where(x => x.Id == id).FirstOrDefaultAsync();
            var qr = challenge.QRCode;
            if (qr == null) return null;
            return qr;
        }

        public Boolean UpdateChallange(ChallengeViewModel challenge)
        {
            Challenge challangeindb = GetChallenge(challenge);
            challangeindb.QRCode = challenge.ChallengeQr;
            try
            {
                _context.Challenges.Update(challangeindb);
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

            Challenge c =  _context.Challenges
                .Where(image => image.Id == challangeId)
                .FirstOrDefault();
            return c.Image;
        }

        public Byte[] GetQRImage(Int32? challangeId)
        {
            if (challangeId == null)
                return null;

            Challenge c = _context.Challenges
                .Where(image => image.Id == challangeId)
                .FirstOrDefault();
            return c.QRCode;
        }

        public Byte[] GetCuponImage(Int32? cuponId)
        {
            if (cuponId == null)
                return null;

            Cupon c = _context.Cupons
                .Where(image => image.Id == cuponId)
                .FirstOrDefault();
            return c.Image;
        }

        public Challenge GetChallenge(ChallengeViewModel challenge)
        {
            if (challenge == null)
                return null;
            Challenge c = _context.Challenges
                .Where(ch => ch.Name == challenge.ChallengeName)
                .Where(ch => ch.StartDate == challenge.ChallengeStartDate)
                .Where(ch => ch.EndDate == challenge.ChallengeEndDate)
                .Where(ch => ch.Reward == challenge.ChallengeReward)
                .Where(ch => ch.Description == challenge.ChallengeDescription)
                .FirstOrDefault();
            return c;

        }
    }
}
