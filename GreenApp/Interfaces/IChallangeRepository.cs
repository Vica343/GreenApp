using GreenApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenApp.Interfaces
{
    public interface IChallangeRepository
    {
        bool DoesItemExist(int id);
        IEnumerable<Challange> All { get; }
        Challange Find(int id);
    }
}
