using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Models
{
    public class GuestViewModel
    {
      
        [Required(ErrorMessage = "A név megadása kötelező.")] 
        [StringLength(60, ErrorMessage = "A foglaló neve maximum 60 karakter lehet.")]
        public String GuestFirstName { get; set; }

        [Required(ErrorMessage = "A név megadása kötelező.")] 
        [StringLength(60, ErrorMessage = "A foglaló neve maximum 60 karakter lehet.")]
        public String GuestLastName { get; set; }
      
        [Required(ErrorMessage = "Az e-mail cím megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Az e-mail cím nem megfelelő formátumú.")]
        [DataType(DataType.EmailAddress)] 
        public String GuestEmail { get; set; }
       
        [Required(ErrorMessage = "A cég megadása kötelező.")]
        public String GuestCompany { get; set; }
    
        [Required(ErrorMessage = "A telefonszám megadása kötelező.")]
        [Phone(ErrorMessage = "A telefonszám formátuma nem megfelelő.")]
        [DataType(DataType.PhoneNumber)]
        public String GuestPhoneNumber { get; set; }
    }
}
