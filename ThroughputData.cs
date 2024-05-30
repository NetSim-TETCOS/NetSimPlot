using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace NetSimPlot
{
    public class ThroughputData
    {
        //Moving average
        private readonly IList<DataPoint> movingAvgThroughput;
        private IList<DataPoint> movingAvgThroughput_forPlot;
        public int MovingAvgThroughputPlotSeriesIndex { get; private set; }

        //Time average
        private double timeAvgThroughput;
        public int TimeAvgThroughputPlotSeriesIndex { get; private set; }

        //Sliding window
        private readonly IList<DataPoint> slidingWindowThroughput;
        private IList<DataPoint> slidingWindowThroughput_forPlot;
        public int SlidingWindowThroughputSeriesIndex { get; private set; }


        public ThroughputData(NetSimPlotPoint netSimPlotPoint)
        {
            movingAvgThroughput = new List<DataPoint>();
            movingAvgThroughput_forPlot = new List<DataPoint>();

            slidingWindowThroughput = new List<DataPoint>();
            slidingWindowThroughput_forPlot = new List<DataPoint>();

            NetSimPlotModel.xAxisTitle = "Time";
            NetSimPlotModel.yAxisTitle = "Throughput";

            Calculate_moving_average_throughput(netSimPlotPoint);
            Calculate_sliding_window_throughput(netSimPlotPoint);
        }

        private void Calculate_moving_average_throughput(NetSimPlotPoint netSimPlotPoint)
        {
            double maxThroughput = 0;
            double maxTime = 0;
            IList<PlotPoint> dataPoints = netSimPlotPoint.Points;
            double size = 0;
            foreach (PlotPoint p in dataPoints)
            {
                double t = p.X;
                double s = p.Y;

                size += s;
                if (t < NetSimPlotModel.plotWindow) { continue; }

                DataPoint d = new DataPoint(t, size * 8.0 / t);
                movingAvgThroughput.Add(d);
                timeAvgThroughput = d.Y;
                if (d.Y > maxThroughput) { maxThroughput = d.Y; }
                if (d.X > maxTime) { maxTime = d.X; }
            }

            if (maxThroughput < 1)
            {
                NetSimPlotModel.yAxisUnit = "kbps";
                NetSimPlotModel.YAxisMultiplier = 1000;
            }
            else if (maxThroughput > 1000)
            {
                NetSimPlotModel.yAxisUnit = "gbps";
                NetSimPlotModel.YAxisMultiplier = 0.001;
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

        private double Find_size(List<PlotPoint> dataPoints, int index, double time)
        {
            double curr = dataPoints[index].X;
            double siz = 0;
            for (int i = index; i < dataPoints.Count; i++)
            {
                if (curr - dataPoints[i].X <= time)
                {
                    siz += dataPoints[i].Y;
                }
                else { break; }
            }
            return siz;
        }

        private void Calculate_sliding_window_throughput(NetSimPlotPoint netSimPlotPoint)
        {
            slidingWindowThroughput.Clear();
            DataPoint prev = new DataPoint(netSimPlotPoint.maxX, 0);
            List<PlotPoint> revList = new List<PlotPoint>(netSimPlotPoint.Points);
            revList.Reverse();
            for (int i = 0; i < revList.Count; i++)
            {
                double siz = Find_size(revList, i, NetSimPlotModel.plotWindow);
                if (prev.X - revList[i].X > NetSimPlotModel.plotWindow)
                {
                    slidingWindowThroughput.Add(new DataPoint(prev.X - NetSimPlotModel.plotWindow, 0));
                    slidingWindowThroughput.Add(new DataPoint(revList[i].X + NetSimPlotModel.plotWindow, 0));
                }
                slidingWindowThroughput.Add(new DataPoint(revList[i].X, siz * 8 / NetSimPlotModel.plotWindow));
                prev = new DataPoint(revList[i].X, 0);
            }
            _ = slidingWindowThroughput.Reverse<DataPoint>();
        }

        public void OptimiseSeries()
        {
            ProgressBarStatus.ReportProgress("Optimizing throughput series.");
            movingAvgThroughput_forPlot = NetSimPlotModel.OptimiseList(movingAvgThroughput);
            slidingWindowThroughput_forPlot = NetSimPlotModel.OptimiseList(slidingWindowThroughput);
        }

        private void PlotMovingAverage()
        {
            MovingAvgThroughputPlotSeriesIndex = MainViewModel.Oxyplot_add_series(movingAvgThroughput_forPlot, Colors.OrangeRed);
            
        }

        private void PlotSlidingWindow()
        {
            SlidingWindowThroughputSeriesIndex = MainViewModel.Oxyplot_add_series(slidingWindowThroughput_forPlot, Colors.Gold);
        }
        private void PlotTimeAverage()
        {
            TimeAvgThroughputPlotSeriesIndex = MainViewModel.Oxyplot_add_function_series(timeAvgThroughput, Colors.SkyBlue);
        }
        public void Plot()
        {
            PlotSlidingWindow();
            PlotMovingAverage();
            PlotTimeAverage();
        }
        private void ReplotMovingAverage()
        {
            MainViewModel.Oxyplot_update_series(MovingAvgThroughputPlotSeriesIndex, movingAvgThroughput_forPlot);
        }

        private void ReplotSlidingWindow()
        {
            MainViewModel.Oxyplot_update_series(SlidingWindowThroughputSeriesIndex, slidingWindowThroughput_forPlot);
        }
        private void ReplotTimeAverage()
        {
            MainViewModel.Oxyplot_update_function_series(TimeAvgThroughputPlotSeriesIndex, timeAvgThroughput);
        }
        public void Replot()
        {
            ReplotMovingAverage();
            ReplotTimeAverage();
            ReplotSlidingWindow();
        }
        public void Update(NetSimPlotPoint netSimPlotPoint)
        {
            Calculate_sliding_window_throughput(netSimPlotPoint);
        }

        private void InstantaneousPlotColorChanges(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            int index = NetSimPlotModel.netSimPlotPoints[0].throughputData.SlidingWindowThroughputSeriesIndex;
            PlotModel model = OxyPlotInterface.GetPlotModel();

            if (model.Series.Count > 0)
            {
                ColorPicker colorPicker = (ColorPicker)sender;
                OxyPlot.Series.LineSeries series1 = (OxyPlot.Series.LineSeries)model.Series[index];
                series1.Color = colorPicker.SelectedColor.Value.ToOxyColor();
                OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            }
        }
        private void SlidingWindowCheckbox_Click(object sender, RoutedEventArgs e)
        {
            PlotModel model = OxyPlotInterface.GetPlotModel();
            if (model.Series.Count > 0)
            {
                int index = NetSimPlotModel.netSimPlotPoints[0].throughputData.SlidingWindowThroughputSeriesIndex;
                CheckBox box = (CheckBox)sender;
                OxyPlot.Series.LineSeries series1 = (OxyPlot.Series.LineSeries)model.Series[index];
                series1.IsVisible = (bool)box.IsChecked;
                OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            }
        }

        private void MovingAvgColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

            int index = NetSimPlotModel.netSimPlotPoints[0].throughputData.MovingAvgThroughputPlotSeriesIndex;
            PlotModel model = OxyPlotInterface.GetPlotModel();

            if (model.Series.Count > 0)
            {
                ColorPicker colorPicker = (ColorPicker)sender;
                OxyPlot.Series.LineSeries series1 = (OxyPlot.Series.LineSeries)model.Series[index];
                series1.Color = colorPicker.SelectedColor.Value.ToOxyColor();
                OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            }
        }

        private void TimeAvgColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

            int index = NetSimPlotModel.netSimPlotPoints[0].throughputData.TimeAvgThroughputPlotSeriesIndex;
            PlotModel model = OxyPlotInterface.GetPlotModel();

            if (model.Series.Count > 0)
            {
                ColorPicker colorPicker = (ColorPicker)sender;
                OxyPlot.Series.LineSeries series1 = (OxyPlot.Series.LineSeries)model.Series[index];
                series1.Color = colorPicker.SelectedColor.Value.ToOxyColor();
                OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            }
        }

        private void MovingAvgCheckbox_Click(object sender, RoutedEventArgs e)
        {
            PlotModel model = OxyPlotInterface.GetPlotModel();
            if (model.Series.Count > 0)
            {
                int index = NetSimPlotModel.netSimPlotPoints[0].throughputData.MovingAvgThroughputPlotSeriesIndex;
                CheckBox box = (CheckBox)sender;
                OxyPlot.Series.LineSeries series1 = (OxyPlot.Series.LineSeries)model.Series[index];
                series1.IsVisible = (bool)box.IsChecked;
                OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            }
        }
        private void TimeAvgCheckbox_Click(object sender, RoutedEventArgs e)
        {
            PlotModel model = OxyPlotInterface.GetPlotModel();
            if (model.Series.Count > 0)
            {
                int index = NetSimPlotModel.netSimPlotPoints[0].throughputData.TimeAvgThroughputPlotSeriesIndex;
                CheckBox box = (CheckBox)sender;
                OxyPlot.Series.LineSeries series1 = (OxyPlot.Series.LineSeries)model.Series[index];
                series1.IsVisible = (bool)box.IsChecked;
                OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            }
        }
        public void UpdateUI()
        {
            _ = LeftPanelModel.AddCustomCheckBox("Instantaneous", Colors.Gold, InstantaneousPlotColorChanges, SlidingWindowCheckbox_Click);
            _ = LeftPanelModel.AddCustomCheckBox("Cumulative Moving Avg.", Colors.OrangeRed, MovingAvgColorChanged, MovingAvgCheckbox_Click);
            _ = LeftPanelModel.AddCustomCheckBox("Time Avg.", Colors.SkyBlue, TimeAvgColorChanged, TimeAvgCheckbox_Click);

            LeftPanelModel.SetRowVisibility(3, true);
            LeftPanelModel.SetRowVisibility(4, true);
        }

        private static bool GetWriteIndex(List<DataPoint> l1, List<DataPoint> l2,
            ref int i1, ref int i2, ref double x, ref double y1, ref double y2)
        {
            if (i1 == l1.Count && i2 == l2.Count) { return false; }
            if (i1 == l1.Count)
            {
                x = l2[i2].X;
                y2 = l2[i2].Y;
                i2++;
                return true;
            }

            if (i1 == l1.Count)
            {
                x = l1[i1].X;
                y1 = l1[i1].Y;
                i1++;
                return true;
            }

            if (l1[i1].X < l2[i2].X)
            {
                x = l1[i1].X;
                y1 = l1[i1].Y;
                i1++;
            }
            else if (l1[i1].X > l2[i2].X)
            {
                x = l2[i2].X;
                y2 = l2[i2].Y;
                i2++;
            }
            else
            {
                x = l1[i1].X;
                y1 = l1[i1].Y;
                y2 = l2[i2].Y;
                i1++;
                i2++;
            }
            return true;
        }

        public static string WriteOutputData()
        {
            foreach (NetSimPlotPoint plotPoint in NetSimPlotModel.netSimPlotPoints)
            {
                if (plotPoint.PlotType == NetSimPlotPoint.PLOTTYPE.THROUGHPUT)
                {
                    List<DataPoint> movDataPoint = plotPoint.throughputData.movingAvgThroughput.ToList();
                    List<DataPoint> InsDataPoint = plotPoint.throughputData.slidingWindowThroughput.ToList();
                    InsDataPoint.Reverse();
                    int movIndex = 0;
                    int InsIndex = 0;
                    double x = 0;
                    double y1 = 0;
                    double y2 = 0;
                    try
                    {
                        string p = Path.GetTempPath() + "PlotData.csv";
                        StreamWriter sw = new StreamWriter(p);

                        sw.WriteLine("Time(microsec),Moving Average throughput(mbps),Instantaneous throughput(mbps),Time average throughput(mbps),");
                        while (GetWriteIndex(movDataPoint, InsDataPoint, ref movIndex, ref InsIndex, ref x, ref y1, ref y2))
                        {
                            sw.WriteLine(x.ToString() + "," +
                                y1.ToString() + "," +
                                y2.ToString() + "," +
                                plotPoint.throughputData.timeAvgThroughput.ToString() + ",");
                        }
                        sw.Close();
                        return p;
                    }
                    catch (Exception e)
                    {
                        _ = System.Windows.MessageBox.Show(e.Message, "NetSim plot error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            return null;
        }
    }
}
