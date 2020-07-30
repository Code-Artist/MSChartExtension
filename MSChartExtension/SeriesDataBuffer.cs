using System.Collections.Generic;

namespace System.Windows.Forms.DataVisualization.Charting
{

    internal struct PointD
    {
        public PointD(double x, double y) { X = x; Y = y; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    internal class SeriesDataBuffer
    {
        public ChartData Parent { get; set; }
        public Series Series { get; set; }
        public List<PointD> DataBuffer { get; set; } = new List<PointD>();

        public double XMin { get; internal set; }
        public double XMax { get; internal set; }
        public double YMin { get; internal set; }
        public double YMax { get; internal set; }

        public PointD XMinPoint = new PointD();
        public PointD XMaxPoint = new PointD();

        public void AddPoint(double x, double y)
        {
            DataBuffer.Add(new PointD(x, y));

            if (x < XMin)
            {
                XMinPoint.X = x;
                XMinPoint.Y = y;
            }
            if (x > XMax)
            {
                XMaxPoint.X = x;
                XMaxPoint.Y = y;
            }

            XMin = Math.Min(XMin, x);
            XMax = Math.Max(XMax, x);
            YMin = Math.Min(YMin, y);
            YMax = Math.Max(YMax, y);
        }

        public void Clear()
        {
            DataBuffer.Clear();
            XMin = YMin = Double.MaxValue;
            XMax = YMax = Double.MinValue;
        }

    }
}
