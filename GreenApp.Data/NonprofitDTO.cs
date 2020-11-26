using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Data
{
    public class NonprofitDTO
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public Byte[] Image { get; set; }
        public Int32 CollectedMoney { get; set; }
    }
}
