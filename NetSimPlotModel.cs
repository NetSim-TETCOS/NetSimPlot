using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetSimPlot
{
    public class NetSimPlotModel
    {
        private static readonly int maxPointCountInScreen = 5000;
        public static IList<NetSimPlotPoint> netSimPlotPoints;

        public static double minXAxis = -1;

        public static double maxXAxis = -1;

        public static double plotWindow = 50000;

        public static double prevPlotWindow = 50000;

        public static string XAxisUnit = "microsec";

        public static string yAxisUnit = "Mbps";

        public static string xAxisTitle;
        public static string yAxisTitle;
        public static string plotTitle;

        public static double XAxisMultiplier { get; set; } = 1;
        public static double YAxisMultiplier { get; set; } = 1;

        public static string[] plotName;

        public static void Add(string file)
        {
            NetSimPlotPoint netSimPlotPoint = new NetSimPlotPoint(file);
            netSimPlotPoints.Add(netSimPlotPoint);
        }

        public static void Init()
        {
            netSimPlotPoints = new List<NetSimPlotPoint>();
            if (OxyPlotInterface.cmdValue.TryGetValue("plotfile", out string plotfile))
            {
                if (OxyPlotInterface.cmdValue.TryGetValue("path", out string path))
                    Add(path + plotfile);
                else Add(plotfile);
            }
            else
                Add("plot.txt");

            foreach (var netSimPlotPoint in netSimPlotPoints)
            {
                if (minXAxis == -1) minXAxis = netSimPlotPoint.minX;
                else if (minXAxis > netSimPlotPoint.minX) minXAxis = netSimPlotPoint.minX;

                if (maxXAxis == -1) maxXAxis = netSimPlotPoint.maxX;
                else if (maxXAxis < netSimPlotPoint.maxX) maxXAxis = netSimPlotPoint.maxX;

                if (plotWindow == -1) plotWindow = 50000;
            }

            prevPlotWindow = plotWindow;

            if (XAxisUnit != "")
                xAxisTitle += " (" + XAxisUnit + ")";
            if (yAxisUnit != "")
                yAxisTitle += " (" + yAxisUnit + ")";
        }

        public static void UpdateUIBeforePlot()
        {
            foreach (var netSimPlotPoint in netSimPlotPoints)
                netSimPlotPoint.UpdateUI();

            LeftPanelModel.UpdateUI();
        }
        public static void Plot()
        {
            MainViewModel.Oxyplot_ChangeTitle(plotTitle, xAxisTitle, yAxisTitle);
            OxyPlotInterface.GetMainWindow().UpdateUIBeforePlot();

            foreach (var netSimPlotPoint in netSimPlotPoints)
                netSimPlotPoint.Plot();
        }

        public static void Optimise()
        {
            foreach (NetSimPlotPoint netSimPlotPoint in netSimPlotPoints)
                netSimPlotPoint.Optimise();

        }

        public static void Replot()
        {
            foreach (NetSimPlotPoint netSimPlotPoint in netSimPlotPoints)
                netSimPlotPoint.Replot();

        }
        public static void Update()
        {
            if (prevPlotWindow != plotWindow)
            {
                foreach (NetSimPlotPoint netSimPlotPoint in netSimPlotPoints)
                    netSimPlotPoint.Update();
            }
            prevPlotWindow = plotWindow;
        }
        private static IList<DataPoint> Trim_list(IList<DataPoint> points, double min, double max)
        {
            IList<DataPoint> ret = new List<DataPoint>();
            foreach (DataPoint point in points)
            {
                if (point.X >= min && point.X <= max)
                    ret.Add(point);
            }
            return ret;
        }

        private static List<int> GenerateUniformRandomNumber(int max, int count)
        {
            List<int> ret = new List<int>();
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                bool isFound = false;
                int r = rand.Next(max);
                foreach (int n in ret)
                {
                    if (n == r)
                    {
                        isFound = true;
                        break;
                    }
                }
                if (isFound) i--;
                else ret.Add(r);
            }

            ret.Sort();
            return ret;
        }

        public static IList<DataPoint> OptimiseList(IList<DataPoint> inputList)
        {
            IList<DataPoint> outputList = new List<DataPoint>();

            IList<DataPoint> trimList = NetSimPlotModel.Trim_list(inputList, NetSimPlotModel.minXAxis, NetSimPlotModel.maxXAxis);

            if (trimList.Count > 2 * maxPointCountInScreen)
            {
                List<int> index = NetSimPlotModel.GenerateUniformRandomNumber(trimList.Count, maxPointCountInScreen);
                foreach (int i in index)
                {
                    outputList.Add(trimList[i]);
                }
            }
            else
            {
                outputList = trimList.ToList<DataPoint>();
            }
            return outputList;
        }
    }
}
