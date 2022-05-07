using System;

namespace Hamburger.Helpers.Extensions
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Calculate age base on birthDate
        /// </summary>
        /// <param name="birthDate">Birth date.</param>
        /// <returns>Age.</returns>
        public static int CalculateAge(this DateTime birthDate)
        {
            // Save today's date.
            var today = DateTime.Today;

            // Calculate the age.
            var age = today.Year - birthDate.Year;

            // Go back to the year in which the person was born in case of a leap year
            if (birthDate.Date > today.AddYears(-age)) age--;

            return age;
        }
    }
}
