using System;
using System.IO;

namespace Beacon
{
    public static class Extensions
    {
        public static bool EqualsIgnoreCase(this string value, string otherValue)
        {
            return string.Compare(value, otherValue, StringComparison.OrdinalIgnoreCase) == 0;
        }
        
        public static bool ContainsIgnoreCase(this string value, string otherValue)
        {
            return value.ToLower().Contains(otherValue.ToLower());
        }

        public static bool StartsWithIgnoreCase(this string value, string otherValue)
        {
            return value.StartsWith(otherValue, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(this string value, string otherValue)
        {
            return value.EndsWith(otherValue, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsBlank(this string value)
        {
            return value == null || string.IsNullOrEmpty(value.Trim());
        }
        
        public static void EnsureExists(this DirectoryInfo directoryInfo)
        {
            if (directoryInfo.Name.EndsWith("$"))
            {
                return;
            }

            directoryInfo.Parent?.EnsureExists();

            if (!directoryInfo.Exists)
            {
                try
                {
                    directoryInfo.Create();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("It was not possible to create the path " + directoryInfo.FullName, ex);
                }
            }
        }
    }
}