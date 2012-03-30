using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms.DataVisualization.Charting
{
    public static class MSChartExtension
    {
        /// <summary>
        /// Speed up MSChart data points clear operations.
        /// </summary>
        /// <param name="sender"></param>
        public static void ClearPoints(this Series sender)
        {
            sender.Points.SuspendUpdates();
            while (sender.Points.Count > 0)
                sender.Points.RemoveAt(sender.Points.Count - 1);
            sender.Points.ResumeUpdates();
        }
        /// <summary>
        /// Enable Zoom and Pan Controls.
        /// </summary>
        /// <param name="sender"></param>
        public static void EnableZoomAndPanControls(this Chart sender)
        {
            if (ChartContextMenuStrip == null) CreateChartContextMenu();
            if (!ChartTool.ContainsKey(sender))
            {
                ChartTool[sender] = new ChartData(sender);
                ChartTool[sender].Backup();

                //Populate Context menu
                Chart ptrChart = sender;
                ptrChart.ContextMenuStrip = ChartContextMenuStrip;
                ptrChart.MouseDown += ChartControl_MouseDown;
                ptrChart.MouseMove += ChartControl_MouseMove;
                ptrChart.MouseUp += ChartControl_MouseUp;

                ptrChart.ChartAreas[0].CursorX.AutoScroll = false;
                ptrChart.ChartAreas[0].CursorX.Interval = 1e-06;
                ptrChart.ChartAreas[0].CursorY.AutoScroll = false;
                ptrChart.ChartAreas[0].CursorY.Interval = 1e-06;

                SetChartControlState(sender, ChartToolState.Select);
            }
        }
        /// <summary>
        /// Disable Zoom and Pan Controls
        /// </summary>
        /// <param name="sender"></param>
        public static void DisableZoomAndPanControls(this Chart sender)
        {
            Chart ptrChart = sender;
            ptrChart.ContextMenuStrip = null;
            if (ChartTool.ContainsKey(ptrChart))
            {
                ptrChart.MouseDown -= ChartControl_MouseDown;
                ptrChart.MouseMove -= ChartControl_MouseMove;
                ptrChart.MouseUp -= ChartControl_MouseUp;

                ChartTool[ptrChart].Restore();
                ChartTool.Remove(ptrChart);
            }
        }

        #region [ Chart Context Menu ]
        private static ContextMenuStrip ChartContextMenuStrip;
        private static ToolStripMenuItem ChartToolSelect;
        private static ToolStripMenuItem ChartToolZoom;
        private static ToolStripMenuItem ChartToolPan;
        private static ToolStripMenuItem ChartToolZoomOut;
        private static ToolStripSeparator ChartToolZoomOutSeparator;
        private static void CreateChartContextMenu()
        {
            ChartContextMenuStrip = new ContextMenuStrip();
            ChartToolZoomOut = new ToolStripMenuItem("Zoom Out");
            ChartToolZoomOutSeparator = new ToolStripSeparator();
            ChartToolSelect = new ToolStripMenuItem("Select");
            ChartToolZoom = new ToolStripMenuItem("Zoom");
            ChartToolPan = new ToolStripMenuItem("Pan");

            ChartContextMenuStrip.Items.Add(ChartToolZoomOut);
            ChartContextMenuStrip.Items.Add(ChartToolZoomOutSeparator);
            ChartContextMenuStrip.Items.Add(ChartToolSelect);
            ChartContextMenuStrip.Items.Add(ChartToolZoom);
            ChartContextMenuStrip.Items.Add(ChartToolPan);
            ChartContextMenuStrip.Items.Add(new ToolStripSeparator());

            ChartContextMenuStrip.Opening += ChartContext_Opening;
            ChartContextMenuStrip.ItemClicked += ChartContext_ItemClicked;
        }

        private static void ChartContext_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menuStrip = (ContextMenuStrip)sender;
            Chart senderChart = (Chart)menuStrip.SourceControl;

            //Check Zoomed state
            if (senderChart.ChartAreas[0].AxisX.ScaleView.IsZoomed ||
                senderChart.ChartAreas[0].AxisY.ScaleView.IsZoomed ||
                senderChart.ChartAreas[0].AxisY2.ScaleView.IsZoomed)
            {
                ChartToolZoomOut.Visible = true;
                ChartToolZoomOutSeparator.Visible = true;
            }
            else
            {
                ChartToolZoomOut.Visible = false;
                ChartToolZoomOutSeparator.Visible = false;
            }

            //Get Chart Control State
            if (!ChartTool.ContainsKey(senderChart))
            {
                //Initialize Chart Tool
                SetChartControlState(senderChart, ChartToolState.Select);
            }

            //Update menu based on current state.
            ChartToolSelect.Checked = false;
            ChartToolZoom.Checked = false;
            ChartToolPan.Checked = false;
            switch (ChartTool[senderChart].ToolState)
            {
                case ChartToolState.Select:
                    ChartToolSelect.Checked = true;
                    break;
                case ChartToolState.Zoom:
                    ChartToolZoom.Checked = true;
                    break;
                case ChartToolState.Pan:
                    ChartToolPan.Checked = true;
                    break;
            }

            //Update series
            for (int x = 0; x < menuStrip.Items.Count; x++)
            {
                if (menuStrip.Items[x].Tag != null)
                {
                    if (menuStrip.Items[x].Tag.ToString() == "Series")
                    {
                        menuStrip.Items.RemoveAt(x);
                        x--;
                    }
                }
            }

            SeriesCollection chartSeries = ((Chart)menuStrip.SourceControl).Series;
            foreach (Series ptrSeries in chartSeries)
            {
                ToolStripItem ptrItem = ChartContextMenuStrip.Items.Add(ptrSeries.Name);
                ToolStripMenuItem ptrMenuItem = (ToolStripMenuItem)ptrItem;
                ptrMenuItem.Checked = ptrSeries.Enabled;
                ptrItem.Tag = "Series";
            }
        }
        private static void ChartContext_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip ptrMenuStrip = (ContextMenuStrip)sender;
            if (e.ClickedItem == ChartToolSelect)
                SetChartControlState((Chart)ChartContextMenuStrip.SourceControl, ChartToolState.Select);
            else if (e.ClickedItem == ChartToolZoom)
                SetChartControlState((Chart)ChartContextMenuStrip.SourceControl, ChartToolState.Zoom);
            else if (e.ClickedItem == ChartToolPan)
                SetChartControlState((Chart)ChartContextMenuStrip.SourceControl, ChartToolState.Pan);
            else if (e.ClickedItem == ChartToolZoomOut)
            {
                Chart ptrChart = (Chart)ptrMenuStrip.SourceControl;
                ptrChart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                ptrChart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                ptrChart.ChartAreas[0].AxisY2.ScaleView.ZoomReset();
            }

            if (e.ClickedItem.Tag == null) return;
            if (e.ClickedItem.Tag.ToString() != "Series") return;

            //Series enable / disable changed.
            SeriesCollection chartSeries = ((Chart)ptrMenuStrip.SourceControl).Series;
            chartSeries[e.ClickedItem.Text].Enabled = !((ToolStripMenuItem)e.ClickedItem).Checked;
        }
        #endregion

        #region [ Chart Control State + Events ]
        private class ChartData
        {
            private Chart Source;
            public ChartData(Chart chartSource) { Source = chartSource; }

            public ChartToolState ToolState { get; set; }

            public void Backup()
            {
                ContextMenuStrip = Source.ContextMenuStrip;
                ChartArea ptrChartArea = Source.ChartAreas[0];
                CursorXUserEnabled = ptrChartArea.CursorX.IsUserEnabled;
                CursorYUserEnabled = ptrChartArea.CursorY.IsUserEnabled;
                Cursor = Source.Cursor;
                CursorXInterval = ptrChartArea.CursorX.Interval;
                CursorYInterval = ptrChartArea.CursorY.Interval;
                CursorXAutoScroll = ptrChartArea.CursorX.AutoScroll;
                CursorYAutoScroll = ptrChartArea.CursorY.AutoScroll;
            }
            public void Restore()
            {
                Source.ContextMenuStrip = ContextMenuStrip;
                ChartArea ptrChartArea = Source.ChartAreas[0];
                ptrChartArea.CursorX.IsUserEnabled = CursorXUserEnabled;
                ptrChartArea.CursorY.IsUserEnabled = CursorYUserEnabled;
                Source.Cursor = Cursor;
                ptrChartArea.CursorX.Interval = CursorXInterval;
                ptrChartArea.CursorY.Interval = CursorYInterval;
                ptrChartArea.CursorX.AutoScroll = CursorXAutoScroll;
                ptrChartArea.CursorY.AutoScroll = CursorYAutoScroll;
            }

            #region [ Backup Data ]
            public ContextMenuStrip ContextMenuStrip { get; set; }
            private bool CursorXUserEnabled;
            private bool CursorYUserEnabled;
            private System.Windows.Forms.Cursor Cursor;
            private double CursorXInterval, CursorYInterval;
            private bool CursorXAutoScroll, CursorYAutoScroll;
            #endregion
        }
        private enum ChartToolState
        {
            Unknown,
            Select,
            Zoom,
            Pan
        }
        private static Dictionary<Chart, ChartData> ChartTool = new Dictionary<Chart, ChartData>();
        private static void SetChartControlState(Chart sender, ChartToolState state)
        {
            ChartTool[(Chart)sender].ToolState = state;
            switch (state)
            {
                case ChartToolState.Select:
                    sender.Cursor = Cursors.Cross;
                    sender.ChartAreas[0].CursorX.IsUserEnabled = true;
                    sender.ChartAreas[0].CursorY.IsUserEnabled = true;
                    break;
                case ChartToolState.Zoom:
                    sender.Cursor = Cursors.Cross;
                    sender.ChartAreas[0].CursorX.IsUserEnabled = false;
                    sender.ChartAreas[0].CursorY.IsUserEnabled = false;
                    break;
                case ChartToolState.Pan:
                    sender.Cursor = Cursors.Hand;
                    sender.ChartAreas[0].CursorX.IsUserEnabled = false;
                    sender.ChartAreas[0].CursorY.IsUserEnabled = false;
                    break;
            }
        }
        #endregion

        #region [ Chart - Mouse Events ]
        private static bool MouseDowned;
        private static void ChartControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            Chart ptrChart = (Chart)sender;
            ChartArea ptrChartArea = ptrChart.ChartAreas[0];

            MouseDowned = true;

            ptrChartArea.CursorX.SelectionStart = ptrChartArea.AxisX.PixelPositionToValue(e.Location.X);
            ptrChartArea.CursorY.SelectionStart = ptrChartArea.AxisY.PixelPositionToValue(e.Location.Y);
            ptrChartArea.CursorX.SelectionEnd = ptrChartArea.CursorX.SelectionStart;
            ptrChartArea.CursorY.SelectionEnd = ptrChartArea.CursorY.SelectionStart;

            //ToDo: Update coordinate
        }
        private static void ChartControl_MouseMove(object sender, MouseEventArgs e)
        {
            Chart ptrChart = (Chart)sender;
            double selX, selY;
            selX = selY = 0;
            try
            {
                selX = ptrChart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X);
                selY = ptrChart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y);
                //ToDo: Update coordinate
            }
            catch (Exception) { /*ToDo: Set coordinate to 0,0 */ return; } //Handle exception when scrolled out of range.

            switch (ChartTool[ptrChart].ToolState)
            {
                case ChartToolState.Zoom:
                    #region [ Zoom Control ]
                    if (MouseDowned)
                    {
                        ptrChart.ChartAreas[0].CursorX.SelectionEnd = selX;
                        ptrChart.ChartAreas[0].CursorY.SelectionEnd = selY;
                    }
                    #endregion
                    break;

                case ChartToolState.Pan:
                    #region [ Pan Control ]
                    if (MouseDowned)
                    {
                        //Pan Move - Valid only if view is zoomed
                        if (ptrChart.ChartAreas[0].AxisX.ScaleView.IsZoomed ||
                            ptrChart.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                        {
                            double dx = -selX + ptrChart.ChartAreas[0].CursorX.SelectionStart;
                            double dy = -selY + ptrChart.ChartAreas[0].CursorY.SelectionStart;

                            double newX = ptrChart.ChartAreas[0].AxisX.ScaleView.Position + dx;
                            double newY = ptrChart.ChartAreas[0].AxisY.ScaleView.Position + dy;
                            double newY2 = ptrChart.ChartAreas[0].AxisY2.ScaleView.Position + dy;

                            ptrChart.ChartAreas[0].AxisX.ScaleView.Scroll(newX);
                            ptrChart.ChartAreas[0].AxisY.ScaleView.Scroll(newY);
                            ptrChart.ChartAreas[0].AxisY2.ScaleView.Scroll(newY2);
                        }
                    }
                    #endregion
                    break;
            }
        }
        private static void ChartControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            MouseDowned = false;

            Chart ptrChart = (Chart)sender;
            ChartArea ptrChartArea = ptrChart.ChartAreas[0];
            switch (ChartTool[ptrChart].ToolState)
            {
                case ChartToolState.Zoom:
                    //Zoom area.
                    double XStart = ptrChartArea.CursorX.SelectionStart;
                    double XEnd = ptrChartArea.CursorX.SelectionEnd;
                    double YStart = ptrChartArea.CursorY.SelectionStart;
                    double YEnd = ptrChartArea.CursorY.SelectionEnd;

                    //Zoom area for Y2 Axis
                    double YMin = ptrChartArea.AxisY.ValueToPosition(Math.Min(YStart, YEnd));
                    double YMax = ptrChartArea.AxisY.ValueToPosition(Math.Max(YStart, YEnd));

                    if ((XStart == XEnd) && (YStart == YEnd)) return;
                    //Zoom operation
                    ptrChartArea.AxisX.ScaleView.Zoom(
                        Math.Min(XStart, XEnd), Math.Max(XStart, XEnd));
                    ptrChartArea.AxisY.ScaleView.Zoom(
                        Math.Min(YStart, YEnd), Math.Max(YStart, YEnd));
                    ptrChartArea.AxisY2.ScaleView.Zoom(
                        ptrChartArea.AxisY2.PositionToValue(YMin),
                        ptrChartArea.AxisY2.PositionToValue(YMax));

                    //Clear selection
                    ptrChartArea.CursorX.SelectionStart = ptrChartArea.CursorX.SelectionEnd;
                    ptrChartArea.CursorY.SelectionStart = ptrChartArea.CursorY.SelectionEnd;
                    break;

                case ChartToolState.Pan:
                    break;
            }
        }
        #endregion
    }
}
