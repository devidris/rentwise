﻿using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository.IRepository
{
    public interface ICityRepository : IRepository<City>
    {
        void Update(City model);
    }
}
