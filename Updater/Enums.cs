using System;
using System.ComponentModel;
using System.Linq;
using static P3tr0viCh.Utils.Converters;

namespace Updater
{
    public static class Enums
    {
        public static string Description(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            if (field.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes
                && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        [TypeConverter(typeof(EnumDescriptionConverter))]
        public enum Operation
        {
            Check,
            Update,
            Install
        }
    }
}