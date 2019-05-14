using System.Drawing;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Configuration options for <see cref="MSChartExtension"/> 
    /// </summary>
    [Serializable]
    public class ChartOption
    {
        /// <summary>
        /// Enable / Disable controls in ContextMenu to show / hide series
        /// </summary>
        public bool ContextMenuAllowToHideSeries { get; set; } = true;
        /// <summary>
        /// Round value on XAxis to number of decimal place
        /// </summary>
        public int XAxisPrecision { get; set; } = -1;
        /// <summary>
        /// Round value on YAxis to number of decimal place
        /// </summary>
        public int YAxisPrecision { get; set; } = -1;
        /// <summary>
        /// Cursor 1 Color, default is <see cref="Color.Red"/> 
        /// </summary>
        public Color Cursor1Color { get; set; } = Color.Red;
        /// <summary>
        /// Cursor 2 Color, default is <see cref="Color.Green"/> 
        /// </summary>
        public Color Cursor2Color { get; set; } = Color.Green;
        /// <summary>
        /// Cursor 1 Dash Style, default is <see cref="ChartDashStyle.Dash"/> 
        /// </summary>
        public ChartDashStyle Cursor1DashStyle { get; set; } = ChartDashStyle.Dash;
        /// <summary>
        /// Cursor 2 Dash Style, default is <see cref="ChartDashStyle.Dash"/> 
        /// </summary>
        public ChartDashStyle Cursor2DashStyle { get; set; } = ChartDashStyle.Dash;
        /// <summary>
        /// Cursor 1 Line Width, default = 1
        /// </summary>
        public int Cursor1LineWidth { get; set; } = 1;
        /// <summary>
        /// Cursor 2 Line Width, default = 1
        /// </summary>
        public int Cursor2LineWidth { get; set; } = 1;
        /// <summary>
        /// Enable / Disable snap cursor to nearest data points.
        /// This feature is enabled by default.
        /// </summary>
        public bool SnapCursorToData { get; set; } = true;
        /// <summary>
        /// Define string format of cussors value. Default is "F4", 4 digits fixed decimal.
        /// <see cref="double.ToString(string)"/>
        /// </summary>
        /// <remarks>More details regarding string format at https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings </remarks>
        public string CursorLabelStringFormat { get; set; } = "F4";
        /// <summary>
        /// Display cursor value on chart
        /// </summary>
        public bool ShowCursorValue { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public ThemeBase Theme { get; set; } = null;
    }

}
