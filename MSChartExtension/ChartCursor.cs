namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Chart Cursor Position 
    /// </summary>
    public class ChartCursor : ICloneable
    {
        /// <summary>
        /// Return 1 for Cursor1 and 2 for Cursor2
        /// </summary>
        public int CursorIndex { get; set; }
        /// <summary>
        /// X Value based on primary Axis
        /// </summary>
        public double X { get; internal set; } = double.NaN;
        /// <summary>
        /// Y Value based on primary Axis
        /// </summary>
        public double Y { get; internal set; } = double.NaN;
        /// <summary>
        /// Return X Value as string based on <see cref="Series.XValueType"/>.
        /// </summary>
        /// <returns></returns>
        public string XFormattedString { get; internal set; }
        /// <summary>
        /// Return Y Value as string based on <see cref="Series.YValueType"/>
        /// </summary>
        /// <returns></returns>
        public string YFormattedString { get; internal set; }

        /// <summary>
        /// ChartArea where the cursor is located.
        /// </summary>
        public ChartArea ChartArea { get; set; } = null;
        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new ChartCursor()
            {
                X = this.X,
                Y = this.Y,
                ChartArea = this.ChartArea,
                SelectedChartSeries = this.SelectedChartSeries,
                CursorIndex = this.CursorIndex
            };
        }
        /// <summary>
        /// Selected Chart Series
        /// </summary>
        public Series SelectedChartSeries { get; set; } = null;
    }

}
