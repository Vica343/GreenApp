﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Data
{  
    public class GuestDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int CollectedMoney { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
