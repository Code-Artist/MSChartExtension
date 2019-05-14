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
    public class DefaultTheme : ThemeBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultTheme(): base("(Default)")
        {
            BackColor = ChartAreaBackColor = Color.White;
            AxisLineColor = AxisMajorGridColor = AxisLabelColor = TitleColor = Color.Black;

        }
    }
}
