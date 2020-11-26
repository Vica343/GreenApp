using System;
using System.Collections.Generic;
using System.Linq;
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
    public class NonprofitController : ControllerBase
    {
        private readonly GreenAppContext _context;
        private readonly UserManager<Guest> _userManager;
        public NonprofitController(GreenAppContext context, UserManager<Guest> userManager)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetNonprofits()
        {
            try
            {              
                var nonprofits = _context.Nonprofits
                    .Where(s => !s.Disabled)
                    .ToList();

                return Ok(nonprofits
                .ToList()
                .Select(nonprofit => new NonprofitDTO
                {
                    Id = nonprofit.Id,
                    Name = nonprofit.Name,
                    Image = nonprofit.Image,
                    CollectedMoney = nonprofit.CollectedMoney
                }));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("OwnMoney")]
        public async Task<IActionResult> GetMoneyAsync()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                try
            {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);
                   
                    return Ok(new
                    {
                        CollectedMoney = user.CollectedMoney
                    });

                }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("SendMoney/{id}/{money}")]
        public async Task<IActionResult> SendMoney(Int32 id, Int32 money)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                try
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);
                    var nonprofit = _context.Nonprofits.Where(i => i.Id == id).FirstOrDefault();

                    if(money > user.CollectedMoney)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }
                    else
                    {
                        nonprofit.CollectedMoney += money;
                        _context.Nonprofits.Update(nonprofit);
                        await _context.SaveChangesAsync();
                        user.CollectedMoney -= money;
                        await _userManager.UpdateAsync(user);
                    }

                    return Ok();

                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

    }
}