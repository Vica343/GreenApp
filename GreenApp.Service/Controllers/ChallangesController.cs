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
    public class ChallangesController : ControllerBase
    {
        private readonly GreenAppContext _context;
        public ChallangesController(GreenAppContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        [HttpGet]
        public IActionResult GetChallanges()
        {
            try
            {
                return Ok(_context.Challenges                    
                    .ToList()
                    .Select(challange => new ChallangeDTO
                    {
                        Id = challange.Id,
                        Name = challange.Name,
                        Description =challange.Description,
                        StartDate = challange.StartDate,
                        EndDate = challange.EndDate,
                        Type = challange.Type,
                        Reward = challange.Reward
                    }));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
   
    

}