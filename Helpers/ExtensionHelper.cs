using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class ExtensionHelper
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
        
        public static int GetDifferenceBetweenDates(this DateTime startDate, DateTime endDate)
        {
            int years = endDate.Year - startDate.Year;

            if (startDate.AddYears(years) > endDate)
                years--;

            return years;
        }
    }
}