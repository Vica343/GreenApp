using GreenApp.Data;
using System;
using System.Collections.Generic;

namespace GreenApp.Model
{
    public class Challenge
    {       

        public int Id { get; set; }
        public int CreatorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }    
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public ChallengeType Type { get; set; }
        public RewardType Reward { get; set; }
        public int RewardValue { get; set; }
        public StatusType Status { get; set; }
        public byte[] Image { get; set; }
        public byte[] QRCode { get; set; }
        public IList<UserChallenge> UserChallenges { get; set; }

    }
}
