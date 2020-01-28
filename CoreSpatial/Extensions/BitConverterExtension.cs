using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreSpatial.Extensions
{
    internal static class BigOrderBitConverter
    {
        public static IEnumerable<byte> GetBytes(short val)
        {
            return BitConverter.GetBytes(val).Reverse();
        }

        public static IEnumerable<byte> GetBytes(int val)
        {
            return BitConverter.GetBytes(val).Reverse();
        }

        public static IEnumerable<byte> GetBytes(long val)
        {
            return BitConverter.GetBytes(val).Reverse();
        }

        public static IEnumerable<byte> GetBytes(uint val)
        {
            return BitConverter.GetBytes(val).Reverse();
        }

        public static IEnumerable<byte> GetBytes(ulong val)
        {
            return BitConverter.GetBytes(val).Reverse();
        }

        public static IEnumerable<byte> GetBytes(float val)
        {
            return BitConverter.GetBytes(val).Reverse();
        }

        public static IEnumerable<byte> GetBytes(double val)
        {
            return BitConverter.GetBytes(val).Reverse();
        }

        public static IEnumerable<byte> GetBytes(char val)
        {
            return BitConverter.GetBytes(val).Reverse();
        }

        public static IEnumerable<byte> GetBytes(bool val)
        {
            return BitConverter.GetBytes(val).Reverse();
        }

        public static IEnumerable<byte> GetBytes(string val, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes(val).Reverse();
        }

    }
}
