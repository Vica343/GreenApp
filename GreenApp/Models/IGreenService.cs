using GreenApp.Model;
using Microsoft.AspNetCore.Http;
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
        IEnumerable<Challenge> ChallengesWithCreator { get; }
        Task<Boolean> SaveChallengeAsync(String userName, ChallengeViewModel challenge);
        Task<Boolean> UpdateChallengeAsync(String userName, ChallengeViewModel challenge, Int32? id, String input);
        Task<Boolean> UpdateCuponAsync(String userName, CuponViewModel cupon, Int32? id);
        Task<Boolean> DeleteChallengeAsync (Int32? id);
        Task<Boolean> DeleteCuponAsync (Int32? id);
        Task<Boolean> SaveCuponAsync(String userName, CuponViewModel cupon);       
        Task<Boolean> AcceptChallengeSolution(Int32? challengeId, Int32? userId);       
        Task<Boolean> DeclineChallengeSolution(Int32? challengeId, Int32? userId);  
        Byte[] GetChallangeImage(Int32? challangeId);
        Byte[] GetChallangeSolutionImage(Int32? challengeId, Int32? userId);
        Byte[] GetQRImage(Int32? challangeId);
        Byte[] GetCuponImage(Int32? cuponId);
        Task<byte[]> SaveQRAsync(Int32? id);
        Challenge GetChallenge(ChallengeViewModel challenge);
        Challenge GetChallengeById(Int32? challenge);
        Cupon GetCupon(CuponViewModel cupon);
        Cupon GetCuponById(Int32? cupon);
        IEnumerable<Challenge> GetOwnChallenges(Int32? creatorId);
        IEnumerable<Challenge> GetOtherChallenges(Int32? creatorId);
        IEnumerable<Cupon> GetOwnCupons(Int32? creatorId);
        Boolean UpdateChallange(ChallengeViewModel challenge);
        
    }
}
