using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Data
{
    public class ChallangeDTO
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
    
        public DateTime StartDate { get; set; }
     
        public DateTime EndDate { get; set; }

        public String Type { get; set; }
        public String Reward { get; set; }

        public ChallangeDTO()
        {
        }

    }
}
