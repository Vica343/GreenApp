using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using GreenApp.Data;
using GreenApp.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NHibernate.Cfg;

namespace GreenApp.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AppAccountController : ControllerBase
    {
        private readonly SignInManager<Guest> _signInManager;
        private static RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<Guest> _userManager;
        private readonly IConfiguration _configuration;


        public AppAccountController(SignInManager<Guest> signInManager, RoleManager<IdentityRole<int>> roleManager, UserManager<Guest> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;           
        }

      
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]GuestDTO guestDTO)
        {
            var user = await _userManager.FindByNameAsync(guestDTO.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, guestDTO.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                if (!userRoles.Contains("appUser"))
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var appUser in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, appUser));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));

                var creds = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512Signature);


                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(authClaims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new
                {
                    token = tokenHandler.WriteToken(token)
                });
            }
            return Unauthorized();

        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
  
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]GuestDTO guestDTO)
        {
            Guest guest = new Guest
            {
                UserName = guestDTO.Username,
                Email = guestDTO.Email,                
                FirstName = guestDTO.FirstName,
                LastName = guestDTO.LastName,
                CollectedMoney = 0
            };           
            try
            {

                List<string> errors = new List<string>();
                var validators1 = _userManager.PasswordValidators;

                foreach (var validator in validators1)
                {
                    var result = await validator.ValidateAsync(_userManager, null, guestDTO.Password);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            errors.Add(error.Description);
                        }
                    }
                }
                var validators2 = _userManager.UserValidators;

                foreach (var validator in validators2)
                {
                    var result = await validator.ValidateAsync(_userManager, guest);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            errors.Add(error.Description);
                        }
                    }
                }
                if (errors.Count != 0)
                {
                    var errorWrapper = new
                    {
                        errors = errors

                    };

                    return BadRequest(JsonConvert.SerializeObject(errorWrapper));
                }

                var appUser = await _roleManager.FindByNameAsync("appUser");
                var result1 = await _userManager.CreateAsync(guest, guestDTO.Password);
                var result2 = _userManager.AddToRoleAsync(guest, appUser.Name).Result;

                if (!result1.Succeeded)
                {
                    return BadRequest(result1.Errors);
                } 
                if (!result2.Succeeded)
                {
                    return BadRequest(result2.Errors);
                }              

                return Ok();
            }
            catch 
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        { 
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var user = await _userManager.FindByNameAsync(identity.Name);
                    return Ok(new GuestDTO
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Username = user.UserName,
                        CollectedMoney = user.CollectedMoney
                    });

                }
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            catch
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }

}