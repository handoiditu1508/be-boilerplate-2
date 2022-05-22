using Hamburger.Helpers.Extensions;
using Hamburger.Models.Common;
using System;

namespace Hamburger.Helpers
{
    public class CustomException : Exception
    {
        public string Group { get; set; }

        public string Code { get; set; }

        public CustomException(EnumExceptionGroup group, string code, string message) : base(message)
        {
            Group = group.GetDescription();
            Code = code;
        }

        public static class Database
        {
            public static readonly CustomException ExecuteScalarReturnsEmpty = new(EnumExceptionGroup.Database, "DATABASE_001", "Scalar execution returns empty.");
            public static readonly CustomException ParamIsEmpty = new(EnumExceptionGroup.Database, "DATABASE_002", "Parameters is empty.");
            public static readonly CustomException NotProvidedEnoughValues = new(EnumExceptionGroup.Database, "DATABASE_003", "Number of values is lesser than number of keys.");
        }

        public static class System
        {
            public static CustomException UnexpectedError(string message = "") => new(EnumExceptionGroup.System, "SYSTEM_001", $"Unexpected error{(!message.IsNullOrWhiteSpace() ? $": {message}" : "")}.");
            public static CustomException Notification(string message) => new(EnumExceptionGroup.System, "SYSTEM_002", message);
        }

        public static class Validation
        {
            public static CustomException PropertyIsNullOrEmpty(string propertyName) => new CustomException(EnumExceptionGroup.Validation, "VALIDATION_001", $"Property {propertyName} is required.");
            public static CustomException PropertyIsInvalid(string propertyName) => new CustomException(EnumExceptionGroup.Validation, "VALIDATION_002", $"Property {propertyName} is invalid.");
            public static readonly CustomException InvalidAge = new CustomException(EnumExceptionGroup.Validation, "VALIDATION_003", "User is not old enough.");
            public static readonly CustomException InvalidEmail = new CustomException(EnumExceptionGroup.Validation, "VALIDATION_004", "Invalid email address.");
            public static readonly CustomException InvalidPhoneNumber = new CustomException(EnumExceptionGroup.Validation, "VALIDATION_005", "Invalid phone number.");
            public static readonly CustomException InvalidImage = new CustomException(EnumExceptionGroup.Validation, "VALIDATION_006", "Invalid image.");
            public static CustomException ImageTooBig(int maximumMb) => new CustomException(EnumExceptionGroup.Validation, "VALIDATION_007", $"Image cannot be bigger than {maximumMb}Mb.");
            public static CustomException InvalidTimeZoneId(string timeZoneId) => new(EnumExceptionGroup.Validation, "VALIDATION_008", $"Time zone id \"{timeZoneId}\" not found.");
        }

        public static class Authentication
        {
            public static readonly CustomException UserNotFound = new CustomException(EnumExceptionGroup.Authentication, "AUTHENTICATION_001", "User not found.");
            public static readonly CustomException IncorrectPassword = new CustomException(EnumExceptionGroup.Authentication, "AUTHENTICATION_002", "Incorrect password.");
            public static readonly CustomException UserExisted = new CustomException(EnumExceptionGroup.Authentication, "AUTHENTICATION_003", "User already existed.");
            public static readonly CustomException InvalidRole = new CustomException(EnumExceptionGroup.Authentication, "AUTHENTICATION_004", "Role is invalid.");
            public static CustomException RegisterFailed(string additionalInfo = "") => new CustomException(EnumExceptionGroup.Authentication, "AUTHENTICATION_005", $"Unable to sign up." + (!additionalInfo.IsNullOrWhiteSpace() ? $" {additionalInfo}." : ""));
            public static CustomException AddRolesFailed(string additionalInfo = "") => new CustomException(EnumExceptionGroup.Authentication, "AUTHENTICATION_006", $"Unable to add roles." + (!additionalInfo.IsNullOrWhiteSpace() ? $" {additionalInfo}." : ""));
            public static readonly CustomException InvalidAccessToken = new CustomException(EnumExceptionGroup.Authentication, "AUTHENTICATION_007", "Access token is invalid.");
            public static readonly CustomException LoginSessionExpired = new CustomException(EnumExceptionGroup.Authentication, "AUTHENTICATION_008", "Login Session Expired.");
        }
    }
}
