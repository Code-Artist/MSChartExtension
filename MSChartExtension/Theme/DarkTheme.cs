using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public DarkTheme(): base("Dark")
        {
            BackColor = ChartAreaBackColor = Color.Black;
            AxisLineColor = AxisMajorGridColor = AxisLabelColor = TitleColor = Color.Gray;

        }
    }
}
