using ServiceSender.Data;
using ServiceSender.Device;
using System.Collections.Concurrent;
using System.Threading;

namespace ServiceSender.ThreadSender
{
    /// <summary>
    /// Поток для обслуживания устройств
    /// </summary>
    public class ThreadSender 
    {
        private IDeviceSender? DeviceSender;

        private Thread? InnerThread;

        private ConcurrentQueue<CommandData> InQueueCmd = new();

        private AutoResetEvent AvaibleCommand = new(false);

        private object LoockObj = new();

        private bool looping;

        /// <summary>
        /// Делегат функции - результат выполнения команды
        /// </summary>
        /// <param name="sender">объект ThreadSender </param>
        /// <param name="e"> Аргумент ResponseData </param>
        public delegate void HandlerAchivedResult(object sender, ResponseData e);

        /// <summary>
        /// Событие - результат выполнения команды готов
        /// </summary>
        public event HandlerAchivedResult? AchivedResult;

        /// <summary>
        /// Статус потока.
        /// </summary>
        public bool IsLooping
        {
            get
            {
                lock (LoockObj)
                {
                    return this.looping;
                }
            }
        }

        public bool SetDevice(IDeviceSender device)
        {
            lock (LoockObj)
            {
                if(DeviceSender is not null)
                {
                    if (this.DeviceSender.IsConnected)
                        return false;

                    this.DeviceSender.Dispose();
                    this.DeviceSender = device;
                    return true;
                }
                else
                {
                    this.DeviceSender = device;
                    return true;
                }
            }
        }

        /// <summary>
        /// Отправить команду на исполнение
        /// </summary>
        /// <param name="command">Команда с данными</param>
        public void PassCommand(CommandData command)
        {
            lock (LoockObj)
            {
                if (this.IsLooping)
                {
                    InQueueCmd.Enqueue(command);
                    AvaibleCommand.Set();
                }
            }
        }

        /// <summary>
        /// Начать работу потока
        /// </summary>
        /// <returns>Результат работы функции</returns>
        public bool Start()
        {
            lock (LoockObj)
            {
                if (DeviceSender is null)
                    return false;
                  
                this.looping = true;
                InnerThread = new Thread(this.ThreadLoop);
                InnerThread.Start();
                return true;
            } 
        }

        /// <summary>
        /// Остановить работу потока.
        /// </summary>
        public void Stop()
        {
            lock (LoockObj)
            {
                this.looping = false;
                InQueueCmd.Clear();
                AvaibleCommand.Set();
            }
        }

        private void ThreadLoop()
        {
            // Init scope
            DeviceSender!.Open();

            // Loop
            while (looping)
            {
                if(InQueueCmd.IsEmpty)
                    AvaibleCommand.WaitOne();

                if (!looping) break;

                CommandData? cmd;
                if (!InQueueCmd.TryDequeue(out cmd))
                {
                    // Нужно что-то сделать, если произошел ложный вызов
                    continue;
                }

                ResponseData rsp;
                if(cmd.Command.Direction == 0)
                {
                    // Лог отправленных команд
                    rsp = DeviceSender!.WriteBulk(cmd);
                }
                else
                {
                    // Лог команд чтения
                    rsp = DeviceSender!.ReadBulk(cmd);
                }

                AchivedResult?.Invoke(this, rsp);
            }

            // Finish
            DeviceSender!.Close();
        }
    }
}
