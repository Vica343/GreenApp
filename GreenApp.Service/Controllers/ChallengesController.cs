using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using GreenApp.Data;
using GreenApp.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GreenApp.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ChallengesController : ControllerBase
    {
        private readonly GreenAppContext _context;
        private readonly UserManager<Guest> _userManager;
        public ChallengesController(GreenAppContext context, UserManager<Guest> userManager)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetChallenges()
        {
            try
            {
                return Ok(_context.Challenges
                    .ToList()
                    .Select(challenge => new ChallangeDTO
                    {
                        Id = challenge.Id,
                        Name = challenge.Name,
                        Description = challenge.Description,
                        StartDate = challenge.StartDate,
                        EndDate = challenge.EndDate,
                        Company = _context.Users.Where(u => u.Id == challenge.CreatorId).Select(u => u.Company).FirstOrDefault(),
                        Type = challenge.Type,
                        Reward = challenge.Reward
                    }));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Future")]
        public async Task<IActionResult> GetFutureChallenges()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                try
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);
                    var userchallenge = _context.UserChallenges
                     .ToList()
                     .Where(c => c.User == user);
                    var userchallengeid = userchallenge.Select(s => s.ChallengeId);
                    var userchallengestatus = userchallenge.Select(s => s.Status);

                    var challenges = _context.Challenges
                        .ToList()
                        .Where(c => !(userchallengeid.Contains(c.Id)) || userchallengestatus.Contains(StatusType.Declined))
                        .Where(c => c.Disabled == false);

                    return Ok(challenges
                        .ToList()
                        .Select(challenge => new ChallangeDTO
                        {
                            Id = challenge.Id,
                            Name = challenge.Name,
                            Description = challenge.Description,
                            StartDate = challenge.StartDate,
                            EndDate = challenge.EndDate,
                            Company = _context.Users.Where(u => u.Id == challenge.CreatorId).Select(u => u.Company).FirstOrDefault(),
                            Type = challenge.Type,
                            Status = _context.UserChallenges.Where(c => c.ChallengeId == challenge.Id).Where(c => c.User == user).FirstOrDefault() == null ? StatusType.Future : _context.UserChallenges.Where(c => c.ChallengeId == challenge.Id).Where(c => c.User == user).FirstOrDefault().Status,
                            Reward = challenge.Reward
                        }));
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        [HttpGet("Image/{id}")]
        public IActionResult GetImageChallenge(Int32 id)
        {
            var challenge = _context.Challenges.FirstOrDefault(c => c.Id == id);
            var image = challenge.Image;

            if (image == null)
                return NotFound();

            return File(image, "image/jpeg");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChallenge(Int32 id)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);

                    var userchallenge = _context.UserChallenges
                       .ToList()
                       .Where(c => c.User == user)
                       .Where(c => c.ChallengeId == id)
                       .FirstOrDefault();

                    StatusType status;
                    var challenge = _context.Challenges.Where(c => c.Id == id).FirstOrDefault();
                    if (userchallenge == null)
                    {
                        status = StatusType.Future;
                    } else
                    {
                        status = userchallenge.Status;
                    }


                    String cupon = "";
                    Int32 money = -1;
                    if (challenge.Reward == RewardType.Cupon)
                    {
                        cupon = _context.Cupons.Where(c => c.Id == challenge.RewardValue).FirstOrDefault().Name;
                    }
                    else
                    {
                        money = challenge.RewardValue;
                    } 

                        return Ok(new ChallangeDTO
                        {
                            Id = challenge.Id,
                            Name = challenge.Name,
                            Description = challenge.Description,
                            StartDate = challenge.StartDate,
                            EndDate = challenge.EndDate,
                            Company = _context.Users.Where(u => u.Id == challenge.CreatorId).Select(u => u.Company).FirstOrDefault(),
                            Type = challenge.Type,
                            Reward = challenge.Reward,
                            CuponReward = cupon,
                            MoneyReward = money,
                            Status = status,
                            Image = challenge.Image
                        });
                } else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        
       
    }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Image/{id}")]
        public async Task<IActionResult> PostImage(IFormFile file, Int32 id)
        {
            if (file.Length == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            byte[] bytes = null;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                try
                {
                    Challenge challenge = _context.Challenges
                       .Where(ch => ch.Id == id)
                       .FirstOrDefault();                                       
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);

                    if (challenge.StartDate > DateTime.Now)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }
                    else if (challenge.EndDate < DateTime.Now)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }

                    var uc = _context.UserChallenges.Where(c => c.ChallengeId == challenge.Id).Where(c => c.UserId == user.Id).FirstOrDefault();
                    if (uc != null && uc.Status == StatusType.Accepted)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }

                    if (uc == null)
                    {
                        if (challenge.UserChallenges == null)
                        {
                            challenge.UserChallenges = new List<UserChallenge>
                            {
                                new UserChallenge
                                {
                                    Challenge = challenge,
                                    User = user,
                                    Status = StatusType.Pending,
                                    Image = bytes
                                }
                            };
                        }
                        else
                        {
                            challenge.UserChallenges.Add(new UserChallenge
                            {
                                Challenge = challenge,
                                User = user,
                                Status = StatusType.Pending,
                                Image = bytes
                            });
                        }

                        if (challenge.Reward == RewardType.Cupon)
                        {
                            var cupon = _context.Cupons.Where(c => c.Id == challenge.RewardValue).FirstOrDefault();
                            var ucupon = _context.UserCupons.Where(c => c.CuponId == cupon.Id).Where(c => c.UserId == user.Id).FirstOrDefault();
                            if (ucupon == null)
                            {
                                if (cupon.UserCupons == null)
                                {
                                    cupon.UserCupons = new List<UserCupon>
                                {
                                    new UserCupon
                                    {
                                        Cupon = cupon,
                                        User = user,
                                        State = StateType.UnUsed
                                    }
                                };
                                }
                                else
                                {
                                    cupon.UserCupons.Add(new UserCupon
                                    {
                                        Cupon = cupon,
                                        User = user,
                                        State = StateType.UnUsed
                                    });
                                }
                                _context.Cupons.Update(cupon);
                            }                            
                        }                        
                    }
                    else
                    {
                        uc.Image = bytes;
                        uc.Status = StatusType.Pending;

                    }
                    _context.Challenges.Update(challenge);
                    _context.SaveChanges();

                    return Ok();
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

            }
            return StatusCode(StatusCodes.Status401Unauthorized);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("QR/{id}")]
        public async Task<IActionResult> QrScan(Int32 id)
        {
            var challenge = _context.Challenges
                .Single(c => c.Id == id);
            if (challenge.StartDate > DateTime.Now)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            else if (challenge.EndDate < DateTime.Now)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }


            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                try
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);
                    if (challenge.UserChallenges == null)
                    {
                        challenge.UserChallenges = new List<UserChallenge>
                    {
                        new UserChallenge
                        {
                            Challenge = challenge,
                            User = user,
                            Status = StatusType.Accepted
                        }
                    };
                    }
                    else
                    {
                        challenge.UserChallenges.Add(new UserChallenge
                        {
                            Challenge = challenge,
                            User = user,
                            Status = StatusType.Accepted,
                        });
                    }

                    if (challenge.Reward == RewardType.Cupon)
                    {
                        var cupon = _context.Cupons.Where(c => c.Id == challenge.RewardValue).FirstOrDefault();
                        var ucupon = _context.UserCupons.Where(c => c.CuponId == cupon.Id).Where(c => c.UserId == user.Id).FirstOrDefault();
                        if (ucupon == null)
                        {
                            if (cupon.UserCupons == null)
                            {
                                cupon.UserCupons = new List<UserCupon>
                            {
                                new UserCupon
                                {
                                    Cupon = cupon,
                                    User = user,
                                    State = StateType.UnUsed
                                }
                            };
                            }
                            else
                            {
                                cupon.UserCupons.Add(new UserCupon
                                {
                                    Cupon = cupon,
                                    User = user,
                                    State = StateType.UnUsed
                                });
                            }
                            _context.Cupons.Update(cupon);
                        }
                    }
                    else
                    {
                        user.CollectedMoney += challenge.RewardValue;
                        await _userManager.UpdateAsync(user);
                    }
                    _context.Challenges.Update(challenge);
                    _context.SaveChanges();

                    return Ok();
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingChallenges()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);

                    var userchallenges = _context.UserChallenges
                       .ToList()
                       .Where(c => c.User == user)
                       .Where(c => c.Status == StatusType.Pending)
                       .Select(c => c.ChallengeId);

                    var challenges = _context.Challenges
                        .ToList()
                        .Where(c => userchallenges.Contains(c.Id))
                        .Where(c => c.Disabled == false);

                    return Ok(challenges
                        .ToList()
                        .Select(challenge => new ChallangeDTO
                        {
                            Id = challenge.Id,
                            Name = challenge.Name,
                            Description = challenge.Description,
                            StartDate = challenge.StartDate,
                            EndDate = challenge.EndDate,
                            Company = _context.Users.Where(u => u.Id == challenge.CreatorId).Select(u => u.Company).FirstOrDefault(),
                            Type = challenge.Type,
                            Status = _context.UserChallenges.Where(c => c.ChallengeId == challenge.Id).FirstOrDefault().Status,
                            Reward = challenge.Reward
                        }));
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Accepted")]
        public async Task<IActionResult> GetAcceptedChallenges()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);

                    var userchallenges = _context.UserChallenges
                       .ToList()
                       .Where(c => c.User == user)
                       .Where(c => c.Status == StatusType.Accepted)
                       .Select(c => c.ChallengeId);

                    var challenges = _context.Challenges
                        .ToList()
                        .Where(c => userchallenges.Contains(c.Id))
                        .Where(c => c.Disabled == false); 

                    return Ok(challenges
                        .ToList()
                        .Select(challenge => new ChallangeDTO
                        {
                            Id = challenge.Id,
                            Name = challenge.Name,
                            Description = challenge.Description,
                            StartDate = challenge.StartDate,
                            EndDate = challenge.EndDate,
                            Company = _context.Users.Where(u => u.Id == challenge.CreatorId).Select(u => u.Company).FirstOrDefault(),
                            Type = challenge.Type,
                            Status = _context.UserChallenges.Where(c => c.ChallengeId == challenge.Id).FirstOrDefault().Status,
                            Reward = challenge.Reward
                        }));
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Declined")]
        public async Task<IActionResult> GetDeclinedChallenges()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);

                    var userchallenges = _context.UserChallenges
                       .ToList()
                       .Where(c => c.User == user)
                       .Where(c => c.Status == StatusType.Declined)
                       .Select(c => c.ChallengeId);

                    var challenges = _context.Challenges
                        .ToList()
                        .Where(c => userchallenges.Contains(c.Id))
                        .Where(c => c.Disabled == false);

                    return Ok(challenges
                        .ToList()
                        .Select(challenge => new ChallangeDTO
                        {
                            Id = challenge.Id,
                            Name = challenge.Name,
                            Description = challenge.Description,
                            StartDate = challenge.StartDate,
                            EndDate = challenge.EndDate,
                            Company = _context.Users.Where(u => u.Id == challenge.CreatorId).Select(u => u.Company).FirstOrDefault(),
                            Type = challenge.Type,
                            Status = _context.UserChallenges.Where(c => c.ChallengeId == challenge.Id).FirstOrDefault().Status,
                            Reward = challenge.Reward
                        }));
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Search/{search}")]
        public async Task<IActionResult> SearchChallenges(string search)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);

                    var challenges = _context.Challenges
                        .ToList()
                        .Where(c => c.Name.ToLower().Contains(search.Trim().ToLower()) || c.Description.ToLower().Contains(search.Trim().ToLower()))
                        .Where(c => c.Disabled == false);


                    return Ok(challenges
                        .ToList()
                        .Select(challenge => new ChallangeDTO
                        {
                            Id = challenge.Id,
                            Name = challenge.Name,
                            Description = challenge.Description,
                            StartDate = challenge.StartDate,
                            EndDate = challenge.EndDate,
                            Company = _context.Users.Where(u => u.Id == challenge.CreatorId).Select(u => u.Company).FirstOrDefault(),
                            Status = (_context.UserChallenges.Where(c => c.ChallengeId == challenge.Id).Where(c => c.User == user).FirstOrDefault() == null) ? 0 : _context.UserChallenges.Where(c => c.ChallengeId == challenge.Id).FirstOrDefault().Status,
                            Type = challenge.Type,
                            Reward = challenge.Reward
                        }));
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



    }
}




