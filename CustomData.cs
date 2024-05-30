using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace NetSimPlot
{
    public class CustomData
    {
        private readonly int plotCount;
        private readonly IList<DataPoint>[] plotData;
        private readonly IList<DataPoint>[] plotData_forPlot;
        public int[] PlotSeriesIndex { get; private set; }

        private Color[] PlotColor = { Colors.Gold, Colors.OrangeRed, Colors.SkyBlue };

        private CustomCheckBox[] customCheckBoxes;
        public CustomData(NetSimPlotPoint netSimPlotPoint)
        {
            plotCount = netSimPlotPoint.Points[0].extraYCount + 1;
            plotData = new List<DataPoint>[plotCount];
            plotData_forPlot = new List<DataPoint>[plotCount];
            PlotSeriesIndex = new int[plotCount];
            customCheckBoxes = new CustomCheckBox[plotCount];
            var random = new Random();
            for (int i = 0; i < plotCount; i++)
            {
                plotData[i] = new List<DataPoint>();
                plotData_forPlot[i] = new List<DataPoint>();

                if (i >= 3)
                {
                    var color = Color.FromRgb((byte)random.Next(250), (byte)random.Next(250), (byte)random.Next(250));
                    PlotColor = PlotColor.Concat(new Color[] { color }).ToArray();
                }

                Init_plot_data(netSimPlotPoint, i);
            }

            if (netSimPlotPoint.maxX > 1000)
            {
                NetSimPlotModel.XAxisUnit = "ms";
                NetSimPlotModel.XAxisMultiplier = 0.001;
            }

            if (netSimPlotPoint.maxX > 1000000)
            {
                NetSimPlotModel.XAxisUnit = "sec";
                NetSimPlotModel.XAxisMultiplier = 0.000001;
            }
            NetSimPlotModel.xAxisTitle = "Time";
            NetSimPlotModel.yAxisUnit = "";
        }

        private void Init_plot_data(NetSimPlotPoint netSimPlotPoint, int index)
        {
            if (index == 0)
            {
                for (int i = 0; i < netSimPlotPoint.Points.Count; i++)
                    plotData[index].Add(new DataPoint(netSimPlotPoint.Points[i].X, netSimPlotPoint.Points[i].Y));
            }
            else
            {
                for (int i = 0; i < netSimPlotPoint.Points.Count; i++)
                    plotData[index].Add(new DataPoint(netSimPlotPoint.Points[i].X, netSimPlotPoint.Points[i].extraY[index - 1]));
            }

        }

        public void OptimiseSeries()
        {
            ProgressBarStatus.ReportProgress("Optimising series.");
            for (int i = 0; i < plotCount; i++)
                plotData_forPlot[i] = NetSimPlotModel.OptimiseList(plotData[i]);
        }

        public void Plot()
        {
            for (int i = 0; i < plotCount; i++)
            {
                PlotSeriesIndex[i] = MainViewModel.Oxyplot_add_series(plotData_forPlot[i], PlotColor[i]);
                customCheckBoxes[i].checkBox.Tag = (Object)PlotSeriesIndex[i];
                customCheckBoxes[i].ColorPicker.Tag = (Object)PlotSeriesIndex[i];
            }
        }

        public void Replot()
        {
            for (int i = 0; i < plotCount; i++)
                MainViewModel.Oxyplot_update_series(PlotSeriesIndex[i], plotData_forPlot[i]);
        }

        private void PlotColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            PlotModel model = OxyPlotInterface.GetPlotModel();

            if (model.Series.Count > 0)
            {
                ColorPicker colorPicker = (ColorPicker)sender;
                int index = (int)(colorPicker.Tag);
                OxyPlot.Series.LineSeries series1 = (OxyPlot.Series.LineSeries)model.Series[index];
                series1.Color = colorPicker.SelectedColor.Value.ToOxyColor();
                OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            }
        }

        private void PlotCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            PlotModel model = OxyPlotInterface.GetPlotModel();
            if (model.Series.Count > 0)
            {
                int index = (int)box.Tag;
                OxyPlot.Series.LineSeries series1 = (OxyPlot.Series.LineSeries)model.Series[index];
                series1.IsVisible = (bool)box.IsChecked;
                OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            }
        }
        public void UpdateUI()
        {
            for (int i = 0; i < plotCount; i++)
                customCheckBoxes[i] = LeftPanelModel.AddCustomCheckBox(NetSimPlotModel.plotName[i], PlotColor[i], PlotColorChanged, PlotCheckBox_Click);
        }
    }
}
