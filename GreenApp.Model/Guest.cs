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

        public string Name { get; set; }

        /// <summary>
        /// Cím.
        /// </summary>
        public string Address { get; set; }


        /// <summary>
        /// Foglalások.
        /// </summary>
        public ICollection<Challange> Challanges { get; set; }

    }
}
