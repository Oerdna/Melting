using System;

namespace Melting.ServiceSender.Data
{

    /// <summary>
    /// Ответное слово от устройства
    /// </summary>
    public class ResponseWord
    {

        private ushort addr;
        /// <summary>
        /// Поле адреса ОУ.
        /// </summary>
        public ushort ADDR
        {
            get
            {
                return addr;
            }
            protected set
            {
                addr = (ushort)(value & 0x1F);
            }
        }

        /// <summary>
        /// Бит ошибки сообщения.
        /// </summary>
        public bool ERROR { get; private set; }
        /// <summary>
        /// Аппаратный бит.
        /// </summary>
        public bool HBIT { get; private set; }
        /// <summary>
        /// Бит "Запрос обслуживания системы".
        /// </summary>
        public bool SREQ { get; private set; }
        /// <summary>
        /// Бит "Принята групповая команда".
        /// </summary>
        public bool BRSCT { get; private set; }
        /// <summary>
        /// Бит "Подсистема занята".
        /// </summary>
        public bool BUSY { get; private set; }
        /// <summary>
        /// Бит "Неисправность подсистемы".
        /// </summary>
        public bool SSFL { get; private set; }
        /// <summary>
        /// Бит "Принято управление каналом".
        /// </summary>
        public bool DNBA { get; private set; }
        /// <summary>
        /// Бит "Неиправность терминала".
        /// </summary>
        public bool RTFL { get; private set; }

        private ushort rawOS;
        public ushort RawOS
        {
            get
            {
                return rawOS;
            }
            set
            {
                rawOS = value;
                decodeOS(value);
            }
        }

        /// <summary>
        /// Свойство, указывающие на наличие бита состояния в ответном слове
        /// </summary>
        public bool IsError { get
            {
                return (RawOS & 0x71F) > 0;
            } 
        }

        /// <summary>
        /// Конструктор ответного слова.
        /// </summary>
        /// <param name="OS">ushort ОС.</param>
        public ResponseWord(ushort OS)
        {
            RawOS = OS;
        }

        /// <summary>
        /// Конструктор ответного слова.
        /// </summary>
        /// <param name="OS">byte[] OS</param>
        public ResponseWord(byte[] OS)
        {
            if(OS.Length != 2)
            {
                RawOS = 0xFF;
                return;
            }
            RawOS = BitConverter.ToUInt16(OS);
        }

        /// <summary>
        /// Декодирует отвеное слово ОУ.
        /// </summary>
        /// <param name="OS">ushort ОС.</param>
        private void decodeOS(ushort OS)
        {
            ADDR    = (ushort)(OS >> 11);
            ERROR   = (OS >> 10 & 0x1) != 0;
            HBIT    = (OS >> 9 & 0x1) != 0;
            SREQ    = (OS >> 8 & 0x1) != 0;
            BRSCT   = (OS >> 4 & 0x1) != 0;
            BUSY    = (OS >> 3 & 0x1) != 0;
            SSFL    = (OS >> 2 & 0x1) != 0;
            DNBA    = (OS >> 1 & 0x1) != 0;
            RTFL    = (OS & 0x1) != 0;
        }

        /// <summary>
        /// Функция кодирует заданное ответное слово в ushort.
        /// </summary>
        public static ushort EncodeOS(ushort Addr,
            bool ERROR  = false,
            bool HBIT   = false,
            bool SREQ   = false,
            bool BRSCT  = false,
            bool BUSY   = false,
            bool SSFL   = false,
            bool DNBA   = false,
            bool RTFL   = false)
        {
            ushort tmp = (ushort)(Addr << 11);
            tmp |= (ushort)(Convert.ToUInt16(ERROR) << 10);
            tmp |= (ushort)(Convert.ToUInt16(HBIT) << 9);
            tmp |= (ushort)(Convert.ToUInt16(SREQ) << 8);
            tmp |= (ushort)(Convert.ToUInt16(BRSCT) << 4);
            tmp |= (ushort)(Convert.ToUInt16(BUSY) << 3);
            tmp |= (ushort)(Convert.ToUInt16(SSFL) << 2);
            tmp |= (ushort)(Convert.ToUInt16(DNBA) << 1);
            tmp |= Convert.ToUInt16(RTFL);
            return tmp;
        }
    }
}
