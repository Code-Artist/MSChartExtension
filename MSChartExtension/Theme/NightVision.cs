using System.Drawing;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Night Vision Theme
    /// </summary>
    public class NightVisionTheme : ThemeBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NightVisionTheme() : base("Night Vision")
        {
            BackColor = ChartAreaBackColor = Color.FromArgb(0, 28, 0);
            AxisLineColor = AxisMajorGridColor = AxisMinorGridColor = Color.FromArgb(4, 120, 11);
            AxisLabelColor = TitleColor = Color.FromArgb(22, 198, 12);
        }
    }
}
