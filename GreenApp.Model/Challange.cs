using System;
using System.ComponentModel.DataAnnotations;

namespace GreenApp.Model
{
    public class Challange
    {       

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string Type { get; set; }
        public string Reward { get; set; }

    }
}
