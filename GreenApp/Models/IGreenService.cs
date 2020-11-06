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

        Task<Boolean> SaveChallengeAsync(String userName, ChallangeViewModel challenge);
        Task<Boolean> SaveImageAsync(String userName, ImageUploadViewModel image);
        Image GetImage(Int32? imageId);
        Task<Image> DownLoadFileAsync(Int32? id);
    }
}
