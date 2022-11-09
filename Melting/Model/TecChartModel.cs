using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using ServiceSender.Data;
using ServiceSender.ThreadSender;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace Melting.Model
{

    public class TecPoint
    {
        public float Temperature { get; set; }

        public DateTime Time { get; set; } 
    }

    [ObservableObject]
    public partial class TecChartModel
    {

        public TecChartModel(ThreadSender sender)
        {
            this.sender = sender;
            this.sender.AchivedResult += this.HandlerChart;

            Data = new ObservableCollection<TecPoint>();

            Series = new ISeries[]
            {
                new LineSeries<TecPoint>
                {
                    Values = Data,
                    Name = "Temperature",
                    Fill = null,
                    GeometrySize = 0,
                    Mapping = (tec, point) =>
                    {
                        point.PrimaryValue = (float)tec.Temperature;
                        point.SecondaryValue = tec.Time.Ticks;
                    }
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Время",
                    Labeler = (value) => new DateTime((long)value).ToString("mm:ss"),
                    UnitWidth = TimeSpan.FromSeconds(1).Ticks,
                    MinStep = TimeSpan.FromSeconds(1).Ticks,
                    TextSize = 14,
                    SeparatorsPaint = new SolidColorPaint
                    {
                        Color = SKColors.LightGray,
                        StrokeThickness = 1,
                        PathEffect = new DashEffect(new float[] { 2, 5 })
                    }
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Температура, °C",
                    TextSize = 14,
                    SeparatorsPaint = new SolidColorPaint
                    {
                        Color = SKColors.LightGray,
                        StrokeThickness = 1,
                    }
                }
            };
        }

        public ObservableCollection<TecPoint> Data { get; set; }

        public ISeries[] Series { get; set; }

        public Axis[] XAxes { get; set; }

        public Axis[] YAxes { get; set; }

        public int TimeWindow { get; private set; } = 90;

        private void AddTecPoint(float sensorTemp)
        {
            // Add new point
            Data.Add(new() { Time = new DateTime(DateTime.Now.Ticks), Temperature = sensorTemp });
            
            // Remove old - make constant change
            if ((Data[^1].Time - Data[0].Time) > TimeSpan.FromSeconds(TimeWindow))
            {
                Data.RemoveAt(0);
            }

            // Move frame
            XAxes[0].MinLimit = Data[0].Time.Ticks;
            XAxes[0].MaxLimit = Data[0].Time.Ticks + TimeSpan.FromSeconds(TimeWindow).Ticks; 
        }

        private readonly ThreadSender? sender;

        private void HandlerChart(object sender, ResponseData e)
        {
            if (e.IsSuccess)
            {
                switch (e.BoundCommand.Command.SubAddr)
                {
                    case 5:
                        {
                            if (e.BoundCommand.Command.NumOfWords == e.Data!.Length)
                            {
                                float SensorTemp = BitConverter.ToSingle(e.Data);
                                AddTecPoint(SensorTemp);
                            }
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
        }

    }
}
