using Melting.ServiceSender.Data;
using System;


namespace ServiceSender.Data
{
    public class CommandData
    {
        /// <summary>
        /// Командное слово
        /// </summary>
        public CommandWord Command { get; private set; }

        /// <summary>
        /// Нагрузка
        /// </summary>
        public byte[]? Payload { get; set; }

        /// <summary>
        /// Время создания
        /// </summary>
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// Конструктор класса CommandData
        /// </summary>
        /// <param name="command">Командное слово</param>
        public CommandData(CommandWord command)
        {
            Command = command;
            CreateTime = DateTime.Now;
        }

        /// <summary>
        /// Конструктор класса CommandData
        /// </summary>
        /// <param name="command">Командное слово</param>
        /// <param name="payload">Байтовая нагрузка</param>
        public CommandData(CommandWord command, byte[]? payload) : this(command)
        {
            Payload = payload;
        }
    }
}
