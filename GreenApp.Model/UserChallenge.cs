using GreenApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Model
{
    public class UserChallenge
    {
        public int ChallengeId { get; set; }
        public Challenge Challenge { get; set; }        
        public int UserId { get; set; }
        public Guest User { get; set; }
        public StatusType Status { get; set; }
        public byte[] Image { get; set; }
    }
}
