using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GreenApp.Data;
using GreenApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GreenApp.Service.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] 
    [ApiConventionType(typeof(DefaultApiConventions))] 
    public class ChallengesController : ControllerBase
    {
        private readonly GreenAppContext _context;
        public ChallengesController(GreenAppContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
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
                        Type = challenge.Type,
                        Reward = challenge.Reward,
                        Image = challenge.Image
                    }));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
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


        [HttpGet("{id}")]
        public IActionResult GetChallenge(Int32 id)
        {
            try
            {
                var challenge = _context.Challenges
                    .Single(c => c.Id == id);
                return Ok(new ChallangeDTO
                {
                        Id = challenge.Id,
                        Name = challenge.Name,
                        Description = challenge.Description,
                        StartDate = challenge.StartDate,
                        EndDate = challenge.EndDate,
                        Type = challenge.Type,
                        Reward = challenge.Reward,
                        Image = challenge.Image
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
   
    

}