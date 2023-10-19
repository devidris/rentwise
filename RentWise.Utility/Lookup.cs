using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Utility
{
    public static class Lookup
    {
       public static Dictionary<int, string> Value = new Dictionary<int, string>
        {
            {1, "Construction Equipments" },
            {2, "Car Rentals"},
            {3, "Office Items/Personal Items"},
            {4, "Events/ Equipment Rentals"},
            {5, "Sales Of Vehicles Trackers"},
        };
    }




}
