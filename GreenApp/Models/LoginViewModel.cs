using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "A felhasználónév megadása kötelező.")]
        public String UserName { get; set; }
   
        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        [DataType(DataType.Password)]
        public String UserPassword { get; set; }

        public Boolean RememberLogin { get; set; }
    }
}
