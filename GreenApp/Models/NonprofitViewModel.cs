using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Models
{
    public class NonprofitViewModel
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(60, ErrorMessage = "A kihívás neve maximum 60 karakter lehet.")]
        public String NonprofitName { get; set; }


        [DataType(DataType.Upload)]
        public IFormFile NonprofitImage { get; set; }

    }
}
