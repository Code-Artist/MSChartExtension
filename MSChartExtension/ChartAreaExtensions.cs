namespace System.Windows.Forms.DataVisualization.Charting
{
    public static class ChartAreaExtensions
    {
        /// <summary>
        /// Gets the boundaries (top, left, bottom, right) of this chart area's
        /// data in the same units as the data.
        /// </summary>
        /// <param name="ptrChartArea">The chart area.</param>
        /// <param name="justVisible">If set to <c>true</c>, 
        /// get only the visible area boundaries; otherwise,
        /// get the overall boundaries.</param>
        /// <returns></returns>
        public static ChartExtents GetBoundariesOfData(this ChartArea ptrChartArea, bool justVisible = false)
        {
            return new ChartExtents(ptrChartArea, justVisible);
        }
    }
}