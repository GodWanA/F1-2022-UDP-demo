using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Helpers
{
    internal static class ByteReader
    {
        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToUInt8(byte[] array, int index, out byte value)
        {
            value = array[index];
            return 1;
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToInt8(byte[] array, int index, out sbyte value)
        {
            value = (sbyte)array[index];
            return 1;
        }

        internal static int ToUInt8(byte[] array, int index, out object uint8)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToUInt16(byte[] array, int index, out UInt16 value)
        {
            value = BitConverter.ToUInt16(array, index);
            return BitConverter.GetBytes(value).Length;
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToInt16(byte[] array, int index, out Int16 value)
        {
            value = BitConverter.ToInt16(array, index);
            return BitConverter.GetBytes(value).Length;
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToUInt32(byte[] array, int index, out UInt32 value)
        {
            value = BitConverter.ToUInt32(array, index);
            return BitConverter.GetBytes(value).Length;
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToInt32(byte[] array, int index, out Int32 value)
        {
            value = BitConverter.ToInt32(array, index);
            return BitConverter.GetBytes(value).Length;
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToUInt64(byte[] array, int index, out UInt64 value)
        {
            value = BitConverter.ToUInt64(array, index);
            return BitConverter.GetBytes(value).Length;
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToInt64(byte[] array, int index, out Int64 value)
        {
            value = BitConverter.ToInt64(array, index);
            return BitConverter.GetBytes(value).Length;
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToFloat(byte[] array, int index, out float value)
        {
            value = BitConverter.ToSingle(array, index);
            return BitConverter.GetBytes(value).Length;
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToDouble(byte[] array, int index, out double value)
        {
            value = BitConverter.ToDouble(array, index);
            return BitConverter.GetBytes(value).Length;
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int ToBoolFromUint8(byte[] array, int index, out bool value)
        {
            byte raw = array[index];

            switch (raw)
            {
                case 1:
                    value = true;
                    break;
                case 0:
                    value = false;
                    break;
                default:
                    throw new Exception("Not a bool (0 | 1) value. Value=" + raw);
            }

            return 1;
        }

        /// <summary>
        /// Gets the value of bytearray at prefered index.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="index"></param>
        /// <param name="charLength">prefered index</param>
        /// <param name="value">saved value</param>
        /// <returns>Number of readen bytes</returns>
        internal static int toStringFromUint8(byte[] array, int index, int charLength, out string value)
        {
            byte uint8b;
            int count = 0;

            byte[] c = new byte[charLength];
            for (int i = 0; i < c.Length; i++)
            {
                count += ByteReader.ToUInt8(array, index + count, out uint8b);
                c[i] = uint8b;
            }

            value = Encoding.UTF8.GetString(c).Trim().Trim('\0');

            return count;
        }

        public static int TyreArray(byte[] array, int index, out TyreCompounds[] result, int size, bool isVisual)
        {
            int ret = 0;
            result = new TyreCompounds[size];
            byte valb;

            for (int i = 0; i < result.Length; i++)
            {
                ret += ByteReader.ToUInt8(array, index + ret, out valb);

                if (isVisual) result[i] = Appendences.SetVisualTyre(valb);
                else result[i] = (TyreCompounds)valb;
            }

            return ret;
        }


        internal static class ToWheelData
        {
            internal static int FromFloat(byte[] array, int index, out Dictionary<string, float> dictionary)
            {
                float tmp;
                dictionary = new Dictionary<string, float>();
                int ret = 0;

                ret += ByteReader.ToFloat(array, index + ret, out tmp);
                dictionary.Add("RearLeft", tmp);
                ret += ByteReader.ToFloat(array, index + ret, out tmp);
                dictionary.Add("RearRight", tmp);
                ret += ByteReader.ToFloat(array, index + ret, out tmp);
                dictionary.Add("FrontLeft", tmp);
                ret += ByteReader.ToFloat(array, index + ret, out tmp);
                dictionary.Add("FrontRight", tmp);

                return ret;
            }

            internal static int FromUint8(byte[] array, int index, out Dictionary<string, byte> dictionary)
            {
                byte tmp;
                dictionary = new Dictionary<string, byte>();
                int ret = 0;

                ret += ByteReader.ToUInt8(array, index + ret, out tmp);
                dictionary.Add("RearLeft", tmp);
                ret += ByteReader.ToUInt8(array, index + ret, out tmp);
                dictionary.Add("RearRight", tmp);
                ret += ByteReader.ToUInt8(array, index + ret, out tmp);
                dictionary.Add("FrontLeft", tmp);
                ret += ByteReader.ToUInt8(array, index + ret, out tmp);
                dictionary.Add("FrontRight", tmp);

                return ret;
            }

            internal static int FromUint16(byte[] array, int index, out Dictionary<string, ushort> dictionary)
            {
                ushort tmp;
                dictionary = new Dictionary<string, ushort>();
                int ret = 0;

                ret += ByteReader.ToUInt16(array, index + ret, out tmp);
                dictionary.Add("RearLeft", tmp);
                ret += ByteReader.ToUInt16(array, index + ret, out tmp);
                dictionary.Add("RearRight", tmp);
                ret += ByteReader.ToUInt16(array, index + ret, out tmp);
                dictionary.Add("FrontLeft", tmp);
                ret += ByteReader.ToUInt16(array, index + ret, out tmp);
                dictionary.Add("FrontRight", tmp);

                return ret;
            }

            internal static int fromFloatOld(byte[] array, int index, out Dictionary<string, float> dictionary)
            {
                float tmp;
                dictionary = new Dictionary<string, float>();
                int ret = 0;

                ret += ByteReader.ToFloat(array, index + ret, out tmp);
                dictionary.Add("FrontLeft", tmp);
                dictionary.Add("FrontRight", tmp);
                ret += ByteReader.ToFloat(array, index + ret, out tmp);
                dictionary.Add("RearLeft", tmp);
                dictionary.Add("RearRight", tmp);

                return ret;
            }
        }

        internal static class ToVector3
        {
            internal static int fromFloat(byte[] array, int index, out Vector3 vector)
            {
                float x, y, z;
                int ret = 0;

                ret += ByteReader.ToFloat(array, index + ret, out x);
                ret += ByteReader.ToFloat(array, index + ret, out y);
                ret += ByteReader.ToFloat(array, index + ret, out z);

                vector = new Vector3(x, y, z);

                return ret;
            }

            internal static int fromUint16(byte[] array, int index, out Vector3 vector)
            {
                ushort x, y, z;
                int ret = 0;

                ret += ByteReader.ToUInt16(array, index + ret, out x);
                ret += ByteReader.ToUInt16(array, index + ret, out y);
                ret += ByteReader.ToUInt16(array, index + ret, out z);

                vector = new Vector3(x, y, z);

                return ret;
            }

            internal static int fromInt16(byte[] array, int index, out Vector3 vector)
            {
                short x, y, z;
                int ret = 0;

                ret += ByteReader.ToInt16(array, index + ret, out x);
                ret += ByteReader.ToInt16(array, index + ret, out y);
                ret += ByteReader.ToInt16(array, index + ret, out z);

                vector = new Vector3(x, y, z);

                return ret;
            }
        }
    }
}
