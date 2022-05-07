using System;
using System.ComponentModel;

namespace Hamburger.Models.Common
{
    public enum EnumExceptionGroup
    {
        [Description("DATABASE")]
        Database,
        [Description("SYSTEM")]
        System,
        [Description("VALIDATION")]
        Validation,
        [Description("AUTHENTICATE")]
        Authenticate
    }

    public enum EnumComparisonOperator : byte
    {
        LesserThan,
        EqualTo,
        GreaterThan
    }

    public enum EnumDefaultRole
    {
        [Description("Admin")]
        Admin,
        [Description("User")]
        User
    }

    public class NameValueAttribute : Attribute
    {
        internal NameValueAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}
