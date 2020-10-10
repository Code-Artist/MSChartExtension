using System.Drawing;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Dark Theme
    /// </summary>
    public class FuturisticTheme : ThemeBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FuturisticTheme() : base("Futuristic")
        {
            BackColor = Color.FromArgb(12, 25, 48);
            ChartAreaBackColor = Color.FromArgb(14, 37, 61);
            AxisLineColor = AxisMajorGridColor = AxisMinorGridColor = Color.FromArgb(47, 97, 135);
            AxisLabelColor = TitleColor = Color.White;
        }
    }
}
