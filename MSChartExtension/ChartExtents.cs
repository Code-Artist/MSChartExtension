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
        public RectangleF PrimaryExtents { get; set; }
    }
}