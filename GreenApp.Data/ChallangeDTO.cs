using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Data
{
    public enum StatusType
    {
        Future,
        Pending,
        Accepted,
        Declined        
    }

    public enum RewardType
    {
        Cupon,
        Money
    }

    public enum ChallengeType
    {
        QRCode,
        Upload
    }


    public class ChallangeDTO
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }    
        public String Company { get; set; }    
        public DateTime StartDate { get; set; }     
        public DateTime EndDate { get; set; }
        public ChallengeType Type { get; set; }
        public RewardType Reward { get; set; }
        public StatusType Status { get; set; }
        public Byte[] Image { get; set; }
        public String CuponReward { get; set; }
        public Int32 MoneyReward { get; set; }

        public ChallangeDTO()
        {
        }

    }
}
