using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using EventHandlerSupport;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Chart control delegate function prototype.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public delegate void CursorPositionChanged(double x, double y);

    /// <summary>
    /// Form of the callback when the user has zoomed the chart.
    /// </summary>
    /// <param name="extents"></param>
    public delegate void ZoomChanged(ChartExtents extents);

    /// <summary>
    /// MSChart Control Extension's States
    /// </summary>
    public enum MSChartExtensionToolState
    {
        /// <summary>
        /// Undefined
        /// </summary>
        Unknown,
        /// <summary>
        /// Point Select Mode
        /// </summary>
        Select,
        /// <summary>
        /// Zoom
        /// </summary>
        Zoom,
        /// <summary>
        /// Zoom only along the X axis.
        /// </summary>
        ZoomX,
        /// <summary>
        /// Pan
        /// </summary>
        Pan
    }

    /// <summary>
    /// Extension class for MSChart
    /// </summary>
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
            sender.Points.Clear(); //Force refresh.
        }

        /// <summary>
        /// Enable Zoom and Pan Controls.
        /// </summary>
        public static void EnableZoomAndPanControls(this Chart sender)
        {
            EnableZoomAndPanControls(sender, null, null);
        }

        /// <summary>
        /// Enable Zoom and Pan Controls.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="selectionChanged">Selection changed callabck. Triggered when user select a point with selec tool.</param>
        /// <param name="cursorMoved">Cursor moved callabck. Triggered when user move the mouse in chart area.</param>
        /// <param name="zoomChanged">Callback triggered when chart has 
        /// zoomed in or out (and on first painting of the chart).</param>
        /// <remarks>Callback are optional (pass in null to ignore).</remarks>
        public static void EnableZoomAndPanControls(this Chart sender,
            CursorPositionChanged selectionChanged,
            CursorPositionChanged cursorMoved,
            ZoomChanged zoomChanged = null)
        {
            if (!ChartTool.ContainsKey(sender))
            {
                ChartTool[sender] = new ChartData(sender);
                ChartData ptrChartData = ChartTool[sender];
                ptrChartData.Backup();
                ptrChartData.SelectionChangedCallback = selectionChanged;
                ptrChartData.CursorMovedCallback = cursorMoved;
                ptrChartData.ZoomChangedCallback = zoomChanged;

                //Populate Context menu
                Chart ptrChart = sender;
                if (ptrChart.ContextMenuStrip == null)
                {
                    //Context menu is empty, use ChartContextMenuStrip directly
                    ptrChart.ContextMenuStrip = new ContextMenuStrip();
                    ptrChart.ContextMenuStrip.Items.AddRange(ChartTool[ptrChart].MenuItems.ToArray());
                }
                else
                {
                    //User assigned context menu to chart. Merge current menu with ChartContextMenuStrip.
                    ContextMenuStrip newMenu = new ContextMenuStrip();
                    newMenu.Items.AddRange(ChartTool[sender].MenuItems.ToArray());

                    foreach (object ptrItem in ChartTool[sender].ContextMenuStrip.Items)
                    {
                        if (ptrItem is ToolStripMenuItem) newMenu.Items.Add(((ToolStripMenuItem)ptrItem).Clone());
                        else if (ptrItem is ToolStripSeparator) newMenu.Items.Add(new ToolStripSeparator());
                    }
                    newMenu.Items.Add(new ToolStripSeparator());
                    ptrChart.ContextMenuStrip = newMenu;
                    ptrChart.ContextMenuStrip.AddHandlers(ChartTool[sender].ContextMenuStrip);
                }
                ptrChart.ContextMenuStrip.Opening += ChartContext_Opening;
                ptrChart.ContextMenuStrip.ItemClicked += ChartContext_ItemClicked;
                ptrChart.MouseDown += ChartControl_MouseDown;
                ptrChart.MouseMove += ChartControl_MouseMove;
                ptrChart.MouseUp += ChartControl_MouseUp;
                ptrChart.PostPaint += ChartOnPostPaint; // Necessary to kickstart ZoomChanged callback

                // The following is for testing out the built-in events. 
                //  They don't seem to be as reliable as just handling mouse up/move/down
                //ptrChart.CursorPositionChanging += (sender1, e) =>
                //    {
                //        // Changed event isn't triggered with any zoom or select operations! From looking at the Cursor.cs code, it seems to be a bug.
                //        // Changing event is raised twice, once for each cursor (X, Y)
                //        var axis = e.Axis;
                //    };
                //ptrChart.SelectionRangeChanging += (o, args) =>
                //    {
                //        // Changed event isn't triggered with any zoom or select operations!
                //        // Neither is changed event... odd
                //        Console.WriteLine("SelectionRangeChanging raised " + args.ToString());
                //        var axis = args.Axis;
                //        var chartArea = args.ChartArea;
                //    };


                //Override settings.
                ChartArea ptrChartArea = ptrChart.ChartAreas[0];
                ptrChartArea.CursorX.AutoScroll = false;
                ptrChartArea.CursorX.Interval = 1e-06;
                ptrChartArea.CursorY.AutoScroll = false;
                ptrChartArea.CursorY.Interval = 1e-06;

                ptrChartArea.AxisX.ScrollBar.Enabled = false;
                ptrChartArea.AxisX2.ScrollBar.Enabled = false;
                ptrChartArea.AxisY.ScrollBar.Enabled = false;
                ptrChartArea.AxisY2.ScrollBar.Enabled = false;

                SetChartControlState(sender, MSChartExtensionToolState.Select);
            }
        }

        private static void ChartOnPostPaint(object sender, ChartPaintEventArgs chartPaintEventArgs)
        {
            // Subscribing to PostPaint allows us to get the correct 
            //  bounds of the chart to send to the user. Some other events
            //  don't seem to be sufficient.
            Chart ptrChart = sender as Chart;
            if (ptrChart == null) return;
            ptrChart.PostPaint -= ChartOnPostPaint; // Only run once
            OnZoomChanged(ptrChart);
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
        /// <summary>
        /// Get current control state.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static MSChartExtensionToolState GetChartToolState(this Chart sender)
        {
            if (!ChartTool.ContainsKey(sender))
                return MSChartExtensionToolState.Unknown;
            else
                return ChartTool[sender].ToolState;

        }

        #region [ ContextMenu - Event Handler ]

        private static void ChartContext_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menuStrip = (ContextMenuStrip)sender;
            Chart senderChart = (Chart)menuStrip.SourceControl;
            ChartData ptrData = ChartTool[senderChart];

            //Check Zoomed state
            if (senderChart.ChartAreas[0].AxisX.ScaleView.IsZoomed ||
                senderChart.ChartAreas[0].AxisY.ScaleView.IsZoomed ||
                senderChart.ChartAreas[0].AxisY2.ScaleView.IsZoomed)
            {
                ptrData.ChartToolZoomOut.Visible = true;
                ptrData.ChartToolZoomOutSeparator.Visible = true;
            }
            else
            {
                ptrData.ChartToolZoomOut.Visible = false;
                ptrData.ChartToolZoomOutSeparator.Visible = false;
            }

            //Get Chart Control State
            if (!ChartTool.ContainsKey(senderChart))
            {
                //Initialize Chart Tool
                SetChartControlState(senderChart, MSChartExtensionToolState.Select);
            }

            //Update menu (uncheck all, check current) based on current state.
            ptrData.UpdateState();

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
                ToolStripItem ptrItem = menuStrip.Items.Add(ptrSeries.Name);
                ToolStripMenuItem ptrMenuItem = (ToolStripMenuItem)ptrItem;
                ptrMenuItem.Checked = ptrSeries.Enabled;
                ptrItem.Tag = "Series";
            }
        }
        private static void ChartContext_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip ptrMenuStrip = (ContextMenuStrip)sender;
            Chart ptrChart = (Chart)ptrMenuStrip.SourceControl;
            switch (e.ClickedItem.Text)
            {
                case "Select":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.Select);
                    break;
                case "Zoom":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.Zoom);
                    break;
                case "Zoom XAxis":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.ZoomX);
                    break;
                case "Pan":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.Pan);
                    break;
                case "Zoom Out":
                    {
                        WindowMessagesNativeMethods.SuspendDrawing(ptrChart);
                        ChartArea ptrChartArea = ptrChart.ChartAreas[0];
                        ptrChartArea.AxisX.ScaleView.ZoomReset();
                        ptrChartArea.AxisX2.ScaleView.ZoomReset();
                        ptrChartArea.AxisY.ScaleView.ZoomReset();
                        ptrChartArea.AxisY2.ScaleView.ZoomReset();
                        WindowMessagesNativeMethods.ResumeDrawing(ptrChart);
                        OnZoomChanged(ptrChart);
                    }
                    break;
            }

            if (e.ClickedItem.Tag == null) return;
            if (e.ClickedItem.Tag.ToString() != "Series") return;

            //Series enable / disable changed.
            SeriesCollection chartSeries = ptrChart.Series;
            chartSeries[e.ClickedItem.Text].Enabled = !((ToolStripMenuItem)e.ClickedItem).Checked;
        }

        #endregion

        #region [ Chart Control State + Events ]
        private class ChartData
        {
            //Store chart settings. Used to backup and restore chart settings.

            private Chart Source;
            public ChartData(Chart chartSource)
            {
                Source = chartSource;
                CreateChartContextMenu();
            }

            public MSChartExtensionToolState ToolState { get; set; }
            public CursorPositionChanged SelectionChangedCallback;
            public CursorPositionChanged CursorMovedCallback;
            public ZoomChanged ZoomChangedCallback { get; set; }

            private void CreateChartContextMenu()
            {
                ChartToolZoomOut = new ToolStripMenuItem("Zoom Out");
                ChartToolZoomOutSeparator = new ToolStripSeparator();
                ChartToolSelect = new ToolStripMenuItem("Select");
                ChartToolZoom = new ToolStripMenuItem("Zoom");
                ChartToolZoomX = new ToolStripMenuItem("Zoom XAxis");
                ChartToolPan = new ToolStripMenuItem("Pan");
                ChartContextSeparator = new ToolStripSeparator();

                MenuItems = new List<ToolStripItem>();
                MenuItems.Add(ChartToolZoomOut);
                MenuItems.Add(ChartToolZoomOutSeparator);
                MenuItems.Add(ChartToolSelect);
                MenuItems.Add(ChartToolZoom);
                MenuItems.Add(ChartToolZoomX);
                MenuItems.Add(ChartToolPan);
                MenuItems.Add(ChartContextSeparator);

                StateMenu = new Dictionary<MSChartExtensionToolState, ToolStripMenuItem>
                {
                    {MSChartExtensionToolState.Select, ChartToolSelect},
                    {MSChartExtensionToolState.Pan, ChartToolPan},
                    {MSChartExtensionToolState.Zoom, ChartToolZoom},
                    {MSChartExtensionToolState.ZoomX, ChartToolZoomX}
                };
            }

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
                ScrollBarX = ptrChartArea.AxisX.ScrollBar.Enabled;
                ScrollBarX2 = ptrChartArea.AxisX2.ScrollBar.Enabled;
                ScrollBarY = ptrChartArea.AxisY.ScrollBar.Enabled;
                ScrollBarY2 = ptrChartArea.AxisY2.ScrollBar.Enabled;
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
                ptrChartArea.AxisX.ScrollBar.Enabled = ScrollBarX;
                ptrChartArea.AxisX2.ScrollBar.Enabled = ScrollBarX2;
                ptrChartArea.AxisY.ScrollBar.Enabled = ScrollBarY;
                ptrChartArea.AxisY2.ScrollBar.Enabled = ScrollBarY2;
            }
            public void UncheckAllMenuItems()
            {
                foreach (ToolStripMenuItem ptrItem in StateMenu.Values) ptrItem.Checked = false;
            }
            public void UpdateState()
            {
                UncheckAllMenuItems();
                StateMenu[ToolState].Checked = true;
            }

            #region [ Backup Data ]

            public ContextMenuStrip ContextMenuStrip { get; set; }
            private bool CursorXUserEnabled;
            private bool CursorYUserEnabled;
            private System.Windows.Forms.Cursor Cursor;
            private double CursorXInterval, CursorYInterval;
            private bool CursorXAutoScroll, CursorYAutoScroll;
            private bool ScrollBarX, ScrollBarX2, ScrollBarY, ScrollBarY2;

            #endregion

            #region [ Extended Context Menu ]

            public List<ToolStripItem> MenuItems { get; private set; }
            public ToolStripMenuItem ChartToolSelect { get; private set; }
            public ToolStripMenuItem ChartToolZoom { get; private set; }
            public ToolStripMenuItem ChartToolZoomX { get; private set; }
            public ToolStripMenuItem ChartToolPan { get; private set; }
            public ToolStripMenuItem ChartToolZoomOut { get; private set; }
            public ToolStripSeparator ChartToolZoomOutSeparator { get; private set; }
            public ToolStripSeparator ChartContextSeparator { get; private set; }
            private Dictionary<MSChartExtensionToolState, ToolStripMenuItem> StateMenu;


            #endregion

        }
        private static Dictionary<Chart, ChartData> ChartTool = new Dictionary<Chart, ChartData>();
        private static void SetChartControlState(Chart sender, MSChartExtensionToolState state)
        {
            ChartTool[(Chart)sender].ToolState = state;
            sender.ChartAreas[0].CursorX.IsUserEnabled = false;
            sender.ChartAreas[0].CursorY.IsUserEnabled = false;
            switch (state)
            {
                case MSChartExtensionToolState.Select:
                    sender.Cursor = Cursors.Cross;
                    sender.ChartAreas[0].CursorX.IsUserEnabled = true;
                    sender.ChartAreas[0].CursorY.IsUserEnabled = true;
                    break;
                case MSChartExtensionToolState.Zoom:
                case MSChartExtensionToolState.ZoomX:
                    sender.Cursor = Cursors.Cross;
                    break;
                case MSChartExtensionToolState.Pan:
                    sender.Cursor = Cursors.Hand;
                    break;
            }
        }
        #endregion

        #region [ Chart - Mouse Events ]
        private static bool MouseDowned;
        private static double X2Start, Y2Start;
        private static void ChartControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            Chart ptrChart = (Chart)sender;
            ChartArea ptrChartArea = ptrChart.ChartAreas[0];

            MouseDowned = true;

            //NOTE: Clicking on the chart in selection mode will draw a cross whether or not the following
            //  code is run (since Cursor.IsUserEnabled is true)

            // We must set the selection start because it doesn't seem to get
            //    reset automatically (remove the next two lines and zoom a few times to see)
            Point startAndEndPt = e.Location;
            const bool roundToBoundary = true;
            ptrChartArea.CursorX.SetSelectionPixelPosition(startAndEndPt, startAndEndPt, roundToBoundary);
            ptrChartArea.CursorY.SetSelectionPixelPosition(startAndEndPt, startAndEndPt, roundToBoundary);

            ChartData chartData = GetDataForChart(ptrChart);
            if (chartData.SelectionChangedCallback != null)
            {
                // If we use Position, there's no need to set/get the Selection
                chartData.SelectionChangedCallback(
                    ptrChartArea.CursorX.Position,
                    ptrChartArea.CursorY.Position);
            }
        }

        private static void ChartControl_MouseMove(object sender, MouseEventArgs e)
        {
            Chart ptrChart = (Chart)sender;
            double selX, selY, selX2, selY2;
            selX = selY = selX2 = selY2 = 0;
            try
            {
                selX = ptrChart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X);
                selY = ptrChart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y);
                selX2 = ptrChart.ChartAreas[0].AxisX2.PixelPositionToValue(e.Location.X);
                selY2 = ptrChart.ChartAreas[0].AxisY2.PixelPositionToValue(e.Location.Y);

                if (ChartTool[ptrChart].CursorMovedCallback != null)
                    ChartTool[ptrChart].CursorMovedCallback(selX, selY);
            }
            catch (Exception) { /*ToDo: Set coordinate to 0,0 */ return; } //Handle exception when scrolled out of range.

            switch (ChartTool[ptrChart].ToolState)
            {
                case MSChartExtensionToolState.Zoom:
                    #region [ Zoom Control ]
                    if (MouseDowned)
                    {
                        ptrChart.ChartAreas[0].CursorX.SelectionEnd = selX;
                        ptrChart.ChartAreas[0].CursorY.SelectionEnd = selY;
                    }
                    #endregion
                    break;

                case MSChartExtensionToolState.ZoomX:
                    if (MouseDowned)
                    {
                        ptrChart.ChartAreas[0].CursorX.SelectionEnd = selX;
                    }
                    break;

                case MSChartExtensionToolState.Pan:
                    #region [ Pan Control ]
                    if (MouseDowned)
                    {
                        //Pan Move - Valid only if view is zoomed
                        if (ptrChart.ChartAreas[0].AxisX.ScaleView.IsZoomed ||
                            ptrChart.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                        {
                            double dx = -selX + ptrChart.ChartAreas[0].CursorX.SelectionStart;
                            double dy = -selY + ptrChart.ChartAreas[0].CursorY.SelectionStart;
                            double dx2 = -selX2 + X2Start;
                            double dy2 = -selY2 + Y2Start;

                            double newX = ptrChart.ChartAreas[0].AxisX.ScaleView.Position + dx;
                            double newY = ptrChart.ChartAreas[0].AxisY.ScaleView.Position + dy;
                            double newX2 = ptrChart.ChartAreas[0].AxisX2.ScaleView.Position + dx2;
                            double newY2 = ptrChart.ChartAreas[0].AxisY2.ScaleView.Position + dy2;

                            ptrChart.ChartAreas[0].AxisX.ScaleView.Scroll(newX);
                            ptrChart.ChartAreas[0].AxisY.ScaleView.Scroll(newY);
                            ptrChart.ChartAreas[0].AxisX2.ScaleView.Scroll(newX2);
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
            ChartData data = GetDataForChart(ptrChart);
            MSChartExtensionToolState state = data.ToolState;
            switch (state)
            {
                case MSChartExtensionToolState.Zoom:
                case MSChartExtensionToolState.ZoomX:
                    
                    //Zoom area (Pixel)
                    double XStart = ptrChartArea.CursorX.SelectionStart;
                    double XEnd = ptrChartArea.CursorX.SelectionEnd;
                    double YStart = ptrChartArea.CursorY.SelectionStart;
                    double YEnd = ptrChartArea.CursorY.SelectionEnd;

                    if ((XStart == XEnd) && (YStart == YEnd)) return;

                    //Zoom area for X Axis
                    double left = Math.Min(XStart, XEnd);
                    double right = Math.Max(XStart, XEnd);
                    double XMin = ptrChartArea.AxisX.ValueToPosition(left);
                    double XMax = ptrChartArea.AxisX.ValueToPosition(right);
                    
                    //Zoom area for Y Axis
                    double bottom = Math.Min(YStart, YEnd);
                    double top = Math.Max(YStart, YEnd);
                    double YMin = ptrChartArea.AxisY.ValueToPosition(bottom);
                    double YMax = ptrChartArea.AxisY.ValueToPosition(top);


                    // NOTE: left <= right, even if Axis.IsReversed

                    //X-Axis
                    ptrChartArea.AxisX.ScaleView.Zoom(left, right);
                    ptrChartArea.AxisX2.ScaleView.Zoom(
                        ptrChartArea.AxisX2.PositionToValue(XMin),
                        ptrChartArea.AxisX2.PositionToValue(XMax));

                    //Y-Axis
                    if (state == MSChartExtensionToolState.Zoom)
                    {
                        ptrChartArea.AxisY.ScaleView.Zoom(bottom, top);
                        ptrChartArea.AxisY2.ScaleView.Zoom(
                            ptrChartArea.AxisY2.PositionToValue(YMin),
                            ptrChartArea.AxisY2.PositionToValue(YMax));

                    }

                    //Clear selection (the following seem to be equivalent)
                    ptrChartArea.CursorX.SetSelectionPosition(0, 0);
                    ptrChartArea.CursorY.SetSelectionPosition(0, 0);
                    //ptrChartArea.CursorX.SelectionStart = ptrChartArea.CursorX.SelectionEnd;
                    //ptrChartArea.CursorY.SelectionStart = ptrChartArea.CursorY.SelectionEnd;

                    //NOTE: At this point, the scaled view has already been updated to reflect zoom
                    OnZoomChanged(ptrChart);
                    break;

                case MSChartExtensionToolState.Pan:
                    break;
            }
        }

        /// <summary>
        /// Gets the boundaries (top, left, bottom, right) of this chart's visible 
        /// data in the same units and direction as the data. 
        /// </summary>
        /// <remarks>The ZoomChanged callback provides
        /// the same data. Alternately, if you have the chart area of interest,
        /// use <see cref="ChartAreaExtensions.GetBoundariesOfData"/>.</remarks>
        /// <param name="ptrChart">The chart.</param>
        /// <returns>Boundaries (<see cref="ChartExtents"/>) of the chart.</returns>
        public static ChartExtents GetBoundariesOfVisibleData(this Chart ptrChart)
        {
            return CheckAndGetArea(ptrChart).GetBoundariesOfData(justVisible: true);
        }

        /// <summary>
        /// Gets the boundaries (top, left, bottom, right) of this chart's
        /// data in the same units and direction as the data. 
        /// </summary>
        /// <remarks>Alternately, if you have the chart area of interest,
        /// use <see cref="ChartAreaExtensions.GetBoundariesOfData"/>.</remarks>
        /// <param name="ptrChart">The chart.</param>
        /// <returns>Boundaries (<see cref="ChartExtents"/>) of the chart.</returns>
        public static ChartExtents GetBoundariesOfData(this Chart ptrChart)
        {
            return CheckAndGetArea(ptrChart).GetBoundariesOfData(justVisible: false);
        }

        private static ChartArea CheckAndGetArea(Chart ptrChart, int chartAreaIndex=0)
        {
            if (ptrChart.ChartAreas.Count <= chartAreaIndex) throw new InvalidOperationException("Missing chart area");
            ChartArea ptrChartArea = ptrChart.ChartAreas[chartAreaIndex];
            return ptrChartArea;
        }

        private static void OnZoomChanged(Chart ptrChart)
        {
            var data = GetDataForChart(ptrChart);
            if (data.ZoomChangedCallback == null)
                return;
            //Precondition: At this point, the scaled view has already been updated to reflect zoom
            data.ZoomChangedCallback(ptrChart.GetBoundariesOfVisibleData());
        }

        private static ChartData GetDataForChart(Chart ptrChart)
        {
            return ChartTool[ptrChart];
        }

        #endregion

        #region [ Annotations ]

        /// <summary>
        /// Draw a horizontal line on chart.
        /// </summary>
        /// <param name="sender">Source Chart.</param>
        /// <param name="y">YAxis value.</param>
        /// <param name="lineColor">Line color.</param>
        /// <param name="name">Annotation name.</param>
        /// <param name="lineWidth">Line width</param>
        /// <param name="lineStyle">Line style</param>
        public static void DrawHorizontalLine(this Chart sender, double y,
            Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid)
        {
            HorizontalLineAnnotation horzLine = new HorizontalLineAnnotation();
            string chartAreaName = sender.ChartAreas[0].Name;
            horzLine.ClipToChartArea = chartAreaName;
            horzLine.AxisXName = chartAreaName + "\\rX";
            horzLine.YAxisName = chartAreaName + "\\rY";
            horzLine.IsInfinitive = true;
            horzLine.IsSizeAlwaysRelative = false;

            horzLine.Y = y;
            horzLine.LineColor = lineColor;
            horzLine.LineWidth = lineWidth;
            horzLine.LineDashStyle = lineStyle;
            sender.Annotations.Add(horzLine);

            if (!string.IsNullOrEmpty(name)) horzLine.Name = name;
        }

        /// <summary>
        /// Draw a vertical line on chart.
        /// </summary>
        /// <param name="sender">Source Chart.</param>
        /// <param name="x">XAxis value.</param>
        /// <param name="lineColor">Line color.</param>
        /// <param name="name">Annotation name.</param>
        /// <param name="lineWidth">Line width</param>
        /// <param name="lineStyle">Line style</param>
        public static void DrawVerticalLine(this Chart sender, double x,
            Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid)
        {

            VerticalLineAnnotation vertLine = new VerticalLineAnnotation();
            string chartAreaName = sender.ChartAreas[0].Name;
            vertLine.ClipToChartArea = chartAreaName;
            vertLine.AxisXName = chartAreaName + "\\rX";
            vertLine.YAxisName = chartAreaName + "\\rY";
            vertLine.IsInfinitive = true;
            vertLine.IsSizeAlwaysRelative = false;

            vertLine.X = x;
            vertLine.LineColor = lineColor;
            vertLine.LineWidth = lineWidth;
            vertLine.LineDashStyle = lineStyle;
            sender.Annotations.Add(vertLine);

            if (!string.IsNullOrEmpty(name)) vertLine.Name = name;
        }

        /// <summary>
        /// Draw a rectangle on chart.
        /// </summary>
        /// <param name="sender">Source Chart.</param>
        /// <param name="x">XAxis value</param>
        /// <param name="y">YAxis value</param>
        /// <param name="width">rectangle width using XAis value.</param>
        /// <param name="height">rectangle height using YAis value.</param>
        /// <param name="lineColor">Outline color.</param>
        /// <param name="name">Annotation name.</param>
        /// <param name="lineWidth">Line width</param>
        /// <param name="lineStyle">Line style</param>
        public static void DrawRectangle(this Chart sender, double x, double y,
            double width, double height,
            Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid)
        {
            RectangleAnnotation rect = new RectangleAnnotation();
            string chartAreaName = sender.ChartAreas[0].Name;
            rect.ClipToChartArea = chartAreaName;
            rect.AxisXName = chartAreaName + "\\rX";
            rect.YAxisName = chartAreaName + "\\rY";
            rect.BackColor = Drawing.Color.Transparent;
            rect.ForeColor = Drawing.Color.Transparent;
            rect.IsSizeAlwaysRelative = false;

            rect.LineColor = lineColor;
            rect.LineWidth = lineWidth;
            rect.LineDashStyle = lineStyle;

            //Limit rectangle within chart area
            Axis ptrAxis = sender.ChartAreas[0].AxisX;
            if (x < ptrAxis.Minimum)
            {
                width = width - (ptrAxis.Minimum - x);
                x = ptrAxis.Minimum;
            }
            else if (x > ptrAxis.Maximum)
            {
                width = width - (x - ptrAxis.Maximum);
                x = ptrAxis.Maximum;
            }
            if ((x + width) > ptrAxis.Maximum) width = ptrAxis.Maximum - x;

            ptrAxis = sender.ChartAreas[0].AxisY;
            if (y < ptrAxis.Minimum)
            {
                height = height - (ptrAxis.Minimum - y);
                y = ptrAxis.Minimum;
            }
            else if (y > ptrAxis.Maximum)
            {
                height = height - (y - ptrAxis.Maximum);
                y = ptrAxis.Maximum;
            }
            if ((y + height) > ptrAxis.Maximum) height = ptrAxis.Maximum - y;

            rect.X = x;
            rect.Y = y;
            rect.Width = width;
            rect.Height = height;
            rect.LineColor = lineColor;
            sender.Annotations.Add(rect);

            if (!string.IsNullOrEmpty(name)) rect.Name = name;

        }

        /// <summary>
        /// Draw a line on chart.
        /// </summary>
        /// <param name="sender">Source Chart.</param>
        /// <param name="x0">First point on XAxis.</param>
        /// <param name="x1">Second piont on XAxis.</param>
        /// <param name="y0">First point on YAxis.</param>
        /// <param name="y1">Second point on YAxis.</param>
        /// <param name="lineColor">Outline color.</param>
        /// <param name="name">Annotation name.</param>
        /// <param name="lineWidth">Line width</param>
        /// <param name="lineStyle">Line style</param>
        public static void DrawLine(this Chart sender, double x0, double x1,
            double y0, double y1, Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid)
        {
            LineAnnotation line = new LineAnnotation();
            string chartAreaName = sender.ChartAreas[0].Name;
            line.ClipToChartArea = chartAreaName;
            line.AxisXName = chartAreaName + "\\rX";
            line.YAxisName = chartAreaName + "\\rY";
            line.IsSizeAlwaysRelative = false;

            line.X = x0;
            line.Y = y0;
            line.Height = y1 - y0;
            line.Width = x1 - x0;
            line.LineColor = lineColor;
            line.LineWidth = lineWidth;
            line.LineDashStyle = lineStyle;
            sender.Annotations.Add(line);

            if (!string.IsNullOrEmpty(name)) line.Name = name;
        }

        /// <summary>
        /// Add text annotation to chart.
        /// </summary>
        /// <param name="sender">Source Chart.</param>
        /// <param name="text">Text to display.</param>
        /// <param name="x">Text box upper left X Coordinate.</param>
        /// <param name="y">Text box upper left Y coordinate.</param>
        /// <param name="textColor">Text color.</param>
        /// <param name="name">Annotation name.</param>
        /// <param name="textStyle">Style of text.</param>
        public static void AddText(this Chart sender, string text,
            double x, double y,
            Drawing.Color textColor, string name = "",
            TextStyle textStyle = TextStyle.Default)
        {
            TextAnnotation textAnn = new TextAnnotation();
            string chartAreaName = sender.ChartAreas[0].Name;
            textAnn.ClipToChartArea = chartAreaName;
            textAnn.AxisXName = chartAreaName + "\\rX";
            textAnn.YAxisName = chartAreaName + "\\rY";
            textAnn.IsSizeAlwaysRelative = false;

            textAnn.Text = text;
            textAnn.ForeColor = textColor;
            textAnn.X = x;
            textAnn.Y = y;
            textAnn.TextStyle = textStyle;

            sender.Annotations.Add(textAnn);
            if (!string.IsNullOrEmpty(name)) textAnn.Name = name;
        }

        #endregion
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
