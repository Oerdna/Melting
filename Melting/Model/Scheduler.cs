using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Melting.ServiceSender.Data;
using ServiceSender.Data;
using ServiceSender.ThreadSender;
using System;
using System.Threading;

namespace Melting.Model
{
    public partial class Scheduler : ObservableObject
    {
        private readonly ThreadSender? sender;

        Timer? timer;

        private int? timePeriode;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StopLoggingCommand))]
        [NotifyCanExecuteChangedFor(nameof(StartLoggingCommand))]
        private bool isLogging = false;

        private bool EnableStartCommand { get
            {
                return !IsLogging;
            } 
        } 

        public Scheduler(ThreadSender sender)
        {
            this.sender = sender;
        }


        [RelayCommand]
        private void ActiveXScheduler(int? elapsedtime)
        {
            timePeriode = 1000 / elapsedtime;
            if(timePeriode is not null)
                timer?.Change(0, (int)timePeriode);
        }

        [RelayCommand(CanExecute = nameof(EnableStartCommand))]
        private void StartLogging()
        {
            if(timePeriode is not null)
            {
                timer = new Timer(SendMsg, null, 0, (int)timePeriode);
                IsLogging = true;
            }
        }

        [RelayCommand(CanExecute = nameof(IsLogging))]
        private void StopLogging()
        {
            timer?.Dispose();
            IsLogging = false;
        }

        private void SendMsg(Object? stateInfo)
        {
            CommandWord word_get_temp = new CommandWord(1, 1, 5, 4);
            CommandData cmd_get_temp = new CommandData(word_get_temp);
            sender?.PassCommand(cmd_get_temp);
        }

    }
}
