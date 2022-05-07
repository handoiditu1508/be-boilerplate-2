using Hamburger.Models.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hamburger.Helpers.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example>string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;</example>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static string GetDescription(this Enum enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? ((DescriptionAttribute)attributes[0]).Description : null;
        }

        public static string GetValue(this Enum enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(NameValueAttribute), false);
            return (attributes.Length > 0) ? ((NameValueAttribute)attributes[0]).Value : null;
        }

        public static string GetName(this Enum enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(NameValueAttribute), false);
            return (attributes.Length > 0) ? ((NameValueAttribute)attributes[0]).Name : null;
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", nameof(description));
            // or return default(T);
        }

        public static T GetFromValue<T>(string value)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(NameValueAttribute)) as NameValueAttribute;
                if (attribute != null)
                {
                    if (attribute.Value == value)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == value)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", nameof(value));
            // or return default(T);
        }

        public static T GetFromName<T>(string value)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(NameValueAttribute)) as NameValueAttribute;
                if (attribute != null)
                {
                    if (attribute.Name == value)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", nameof(value));
            // or return default(T);
        }

        public class ValidEnumValueAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                Type enumType = value.GetType();
                bool valid = Enum.IsDefined(enumType, value);
                if (!valid)
                {
                    return new ValidationResult(string.Format("{0} is not a valid value for type {1}", value, enumType.Name));
                }
                return ValidationResult.Success;
            }
        }
    }
}
