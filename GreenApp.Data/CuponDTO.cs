using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Data
{
    public enum StateType
    {
        Used,
        UnUsed
    }

    public enum ValidType
    {
        Valid,
        NotValid
    }

    public class CuponDTO
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String Company { get; set; }
        public Byte[] Image { get; set; }
        public ValidType Valid { get; set; }
        public StateType State { get; set; }
    }
}
