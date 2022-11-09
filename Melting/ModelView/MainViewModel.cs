using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Melting.Model;
using Melting.View;
using ServiceSender.ThreadSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melting.ModelView
{
    public partial class MainWinViewModel
    {
        [RelayCommand]
        public void OpenTecSetting()
        {
            TecChartModel tecChart = new TecChartModel(Sender);

            ChartTecViewModel vm = new ChartTecViewModel(TecDevice, tecChart);
            ChartTEC window_tec_setting = new ChartTEC(vm);
            window_tec_setting.Show();
        }

        public ThreadSender Sender { get; set; }

        public ComPorts Ports { get; set; }

        public TecDeviceModel TecDevice { get; set; }

        public Scheduler Sched { get; set; }

        public MainWinViewModel()
        {
            Sender = new ThreadSender();
            TecDevice = new TecDeviceModel(Sender);
            Ports = new ComPorts(Sender);
            Sched = new Scheduler(Sender);
        }
    }
}
