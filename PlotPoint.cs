using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSimPlot
{
    public class PlotPoint
    {
        public double X;
        public double Y;

        public int extraYCount;
        public double[] extraY;

        public string extra;

        public PlotPoint(double x, double y)
        {
            X = x;
            Y = y;
            extra = null;
        }

        public PlotPoint(double x, double y, string ex)
        {
            X = x;
            Y = y;
            extra = ex;
        }

        public PlotPoint(double x, double y, int numY, double[] exy)
        {
            X = x;
            Y = y;
            extraYCount = numY;
            if (numY > 0)
            {
                extraY = new double[numY];
                for (int i = 0; i < numY; i++) extraY[i] = exy[i];
            }
        }
    }
}
