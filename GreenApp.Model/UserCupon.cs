using GreenApp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GreenApp.Model
{
    public class UserCupon
    {
        public int CuponId { get; set; }

  
        public Cupon Cupon { get; set; }

        public int UserId { get; set; }

     
        public Guest User { get; set; }

        public StateType State { get; set; }
    }
}
