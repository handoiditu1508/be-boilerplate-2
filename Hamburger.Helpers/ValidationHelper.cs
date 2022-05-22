using Hamburger.Helpers.Abstractions;
using Hamburger.Helpers.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamburger.Helpers
{
    public class ValidationHelper : IValidationHelper
    {
        private readonly byte[][] _allowedByteHeaders;
        private readonly string[] _allowedImageContentTypes;
        private readonly int _imageAllowedSize;

        public ValidationHelper()
        {
            _allowedByteHeaders = new byte[][]
            {
                new byte[]{ 137, 80, 78, 71, 13, 10, 26, 10 },//png
		        Encoding.ASCII.GetBytes("GIF"),//gif
		        new byte[] { 255, 216, 255 }//jpg
	        };

            _allowedImageContentTypes = new string[]
            {
                "image/jpeg",
                "image/gif",
                "image/png"
            };

            _imageAllowedSize = 5242880;//5Mb
        }

        #region Birth date
        public bool IsValidDOB(DateTime birthDate, int minimumAge) => birthDate.CalculateAge() >= minimumAge;

        public void ValidateDOB(DateTime birthDate)
        {
            if (IsValidDOB(birthDate, 18))
                throw CustomException.Validation.InvalidAge;
        }
        #endregion

        public bool IsValidPhoneNumber(string phoneNumber) => new PhoneAttribute().IsValid(phoneNumber);

        public bool IsValidEmail(string email) => new EmailAddressAttribute().IsValid(email);

        #region Image
        public bool IsImage(byte[] data, string contentType)
        {
            if (_allowedByteHeaders.Any(b => b.SequenceEqual(data.Take(b.Length))) && _allowedImageContentTypes.Any(t => t == contentType))
                return true;
            return false;
        }

        public async Task ValidateImage(IFormFile image)
        {
            var data = await image.GetBytes();
            var contentType = image.ContentType;

            if (!IsImage(data, contentType))
                throw CustomException.Validation.InvalidImage;

            // check image size
            if (data.Length > _imageAllowedSize)
                throw CustomException.Validation.ImageTooBig(5);
        }
        #endregion
    }
}
