using GreenApp.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public IEnumerable<Challange> Challanges => _context.Challanges;

    }
}
