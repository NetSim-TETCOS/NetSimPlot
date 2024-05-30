using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Color = System.Windows.Media.Color;

namespace NetSimPlot
{
    public class BufferData
    {
        private readonly IList<DataPoint> BufferSize;
        private IList<DataPoint> BufferSize_forPlot;
        public int BufferSizePlotIndex { get; private set; }

        private readonly IList<DataPoint> PacketCount;
        private IList<DataPoint> PacketCount_forPlot;
        public int PacketCountPlotIndex { get; private set; }

        private readonly IList<DataPoint> DropPacketSize;
        private IList<DataPoint> DropPacketSize_forPlot;
        public int DropPacketSizePlotIndex { get; private set; }

        private readonly IList<DataPoint> DropPacketCount;
        private IList<DataPoint> DropPacketCount_forPlot;
        public int DropPacketCountPlotIndex { get; private set; }

        public BufferData(NetSimPlotPoint netSimPlotPoint)
        {
            BufferSize = new List<DataPoint>();
            BufferSize_forPlot = new List<DataPoint>();

            PacketCount = new List<DataPoint>();
            PacketCount_forPlot = new List<DataPoint>();

            DropPacketSize = new List<DataPoint>();
            DropPacketSize_forPlot = new List<DataPoint>();

            DropPacketCount = new List<DataPoint>();
            DropPacketCount_forPlot = new List<DataPoint>();

            NetSimPlotModel.xAxisTitle = "Time";
            NetSimPlotModel.yAxisTitle = "Buffer Occupancy";

            Calculate_buffer_size(netSimPlotPoint);
        }

        private void Calculate_buffer_size(NetSimPlotPoint netSimPlotPoint)
        {
            double maxBufferSize = 0;
            double maxTime = 0;
            double bufferSize = 0;
            double bufferCount = 0;
            foreach (PlotPoint p in netSimPlotPoint.Points)
            {
                if (String.Compare(p.extra, "drop", true) == 0)
                {
                    DataPoint d = new DataPoint(p.X, p.Y);
                    DropPacketSize.Add(d);

                    DataPoint d1 = new DataPoint(p.X, 1);
                    DropPacketCount.Add(d1);
                }
                else if (String.Compare(p.extra, "enqueue", true) == 0)
                {
                    bufferSize += p.Y;
                    bufferCount++;
                    DataPoint d = new DataPoint(p.X, bufferSize);
                    BufferSize.Add(d);

                    DataPoint d1 = new DataPoint(p.X, bufferCount);
                    PacketCount.Add(d);
                }
                else if (String.Compare(p.extra, "dequeue", true) == 0)
                {
                    bufferSize -= p.Y;
                    bufferCount--;
                    DataPoint d = new DataPoint(p.X, bufferSize);
                    BufferSize.Add(d);

                    DataPoint d1 = new DataPoint(p.X, bufferCount);
                    PacketCount.Add(d);
                }

                if (bufferSize > maxBufferSize) maxBufferSize = bufferSize;
                if (p.X > maxTime) maxTime = p.X;
            }

            if (maxBufferSize > 1024 * 1024 * 1024)
            {
                NetSimPlotModel.yAxisUnit = "GB";
                NetSimPlotModel.YAxisMultiplier = 1.0 / (1024 * 1024 * 1024);
            }
            else if (maxBufferSize > 1024 * 1024)
            {
                NetSimPlotModel.yAxisUnit = "MB";
                NetSimPlotModel.YAxisMultiplier = 1.0 / (1024 * 1024);
            }
            else if (maxBufferSize > 1024)
            {
                NetSimPlotModel.yAxisUnit = "KB";
                NetSimPlotModel.YAxisMultiplier = 1 / 1024.0;
            }
            else
            {
                NetSimPlotModel.yAxisUnit = "B";
                NetSimPlotModel.YAxisMultiplier = 1;
            }

            if (maxTime > 1000)
            {
                NetSimPlotModel.XAxisUnit = "ms";
                NetSimPlotModel.XAxisMultiplier = 0.001;
            }

            if (maxTime > 1000000)
            {
                NetSimPlotModel.XAxisUnit = "sec";
                NetSimPlotModel.XAxisMultiplier = 0.000001;
            }
        }
        public void OptimiseSeries()
        {
            ProgressBarStatus.ReportProgress("Optimising Buffer size series.");
            BufferSize_forPlot = NetSimPlotModel.OptimiseList(BufferSize);
            DropPacketSize_forPlot = NetSimPlotModel.OptimiseList(DropPacketSize);
        }
        private void PlotBufferSize()
        {
            BufferSizePlotIndex = MainViewModel.Oxyplot_add_series(BufferSize_forPlot, Colors.Gold);
            //BufferSizePlotIndex = MainViewModel.Oxyplot_add_bar_series(BufferSize_forPlot);
        }

        private void PlotDropSize()
        {
            DropPacketSizePlotIndex = MainViewModel.Oxyplot_add_series(DropPacketSize_forPlot, Colors.OrangeRed);
        }
        public void Plot()
        {
            PlotBufferSize();
            //PlotDropSize();
        }
        private void ReplotBufferSize()
        {
            MainViewModel.Oxyplot_update_series(BufferSizePlotIndex, BufferSize_forPlot);
        }
        private void ReplotDropSize()
        {
            MainViewModel.Oxyplot_update_series(DropPacketSizePlotIndex, DropPacketSize_forPlot);
        }
        public void Replot()
        {
            ReplotBufferSize();
            //ReplotDropSize();
        }

        private void BufferPlotColorChanges(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            int index = NetSimPlotModel.netSimPlotPoints[0].bufferData.BufferSizePlotIndex;
            PlotModel model = OxyPlotInterface.GetPlotModel();

            if (model.Series.Count > 0)
            {
                ColorPicker colorPicker = (ColorPicker)sender;
                OxyPlot.Series.LineSeries series1 = (OxyPlot.Series.LineSeries)model.Series[index];
                series1.Color = colorPicker.SelectedColor.Value.ToOxyColor();
                OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            }
        }

        private void BufferPlotCheckbox_Click(object sender, RoutedEventArgs e)
        {
            PlotModel model = OxyPlotInterface.GetPlotModel();
            if (model.Series.Count > 0)
            {
                int index = NetSimPlotModel.netSimPlotPoints[0].bufferData.BufferSizePlotIndex;
                CheckBox box = (CheckBox)sender;
                OxyPlot.Series.LineSeries series1 = (OxyPlot.Series.LineSeries)model.Series[index];
                series1.IsVisible = (bool)box.IsChecked;
                OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            }
        }

        public void UpdateUI()
        {
            LeftPanelModel.AddCustomCheckBox("Buffer Occupancy", Colors.Gold, BufferPlotColorChanges, BufferPlotCheckbox_Click);

            LeftPanelModel.SetRowVisibility(3, false);
            LeftPanelModel.SetRowVisibility(4, false);

        }
    }
}
