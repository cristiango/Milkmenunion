using System;

namespace MilkmenUnion
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Working with date and time is a complicated topic. We are not considering DTS or leap years.
        /// Possible historical calendar changes based on country also not considered. This will become an exercise in itself
        /// </summary>
        /// <returns></returns>
        public static int GetAge(this DateTime birthdate, DateTime today)
        {
            var age = today.Year - birthdate.Year;
            if (birthdate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}