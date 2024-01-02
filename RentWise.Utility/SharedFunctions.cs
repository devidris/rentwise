using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Utility
{
    public class SharedFunctions
    {
        public static string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToUpper(input[0]) + input.Substring(1);
        }
        public static string CapitalizeAllWords(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            string[] words = input.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
            }

            return string.Join(" ", words);
        }

        public static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth radius in kilometers

            var dLat = (lat2 - lat1) * (Math.PI / 180);
            var dLon = (lon2 - lon1) * (Math.PI / 180);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * (Math.PI / 180)) * Math.Cos(lat2 * (Math.PI / 180)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distance = R * c; // Distance in kilometers

            return distance;
        }
        public static double GetDoubleValue(string stringValue)
        {
            if (double.TryParse(stringValue, out double result))
            {
                return result;
            }
            else
            {
                // Handle the case where parsing fails, you might want to log an error or return a default value.
                return 0.0; // Or any other appropriate default value
            }
        }
        public static string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalMinutes < 1)
            {
                return $"{(int)duration.TotalSeconds}s";
            }
            else if (duration.TotalHours < 1)
            {
                return $"{(int)duration.TotalMinutes}m";
            }
            else if (duration.TotalDays < 1)
            {
                return $"{(int)duration.TotalHours}h";
            }
            else if (duration.TotalDays < 7)
            {
                return $"{(int)duration.TotalDays}d";
            }
            else if (duration.TotalDays < 365)
            {
                return $"{(int)(duration.TotalDays / 7)}w";
            }
            else
            {
                return $"{(int)(duration.TotalDays / 365)}y";
            }
        }

    }
}
