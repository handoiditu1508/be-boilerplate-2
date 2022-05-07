using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Hamburger.Helpers.Abstractions
{
    public interface IValidationHelper
    {
        /// <summary>
        /// Check age.
        /// </summary>
        /// <param name="birthDate">Birth date.</param>
        /// <param name="minimumAge">Minimum age require to pass.</param>
        /// <returns>true if birth date is greater than or equal to minimum age; otherwise, false.</returns>
        bool IsValidDOB(DateTime birthDate, int minimumAge);

        /// <summary>
        /// Validate age is greater than 18.
        /// </summary>
        /// <param name="birthDate">Birth date.</param>
        void ValidateDOB(DateTime birthDate);

        /// <summary>
        /// Check valid phone number.
        /// </summary>
        /// <param name="phoneNumber">Phone number.</param>
        /// <returns>true if phone number is valid; otherwise, false.</returns>
        bool IsValidPhoneNumber(string phoneNumber);

        /// <summary>
        /// Check valid email address.
        /// </summary>
        /// <param name="email">email address.</param>
        /// <returns>true if email is valid; otherwise, false.</returns>
        bool IsValidEmail(string email);

        /// <summary>
        /// Check whether data is a legit image.
        /// </summary>
        /// <param name="data">Byte array of the file.</param>
        /// <param name="contentType">Content type of the file</param>
        /// <returns>true if image is valid; otherwise, false.</returns>
        bool IsImage(byte[] data, string contentType);

        /// <summary>
        /// Validation image content and size.
        /// </summary>
        /// <param name="image">Image uploaded from form.</param>
        Task ValidateImage(IFormFile image);
    }
}
