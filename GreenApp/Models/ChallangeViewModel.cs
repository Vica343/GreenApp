using GreenApp.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Models
{
    public class ChallangeViewModel
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(60, ErrorMessage = "A kihívás neve maximum 60 karakter lehet.")]
        public String ChallangeName { get; set; }

        [Required(ErrorMessage = "A leírás megadása kötelező.")]
        [StringLength(200, ErrorMessage = "A leírás maximum 200 karakter lehet.")]
        public String ChallangDescription { get; set; }


        [Required(ErrorMessage = "A kezdődátum megadása kötelező.")]
        [DataType(DataType.Date)]
        public DateTime ChallangStartDate { get; set; }

        [Required(ErrorMessage = "A végső dátum megadása kötelező.")]
        [DataType(DataType.Date)]
        public DateTime ChallangEndDate { get; set; }

        [Required(ErrorMessage = "A típus megadása kötelező.")]
        [EnumDataType(typeof(ChallengeType))]
        public ChallengeType ChallengeSelectedType { get; set; }
        

        [Required(ErrorMessage = "A jutalom kötelező.")]
        [EnumDataType(typeof(RewardType))]
        public RewardType ChallengeReward { get; set; }

        [DataType(DataType.Upload)]
        public Byte[] ChallengeImage { get; set; }


    }
}
