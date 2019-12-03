using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MSChartExtensionDemo
{
    public partial class ChartForm : Form
    {
        public ChartForm()
        {
            InitializeComponent();
        }

        private void selectionChanged(Chart sender, ChartCursor cursor)
        {
        }

        private void cursorMoved(Chart sender, ChartCursor cursor)
        {
        }

        private void ChartForm_Shown(object sender, EventArgs e)
        {
            chart1.EnableZoomAndPanControls(selectionChanged, cursorMoved);
            Series ptrSeries = chart1.Series[0];
            ptrSeries.ClearPoints();
            for (int x = 0; x < 10; x++) ptrSeries.Points.AddXY(10, x);

        }
    }
}
