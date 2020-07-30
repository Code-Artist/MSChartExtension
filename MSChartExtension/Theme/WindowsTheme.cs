using System.Drawing;

namespace System.Windows.Forms.DataVisualization.Charting.Theme
{
    /// <summary>
    /// Original WinForm Theme
    /// </summary>
    public class WindowsTheme : ThemeBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WindowsTheme() : base("Windows")
        {
            Chart tChart = new Chart();
            BackColor = tChart.BackColor;
            TitleColor = Color.FromKnownColor(KnownColor.WindowText);

            ChartArea tChartArea = new ChartArea();
            AxisLabelColor = tChartArea.AxisX.TitleForeColor;
            AxisMajorGridColor = AxisMinorGridColor = tChartArea.AxisX.MajorGrid.LineColor;
            AxisLineColor = tChartArea.AxisX.LineColor;
            ChartAreaBackColor = tChartArea.BackColor;
        }
    }
}
