using System.Drawing;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Contains the boundaries (top, bottom, left, right) for the chart
    /// and provides access to those boundaries in units of the data
    /// for desired axes.
    /// </summary>
    public class ChartExtents
    {
        /// <summary>Gets or sets the boundaries of the primary axis.</summary>
        /// <value>The primary extents.</value>
        public RectangleF PrimaryExtents { get; set; }

        /// <summary>Gets or sets the boundaries of the secondary axis.</summary>
        /// <value>The secondary extents.</value>
        public RectangleF SecondaryExtents { get; set; }

        public override string ToString()
        {
            return string.Format("PrimaryExtents: {0}, SecondaryExtents: {1}", 
                PrimaryExtents.ToStringWithBoundaries(), 
                SecondaryExtents.ToStringWithBoundaries());
        }

        internal ChartExtents(ChartArea ptrChartArea, bool justVisible)
        {
            var primary = GetBoundariesOfDataCore(ptrChartArea.AxisX, ptrChartArea.AxisY, justVisible);
            var secondary = GetBoundariesOfDataCore(ptrChartArea.AxisX2, ptrChartArea.AxisY2, justVisible);
            PrimaryExtents = primary;
            SecondaryExtents = secondary;
        }

        internal static RectangleF GetBoundariesOfDataCore(Axis axisX, Axis axisY, bool justVisible)
        {
            double left;
            double right;
            double bottom;
            double top;
            axisX.GetMinMax(out left, out right, justVisible);
            if (axisX.IsReversed)
            {
                var temp = left;
                left = right;
                right = temp;
            }
            axisY.GetMinMax(out bottom, out top, justVisible);
            if (axisY.IsReversed)
            {
                var temp = top;
                top = bottom;
                bottom = temp;
            }
            return ExtentsFromDataCoordinates(left, top, right, bottom);
        }

        internal static RectangleF ExtentsFromDataCoordinates(
            double left, double top, double right, double bottom)
        {
            //NOTE: Height needs to be negative because we always 
            //  specify the *top* left corner
            var rect = new RectangleF((float)left, (float)top,
                (float)(right - left), (float)(bottom - top));
            return rect;
        }
    }
}