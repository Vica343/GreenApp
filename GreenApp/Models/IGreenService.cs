using GreenApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Models
{
    public interface IGreenService
    {
        IEnumerable<Challenge> Challenges { get; }

        Task<Boolean> SaveChallengeAsync(String userName, ChallangeViewModel challenge);

    }
}
