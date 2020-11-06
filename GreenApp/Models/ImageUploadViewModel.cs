using GreenApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Models
{
    public class ImageUploadViewModel
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public String FileType { get; set; }
        public Int32 UploadedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Byte[] Data { get; set; }
        public string Extension { get; set; }
    }
}
