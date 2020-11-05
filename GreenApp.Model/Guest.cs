using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Model
{
   public class Guest : IdentityUser<int>
   {
        public Guest()
        {
            Challanges = new HashSet<Challange>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Company { get; set; }

        public ICollection<Challange> Challanges { get; set; }

    }
}
