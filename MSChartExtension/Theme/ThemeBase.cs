using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Theme - Base Class
    /// </summary>
    public abstract class ThemeBase
    {
        /// <summary>
        /// Base Constructor, create new theme.
        /// </summary>
        /// <param name="name">Display name</param>
        public ThemeBase(string name) { Name = name; }
        /// <summary>
        /// User Friendly Name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Chart -> BackColor
        /// </summary>
        public Color BackColor { get; set; } = Color.Empty;
        /// <summary>
        /// ChartArea -> BackColor
        /// </summary>
        public Color ChartAreaBackColor { get; set; } = Color.Empty;
        /// <summary>
        /// All Axis: Line Color
        /// </summary>
        public Color AxisLineColor { get; set; } = Color.Empty;
        /// <summary>
        /// All Axis: Major Grid Color, Major Tick Mark
        /// </summary>
        public Color AxisMajorGridColor { get; set; } = Color.Empty;
        /// <summary>
        /// All Axis: LabelStyel -> ForeColor
        /// </summary>
        public Color AxisLabelColor { get; set; } = Color.Empty;
        /// <summary>
        /// Chart Title Color
        /// </summary>
        public Color TitleColor { get; set; } = Color.Empty;

    }
}
