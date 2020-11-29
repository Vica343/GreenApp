using GreenApp.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GreenApp.Model
{
   public class Guest : IdentityUser<int>
   {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Company { get; set; }

        public StatusType Status { get; set; }
        public int CollectedMoney { get; set; }

        public virtual ICollection<Challenge> CreatedChallenges { get; set; }
        public virtual ICollection<Cupon> CreatedCupons { get; set; }
        public virtual IList<UserChallenge> UserChallenges { get; set; }
        public virtual IList<UserCupon> UserCupons { get; set; }

    }
}
