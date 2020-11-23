using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Model
{
    public class Cupon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int CreatorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte[] Image { get; set; }
        public virtual IList<UserCupon> UserCupons { get; set; }
    }
}
