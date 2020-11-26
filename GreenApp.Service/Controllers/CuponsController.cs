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
    public class CuponsController : ControllerBase
    {

        private readonly GreenAppContext _context;
        private readonly UserManager<Guest> _userManager;
        public CuponsController(GreenAppContext context, UserManager<Guest> userManager)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
            _userManager = userManager;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetCupons()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                try
                {

                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);
                    var usercupons = _context.UserCupons
                        .ToList()
                        .Where(c => c.User == user)
                        .Select(c => c.CuponId);

                    var cupons = _context.Cupons
                        .ToList()
                        .Where(c => usercupons.Contains(c.Id));                    

                    return Ok(cupons
                    .ToList()
                    .Select(cupon => new CuponDTO
                    {
                        Id = cupon.Id,
                        Name = cupon.Name,
                        StartDate = cupon.StartDate,
                        EndDate = cupon.EndDate,
                        Company = _context.Users.Where(u => u.Id == cupon.CreatorId).Select(u => u.Company).FirstOrDefault(),
                        Image = cupon.Image,
                        Value = cupon.Value,
                        Valid = (cupon.EndDate >DateTime.Now && cupon.StartDate < DateTime.Now) ? ValidType.Valid : ValidType.NotValid
                    }));
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("used")]
        public async Task<IActionResult> GetUsed()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                try
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);
                    var usercupons = _context.UserCupons
                        .ToList()
                        .Where(c => c.User == user)
                        .Where(c => c.State == StateType.Used)
                        .Select(c => c.CuponId);

                    var cupons = _context.Cupons
                        .ToList()
                        .Where(c => usercupons.Contains(c.Id));

                    return Ok(cupons
                    .ToList()
                    .Select(cupon => new CuponDTO
                    {
                        Id = cupon.Id,
                        Name = cupon.Name,
                        StartDate = cupon.StartDate,
                        EndDate = cupon.EndDate,
                        Company = _context.Users.Where(u => u.Id == cupon.CreatorId).Select(u => u.Company).FirstOrDefault(),
                        Valid = (cupon.EndDate > DateTime.Now && cupon.StartDate < DateTime.Now) ? ValidType.Valid : ValidType.NotValid,
                        Image = cupon.Image,
                        Value = cupon.Value
                    }));
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("unUsed")]
        public async Task<IActionResult> GetUnUsed()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                try
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);
                    var usercupons = _context.UserCupons
                        .ToList()
                        .Where(c => c.User == user)
                        .Where(c => c.State == StateType.UnUsed)
                        .Select(c => c.CuponId);

                    var cupons = _context.Cupons
                        .ToList()
                        .Where(c => usercupons.Contains(c.Id));

                    return Ok(cupons
                    .ToList()
                    .Select(cupon => new CuponDTO
                    {
                        Id = cupon.Id,
                        Name = cupon.Name,
                        StartDate = cupon.StartDate,
                        EndDate = cupon.EndDate,
                        Company = _context.Users.Where(u => u.Id == cupon.CreatorId).Select(u => u.Company).FirstOrDefault(),
                        Valid = (cupon.EndDate > DateTime.Now && cupon.StartDate < DateTime.Now) ? ValidType.Valid : ValidType.NotValid,
                        Value = cupon.Value,
                        Image = cupon.Image                        
                    }));
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("valid")]
        public async Task<IActionResult> GetValid()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                try
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);
                    var usercupons = _context.UserCupons
                        .ToList()
                        .Where(c => c.User == user)
                        .Select(c => c.CuponId);

                    var cupons = _context.Cupons
                        .ToList()
                        .Where(c => usercupons.Contains(c.Id))
                        .Where(c => c.EndDate > DateTime.Now && c.StartDate < DateTime.Now);

                    return Ok(cupons
                    .ToList()
                    .Select(cupon => new CuponDTO
                    {
                        Id = cupon.Id,
                        Name = cupon.Name,
                        StartDate = cupon.StartDate,
                        EndDate = cupon.EndDate,
                        Company = _context.Users.Where(u => u.Id == cupon.CreatorId).Select(u => u.Company).FirstOrDefault(),
                        State = _context.UserCupons.Where(c => c.CuponId == cupon.Id).FirstOrDefault().State,
                        Value = cupon.Value,
                        Image = cupon.Image
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
        public IActionResult GetImageCupon(Int32 id)
        {
            var cupon = _context.Cupons.FirstOrDefault(c => c.Id == id);
            var image = cupon.Image;

            if (image == null)
                return NotFound();

            return File(image, "image/jpeg");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("notValid")]
        public async Task<IActionResult> GetNotValid()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                try
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);
                    var usercupons = _context.UserCupons
                        .ToList()
                        .Where(c => c.User == user)
                        .Select(c => c.CuponId);

                    var cupons = _context.Cupons
                        .ToList()
                        .Where(c => usercupons.Contains(c.Id))
                        .Where(c => !(c.EndDate > DateTime.Now && c.StartDate < DateTime.Now));

                    return Ok(cupons
                    .ToList()
                    .Select(cupon => new CuponDTO
                    {
                        Id = cupon.Id,
                        Name = cupon.Name,
                        StartDate = cupon.StartDate,
                        EndDate = cupon.EndDate,
                        Company = _context.Users.Where(u => u.Id == cupon.CreatorId).Select(u => u.Company).FirstOrDefault(),
                        State = _context.UserCupons.Where(c => c.CuponId == cupon.Id).FirstOrDefault().State,
                        Value = cupon.Value,
                        Image = cupon.Image
                    }));
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