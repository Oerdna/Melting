using System;
using Melting.ServiceSender.Data;
using ServiceSender.Data;

namespace ServiceSender.Device
{
    /// <summary>
    /// Интерфейс для общения с устрйоствами различной реализации
    /// </summary>
    public interface IDeviceSender : IDisposable
    {

        /// <summary>
        /// Статус устройства
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// Запись блока сообщения в устройство
        /// </summary>
        /// <param name="command">Комманда для отправки</param>
        /// <returns>Результат работы ResponseData</returns>
        ResponseData WriteBulk(CommandData command);

        /// <summary>
        /// Чтение блока сообщения в устройство
        /// </summary>
        /// <param name="command">Комманда для отправки</param>
        /// <returns>Результат работы ResponseData</returns>
        ResponseData ReadBulk(CommandData command);

        /// <summary>
        /// Приводит устрйоство в рабочий режим
        /// </summary>
        void Open();

        /// <summary>
        /// Выводит устрйоство из рабочего режима
        /// </summary>
        void Close();
    }
}
