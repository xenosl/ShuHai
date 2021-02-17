using System;

namespace ShuHai
{
    public static class BitConverterEx
    {
        public static string ToString(bool value) { return BitConverter.ToString(BitConverter.GetBytes(value)); }
        public static string ToString(char value) { return BitConverter.ToString(BitConverter.GetBytes(value)); }
        public static string ToString(short value) { return BitConverter.ToString(BitConverter.GetBytes(value)); }
        public static string ToString(ushort value) { return BitConverter.ToString(BitConverter.GetBytes(value)); }
        public static string ToString(int value) { return BitConverter.ToString(BitConverter.GetBytes(value)); }
        public static string ToString(uint value) { return BitConverter.ToString(BitConverter.GetBytes(value)); }
        public static string ToString(long value) { return BitConverter.ToString(BitConverter.GetBytes(value)); }
        public static string ToString(ulong value) { return BitConverter.ToString(BitConverter.GetBytes(value)); }
        public static string ToString(float value) { return BitConverter.ToString(BitConverter.GetBytes(value)); }
        public static string ToString(double value) { return BitConverter.ToString(BitConverter.GetBytes(value)); }

        public static byte[] FromString(string str)
        {
            var strings = str.Split('-');
            var bytes = new byte[strings.Length];
            for (int i = 0; i < strings.Length; i++)
                bytes[i] = Convert.ToByte(strings[i], 16);
            return bytes;
        }
    }
}