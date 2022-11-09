using Melting.ServiceSender.Data;
using ServiceSender.Data;
using ServiceSender.Protocol;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ServiceSender.Device
{
    public class ReceiveStringEventArgs
    {
        public int Count { get; private set; }

        public string Text { get; private set; }

        public ReceiveStringEventArgs(int count, string text)
        {
            Count = count;
            Text = text;
        }

    }

    public class DeviceSerial : IDeviceSender, IDisposable
    {

        const int DataBufferSize        = 64;
        const int MaxAsciiSize          = 38;
        const int CommandWordAsciiSize  = 4;
        const int CommandWordBytesSize  = 2;
        const int ResponseWordAsciiSize = 4;
        const int ResponseWordBytesSize = 2;
        const int SufficsFinishSize     = 2;

        /// <summary>
        /// Делегат метода обработки события - получена строка
        /// </summary>
        /// <param name="sender">Объект вызвовший событие</param>
        /// <param name="e">Аргументы</param>
        public delegate void HandlerReceiveString(object sender, ReceiveStringEventArgs e);

        /// <summary>
        /// Событие - получена строка
        /// </summary>
        public event HandlerReceiveString? ReceiveString;

        private bool isConnected;

        /// <summary>
        /// Статус устройства
        /// </summary>
        public bool IsConnected { 
            get
            {
                return isConnected;
            } 
        }

        private readonly SerialPort sport;

        private bool DataRead;

        private int DataAsciiCount;

        private int DataSize;

        private byte LastSym;

        private byte PenultSym;

        private byte[] bufferData = new byte[DataBufferSize];

        private ResponseWord OSword = new ResponseWord(0xFF);

        private AutoResetEvent releaseOS = new AutoResetEvent(false);

        private AutoResetEvent releaseData = new AutoResetEvent(false);

        private List<byte> listSymbols = new List<byte>();


        /// <summary>
        /// Строка с символами
        /// </summary>
        public string Symbols
        {
            get
            {
                string text = Encoding.ASCII.GetString(listSymbols.ToArray());
                listSymbols.Clear();
                return text;
            }
        }


        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="portName">Наименование порта</param>
        public DeviceSerial(string portName)
        {
            sport = new SerialPort
            {
                PortName = portName,
                BaudRate = 115200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
            };
        }


        /// <summary>
        /// Открыть устройство
        /// </summary>
        public void Open()
        {
            sport.Open();
            sport.DiscardInBuffer();
            sport.DiscardOutBuffer();
            sport.DataReceived += CheckDataHandler;
            isConnected = true;
        }


        /// <summary>
        /// Закрыть устройство
        /// </summary>
        public void Close()
        {
            sport.DataReceived -= CheckDataHandler;
            sport.Close();
            isConnected = false;
        }


        /// <summary>
        /// Обработчик события - поступление байтов информации
        /// </summary>
        /// <param name="sender">объект вызвавший обработчик</param>
        /// <param name="e">аргуметы</param>
        private void CheckDataHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            byte[] buffer = new byte[sp.BytesToRead];
            sp.Read(buffer, 0, buffer.Length);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (DataRead)
                {
                    if (DataAsciiCount == ResponseWordAsciiSize)
                    {
                        byte[] respenseWord = ConverterASCII.AsciiToByte(bufferData, 0, ResponseWordBytesSize);
                        ushort OSshort = (ushort)(respenseWord[0] | (respenseWord[1] << 8));
                        OSword = new ResponseWord(OSshort);
                        releaseOS.Set();
                    } 

                    if (DataAsciiCount == MaxAsciiSize)
                    {
                        DataRead = false;
                        continue;
                    }

                    PenultSym = LastSym;
                    LastSym = buffer[i];

                    bufferData[DataAsciiCount++] = buffer[i];

                    if ((PenultSym == '\n') && (LastSym == '\r'))
                    {
                        DataRead = false;
                        DataSize = DataAsciiCount - ResponseWordAsciiSize - SufficsFinishSize;
                        if (DataSize > 0) releaseData.Set();
                        continue;
                    }

                } 
                else if (buffer[i] == ':')
                {
                    DataRead = true;
                    DataAsciiCount = 0;
                } 
                else
                    listSymbols.Add(buffer[i]);
            }
            if (listSymbols.Count > 0)
                ReceiveString?.Invoke(this, new ReceiveStringEventArgs(this.listSymbols.Count, this.Symbols));
        }


        /// <summary>
        /// Запись данных в устройство
        /// </summary>
        /// <param name="command">Команда типа CommandData</param>
        /// <returns>Результат выполения ResponseData</returns>
        public ResponseData WriteBulk(CommandData command)
        {
            bool success = false;

            if (command.Command.Direction != 0)
                return new ResponseData(success, command);

            // Формируем массив байов для отправки
            int countBytes = command.Command.NumOfWords + CommandWordBytesSize;
            byte[] sendingBytes = new byte[countBytes];
            byte[] rawCommand = command.Command.RawCommandBytes;

            Array.Copy(rawCommand, sendingBytes, CommandWordBytesSize);

            if(command.Payload != null && countBytes > 2)
                Array.Copy(command.Payload, 0, sendingBytes, CommandWordBytesSize, command.Command.NumOfWords);

            // Форматируем под ASCII формат
            byte[] sendingAscii = ConverterASCII.ByteToAscii(sendingBytes);

            // 3 попытки отправить сообщение
            for (int n = 0; n < 3; n++){
                
                sport.Write(":");
                sport.Write(sendingAscii, 0, sendingAscii.Length);
                sport.Write("\n\r");

                if (!releaseOS.WaitOne(1000))
                    continue;
                if (!OSword.IsError)
                    success = true;

                ResponseWord os = new ResponseWord(OSword.RawOS);
                return new ResponseData(success, command, os);
            }

            // Time Out
            return new ResponseData(success, command);
        }


        /// <summary>
        /// Чтение данных из устройства
        /// </summary>
        /// <param name="command">Команда типа CommandData</param>
        /// <returns>Результат выполения ResponseData</returns>
        public ResponseData ReadBulk(CommandData command)
        {
            bool success = false;

            if (command.Command.Direction != 1)
                return new ResponseData(success, command);

            // Формирование массивов чтения и записи
            int countBytes = command.Command.NumOfWords;
            int countAscii = countBytes * 2;
            byte[] readingBytes = new byte[countBytes];
            byte[] readingAscii = new byte[countBytes * 2];

            // Форматируем под ASCII формат
            byte[] sendingBytes = command.Command.RawCommandBytes;
            byte[] sendingAscii = ConverterASCII.ByteToAscii(sendingBytes);

            // 3 попытки отправить сообщение
            for (int n = 0; n < 3; n++)
            {

                sport.Write(":");
                sport.Write(sendingAscii, 0, sendingAscii.Length);
                sport.Write("\n\r");

                if (!releaseOS.WaitOne(1000))
                    continue;

                if (!releaseData.WaitOne(1000))
                    continue;

                if ((DataSize == countAscii) && !OSword.IsError)
                {
                    success = true;
                    Array.Copy(bufferData, ResponseWordAsciiSize, readingAscii, 0, countAscii);
                    readingBytes = ConverterASCII.AsciiToByte(readingAscii);
                }

                ResponseWord os = new ResponseWord(OSword.RawOS);
                return new ResponseData(success, command, os, readingBytes);
            }

            // Time Out
            return new ResponseData(success, command);
        }


        public void Dispose()
        {
            releaseOS.Dispose();
            releaseData.Dispose();
            sport.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}