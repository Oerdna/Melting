using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Melting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melting.ModelView
{
    public partial class ChartTecViewModel
    {
        public TecDeviceModel TecDevice { get; set; }

        public TecChartModel TecChart { get; set; }

        public ChartTecViewModel(TecDeviceModel tecDevice, TecChartModel tecChart)
        {
            TecDevice = tecDevice;
            TecChart = tecChart;
        }
    }
}
