namespace Melting.ServiceSender.Data
{
    public class CommandWord
    {
        private ushort _addr;

        /// <summary>
        /// Аддрес устройтсва
        /// </summary>
        public ushort DeviceAddr
        {
            get
            {
                return _addr;
            }
            protected set
            {
                _addr = (ushort)(value & 0x1F);
            }
        }

        private ushort _direction;

        /// <summary>
        /// Направление передачи
        /// </summary>
        public ushort Direction
        {
            get
            {
                return _direction;
            }
            protected set
            {
                _direction = (ushort)(value & 0x1);
            }
        }

        private ushort _subaddr;

        /// <summary>
        /// Подадрес устройтсва
        /// </summary>
        public ushort SubAddr
        {
            get
            {
                return _subaddr;
            }
            protected set
            {
                _subaddr = (ushort)(value & 0x1F);
            }
        }

        private ushort _numOfWords;

        /// <summary>
        /// Число данных
        /// </summary>
        public ushort NumOfWords
        {
            get
            {
                return _numOfWords;
            }
            protected set
            {
                _numOfWords = (ushort)(value & 0x1F);
            }
        }

        /// <summary>
        /// Конструктор командного слова
        /// </summary>
        /// <param name="deviceAddr">Адрес устройства</param>
        /// <param name="direction">Чтение-запись</param>
        /// <param name="subAddr">Подадрес</param>
        /// <param name="numOfWords">Кол-во байт</param>
        public CommandWord(ushort deviceAddr, ushort direction, ushort subAddr, ushort numOfWords)
        {
            DeviceAddr = deviceAddr;
            Direction = direction;
            SubAddr = subAddr;
            NumOfWords = numOfWords;
        }



        /// <summary>
        /// Выдать целое командное слово
        /// </summary>
        public ushort RawCommandWord
        {
            get
            {
                return (ushort)((DeviceAddr & 0x1F) << 11 | (Direction & 0x1) << 10 | (SubAddr & 0x1F) << 5 | NumOfWords & 0x1F);
            }
        }

        /// <summary>
        /// Выдать командное слово в байтах
        /// </summary>
        public byte[] RawCommandBytes
        {
            get
            {
                var temp = (ushort)((DeviceAddr & 0x1F) << 11 | (Direction & 0x1) << 10 | (SubAddr & 0x1F) << 5 | NumOfWords & 0x1F);
                byte[] tmp_array = new byte[2];
                tmp_array[0] = (byte)temp;
                tmp_array[1] = (byte)(temp >> 8);
                return tmp_array;
            }
        }
    }
}
