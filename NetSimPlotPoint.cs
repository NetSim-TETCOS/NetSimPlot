using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace NetSimPlot
{
    public class NetSimPlotPoint
    {
        public IList<PlotPoint> Points { get; private set; }
        public PLOTTYPE PlotType { get; private set; }

        public double maxX = 0;
        public double maxY = 0;
        public double minX = 0;
        public double minY = 0;

        public enum PLOTTYPE
        {
            INVALID,
            THROUGHPUT,
            BUFFER,
            CUSTOM,
        };

        public ThroughputData throughputData { get; private set; }

        public BufferData bufferData { get; private set; }

        public CustomData customData { get; private set; }

        public NetSimPlotPoint(string plotFile)
        {
            Points = new List<PlotPoint>();
            Read_plot_file(plotFile, Points);
            if (PlotType == PLOTTYPE.THROUGHPUT)
                throughputData = new ThroughputData(this);
            else if (PlotType == PLOTTYPE.BUFFER)
                bufferData = new BufferData(this);
            else if (PlotType == PLOTTYPE.CUSTOM)
                customData = new CustomData(this);
        }

        public void Plot()
        {
            if (this.PlotType == PLOTTYPE.THROUGHPUT)
                this.throughputData.Plot();
            else if (this.PlotType == PLOTTYPE.BUFFER)
                this.bufferData.Plot();
            else if (this.PlotType == PLOTTYPE.CUSTOM)
                this.customData.Plot();
        }

        public void Optimise()
        {
            if (this.PlotType == PLOTTYPE.THROUGHPUT)
                this.throughputData.OptimiseSeries();
            else if (this.PlotType == PLOTTYPE.BUFFER)
                this.bufferData.OptimiseSeries();
            else if (this.PlotType == PLOTTYPE.CUSTOM)
                this.customData.OptimiseSeries();
        }

        public void Replot()
        {
            if (this.PlotType == PLOTTYPE.THROUGHPUT)
                this.throughputData.Replot();
            else if (this.PlotType == PLOTTYPE.BUFFER)
                this.bufferData.Replot();
            else if (this.PlotType == PLOTTYPE.CUSTOM)
                this.customData.Replot();
        }
        public void Update()
        {
            if (this.PlotType == PLOTTYPE.THROUGHPUT)
                this.throughputData.Update(this);
        }

        private void Read_plot_file(string plotFile, IList<PlotPoint> pointList)
        {
            StreamReader streamReader = null;
            string line;
            int index = 0;
            int numY = 0;
            try
            {
                streamReader = new StreamReader(plotFile);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("Plot file not found!!\n" + e.Message);
                Console.WriteLine("Plot file not found!!\n" + e.Message + "\n");
                Application.Current.Shutdown();
            }
            while ((line = streamReader.ReadLine()) != null)
            {
                if (index == 0)
                {
                    PlotType = Validate_plotType(line);
                }
                else if (index == 1)
                {
                    NetSimPlotModel.plotTitle = line;
                }
                else if (PlotType == PLOTTYPE.THROUGHPUT)
                {

                    string[] d = line.Split(',');
                    double x = Double.Parse(d[0]);
                    double y = Double.Parse(d[1]);

                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;

                    NetSimPlotModel.minXAxis = minX;
                    NetSimPlotModel.maxXAxis = maxX;

                    PlotPoint p = new PlotPoint(x, y);
                    pointList.Add(p);
                }
                else if (PlotType == PLOTTYPE.BUFFER)
                {
                    string[] d = line.Split(',');
                    double x = Double.Parse(d[0]);
                    double y = Double.Parse(d[1]);

                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;

                    NetSimPlotModel.minXAxis = minX;
                    NetSimPlotModel.maxXAxis = maxX;

                    PlotPoint p = new PlotPoint(x, y, d[2]);
                    pointList.Add(p);
                }
                else if (PlotType == PLOTTYPE.CUSTOM)
                {
                    if (index == 2)
                    {
                        NetSimPlotModel.yAxisTitle = line;
                    }
                    else if (index == 3)
                    {
                        numY = int.Parse(line);
                        NetSimPlotModel.plotName = new string[numY];
                        for (int i = 0; i < numY; i++)
                            NetSimPlotModel.plotName[i] = streamReader.ReadLine();
                        index += numY;
                        numY--;
                    }
                    else
                    {
                        string[] d = line.Split(',');
                        double x = Double.Parse(d[0]);
                        double y = Double.Parse(d[1]);

                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;

                        NetSimPlotModel.minXAxis = minX;
                        NetSimPlotModel.maxXAxis = maxX;

                        double[] exy = new double[numY];
                        for (int i = 0; i < numY; i++)
                            exy[i] = Double.Parse(d[2 + i]);
                        PlotPoint p = new PlotPoint(x, y, numY, exy);
                        pointList.Add(p);
                    }
                }
                index++;
            }
        }

        private PLOTTYPE Validate_plotType(string s)
        {
            if (String.Compare(s, "Throughput_plot", true) == 0)
            {
                Console.WriteLine("Plot type is validated\n");
                return PLOTTYPE.THROUGHPUT;
            }
            else if (String.Compare(s, "BUFFER_PLOT", true) == 0)
            {
                Console.WriteLine("Plot type is validated. Buffer plot");
                return PLOTTYPE.BUFFER;
            }
            else if (String.Compare(s, "CUSTOM_PLOT", true) == 0)
            {
                Console.WriteLine("Plot type is validated. Custom plot");
                return PLOTTYPE.CUSTOM;
            }
            else
            {
                MessageBox.Show("Plot type is not validated!!");
                Application.Current.Shutdown();
                return PLOTTYPE.INVALID;
            }
        }

        public void UpdateUI()
        {
            LeftPanelModel.SetChartTitle(NetSimPlotModel.plotTitle);
            if (this.PlotType == PLOTTYPE.THROUGHPUT)
                this.throughputData.UpdateUI();
            else if (this.PlotType == PLOTTYPE.BUFFER)
                this.bufferData.UpdateUI();
            else if (this.PlotType == PLOTTYPE.CUSTOM)
                this.customData.UpdateUI();
        }
    }
}
