using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Utility
{
    public static class Lookup
    {
        public static Dictionary<int, string> Categories = new Dictionary<int, string>
        {
            {1, "Construction Equipments" },
            {2, "Car Rentals"},
            {3, "Office Items/Personal Items"},
            {4, "Events/ Equipment Rentals"},
            {5, "Sales Of Vehicles Trackers"},
            {6, "Bill board" },
            {7,"Shot Stay Rooms" },
            {8,"Motel" },
            {9,"Boat/Yacht" },
            {10,"Games" }
        };

        public static Dictionary<int, string> Roles = new Dictionary<int, string>
        {
            {1, "Admin"},
            {2, "Agent"},
            {3, "User" },
        };

        public static Dictionary<int, string> Upload = new Dictionary<int, string>
        {
            {1, "Logo"},
            {2, "Banner"},
            {3, "Passport" },
            {4, "National Card" },
            {5, "Profile Picture" },
            {6, "Phone Number" },
            {7, "Slug" },
            {8, "Store Name" },
            {9, "Main Image" },
            {10, "Other Images" },
            {11, "Cancellation Policy" },
        };

        public static Dictionary<int, string> ResponseMessages = new Dictionary<int, string>
        {
            {1,"Something went wrong"},
            {2,"Internal Server Error" },
            {3, "Not found"},
            {4,"Unathorized" },
            {5,"OK" }
        };

        public static Dictionary<int, string> OrderStatus = new Dictionary<int, string>
        {
            {1,"Pending"},
            {2,"Accepted" },
            {3, "Rejected"},
            {4,"Paid" },
            {5,"Cancelled" },
            {6,"Expired"}
        };

        public static Dictionary<int, string> PaymentMethod = new Dictionary<int, string>
        {
            {1,"Cash"},
            {2,"Online" },
        };
    }




}
