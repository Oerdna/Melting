using System;
using System.Collections.Generic;

namespace ServiceSender.Protocol
{
    public class ConverterASCII
    {

        /// <summary>
        /// Конвертор ascii в byte
        /// </summary>
        /// <param name="digit_h">Старший разряд числа</param>
        /// <param name="digit_l">Младший разряд числа</param>
        /// <returns>byte value</returns>
        public static byte AsciiToByte(byte digit_h, byte digit_l)
        {
            byte value;
            value = (byte)((digit_h - (digit_h < 58 ? 48 : 55)) << 4);
            value |= (byte)(digit_l - (digit_l < 58 ? 48 : 55));
            return value;
        }

        /// <summary>
        /// Конвертор ascii в byte
        /// </summary>
        /// <param name="segment">срез массива</param>
        /// <returns>byte value</returns>
        public static byte AsciiToByte(ArraySegment<byte> segment)
        {
            if(segment.Count != 2)
            {
                throw new Exception("The binary connot have an odd number of digits");
            }
            if(segment.Array is null)
            {
                throw new Exception("The binary connot be null");
            }
            byte value;
            int offset_h = segment.Offset;
            int offset_l = segment.Offset + 1;
            value = (byte)((segment.Array[offset_h] - (segment.Array[offset_h] < 58 ? 48 : 55)) << 4);
            value |= (byte)(segment.Array[offset_l] - (segment.Array[offset_l] < 58 ? 48 : 55));
            return value;
        }

        /// <summary>
        /// Конвертор byte в ascii
        /// </summary>
        /// <param name="value"></param>
        /// <returns>byte[] ascii</returns>
        public static byte[] ByteToAscii(byte value)
        {
            byte[] ascii = new byte[2];
            int digit_h = (value & 0xF0) >> 4;
            int digit_l = (value & 0x0F);
            ascii[0] = (byte)(digit_h + (digit_h < 10 ? 48 : 55));
            ascii[1] = (byte)(digit_l + (digit_l < 10 ? 48 : 55));
            return ascii;
        }

        /// <summary>
        /// Конвертор byte в ascii
        /// </summary>
        /// <param name="array">Массив чисел</param>
        /// <returns>ascii массив</returns>
        public static byte[] ByteToAscii(byte[] array)
        {
            List<byte> temp = new();
            for(int i = 0; i < array.Length; i++)
            {
                temp.AddRange(ByteToAscii(array[i]));
            }
            return temp.ToArray();
        }

        /// <summary>
        /// Конвертор ascii в byte
        /// </summary>
        /// <param name="ascii">массив ascii</param>
        /// <returns>массив byte</returns>
        public static byte[] AsciiToByte(byte[] ascii)
        {
            if((ascii.Length % 2) != 0 || ascii.Length < 2)
            {
                throw new Exception("The binary connot have an odd number of digits");
            }
            List<byte> temp = new();
            for (int i = 0; i < ascii.Length; i += 2)
            {
                ArraySegment<byte> segment = new ArraySegment<byte>(ascii, i, 2);
                temp.Add(AsciiToByte(segment));
            }
            return temp.ToArray();
        }

        /// <summary>
        /// Конвертор ascii в byte
        /// </summary>
        /// <param name="ascii">массив ascii</param>
        /// <param name="offset">Сдвиг относительно байтового массива</param>
        /// <param name="count">Кол-во байтов</param>
        /// <returns>массив byte</returns>
        public static byte[] AsciiToByte(byte[] ascii, int offset, int count)
        {
            if ((ascii.Length % 2) != 0 || ascii.Length < 2)
            {
                throw new Exception("The binary connot have an odd number of digits");
            }
            if(!(count > 0))
            {
                throw new Exception("The count connot be zero or negative value");
            }
            if(offset < 0){
                throw new Exception("The offset connot be negative value");
            }
            if((offset + count) * 2 > ascii.Length)
            {
                throw new Exception("The count or offset have immposible value");
            }
            List<byte> temp = new();
            for (int i = (offset * 2); i < (offset + count) * 2; i += 2)
            {
                ArraySegment<byte> segment = new ArraySegment<byte>(ascii, i, 2);
                temp.Add(AsciiToByte(segment));
            }
            return temp.ToArray();
        }

        //private static byte GetHexValue(byte value)
        //{
        //    return (byte)(value - (value < 10 ? 48 : 55));
        //}

    }
}
