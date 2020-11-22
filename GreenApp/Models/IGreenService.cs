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
        IEnumerable<Cupon> Cupons { get; }
        IEnumerable<UserChallenge> GetSolutions(Int32? challengeid);
        Task<Boolean> SaveChallengeAsync(String userName, ChallengeViewModel challenge);
        Task<Boolean> SaveCuponAsync(String userName, CuponViewModel cupon);       
        Task<Boolean> AcceptChallengeSolution(Int32? challengeId, Int32? userId);       
        Task<Boolean> DeclineChallengeSolution(Int32? challengeId, Int32? userId);       
        Byte[] GetChallangeImage(Int32? challangeId);
        Byte[] GetChallangeSolutionImage(Int32? challengeId, Int32? userId);
        Byte[] GetQRImage(Int32? challangeId);
        Byte[] GetCuponImage(Int32? cuponId);
        Task<byte[]> SaveQRAsync(Int32? id);
        Challenge GetChallenge(ChallengeViewModel challenge);
        Boolean UpdateChallange(ChallengeViewModel challenge);
    }
}
