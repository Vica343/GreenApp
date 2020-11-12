using GreenApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Models
{
    public interface IGreenService
    {
        IEnumerable<Challenge> Challenges { get; }

        Task<Boolean> SaveChallengeAsync(String userName, ChallengeViewModel challenge);
       
        Byte[] GetChallangeImage(Int32? challangeId);

        Challenge GetChallenge(ChallengeViewModel challenge);
        Boolean UpdateChallange(ChallengeViewModel challenge);
    }
}
