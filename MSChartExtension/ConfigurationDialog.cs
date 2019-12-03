using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cyotek.Windows.Forms;

namespace System.Windows.Forms.DataVisualization.Charting
{
    internal partial class ConfigurationDialog : Form
    {
        private Dictionary<string, ThemeBase> Themes;

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

            Themes = ThemeManager.GetThemes();
            cbTheme.Items.Clear();
            cbTheme.Items.AddRange(Themes.Keys.ToArray());

            ReadSettings();

            chkSeriesList.Items.Clear();
            chkSeriesList.Items.AddRange(chart.Series.Select(x => x.Name).ToArray());
            for (int x = 0; x < chkSeriesList.Items.Count; x++)
            {
                chkSeriesList.SetItemChecked(x, chart.Series[x].Enabled);
            }
        }

        private void ReadSettings()
        {
            btCursor1Color.BackColor = Option.Cursor1Color;
            btCursor2Color.BackColor = Option.Cursor2Color;
            cursor1LineWidth.Text = Option.Cursor1LineWidth.ToString();
            cursor2LineWidth.Text = Option.Cursor2LineWidth.ToString();
            chkAllowToHideSeries.Checked = Option.ContextMenuAllowToHideSeries;
            chkShowCursorValue.Checked = Option.ShowCursorValue;
            txtCursorLabelStringFormat.Text = Option.CursorLabelStringFormat;
            cbCursor1DashStyle.SelectedIndex = (int)Option.Cursor1DashStyle - 1;
            cbCursor2DashStyle.SelectedIndex = (int)Option.Cursor2DashStyle - 1;

            if (Option.Theme == null) cbTheme.SelectedIndex = 0;
            else { ThemeIndex = cbTheme.SelectedIndex = cbTheme.Items.IndexOf(Option.Theme.Name); }
        }

        private void WriteSettings()
        {
            Option.Cursor1Color = btCursor1Color.BackColor;
            Option.Cursor2Color = btCursor2Color.BackColor;
            Option.Cursor1LineWidth = Convert.ToInt32(cursor1LineWidth.Text);
            Option.Cursor2LineWidth = Convert.ToInt32(cursor2LineWidth.Text);
            Option.ContextMenuAllowToHideSeries = chkAllowToHideSeries.Checked;
            Option.ShowCursorValue = chkShowCursorValue.Checked;
            Option.CursorLabelStringFormat = txtCursorLabelStringFormat.Text;
            Option.Cursor1DashStyle = (ChartDashStyle)(cbCursor1DashStyle.SelectedIndex + 1);
            Option.Cursor2DashStyle = (ChartDashStyle)(cbCursor2DashStyle.SelectedIndex + 1);

            if (cbTheme.SelectedIndex != ThemeIndex) Option.Theme = Themes[cbTheme.Text];
        }

        private void BtCursor1Color_Click(object sender, EventArgs e)
        {
            using (ColorPickerDialog dialog = new ColorPickerDialog())
            {
                dialog.Color = Option.Cursor1Color;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Option.Cursor1Color = btCursor1Color.BackColor = dialog.Color;
                }
            }
        }
        private void BtCursor2Color_Click(object sender, EventArgs e)
        {
            using (ColorPickerDialog dialog = new ColorPickerDialog())
            {
                dialog.Color = Option.Cursor2Color;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Option.Cursor2Color = btCursor2Color.BackColor = dialog.Color;
                }
            }
        }

        private void UpdateSeriesVisibleState()
        {
            for (int x = 0; x < chkSeriesList.Items.Count; x++)
            {
                ChartHandler.Series[x].Enabled = chkSeriesList.GetItemChecked(x);
            }
        }

        private void BtOK_Click(object sender, EventArgs e)
        {
            WriteSettings();
            UpdateSeriesVisibleState();
        }

        private void ChkAllowToHideSeries_CheckedChanged(object sender, EventArgs e)
        {
            chkSeriesList.Enabled = chkAllowToHideSeries.Enabled;
        }

        private void BtCheckAll_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < chkSeriesList.Items.Count; x++) chkSeriesList.SetItemChecked(x, true);
        }

        private void BtCheckNone_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < chkSeriesList.Items.Count; x++) chkSeriesList.SetItemChecked(x, false);
        }
    }
}
