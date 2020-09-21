﻿using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sisfarma.Sincronizador.Core.Extensions
{
    public static class StringExtension
    {
        public static string Strip(this string word) => word != null
                ? StripExtended(Regex.Replace(word.Trim(), @"[',\-\\]", string.Empty))
                : string.Empty;

        public static int ToIntegerOrDefault(this string @this, int @default = 0)
        {
            if (string.IsNullOrWhiteSpace(@this))
                return @default;

            if (int.TryParse(@this, out var integer))
                return integer;

            return @default;
        }

        public static long ToLongOrDefault(this string @this, int @default = 0)
        {
            if (string.IsNullOrWhiteSpace(@this))
                return @default;

            if (long.TryParse(@this, out var number))
                return number;

            return @default;
        }

        public static DateTime ToDateTimeOrDefault(this string @this, string format)
            => @this.ToDateTimeOrDefault(format, CultureInfo.InvariantCulture);

        public static DateTime ToDateTimeOrDefault(this string @this, string format, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(@this))
                return default(DateTime);

            if (DateTime.TryParseExact(@this, format, culture, DateTimeStyles.None, out var fecha))
                return fecha;

            return default(DateTime);
        }

        private static string StripExtended(string arg)
        {
            StringBuilder buffer = new StringBuilder(arg.Length);
            foreach (char ch in arg)
            {
                UInt16 num = Convert.ToUInt16(ch);
                if ((num >= 32u) && (num <= 237u)) buffer.Append(ch);
            }
            return buffer.ToString().Replace("%", " % ");
        }
    }
}