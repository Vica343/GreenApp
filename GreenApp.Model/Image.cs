using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Model
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public int UploadedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public byte[] Data { get; set; }
        public string Extension { get; set; }

    }
}
