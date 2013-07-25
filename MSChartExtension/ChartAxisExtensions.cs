namespace System.Windows.Forms.DataVisualization.Charting
{
    public static class ChartAxisExtensions
    {
        /// <summary>Gets the min max.</summary>
        /// <param name="axis">The axis.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <param name="justVisible">if set to true, 
        /// then just get the visible extrema; otherwise, 
        /// get the overall extrema.</param>
        public static void GetMinMax(this Axis axis, 
            out double min, out double max, bool justVisible = false)
        {
            // NOTE: Out params aren't great, but they make the 
            //  client syntax much nicer than Tuple<double, double> 
            //  does. If only they were like Python's tuples...
            if (justVisible)
            {
                min = axis.ScaleView.ViewMinimum;
                max = axis.ScaleView.ViewMaximum;
            }
            else
            {
                min = axis.Minimum;
                max = axis.Maximum;
            }
        }
    }
}