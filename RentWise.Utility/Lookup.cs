﻿using System;
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
            {0, "All Product" },
            {2, "Car Rental"},
            {1, "Construction Equipment" },
            {4, "Events"},
            {10,"Games" },
            {11,"Hotel" },
            {5, "Guest House"},
            {8,"Motel" },
            {7,"Shot Stay" }
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
            {0,"Pending"},
            {1,"Pending"},
            {2,"Accepted" },
            {3, "Rejected"},
            {4,"Paid" },
            {5,"Cancelled" },
            {6,"Expired"},
            {7, "Waiting to pay with cash" },
            {8, "Cart" }
        };

        public static Dictionary<int, string> PaymentMethod = new Dictionary<int, string>
        {
            {1,"Cash"},
            {2,"Online" },
        };

        public static Dictionary<int, string> EmailContent = new Dictionary<int, string>
        {
            {1,"Pending Order Client Waiting"},
            {2,"Pending Order Waiting for Agent"},
            {3,"Agent Accepted Order"},
            {4,"Agent Rejected Order"},
            {5,"Client has made payment online"},
            {6,"Client want to pay with cash"},
            {7,"Agent marked payment as recieved"},
        };

        public static Dictionary<int, string> Banks = new Dictionary<int, string>
{
    {1, "Absa Bank Ghana Limited"},
    {2, "Access Bank Ghana Plc"},
    {3, "Agricultural Development Bank of Ghana"},
    {4, "Bank of Africa Ghana Limited"},
    {5, "CalBank Limited"},
    {6, "Consolidated Bank Ghana Limited"},
    {7, "Ecobank Ghana Limited"},
    {8, "FBN Bank Ghana Limited"},
    {9, "Fidelity Bank Ghana Limited"},
    {10, "First Atlantic Bank Limited"},
    {11, "First National Bank Ghana"},
    {12, "GCB Bank Limited"},
    {13, "Guaranty Trust Bank Ghana Limited"},
    {14, "Mobile Money" },
    {15, "National Investment Bank Limited"},
    {16, "OmniBSIC Bank Ghana Limited"},
    {17, "Prudential Bank Limited"},
    {18, "Republic Bank Ghana Limited"},
    {19, "Société Générale Ghana Limited"},
    {20, "Stanbic Bank Ghana Limited"},
    {21, "Standard Chartered Bank Ghana Limited"},
    {22, "United Bank for Africa Ghana Limited"},
    {23, "Universal Merchant Bank Limited"},
    {24, "Zenith Bank Ghana Limited"}
};
        public static Dictionary<int, (string Name, string Value)> Settings = new Dictionary<int, (string Name, string Value)>
    {
        { 1, ("Product Premium Price", "120") },
        { 2, ("Product Premium Price Weekly", "1200") },
        { 3, ("Product Premium Price Monthly", "12000") },
        { 4, ("Product Premium Price Yearly", "120000") },
    };
    }

}
