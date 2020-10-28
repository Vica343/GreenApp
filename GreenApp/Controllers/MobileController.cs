using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GreenApp.Controllers
{
    public class MobileController : Controller
    {

        private readonly IChallangeRepository _challangeRepository;
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public MobileController(IChallangeRepository challangeRepository)
        {
            _challangeRepository = challangeRepository;
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(_challangeRepository.All);
        }
    }
}
