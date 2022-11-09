using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Melting.ServiceSender.Data;
using ServiceSender.Data;

namespace ServiceSender.Device
{
    public class DeviceStub : IDeviceSender, IDisposable
    {

        private bool _isopen;
        private byte[]? _arrayWrite;
        private byte[]? _arrayRead = {1, 2, 3, 5 };

        public bool IsConnected {
            get
            {
                return _isopen;
            } 
        }

        public DeviceStub()
        {
            _isopen = false;
        }

        // Заглушка открытия порта
        public void Open()
        {
            _isopen = true;
        } 

        // Заглушка закрытия порта
        public void Close()
        {
            _isopen = false;
        }

        // Заглушка запись в устройство
        public ResponseData WriteBulk(CommandData command)
        {
            if(command.Command.Direction != 0)
            {
                return new ResponseData(false, command);
            }
            _arrayWrite = command.Command.RawCommandBytes;
            if(command.Command.NumOfWords > 0)
                _arrayWrite = command.Payload;
            Thread.Sleep(1);
            return new ResponseData(true, command);
        }

        // Заглушка чтения из устройства
        public ResponseData ReadBulk(CommandData command)
        {
            if (command.Command.Direction != 1 && command.Command.NumOfWords > 0)
            {
                return new ResponseData(false, command);
            }
            _arrayWrite = command.Command.RawCommandBytes;
            ResponseWord response = new ResponseWord(ResponseWord.EncodeOS(command.Command.DeviceAddr));
            byte[] data = new byte[command.Command.NumOfWords];
            Thread.Sleep(1);
            return new ResponseData(true, command, response, data);
        }

        public void Dispose()
        {
            ;
        }

    }
}
