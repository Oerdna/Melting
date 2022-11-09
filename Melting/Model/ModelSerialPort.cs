using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ServiceSender.Device;
using ServiceSender.ThreadSender;
using System.Collections.Generic;
using System.IO.Ports;

namespace Melting.Model
{

    public class ComPort
    {
        private string? idValue;
        private string? comPortName;

        public string? IdValue
        {
            get { return idValue; }
            set { idValue = value; }
        }

        public string? ComPortName
        {
            get { return comPortName; }
            set { comPortName = value; }
        }
    }

    public partial class ComPorts : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenPortCommand))]
        [NotifyCanExecuteChangedFor(nameof(ClosePortCommand))]
        private bool isOpen = false;

        private bool IsClose
        {
            get
            {
                return !IsOpen;
            }
        }


        public List<ComPort> Ports { get; set; }

        private readonly ThreadSender? sender;

        public ComPorts(ThreadSender sender)
        {
            // Register sednder
            this.sender = sender;

            // Alloc list
            this.Ports = new List<ComPort>();

            int num = 0;
            var stringPorts = SerialPort.GetPortNames();
            foreach (string port in stringPorts)
            {
                num++;
                this.Ports.Add(new ComPort { ComPortName = port, IdValue = num.ToString() });
            }
        }

        [ObservableProperty]
        private ComPort? selectedPort;

        
        // нужно добавить элемент отправки сообщения в sender
        [RelayCommand(CanExecute = nameof(IsClose))]
        private void OpenPort()
        {
            if ((SelectedPort?.ComPortName is not null) && (sender is not null))
            {
                sender.SetDevice(new DeviceSerial(SelectedPort.ComPortName));
                IsOpen = sender.Start();
            }
        }

        [RelayCommand(CanExecute = nameof(IsOpen))]
        private void ClosePort()
        {
            if(sender is not null)
            {
                sender.Stop();
                IsOpen = false;
            }
        }

    }
}
