namespace NetSimPlot
{
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using OxyPlot.Annotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Media;

    public class MainViewModel : Observable
    {
        private PlotModel model;
        public MainViewModel()
        {
            // Create the plot model
            var tmp = new PlotModel();

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                //Disable the axis zoom
                TitleColor = OxyColors.DimGray,
                AxisTitleDistance = 20,
                TitleFontWeight = FontWeights.Normal,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };
            tmp.Axes.Add(yAxis);

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                TitleColor = OxyColors.DimGray,
                AxisTitleDistance = 20,
                TitleFontWeight = FontWeights.Normal,
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            xAxis.AxisChanged += XAxis_AxisChanged;
            tmp.Axes.Add(xAxis);

            // Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
            Model = tmp;
        }
        public static void Oxyplot_ChangeTitle(string plotTitle, string xTitle, string yTitle)
        {
            var model = OxyPlotInterface.GetPlotModel();
            if (xTitle != null)
                model.Axes[1].Title = xTitle;
            if (yTitle != null)
                model.Axes[0].Title = yTitle;
            if (plotTitle != null)
            {
                model.Title = plotTitle;
                model.TitleColor = OxyColors.DimGray;
                model.TitlePadding = 30;
                model.TitleFontWeight = FontWeights.Normal;
            }

            model.PlotView.InvalidatePlot();
        }

        private void XAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            OxyPlotInterface.Change_position_of_all_shape();
        }

        public PlotModel Model
        {
            get { return model; }
            set
            {
                if (model != value)
                {
                    model = value;
                    RaisePropertyChanged(() => Model);
                }
            }
        }

        private static IList<DataPoint> CalculateSeriesPoint(IList<DataPoint> dataPoints)
        {
            IList<DataPoint> ret = new List<DataPoint>();
            foreach (DataPoint p in dataPoints)
            {
                ret.Add(new DataPoint(x: p.X * NetSimPlotModel.XAxisMultiplier, y: p.Y * NetSimPlotModel.YAxisMultiplier));
            }
            return ret;
        }

        public static int Oxyplot_add_series(IList<DataPoint> dataPoints, Color color)
        {
            PlotModel plotModel = OxyPlotInterface.GetPlotModel();
            OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries();
            IList<DataPoint> seriesPoint = CalculateSeriesPoint(dataPoints);
            lineSeries.Points.AddRange(seriesPoint);
            plotModel.Series.Add(lineSeries);
            lineSeries.Color = OxyColor.FromArgb(color.A, color.R, color.G, color.B);
            OxyPlotInterface.GetPlotView().InvalidatePlot(true);

            return plotModel.Series.Count - 1;
        }
        public static void Oxyplot_update_series(int index, IList<DataPoint> dataPoints)
        {
            OxyPlot.Series.LineSeries lineSeries = (OxyPlot.Series.LineSeries)OxyPlotInterface.GetPlotModel().Series[index];
            lineSeries.Points.Clear();
            IList<DataPoint> seriesPoint = CalculateSeriesPoint(dataPoints);
            lineSeries.Points.AddRange(seriesPoint);
            OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            OxyPlotInterface.GetPlotView().ResetAllAxes();
        }

        public static int Oxyplot_add_function_series(double val, Color color)
        {
            PlotModel plotModel = OxyPlotInterface.GetPlotModel();
            LineSeries s = new LineSeries();
            s.Points.Add(new DataPoint(NetSimPlotModel.minXAxis * NetSimPlotModel.XAxisMultiplier, val * NetSimPlotModel.YAxisMultiplier));
            s.Points.Add(new DataPoint(NetSimPlotModel.maxXAxis * NetSimPlotModel.XAxisMultiplier, val * NetSimPlotModel.YAxisMultiplier));
            plotModel.Series.Add(s);
            s.Color = OxyColor.FromArgb(color.A, color.R, color.G, color.B);
            OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            return plotModel.Series.Count - 1;
        }

        public static void Oxyplot_update_function_series(int index, double val)
        {
            OxyPlot.Series.LineSeries lineSeries = (OxyPlot.Series.LineSeries)OxyPlotInterface.GetPlotModel().Series[index];
            lineSeries.Points.Clear();
            lineSeries.Points.Add(new DataPoint(NetSimPlotModel.minXAxis * NetSimPlotModel.XAxisMultiplier, val * NetSimPlotModel.YAxisMultiplier));
            lineSeries.Points.Add(new DataPoint(NetSimPlotModel.maxXAxis * NetSimPlotModel.XAxisMultiplier, val * NetSimPlotModel.YAxisMultiplier));
            OxyPlotInterface.GetPlotView().InvalidatePlot(true);
            OxyPlotInterface.GetPlotView().ResetAllAxes();
        }

        public static int Oxyplot_add_bar_series(IList<DataPoint> dataPoints)
        {
            PlotModel plotModel = OxyPlotInterface.GetPlotModel();
            OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries();
            IList<DataPoint> seriesPoint = CalculateSeriesPoint(dataPoints);
            foreach (DataPoint p in seriesPoint)
            {
                lineSeries.Points.Add(p);
                plotModel.Annotations.Add(new RectangleAnnotation
                {
                    MinimumX = p.X - 0.5,
                    MaximumX = p.X + 0.5,
                    MinimumY = 0,
                    MaximumY = p.Y,
                    TextRotation = 10,
                    Text = "",
                    Fill = OxyColor.FromAColor(99, OxyColors.Gold),
                    Stroke = OxyColors.Blue,
                    StrokeThickness = 1
                });

            }
            plotModel.Series.Add(lineSeries);
            OxyPlotInterface.GetPlotView().InvalidatePlot(true);

            return plotModel.Series.Count - 1;
        }
    }
}
