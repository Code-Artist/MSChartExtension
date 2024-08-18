#define BUFFER
//#define Dynamic

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MSChartExtensionDemo
{
    public partial class Form1 : Form
    {
        const int DataSizeBase = 200; //Increase this number to plot more points
        const int SamplingSize = 30;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public Form1()
        {
            InitializeComponent();
            new ChartForm().Show();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            ChartOption option = new ChartOption()
            {
                ContextMenuAllowToHideSeries = true,
                //XAxisPrecision = 4,
                //YAxisPrecision = 4
                Theme = new DarkTheme(),
#if BUFFER
                BufferedMode = true,
                DisplayDataSize = SamplingSize
#endif
            };
            option.CursorLabelFormatX1.StringFormat = "F0";
            option.CursorLabelFormatX1.Prefix = "X=";
            option.CursorLabelFormatY1.Prefix = "Y ";
            option.CursorLabelFormatY1.StringFormat = "F4";
            option.CursorLabelFormatY1.Postfix = "V";


            chart1.EnableZoomAndPanControls(ChartCursorSelected, ChartCursorMoved, zoomChanged, option);

            chart2.EnableZoomAndPanControls();

            chart3.EnableZoomAndPanControls(ChartCursorSelected, ChartCursorMoved);
            PlotData();

            PlotChartDate();
        }

        private void PlotChartDate()
        {
            ChartDate.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
            Series ptrSeries = ChartDate.Series[0];
            DateTime StartDate = new DateTime(2020, 1, 1);
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int x = 0; x < 20; x++)
            {
                int id = ptrSeries.Points.AddXY(StartDate.AddDays(x), r.Next(0, 100));
                ptrSeries.Points[id].Label = StartDate.AddDays(x).ToShortDateString();
            }
            ChartOption chartOption = new ChartOption();
            chartOption.CursorLabelFormatX1.StringFormat = "MMM-dd";
            ChartDate.EnableZoomAndPanControls(null, null, option: chartOption);
        }


        private void PlotData(bool reverse = false)
        {

            //Series 1 used primary YAxis
            Series Ser1 = chart1.Series[0];

            //Series 2 used secondary YAxis 
            Series Ser2 = chart1.Series[1];
            Series Ser3 = chart1.Series[2];
            double angFreq = 0.20;

            int HalfSize = DataSizeBase / 10 * 9;
            for (int x = 0; x < HalfSize; x++)
            {
                Ser1.Points.AddXY(x, 10 * Math.Cos(Math.PI * angFreq * x));
#if BUFFER
                Ser2.AddXYBuffered(x, 10 * Math.Cos(Math.PI * angFreq * x));
                //Ser3.AddXYBuffered(x, 10 * Math.Cos(Math.PI * angFreq * x));
#endif
            }
            for (int x = HalfSize; x < DataSizeBase; x++)
            {
                Ser1.Points.AddXY(HalfSize + 10 * (x - HalfSize), 10 * Math.Cos(Math.PI * angFreq * x));
#if BUFFER
                Ser2.AddXYBuffered(HalfSize + 10 * (x - HalfSize), 10 * Math.Cos(Math.PI * angFreq * x));
                //Ser3.AddXYBuffered(HalfSize + 10 * (x - HalfSize), 10 * Math.Cos(Math.PI * angFreq * x));
#endif
            }


#if BUFFER
            Ser2.PlotBufferedData();
            //Ser3.PlotBufferedData(false);
#endif
            int i = 0;
            List<string> S1 = new List<string>();
            foreach (DataPoint p in Ser1.Points) 
            {
                while (p.XValue > i++) S1.Add(" ");
                S1.Add(p.XValue + "," + p.YValues[0]);
            }
            File.WriteAllLines("S1.txt", S1.ToArray());

            List<string> S2 = new List<string>();
            i = 0;
            foreach (DataPoint p in Ser2.Points)
            {
                while (p.XValue > i++) S2.Add(" ");
                S2.Add(p.XValue + "," + p.YValues[0]);
            }
            File.WriteAllLines("S2.txt", S2.ToArray());

            List<string> S3 = new List<string>();
            i = 0;
            foreach (DataPoint p in Ser3.Points)
            {
                while (p.XValue > i++) S3.Add(" ");
                S3.Add(p.XValue + "," + p.YValues[0]);
            }
            File.WriteAllLines("S3.txt", S3.ToArray());



            var chartArea = chart1.ChartAreas.First();
            chartArea.AxisX.IsReversed = reverse;
            chartArea.AxisY.IsReversed = reverse;

            Series ptrSeries = chart2.Series[0];
            ptrSeries.Points.AddXY(1, 1);
            ptrSeries.Points.AddXY(2, 2);
            ptrSeries.Points.AddXY(3, 3);

            //Date Time Series
            Series dateSeries = chart3.Series[0];
            DateTime today = DateTime.Today;
            for (int x = 0; x < 10; x++)
            {
                dateSeries.Points.AddXY(today.AddDays(x), x);
            }
        }
        private void randomDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ClearData();
            //Remove series instead of clear.
            chart1.Series.Clear();
            //Recreate series
            chart1.Series.Add(new Series("radn1"));
            chart1.Series.Add(new Series("rand2"));

            StartStopWatch();
            //Series 1 used primary YAxis
            Series Ser1 = chart1.Series[0];
            Ser1.ChartType = SeriesChartType.FastLine;

            //Series 2 used secondary YAxis 
            Series Ser2 = chart1.Series[1];
            Ser2.ChartType = SeriesChartType.FastLine;

            Random rand = new Random();
            double data = 0;
            for (int x = 0; x < DataSizeBase; x++)
            {
                data += rand.NextDouble() - 0.5;
#if BUFFER
                Ser2.AddXYBuffered(x, data);
#endif
                Ser1.Points.AddXY(x, data);
            }
#if BUFFER

            Ser2.PlotBufferedData();
#endif
            Application.DoEvents();
            CheckStopWatch("Plot random datas");

        }

        private void ClearData()
        {
            foreach (Series ptrSeries in chart1.Series)
                ptrSeries.ClearPoints();
        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            OnPlotClicked(false);
        }

        private void btnPlotInDescendingOrder_Click(object sender, EventArgs e)
        {
            OnPlotClicked(true);
        }

        private void OnPlotClicked(bool dataAsDescending)
        {
            ClearData();
            StartStopWatch();
            PlotData(dataAsDescending);
            Application.DoEvents();
            CheckStopWatch("Plot datas");
        }

        private void btnClearDataFast_Click(object sender, EventArgs e)
        {
            StartStopWatch();
            ClearData();
            Application.DoEvents();
            CheckStopWatch("Clear datas");
        }

        private void btnClearDataSlow_Click(object sender, EventArgs e)
        {
            StartStopWatch();
            foreach (Series ptrSeries in chart1.Series)
                ptrSeries.Points.Clear();
            Application.DoEvents();
            CheckStopWatch("Clear datas");
        }

        readonly System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        private void StartStopWatch() { watch.Restart(); }

        private void CheckStopWatch(string message)
        {
            watch.Stop();
            MessageBox.Show(message + " took " + watch.ElapsedMilliseconds.ToString() + "ms");
        }

        private void ChartCursorSelected(Chart sender, ChartCursor e)
        {
            txtChartSelect.Text = e.XFormattedString + ", " + e.YFormattedString; //e.X.ToString("F4") + ", " + e.Y.ToString("F4");
        }

        private void ChartCursorMoved(Chart sender, ChartCursor e)
        {
            txtChartValue.Text = e.XFormattedString + ", " + e.YFormattedString;
            PointF diff = sender.CursorsDiff();
            txtCursorDelta.Text = diff.X.ToString("F4") + ", " + diff.Y.ToString("F4");
        }

        private void zoomChanged(Chart sender)
        {
            //Trace.WriteLine("Zoom changed");
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text.StartsWith("Item"))
            {
                ToolStripMenuItem ptrMenu = (ToolStripMenuItem)e.ClickedItem;
                if (ptrMenu.HasDropDownItems) return;
                MessageBox.Show(ptrMenu.Text);
            }
        }

        private void item11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test");
        }

        private void item12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test2");
        }

        private void item13ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test3");
        }

        private void item14ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test4");
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            chart1.DrawHorizontalLine(0.5, Color.Green, lineWidth: 3, lineStyle: ChartDashStyle.DashDot);
            chart1.DrawVerticalLine(750, Color.Orange, lineWidth: 3, lineStyle: ChartDashStyle.Dot);
            chart1.DrawRectangle(1000, -0.3, 500, 0.6, Color.Lime, lineWidth: 3);
            chart1.DrawLine(1500, 2000, -1, 1, Color.Pink, lineWidth: 2);
            //chart1.DrawLine(1500, 2000, -1, 1, Color.Red, lineWidth: 2, chartArea: chart1.ChartAreas[1]);
            chart1.AddText("Test chart message", 100, 14, Color.Black, textStyle: TextStyle.Shadow, xAxisType: AxisType.Primary, yAxisType: AxisType.Primary);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            chart1.Annotations.Clear();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.IsLogarithmic = !chart1.ChartAreas[0].AxisX.IsLogarithmic;
            if (chart1.ChartAreas[0].AxisX.IsLogarithmic)
            {
                chart1.ChartAreas[0].AxisX.Maximum = 1000;
                chart1.ChartAreas[0].AxisX.Minimum = 1;
            }
            else
            {
                chart1.ChartAreas[0].AxisX.Maximum = double.NaN;
                chart1.ChartAreas[0].AxisX.Minimum = double.NaN;
            }
        }

        private void btnUpdateVisibleExtents_Click(object sender, EventArgs e)
        {
        }

        private void btnViewChartExtents_ButtonClick(object sender, EventArgs e)
        {
            RectangleF all = chart1.ChartAreas[0].GetChartAreaBoundary();
            RectangleF visible = chart1.ChartAreas[0].GetChartVisibleAreaBoundary();

            RectangleF all2 = chart1.ChartAreas[0].GetChartAreaBoundary(chart1.ChartAreas[0].AxisX2, chart1.ChartAreas[0].AxisY2);
            RectangleF visible2 = chart1.ChartAreas[0].GetChartVisibleAreaBoundary(chart1.ChartAreas[0].AxisX2, chart1.ChartAreas[0].AxisY2);
            const string fmt =
@"PRIMARY AXIS
All data: {0}
Visible data: {1}

SECONDARY AXIS
All data: {2}
Visible data: {3}
";
            MessageBox.Show(string.Format(fmt, all.ToStringWithBoundaries(), visible.ToStringWithBoundaries(),
                all2.ToStringWithBoundaries(), visible2.ToStringWithBoundaries()), "Extents/boundaries of the data");


        }

        private void Chart1_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("Chart Key Down");
        }

        private void Chart1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Debug.WriteLine("Chart Preview Key Down");
        }

        private void Chart1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            AdjustAxisIntervalOffset(ref chart1, 0, 0);
        }

        private void AdjustAxisIntervalOffset(ref Chart ChartObj, int ChartAreaIndex, int AxisIndex)
        {
            //Created by Shin-Hua Tseng <shtsenga@gmail.com>

            double[] unit_base = new double[] { 1.0, 2.0, 2.5, 5.0 };
            double unit = 1.0;
            double value = 0;
            double vmin, vmax;
            int max_count, scale;
            Axis axis = ChartObj.ChartAreas[ChartAreaIndex].Axes[AxisIndex];
            vmin = axis.ScaleView.ViewMinimum;      //min. value of current view area
            vmax = axis.ScaleView.ViewMaximum;      //max. value of current view area

            //Label Rectangle Estimation
            //select max. label count for X-Axis or Y-Axis, this value can be estimated by
            // X-Axis = axis-width/(1.25*MaxLabelWidth)
            // Y-Axis = axis-height/(2*MaxLabelHeight)
            // when max. characters of all label can be obtained.

            //max_count is used to restrict max. label count of this axis in the current view area. 
            // I just select 10 for X - axis, 8 - 20 for Yaxis to skip label rectangle estimation.
            // If you know how to get rectangle for all labels. You can choose a larger label count,
            // then check if some labels will be overlapped. When label overlap occurred, reduce
            // label count, and recheck it again until no label overlap occurs.

            max_count = (AxisIndex % 2) == 0 ? 10 : 10;

            value = (vmax - vmin) / (double)max_count;
            //find best expression label format, we restrict all label unit
            // be one of unit_base[] value  * 10^n n is integer
            scale = (int)Math.Log10(value);
            value = value / Math.Pow(10.0, scale);
            if (value < 0.5)
            {
                scale -= 1;
                value *= 10.0;
            }
            else if (value > 5.0)
            {
                scale += 1;
                value *= 0.1;
            }
            for (int i = 0; i < unit_base.Length; ++i)
            {
                if (unit_base[i] >= value)
                {
                    unit = unit_base[i] * Math.Pow(10.0, scale);
                    break;
                }
            }
            //change axis interval and interval offset
            double offset = unit * (double)(int)(vmin / unit);
            double minor_offset = 0;
            if (offset > vmin)
                offset -= unit;
            offset = offset - vmin;
            axis.Interval = unit;
            axis.IntervalOffset = offset;
            minor_offset = offset - axis.MinorTickMark.Interval * (double)(int)(offset / axis.MinorTickMark.Interval);
            axis.MajorTickMark.IntervalOffset = offset;
            axis.MinorTickMark.IntervalOffset = minor_offset;
        }

    }

    public static class RectangleExtensions
    {
        /// <summary>Returns a string showing left, right, top, and bottom.</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToStringWithBoundaries(this RectangleF value)
        {
            const string fmt = "{{Left={0},Right={1},Top={2},Bottom={3}}}";
            return string.Format(fmt, value.Left, value.Right, value.Top, value.Bottom);
        }
    }

}
