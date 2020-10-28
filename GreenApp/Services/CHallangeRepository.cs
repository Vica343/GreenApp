using GreenApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Models;

namespace GreenApp.Services
{
    public class ChallangeRepository : IChallangeRepository
    {
        private List<Challange> _challangeList;

        public ChallangeRepository()
        {
            InitializeData();
        }

        public IEnumerable<Challange> All
        {
            get { return _challangeList; }
        }

        public bool DoesItemExist(int id)
        {
            return _challangeList.Any(item => item.Id == id);
        }

        public Challange Find(int id)
        {
            return _challangeList.FirstOrDefault(item => item.Id == id);
        }

        private void InitializeData()
        {
            _challangeList = new List<Challange>();

            var challange1 = new Challange
            {
                Id = 2,
                Name = "Futóverseny1",
                Description = "cxcxdf",
                StartDate = new DateTime(2020,11,22),
                EndDate = new DateTime(2021,11,22),
                Reward = "Cupon",
                Type = "QR"
            };

            var challange2 = new Challange
            {
                Id = 3,
                Name = "Futóverseny2",
                Description = "cxcfdxdf",
                StartDate = new DateTime(2020, 12, 22),
                EndDate = new DateTime(2021, 12, 22),
                Reward = "Cupon",
                Type = "QR"
            };

            var challange3 = new Challange
            {
                Id = 4,
                Name = "Futóverseny3",
                Description = "cxc423xdf",
                StartDate = new DateTime(2020, 10, 22),
                EndDate = new DateTime(2021, 10, 22),
                Reward = "Cupon",
                Type = "QR"
            };

            _challangeList.Add(challange1);
            _challangeList.Add(challange2);
            _challangeList.Add(challange3);
        }
    }

}

