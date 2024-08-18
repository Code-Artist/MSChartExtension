using System;
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
            LbSelectedPoint.Text = cursor.XFormattedString + ", " + cursor.YFormattedString;
        }

        private void cursorMoved(Chart sender, ChartCursor cursor)
        {
            LbCursorValue.Text = cursor.XFormattedString + ", " + cursor.YFormattedString;
        }

        private void ChartForm_Shown(object sender, EventArgs e)
        {
            ChartOption chartOption = new ChartOption();
            chartOption.CursorLabelFormatX1.StringFormat = "F0";
            chartOption.CursorLabelFormatY1.StringFormat = "t";

            chart1.EnableZoomAndPanControls(selectionChanged, cursorMoved, option: chartOption);
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "MMM-dd, HH:mm";
            ResourceSeries ptrSeries = chart1.ChartAreas[0].SetupResourceAllocationChart();
            Random r = new Random((int)DateTime.Now.Ticks);
            char serName = 'A';
            DateTime tRef = DateTime.Now;
            for (int x = 0; x < 25; x++) //X Entry
            {
                ResourceRow ptrRow = ptrSeries.Row(serName.ToString());
                serName++;

                int totalSeconds = 0;

                while (totalSeconds < 600)
                {
                    int range = r.Next(50);
                    if (totalSeconds + range >= 600) break;
                    ptrRow.AddTimeBlocks(new TimeBlock(tRef.AddSeconds(totalSeconds), range));
                    totalSeconds += range;
                    totalSeconds += r.Next(20);
                }
            }
        }
    }
}
