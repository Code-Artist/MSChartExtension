using System.Drawing;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Dark Theme
    /// </summary>
    public class DarkTheme : ThemeBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DarkTheme() : base("Dark")
        {
            BackColor = ChartAreaBackColor = Color.Black;
            AxisLineColor = AxisMajorGridColor = AxisMinorGridColor = Color.Gray;
            AxisLabelColor = TitleColor = Color.LightGray;
        }
    }
}
