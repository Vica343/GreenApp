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
        Task<Boolean> SaveChallengeAsync(String userName, ChallengeViewModel challenge);
        Task<Boolean> SaveCuponAsync(String userName, CuponViewModel cupon);       
        Byte[] GetChallangeImage(Int32? challangeId);
        Byte[] GetCuponImage(Int32? cuponId);
        Task<byte[]> SaveQRAsync(Int32? id);
        Challenge GetChallenge(ChallengeViewModel challenge);
        Boolean UpdateChallange(ChallengeViewModel challenge);
    }
}
