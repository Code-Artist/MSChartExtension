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
        /// All Axis: Minor Gird Color, Minor Tick Mark
        /// </summary>
        public Color AxisMinorGridColor { get; set; } = Color.Empty;
        /// <summary>
        /// All Axis: LabelStyel -> ForeColor
        /// </summary>
        public Color AxisLabelColor { get; set; } = Color.Empty;
        /// <summary>
        /// Chart Title Color
        /// </summary>
        public Color TitleColor { get; set; } = Color.Empty;
        /// <summary>
        /// Border Skin Style
        /// </summary>
        public BorderSkinStyle BorderSkin { get; set; } = BorderSkinStyle.None;

        /// <summary>
        /// Assign style to chart
        /// </summary>
        /// <param name="sender"></param>
        public virtual void AssignTheme(Chart sender)
        {
            sender.BackColor = BackColor;
            foreach (Title t in sender.Titles)
            {
                t.ForeColor = TitleColor;
            }
            foreach (ChartArea a in sender.ChartAreas)
            {
                a.BackColor = ChartAreaBackColor;
                foreach (Axis x in a.Axes)
                {
                    x.LineColor = AxisLineColor;
                    x.MajorGrid.LineColor = x.MajorTickMark.LineColor = AxisMajorGridColor;
                    x.MinorGrid.LineColor = x.MinorTickMark.LineColor = AxisMinorGridColor;
                    x.LabelStyle.ForeColor = AxisLabelColor;
                    x.TitleForeColor = AxisLabelColor;
                }
            }
            foreach (Legend l in sender.Legends)
            {
                l.BackColor = BackColor;
                l.ForeColor = AxisLabelColor;
            }
            sender.BorderSkin.SkinStyle = BorderSkin;
        }

    }
}
