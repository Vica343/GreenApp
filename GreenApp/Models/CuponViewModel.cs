using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Models
{
    public class CuponViewModel
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(60, ErrorMessage = "A kihívás neve maximum 60 karakter lehet.")]
        public String CuponName { get; set; }

        [Required(ErrorMessage = "A kupon megadása kötelező.")]
        [StringLength(15, ErrorMessage = "A kupon maximum 15 karakter lehet.")]
        public String CuponValue { get; set; }

        [Required(ErrorMessage = "A kezdődátum megadása kötelező.")]
        [DataType(DataType.Date)]
        public DateTime CuponStartDate { get; set; }

        [Required(ErrorMessage = "A végső dátum megadása kötelező.")]
        [DataType(DataType.Date)]
        public DateTime CuponEndDate { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile CuponImage { get; set; }
    }
}
