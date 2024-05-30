using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Wpf;

namespace NetSimPlot
{
    public class OxyPlotInterface
    {
        public static Dictionary<string, string> cmdValue;
        public static bool isUIShown;

        private DataPoint dataPoint;

        public static MainWindow GetMainWindow()
        {
            return (MainWindow)Application.Current.MainWindow;
        }
        public static PlotView GetPlotView()
        {
            return GetMainWindow().plot1;
        }

        public static PlotModel GetPlotModel()
        {
            return GetPlotView().Model;
        }

        private static Canvas PlotCanvas => GetMainWindow().plotCanvas;


        private void SetDataPoint(Point scrPoint)
        {
            PlotModel m = GetPlotModel();
            ScreenPoint s = new ScreenPoint(scrPoint.X, scrPoint.Y);
            DataPoint d = OxyPlot.Axes.Axis.InverseTransform(s, m.DefaultXAxis, m.DefaultYAxis);
            dataPoint = d;
        }

        public static void Update_all_shapes()
        {
            Canvas c = PlotCanvas;
            for (int i = 0; i < c.Children.Count; i++)
            {
                ContentControl cc = (ContentControl)c.Children[i];
                OxyPlotInterface oxyPlotInterface = (OxyPlotInterface)cc.Tag;

                double y = Canvas.GetTop(cc);
                double x = Canvas.GetLeft(cc);

                Point p = c.TranslatePoint(new Point(x, y), GetPlotView());
                oxyPlotInterface.SetDataPoint(p);
            }
        }

        public static void Change_position_of_all_shape()
        {
            Canvas c = PlotCanvas;
            for (int i = 0; i < c.Children.Count; i++)
            {
                ContentControl cc = (ContentControl)c.Children[i];
                OxyPlotInterface oxyPlotInterface = (OxyPlotInterface)cc.Tag;

                PlotModel model = GetPlotModel();
                ScreenPoint scr = model.DefaultXAxis.Transform(oxyPlotInterface.dataPoint.X,
                    oxyPlotInterface.dataPoint.Y,
                    model.DefaultYAxis);

                Point p = GetPlotView().TranslatePoint(new Point(scr.X, scr.Y), c);
                Canvas.SetLeft(cc, p.X);
                Canvas.SetTop(cc, p.Y);
            }
            c.InvalidateVisual();
        }
    }
}
