using GreenApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Models
{
    public interface IGreenService
    {
        IEnumerable<Challange> Challanges { get; }
    }
}
