using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Data;
using GreenApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GreenApp.Service.Controllers
{
    [ApiController] // API vezérlő osztály: automatikus modell validáció és további auto-konfiguráció: https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#apicontroller-attribute
    [Route("api/[controller]")] // Routing szabály
    [ApiConventionType(typeof(DefaultApiConventions))] // OpenAPI konvenciók alkalmazása az akciók által visszaadható HTTP státusz kódokra
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
                return Ok(_context.Challanges                    
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
                // Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        }
   
    

}