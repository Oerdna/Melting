using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceSender.Protocol
{
    public class ProtocolTune
    {
        /// <summary>
        /// Метод <c>CalcCRC</c> совершает суммирование всех байтов входного массива с учётом переполнения.
        /// </summary>
        /// <param name="array">Входной массива байтов.</param>
        /// <returns>Возвращает сумму байтов с учетом переполенением указанного массива.</returns>
        public static ushort CalcCRC(byte[] array)
        {
            int maxSize = array.Length;
            if (maxSize % 2 != 0) maxSize++;
            int maxSizeUshortArray = maxSize / sizeof(ushort);
            ushort[] arrayForSumming = new ushort[maxSizeUshortArray];
            Buffer.BlockCopy(array, 0, arrayForSumming, 0, maxSize);

            // Подсчет суммы по слову Uint_16
            uint sum = 0;
            for (int i = 0; i < maxSizeUshortArray; i++)
            {
                sum += arrayForSumming[i];
                if (sum > (ushort.MaxValue)) sum += 1;
                sum %= (ushort.MaxValue + 1);
            }
            // Cast
            ushort CRC = (ushort)sum;
            return CRC;
        }

        /// <summary>
        /// Метод <c>CalcCRC</c> совершает суммирование всех байтов входного файла с учётом переполнения.
        /// </summary>
        /// <param name="array">Входной массива ushort.</param>
        /// <returns>Возвращает сумму байтов с учетом переполенением указанного массива.</returns>
        public static ushort CalcCRC(ushort[] array)
        {
            uint sum = 0;
            // Подсчет суммы по слову ushort
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
                if (sum > (ushort.MaxValue)) sum += 1;
                sum %= (ushort.MaxValue + 1);
            }
            // Cast
            ushort CRC = (ushort)sum;
            return CRC;
        }
    }
}
