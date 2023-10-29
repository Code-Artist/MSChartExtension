using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Forms.DataVisualization.Charting
{
    internal partial class ConfigurationDialog : Form
    {
        private readonly Dictionary<string, ThemeBase> Themes;

        public ChartOption Option { get; private set; }

        private Chart ChartHandler { get; set; }

        private int ThemeIndex;

        public ConfigurationDialog(Chart chart, ChartOption chartOption)
        {
            InitializeComponent();

            ChartHandler = chart;
            Option = chartOption;
            //Create List of Dash Style, skip NOTSET option.
            cbCursor1DashStyle.Items.AddRange(Enum.GetNames(typeof(ChartDashStyle)).Skip(1).ToArray());
            cbCursor2DashStyle.Items.AddRange(Enum.GetNames(typeof(ChartDashStyle)).Skip(1).ToArray());

            Text = chart.Name + " Settings";
            if (chart.Titles.Count > 0)
            {
                string chartTitle = chart.Titles.Where(n => !string.IsNullOrEmpty(n.Text)).Select(d => d.Text).FirstOrDefault();
                if (!string.IsNullOrEmpty(chartTitle)) Text = chartTitle + " Settings";
            }

            Themes = ThemeManager.GetThemes();
            cbTheme.Items.Clear();
            cbTheme.Items.AddRange(Themes.Keys.ToArray());

            ReadSettings();

            SeriesGrid.Rows.Clear();
            foreach (Series series in chart.Series)
            {
                int rowID = SeriesGrid.Rows.Add(new object[] { series.Enabled, series.Name, "", ">>" });
                SeriesGrid.Rows[rowID].Cells[colSeriesColor.Index].Style.BackColor = series.Color;
                SeriesGrid.Rows[rowID].Tag = series;
            }
        }

        private void ReadSettings()
        {
            btCursor1TextColor.BackColor = Option.Cursor1TextColor;
            btCursor2TextColor.BackColor = Option.Cursor2TextColor;
            btCursor1Color.BackColor = Option.Cursor1Color;
            btCursor2Color.BackColor = Option.Cursor2Color;
            cursor1LineWidth.Text = Option.Cursor1LineWidth.ToString();
            cursor2LineWidth.Text = Option.Cursor2LineWidth.ToString();
            chkAllowToHideSeries.Checked = Option.ContextMenuAllowToHideSeries;
            chkShowCursorValue.Checked = Option.ShowCursorValue;
            cbCursor1DashStyle.SelectedIndex = (int)Option.Cursor1DashStyle - 1;
            cbCursor2DashStyle.SelectedIndex = (int)Option.Cursor2DashStyle - 1;

            LabelFormatX1.SetContent(Option.CursorLabelFormatX1);
            LabelFormatX2.SetContent(Option.CursorLabelFormatX2);
            LabelFormatY1.SetContent(Option.CursorLabelFormatY1);
            LabelFormatY2.SetContent(Option.CursorLabelFormatY2);


            if (Option.Theme == null) cbTheme.SelectedIndex = ThemeIndex = -1;
            else { ThemeIndex = cbTheme.SelectedIndex = cbTheme.Items.IndexOf(Option.Theme.Name); }
        }

        private void WriteSettings()
        {
            Option.Cursor1TextColor = btCursor1TextColor.BackColor;
            Option.Cursor2TextColor = btCursor2TextColor.BackColor;
            Option.Cursor1Color = btCursor1Color.BackColor;
            Option.Cursor2Color = btCursor2Color.BackColor;
            Option.Cursor1LineWidth = Convert.ToInt32(cursor1LineWidth.Text);
            Option.Cursor2LineWidth = Convert.ToInt32(cursor2LineWidth.Text);
            Option.ContextMenuAllowToHideSeries = chkAllowToHideSeries.Checked;
            Option.ShowCursorValue = chkShowCursorValue.Checked;
            Option.Cursor1DashStyle = (ChartDashStyle)(cbCursor1DashStyle.SelectedIndex + 1);
            Option.Cursor2DashStyle = (ChartDashStyle)(cbCursor2DashStyle.SelectedIndex + 1);

            Option.CursorLabelFormatX1 = LabelFormatX1.GetLabelContent();
            Option.CursorLabelFormatX2 = LabelFormatX2.GetLabelContent();
            Option.CursorLabelFormatY1 = LabelFormatY1.GetLabelContent();
            Option.CursorLabelFormatY2 = LabelFormatY2.GetLabelContent();

            if (cbTheme.SelectedIndex != ThemeIndex) Option.Theme = Themes[cbTheme.Text];
        }

        private Color PickColor(Color color)
        {
            using (ColorDialog dialog = new ColorDialog())
            {
                dialog.Color = color;
                if (dialog.ShowDialog() == DialogResult.OK) return dialog.Color;
                return color;
            }
        }


        private void BtCursor1Color_Click(object sender, EventArgs e)
        {
            btCursor1Color.BackColor = PickColor(btCursor1Color.BackColor);
        }
        private void btCursor1TextColor_Click(object sender, EventArgs e)
        {
            btCursor1TextColor.BackColor = PickColor(btCursor1TextColor.BackColor);
        }


        private void BtCursor2Color_Click(object sender, EventArgs e)
        {
            btCursor2Color.BackColor = PickColor(btCursor2Color.BackColor);
        }

        private void btCursor2TextColor_Click(object sender, EventArgs e)
        {
            btCursor2TextColor.BackColor = PickColor(btCursor2TextColor.BackColor);
        }

        private void UpdateSeriesSettings()
        {
            foreach (DataGridViewRow r in SeriesGrid.Rows)
            {
                Series ptrSeries = r.Tag as Series;
                ptrSeries.Enabled = Convert.ToBoolean(r.Cells[colSeriesEnable.Index].Value);
                ptrSeries.MarkerBorderColor = ptrSeries.MarkerColor = ptrSeries.Color = r.Cells[colSeriesColor.Index].Style.BackColor;
            }
        }

        private void BtOK_Click(object sender, EventArgs e)
        {
            WriteSettings();
            UpdateSeriesSettings();
        }

        private void ChkAllowToHideSeries_CheckedChanged(object sender, EventArgs e)
        {
            SeriesGrid.Enabled = chkAllowToHideSeries.Enabled;
        }

        private void BtCheckAll_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < SeriesGrid.Rows.Count; x++) (SeriesGrid.Rows[x].Cells[colSeriesEnable.Index] as DataGridViewCheckBoxCell).Value = true;
        }

        private void BtCheckNone_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < SeriesGrid.Rows.Count; x++) (SeriesGrid.Rows[x].Cells[colSeriesEnable.Index] as DataGridViewCheckBoxCell).Value = false;
        }

        private void SeriesGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == colSelectColor.Index)
            {
                DataGridViewCell ptrCell = SeriesGrid.Rows[e.RowIndex].Cells[e.ColumnIndex - 1];
                ptrCell.Style.BackColor = PickColor(ptrCell.Style.BackColor);
            }
        }

        private void SeriesGrid_SelectionChanged(object sender, EventArgs e)
        {
            SeriesGrid.ClearSelection();
        }

    }
}
