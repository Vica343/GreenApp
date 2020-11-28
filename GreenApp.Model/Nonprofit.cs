using System;
using System.Collections.Generic;
using System.Text;
using GreenApp.Data;

namespace GreenApp.Model
{
    public class Nonprofit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CollectedMoney { get; set; }
        public byte[] Image { get; set; }
        public bool Disabled { get; set; }

    }
}
