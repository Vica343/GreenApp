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
        IEnumerable<Nonprofit> Nonprofits { get; }        
        IEnumerable<Challenge> ChallengesWithCreator { get; }        
        IEnumerable<UserChallenge> GetSolutions(Int32? challengeid);
        IEnumerable<String> GetCompanies(Int32? id);
        IEnumerable<Challenge> SearchOwnChallenge(Int32? id, String searchstring);
        IEnumerable<Challenge> SelectOwnChallenge(Int32? id, String type);
        IEnumerable<Challenge> SearchOtherChallenge(Int32? id, String searchstring);
        IEnumerable<Challenge> SelectOtherChallenge(Int32? id, String type);
        IEnumerable<Cupon> SearchCupon(Int32? id, String searchstring);
        IEnumerable<Nonprofit> SearchNonprofit(String searchstring);
        Task<IEnumerable<Guest>> SearchUser(String searchString);
        Task<IEnumerable<Guest>> CompanyAdminsAsync();
        Task<IEnumerable<Guest>> CompanyAdminsPendingAsync();
        Task<Boolean> SaveChallengeAsync(String userName, ChallengeViewModel challenge);
        Task<Boolean> UpdateChallengeAsync(String userName, ChallengeViewModel challenge, Int32? id, String input);
        Task<Boolean> UpdateCuponAsync(String userName, CuponViewModel cupon, Int32? id);
        Task<Boolean> DeleteChallengeAsync (Int32? id);
        Task<Boolean> DisableChallengeAsync (Int32? id);
        Task<Boolean> EnableChallengeAsync (Int32? id);
        Task<Boolean> EnableUserAsync (Int32? id);
        Task<Boolean> DisableUserAsync (Int32? id);
        Task<Boolean> DeleteCuponAsync (Int32? id);
        Task<Boolean> SaveCuponAsync(String userName, CuponViewModel cupon);       
        Task<Boolean> AcceptChallengeSolution(Int32? challengeId, Int32? userId);       
        Task<Boolean> DeclineChallengeSolution(Int32? challengeId, Int32? userId);
        Task<Boolean> SaveNonprofitAsync(NonprofitViewModel nonprofit);
        Task<Boolean> DeleteNonprofitAsync(Int32? id);
        Task<Boolean> DisableNonprofitAsync(Int32? id);
        Task<Boolean> EnableNonprofitAsync(Int32? id);
        Byte[] GetChallangeImage(Int32? challangeId);
        Byte[] GetChallangeSolutionImage(Int32? challengeId, Int32? userId);
        Byte[] GetQRImage(Int32? challangeId);
        Byte[] GetCuponImage(Int32? cuponId);
        Byte[] GetNonprofitImage(Int32? id);
        Task<byte[]> SaveQRAsync(Int32? id);
        Challenge GetChallenge(ChallengeViewModel challenge);
        Nonprofit GetNonprofit(NonprofitViewModel nonprofit);
        Challenge GetChallengeById(Int32? challenge);
        Cupon GetCupon(CuponViewModel cupon);
        Cupon GetCuponById(Int32? cupon);
        IEnumerable<Challenge> GetOwnChallenges(Int32? creatorId);
        IEnumerable<Challenge> GetOtherChallenges(Int32? creatorId);
        IEnumerable<Cupon> GetOwnCupons(Int32? creatorId);
        Boolean UpdateChallange(ChallengeViewModel challenge);
        
    }
}
