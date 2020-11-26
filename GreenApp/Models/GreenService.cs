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
using Microsoft.AspNetCore.Http;
using QRCoder;
using Ubiety.Dns.Core;
using System.Drawing;
using System.Drawing.Imaging;

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
        public IEnumerable<Challenge> ChallengesWithCreator => _context.Challenges.Include(u => u.Creator);
        public IEnumerable<Cupon> Cupons => _context.Cupons;
        public IEnumerable<Nonprofit> Nonprofits => _context.Nonprofits;

        public async Task<IEnumerable<Guest>> CompanyAdminsAsync()
        {
           return await _userManager.GetUsersInRoleAsync("companyAdmin");
        }

        public IEnumerable<UserChallenge> GetSolutions(Int32? challengeid)
        {
            return _context.UserChallenges.Where(u => u.ChallengeId == challengeid).Include(i => i.User).Include(c => c.Challenge).OrderBy(u => u.Status);
        }

        public IEnumerable<Challenge> GetOwnChallenges(Int32? creatorId)
        {
            return _context.Challenges.Where(c => c.CreatorId == creatorId);
        }

        public IEnumerable<Challenge> GetOtherChallenges(Int32? creatorId)
        {
            return _context.Challenges.Where(c => c.CreatorId != creatorId);
        }

        public IEnumerable<Cupon> GetOwnCupons(Int32? creatorId)
        {
            return _context.Cupons.Where(c => c.CreatorId == creatorId);
        }

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
                RewardValue = challenge.ChallengeRewardValue,
                Image = bytes,
                QRCode = challenge.ChallengeQr,
                Disabled = false
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

        public async Task<Boolean> UpdateChallengeAsync(String userName, ChallengeViewModel challenge, Int32? id, String input)
        {
            Guest guest = await _userManager.FindByNameAsync(userName);
            Challenge dbchallenge = _context.Challenges.Where(c => c.Id == id).FirstOrDefault();

            if (dbchallenge.Type == ChallengeType.Upload && challenge.ChallengeSelectedType == ChallengeType.QRCode)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    string inputText = input + id;
                    QRCodeGenerator oQRCodeGenerator = new QRCodeGenerator();
                    QRCodeData oQRCodeData = oQRCodeGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                    QRCode oQRCode = new QRCode(oQRCodeData);
                    using (Bitmap oBitmap = oQRCode.GetGraphic(20))
                    {
                        oBitmap.Save(ms, ImageFormat.Png);
                        challenge.ChallengeQr = ms.ToArray();
                    }
                }
                dbchallenge.QRCode = challenge.ChallengeQr;
            } else if (dbchallenge.Type == ChallengeType.QRCode && challenge.ChallengeSelectedType == ChallengeType.Upload)
            {
                dbchallenge.QRCode = null;
            }

            if (challenge.ChallengeImage != null)
            {
                byte[] bytes = null;
                using (var memoryStream = new MemoryStream())
                {
                    await challenge.ChallengeImage.CopyToAsync(memoryStream);
                    bytes = memoryStream.ToArray();

                }
                dbchallenge.Image = bytes;
            }


            dbchallenge.Name = challenge.ChallengeName;
            dbchallenge.Description = challenge.ChallengeDescription;
            dbchallenge.StartDate = challenge.ChallengeStartDate;
            dbchallenge.EndDate = challenge.ChallengeEndDate;
            dbchallenge.Type = challenge.ChallengeSelectedType;
            dbchallenge.Reward = challenge.ChallengeReward;
            dbchallenge.RewardValue = challenge.ChallengeRewardValue;

            try
            {
                _context.Challenges.Update(dbchallenge);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;


        }

        public async Task<Boolean> UpdateCuponAsync(String userName, CuponViewModel cupon, Int32? id)
        {
            Guest guest = await _userManager.FindByNameAsync(userName);
            Cupon dbcupon = await _context.Cupons.Where(c => c.Id == id).FirstOrDefaultAsync();

            dbcupon.Name = cupon.CuponName;
            dbcupon.Value = cupon.CuponValue;
            dbcupon.StartDate = cupon.CuponStartDate;
            dbcupon.EndDate = cupon.CuponEndDate;

            if (cupon.CuponImage != null)
            {
                byte[] bytes = null;
                using (var memoryStream = new MemoryStream())
                {
                    await cupon.CuponImage.CopyToAsync(memoryStream);
                    bytes = memoryStream.ToArray();

                }
                dbcupon.Image = bytes;
            }

            try
            {
                _context.Cupons.Update(dbcupon);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;


        }

        public async Task<Boolean> DeleteChallengeAsync(Int32? id)
        {
            var challenge = await _context.Challenges.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (challenge.UserChallenges != null)
            {
                return false;
            }
            try
            {
                _context.Challenges.Remove(challenge);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }

        public async Task<Boolean> DeleteNonprofitAsync(Int32? id)
        {
            var nonprofit = await _context.Nonprofits.Where(c => c.Id == id).FirstOrDefaultAsync();
           
            try
            {
                _context.Nonprofits.Remove(nonprofit);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> DisableNonprofitAsync(Int32? id)
        {
            Nonprofit nonprofit = await _context.Nonprofits.Where(c => c.Id == id).FirstOrDefaultAsync();
            nonprofit.Disabled = true;

            try
            {
                _context.Nonprofits.Update(nonprofit);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> EnableNonprofitAsync(Int32? id)
        {
            Nonprofit nonprofit = await _context.Nonprofits.Where(c => c.Id == id).FirstOrDefaultAsync();
            nonprofit.Disabled = false;

            try
            {
                _context.Nonprofits.Update(nonprofit);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> DisableChallengeAsync(Int32? id)
        {
            Challenge challenge = await _context.Challenges.Where(c => c.Id == id).FirstOrDefaultAsync();
            challenge.Disabled = true;

            try
            {
                _context.Challenges.Update(challenge);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }

        public async Task<Boolean> EnableChallengeAsync(Int32? id)
        {
            Challenge challenge = await _context.Challenges.Where(c => c.Id == id).FirstOrDefaultAsync();
            challenge.Disabled = false;

            try
            {
                _context.Challenges.Update(challenge);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> EnableUserAsync(Int32? id)
        {
            var user = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            user.Status = StatusType.Accepted;

            try
            {
                await _userManager.UpdateAsync(user);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> DisableUserAsync(Int32? id)
        {
            var user = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            user.Status = StatusType.Declined;

            try
            {
                await _userManager.UpdateAsync(user);             
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> DeleteCuponAsync(Int32? id)
        {
            var cupon = await _context.Cupons.Where(c => c.Id == id).FirstOrDefaultAsync();
            var cuponchallenge = await _context.Challenges.Where(c => c.Reward == RewardType.Cupon).Where(c => c.RewardValue == id).FirstOrDefaultAsync();
            if (cuponchallenge != null)
            {
                return false;
            }
            try
            {
                _context.Cupons.Remove(cupon);
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
                Value = cupon.CuponValue,
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

        public async Task<Boolean> SaveNonprofitAsync(NonprofitViewModel nonprofit)
        {
            if (!Validator.TryValidateObject(nonprofit, new ValidationContext(nonprofit, null, null), null))
                return false;

            byte[] bytes = null;
            using (var memoryStream = new MemoryStream())
            {
                await nonprofit.NonprofitImage.CopyToAsync(memoryStream);
                bytes = memoryStream.ToArray();
            }

            _context.Nonprofits.Add(new Nonprofit
            {
                Name = nonprofit.NonprofitName,
                Disabled = false,
                CollectedMoney = 0,
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

        public async Task<Boolean> AcceptChallengeSolution(Int32? challengeId, Int32? userId)
        {
            UserChallenge c = await _context.UserChallenges
                .Where(i => i.ChallengeId == challengeId)
                .Where(i => i.UserId == userId)
                .FirstOrDefaultAsync();
            c.Status = StatusType.Accepted;
            try
            {
                _context.UserChallenges.Update(c);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> DeclineChallengeSolution(Int32? challengeId, Int32? userId)
        {
            UserChallenge c = await _context.UserChallenges
                .Where(i => i.ChallengeId == challengeId)
                .Where(i => i.UserId == userId)
                .FirstOrDefaultAsync();
            c.Status = StatusType.Declined;
            try
            {
                _context.UserChallenges.Update(c);
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

        public Byte[] GetChallangeSolutionImage(Int32? solutionId, Int32? userId)
        {
            if (solutionId == null)
                return null;

            UserChallenge c = _context.UserChallenges
                .Where(i => i.ChallengeId == solutionId)
                .Where(i => i.UserId == userId)
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

        public Byte[] GetNonprofitImage(Int32? id)
        {
            if (id == null)
                return null;

            Nonprofit c = _context.Nonprofits
                .Where(image => image.Id == id)
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

        public Challenge GetChallengeById(Int32? id)
        {         
            Challenge c = _context.Challenges
                .Where(ch => ch.Id == id)
                .FirstOrDefault();
            return c;

        }

        public Cupon GetCupon(CuponViewModel cupon)
        {
            if (cupon == null)
                return null;
            Cupon c = _context.Cupons
                .Where(ch => ch.Name == cupon.CuponName)
                .Where(ch => ch.Value == cupon.CuponValue)
                .Where(ch => ch.StartDate == cupon.CuponStartDate)
                .Where(ch => ch.EndDate == cupon.CuponEndDate)
                .FirstOrDefault();
            return c;
        }

        public Cupon GetCuponById(Int32? cupon)
        {
            Cupon c = _context.Cupons
                .Where(ch => ch.Id == cupon)
                .FirstOrDefault();
            return c;
        }
       
    }
}
