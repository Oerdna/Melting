using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melting.ServiceSender.Data;
using ServiceSender.Protocol;

namespace ServiceSender.Data
{
    /// <summary>
    /// Класс реализующий хранение свойств на результат работы команды
    /// </summary>
    public class ResponseData
    {

        /// <summary>
        /// Успех выполнения команды
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Ссылка на оригинальную комманду
        /// </summary>
        public CommandData BoundCommand { get; private set; }

        /// <summary>
        /// Ссылка на ответное слово
        /// </summary>
        public ResponseWord? BoundResponse { get; private set; }

        /// <summary>
        /// Ссылка на данные
        /// </summary>
        public byte[]? Data { get; private set; }

        /// <summary>
        /// Конструктор класса ResponseData
        /// </summary>
        /// <param name="isSuccess">Значение успеха выполнения команды</param>
        /// <param name="boundCommand">Ссылка на команду</param>
        public ResponseData(bool isSuccess, CommandData boundCommand)
        {
            IsSuccess = isSuccess;
            BoundCommand = boundCommand;
        }

        /// <summary>
        /// Конструктор класса ResponseData
        /// </summary>
        /// <param name="isSuccess">Значение успеха выполнения команды</param>
        /// <param name="boundCommand">Ссылка на команду</param>
        /// <param name="boundResponse">Ссылка на отвеное слово</param>
        public ResponseData(bool isSuccess, CommandData boundCommand, ResponseWord? boundResponse) : this(isSuccess, boundCommand)
        {
            BoundResponse = boundResponse;
        }

        /// <summary>
        /// Конструктор класса ResponseData
        /// </summary>
        /// <param name="isSuccess">Значение успеха выполнения команды</param>
        /// <param name="boundCommand">Ссылка на команду</param>
        /// <param name="boundResponse">Ссылка на отвеное слово</param>
        /// <param name="data">Ссылка на массив данных</param>
        public ResponseData(bool isSuccess, CommandData boundCommand, ResponseWord? boundResponse, byte[]? data) : this(isSuccess, boundCommand, boundResponse)
        {
            Data = data;
        }

    }

}
