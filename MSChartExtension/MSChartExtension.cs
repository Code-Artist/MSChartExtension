using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using EventHandlerSupport;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Chart callback function when cursor position changed.
    /// </summary>
    /// <param name="sender">Chart object</param>
    /// <param name="cursor">Chart cursor</param>
    public delegate void CursorPositionChanged(Chart sender, ChartCursor cursor);

    /// <summary>
    /// Form of the callback when the user has zoomed the chart.
    /// </summary>
    /// <param name="sender">Chart Object</param>
    public delegate void ZoomChanged(Chart sender);

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
        /// Point Select Mode - Second Cursor
        /// </summary>
        Select2,
        /// <summary>
        /// Zoom
        /// </summary>
        Zoom,
        /// <summary>
        /// Zoom only along the X axis.
        /// </summary>
        ZoomX,
        /// <summary>
        /// Zoom only along the Y axis.
        /// </summary>
        ZoomY,
        /// <summary>
        /// Configure Zoom region from pop up dialog
        /// </summary>
        ZoomDialog,
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
        private static readonly SeriesChartType[] UnsupportedChartType =
            new SeriesChartType[] {
                SeriesChartType.Pie, SeriesChartType.Doughnut,
                SeriesChartType.Radar, SeriesChartType.Polar,
                SeriesChartType.PointAndFigure, SeriesChartType.Funnel,
                SeriesChartType.Pyramid
            };


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
        /// <param name="option">Additional user options</param>
        /// <remarks>
        /// <para>Callback are optional (pass in null to ignore).</para>
        /// <para>WARNING: Add or Remove Chart Area or Chart Series after enabled zoom and pan controls may cause unexpected behavior.</para>
        /// <para>Recommended to enable the zoom and pan controls after configure the <see cref="ChartArea"/> and <see cref="Series"/>.</para>
        /// </remarks>
        public static void EnableZoomAndPanControls(this Chart sender,
            CursorPositionChanged selectionChanged,
            CursorPositionChanged cursorMoved,
            ZoomChanged zoomChanged = null, ChartOption option = null)
        {
            if (!ChartTool.ContainsKey(sender))
            {
                ChartData ptrChartData = ChartTool[sender] = new ChartData(sender);
                ptrChartData.Option = option ?? new ChartOption();
                ptrChartData.Backup();

                //Scan through series to identify valid ChartArea
                Chart ptrChart = sender;
                foreach (ChartArea cArea in ptrChart.ChartAreas)
                {
                    IEnumerable<Series> chartSeries = ptrChart.Series.Where(n => n.ChartArea == cArea.Name);
                    if (chartSeries.Count() == 0)
                    {
                        Debug.WriteLine(string.Format("WARNING: Chart Area [{0}] does not contains any series.", cArea.Name));
                    }
                    else if (chartSeries.Where(n => UnsupportedChartType.Contains(n.ChartType)).Count() > 0)
                    {
                        Debug.WriteLine(string.Format("WARNING: Chart Area [{0}] contains unsupported series.", cArea.Name));
                    }
                    else ptrChartData.SupportedChartArea.Add(cArea);
                }

                if (ptrChartData.SupportedChartArea.Count == 0)
                {
                    //No Supported Chart Area found, disable controls.
                    Debug.WriteLine("WARNING: Chart type not supported! Controls disabled!");
                    ChartTool.Remove(sender);
                    return;
                }
                ptrChartData.PositionChangedCallback = selectionChanged;
                ptrChartData.CursorMovedCallback = cursorMoved;
                ptrChartData.ZoomChangedCallback = zoomChanged;

                //Populate Context menu
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
                    newMenu.Items.Add(new ToolStripSeparator());

                    foreach (object ptrItem in ChartTool[sender].ContextMenuStrip.Items)
                    {
                        if (ptrItem is ToolStripMenuItem) newMenu.Items.Add(((ToolStripMenuItem)ptrItem).Clone());
                        else if (ptrItem is ToolStripSeparator) newMenu.Items.Add(new ToolStripSeparator());
                    }
                    ptrChart.ContextMenuStrip = newMenu;
                    ptrChart.ContextMenuStrip.AddHandlers(ChartTool[sender].ContextMenuStrip);
                }
                ptrChart.ContextMenuStrip.Opening += ChartContext_Opening;
                ptrChart.ContextMenuStrip.ItemClicked += ChartContext_ItemClicked;
                ptrChart.MouseWheel += ChartControl_MouseWheel;
                ptrChart.MouseDown += ChartControl_MouseDown;
                ptrChart.MouseMove += ChartControl_MouseMove;
                ptrChart.MouseUp += ChartControl_MouseUp;

                //Override settings.
                foreach (ChartArea ptrChartArea in ptrChart.ChartAreas)
                {
                    ptrChartArea.CursorX.AutoScroll = true;
                    ptrChartArea.CursorX.Interval = 1e-06;
                    ptrChartArea.CursorY.AutoScroll = false;
                    ptrChartArea.CursorY.Interval = 1e-06;

                    ptrChartArea.AxisX.ScrollBar.Enabled = true;
                    ptrChartArea.AxisX2.ScrollBar.Enabled = true;
                    ptrChartArea.AxisY.ScrollBar.Enabled = false;
                    ptrChartArea.AxisY2.ScrollBar.Enabled = false;

                    ptrChartArea.AxisX.ScrollBar.IsPositionedInside = true;
                    ptrChartArea.AxisX2.ScrollBar.IsPositionedInside = true;
                }
                SetChartControlState(sender, MSChartExtensionToolState.Select);

                if (ptrChartData.Option.Theme != null)
                {
                    ptrChart.AssignTheme(ptrChartData.Option.Theme);
                }

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


        #region [ Theme ]

        private static void AssignTheme(this Chart sender, ThemeBase theme)
        {
            sender.BackColor = theme.BackColor;
            foreach (Title t in sender.Titles)
            {
                t.ForeColor = theme.TitleColor;
            }
            foreach (ChartArea a in sender.ChartAreas)
            {
                a.BackColor = theme.ChartAreaBackColor;
                foreach (Axis x in a.Axes)
                {
                    x.LineColor = theme.AxisLineColor;
                    x.MajorGrid.LineColor = x.MajorTickMark.LineColor = theme.AxisLineColor;
                    x.MinorGrid.LineColor = x.MinorTickMark.LineColor = theme.AxisLineColor;
                    x.LabelStyle.ForeColor = theme.AxisLabelColor;
                    x.TitleForeColor = theme.AxisLabelColor;
                }
            }
        }

        #endregion


        #region [ Cursors ]

        /// <summary>
        /// Return Cursor 1 Object
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static ChartCursor Cursor1(this Chart sender) { return ChartTool[sender].Cursor1.Clone() as ChartCursor; }

        /// <summary>
        /// Return Cursor 2 Object
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static ChartCursor Cursor2(this Chart sender) { return ChartTool[sender].Cursor2.Clone() as ChartCursor; }

        /// <summary>
        /// Calculate different between Cursor2 and Cursor1 where result = Cursor2 - Cursor1.
        /// Return 0 if either cursor is not valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static PointF CursorsDiff(this Chart sender)
        {
            ChartData ptrData = ChartTool[sender];
            if ((ptrData.Cursor1.ChartArea == null) || (ptrData.Cursor2.ChartArea == null)) return new PointF(0, 0);
            return new PointF((float)(ptrData.Cursor2.X - ptrData.Cursor1.X), (float)(ptrData.Cursor2.Y - ptrData.Cursor1.Y));
        }

        #endregion

        #region [ (PRIVATE) ContextMenu - Event Handler ]

        private static void ChartContext_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menuStrip = (ContextMenuStrip)sender;
            Chart senderChart = (Chart)menuStrip.SourceControl;
            ChartData ptrChartData = ChartTool[senderChart];
            ChartArea activeChartArea = ptrChartData.ActiveChartArea;

            //Clear previous series
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
            ptrChartData.ChartToolSelect.DropDownItems.Clear();
            ptrChartData.ChartToolSelect2.DropDownItems.Clear();

            //Add Series to Context Menu
            List<Series> ChartSeries = new List<Series>();
            SeriesCollection chartSeries = ((Chart)menuStrip.SourceControl).Series;
            if (ptrChartData.ActiveChartArea != null)
            {
                int tIndex = menuStrip.Items.IndexOfKey("AboutMenu");
                if (ptrChartData.Option.ContextMenuAllowToHideSeries)
                {
                    ToolStripSeparator separator = new ToolStripSeparator() { Tag = "Series" };
                    menuStrip.Items.Insert(tIndex++, separator);
                }

                foreach (Series ptrSeries in chartSeries)
                {
                    if (ptrSeries.ChartArea != ptrChartData.ActiveChartArea.Name) continue;

                    ChartSeries.Add(ptrSeries);
                    if (ptrChartData.Option.ContextMenuAllowToHideSeries) //Option to show / hide series controls
                    {
                        ToolStripMenuItem ptrMenuItem = new ToolStripMenuItem(ptrSeries.Name);
                        menuStrip.Items.Insert(tIndex++, ptrMenuItem);
                        ptrMenuItem.Checked = ptrSeries.Enabled;
                        ptrMenuItem.Tag = "Series";
                    }
                }

                if (ChartSeries.Count == 1)
                {
                    ptrChartData.Cursor1.SelectedChartSeries = ChartSeries[0];
                    ptrChartData.Cursor2.SelectedChartSeries = ChartSeries[0];
                }
                else if (chartSeries.Count > 1)
                {
                    //Default cursor to first chart series if previous selected series not exist.
                    if (!ChartSeries.Contains(ptrChartData.Cursor1.SelectedChartSeries)) ptrChartData.Cursor1.SelectedChartSeries = ChartSeries[0];
                    if (!ChartSeries.Contains(ptrChartData.Cursor2.SelectedChartSeries)) ptrChartData.Cursor2.SelectedChartSeries = ChartSeries[0];

                    //Populate Context Menu for user to select series for each Chart Cursor.
                    if (ChartSeries.Count > 1)
                    {
                        foreach (Series s in ChartSeries)
                        {
                            //Cursor 1
                            ToolStripMenuItem ptrItem = ptrChartData.ChartToolSelect.DropDownItems.Add(s.Name) as ToolStripMenuItem;
                            ptrItem.Tag = ptrChartData.ChartToolSelect;
                            ptrItem.Click += ChartToolSelect_SeriesChanged;
                            if (s == ptrChartData.Cursor1.SelectedChartSeries) ptrItem.Checked = true;

                            //Cursor 2
                            ptrItem = ptrChartData.ChartToolSelect2.DropDownItems.Add(s.Name) as ToolStripMenuItem;
                            ptrItem.Tag = ptrChartData.ChartToolSelect2;
                            ptrItem.Click += ChartToolSelect_SeriesChanged;
                            if (s == ptrChartData.Cursor2.SelectedChartSeries) ptrItem.Checked = true;
                        }
                    }
                }
            }//Active Chart Area

            if (!ptrChartData.Enabled || IsChartAreaEmpty(senderChart, ptrChartData.ActiveChartArea))
            {
                //Disable Zoom and Pan Controls
                foreach (ToolStripItem item in ptrChartData.MenuItems)
                {
                    if (item.Name.Equals("About")) item.Enabled = true;
                    else item.Enabled = false;
                }
                UpdateChartControlState(senderChart);
                return;
            }
            else
            {
                foreach (ToolStripItem item in ptrChartData.MenuItems)
                    item.Enabled = true;
            }

            //Check Zoom State
            ptrChartData.ChartToolPan.Enabled = activeChartArea.IsZoomed();
            ptrChartData.ChartToolZoomOut.Enabled = ptrChartData.ChartToolPan.Enabled;

            //Get Chart Control State
            if (!ChartTool.ContainsKey(senderChart))
            {
                //Initialize Chart Tool
                SetChartControlState(senderChart, MSChartExtensionToolState.Select);
            }

            //Update menu (uncheck all, check current) based on current state.
            ptrChartData.UpdateState();

        }

        private static void ChartToolSelect_SeriesChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem ptrClickedItem) //Series
            {
                if (ptrClickedItem.Tag is ToolStripMenuItem ptrChartToolSelect) //ChartToolSelect
                {
                    if (ptrChartToolSelect.Tag is ChartData ptrChartData) //Chart Data
                    {
                        Chart ptrChart = ptrChartData.Source;
                        if (ptrChartToolSelect == ptrChartData.ChartToolSelect)
                        {
                            ptrChartData.Cursor1.SelectedChartSeries = ptrChart.Series[ptrClickedItem.Text];
                            SetChartControlState(ptrChart, MSChartExtensionToolState.Select);
                        }
                        else if (ptrChartToolSelect == ptrChartData.ChartToolSelect2)
                        {
                            ptrChartData.Cursor2.SelectedChartSeries = ptrChart.Series[ptrClickedItem.Text];
                            SetChartControlState(ptrChart, MSChartExtensionToolState.Select2);
                        }
                    }
                }
            }
        }

        private static void ChartContext_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip ptrMenuStrip = (ContextMenuStrip)sender;
            Chart ptrChart = (Chart)ptrMenuStrip.SourceControl;
            ChartData ptrChartData = ChartTool[ptrChart];
            ChartArea ptrChartArea = ptrChartData.ActiveChartArea;
            if (ptrChartArea == null) return;

            if (e.ClickedItem.Tag == ptrChartData.ChartToolSelect)
            {
                ptrChartData.Cursor1.SelectedChartSeries = ptrChart.Series[e.ClickedItem.Text];
                SetChartControlState(ptrChart, MSChartExtensionToolState.Select);
                return;
            }
            else if (e.ClickedItem.Tag == ptrChartData.ChartToolSelect2)
            {
                ptrChartData.Cursor2.SelectedChartSeries = ptrChart.Series[e.ClickedItem.Text];
                SetChartControlState(ptrChart, MSChartExtensionToolState.Select2);
                return;
            }

            switch (e.ClickedItem.Text)
            {
                case "Select - Cursor 1":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.Select);
                    break;
                case "Select - Cursor 2":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.Select2);
                    break;
                case "Clear Cursors...":
                    ptrChart.RemoveAnnotation(ptrChartArea.Name + "Cursor_1X");
                    ptrChart.RemoveAnnotation(ptrChartArea.Name + "Cursor_2X");
                    ptrChart.RemoveAnnotation(ptrChartArea.Name + "Cursor_1Y");
                    ptrChart.RemoveAnnotation(ptrChartArea.Name + "Cursor_2Y");
                    ptrChart.RemoveAnnotation(ptrChartArea.Name + "cursor1_Label");
                    ptrChart.RemoveAnnotation(ptrChartArea.Name + "cursor2_Label");
                    ptrChartData.Cursor1.X = double.NaN;
                    ptrChartData.Cursor1.Y = double.NaN;
                    ptrChartData.PositionChangedCallback(ptrChart, ptrChartData.Cursor1);
                    ptrChartData.Cursor2.X = double.NaN;
                    ptrChartData.Cursor2.Y = double.NaN;
                    ptrChartData.PositionChangedCallback(ptrChart, ptrChartData.Cursor2);
                    break;
                case "Zoom Window":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.Zoom);
                    break;
                case "Zoom XAxis":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.ZoomX);
                    break;
                case "Zoom YAxis":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.ZoomY);
                    break;
                case "Pan":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.Pan);
                    break;
                case "Zoom Out":
                    {
                        WindowMessagesNativeMethods.SuspendDrawing(ptrChart);
                        ptrChartArea.AxisX.ScaleView.ZoomReset();
                        ptrChartArea.AxisX2.ScaleView.ZoomReset();
                        ptrChartArea.AxisY.ScaleView.ZoomReset();
                        ptrChartArea.AxisY2.ScaleView.ZoomReset();
                        WindowMessagesNativeMethods.ResumeDrawing(ptrChart);
                        ChartTool[ptrChart].ZoomChangedCallback?.Invoke(ptrChart);
                    }
                    break;
                case "Zoom Dialog...":
                    {
                        using (MSChartExtensionZoomDialog dlg = new Charting.MSChartExtensionZoomDialog(ptrChartArea))
                        {
                            if (dlg.ShowDialog() == DialogResult.OK)
                                ChartTool[ptrChart].ZoomChangedCallback?.Invoke(ptrChart);
                        }
                    }
                    break;
                case "Clear":
                    ptrChartArea.Clear();
                    break;
                case "Settings...":
                    using (ConfigurationDialog dialog = new ConfigurationDialog(ptrChart, ptrChartData.Option))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            ptrChartData.Option = dialog.Option;

                            //Settings Changed. Update Chart to reflect settings.
                            foreach (ChartArea area in ptrChart.ChartAreas)
                            {
                                //Update Cursor 1
                                Annotation x = ptrChart.Annotations.FindByName(area.Name + "Cursor_1X");
                                if (x != null)
                                {
                                    x.LineDashStyle = ptrChartData.Option.Cursor1DashStyle;
                                    x.LineColor = ptrChartData.Option.Cursor1Color;
                                    x.LineWidth = ptrChartData.Option.Cursor1LineWidth;
                                }
                                Annotation y = ptrChart.Annotations.FindByName(area.Name + "Cursor_1Y");
                                if (y != null)
                                {
                                    y.LineDashStyle = ptrChartData.Option.Cursor1DashStyle;
                                    y.LineColor = ptrChartData.Option.Cursor1Color;
                                    y.LineWidth = ptrChartData.Option.Cursor1LineWidth;
                                }
                                Annotation label = ptrChart.Annotations.FindByName(area.Name + "cursor1_Label");
                                if (label != null)
                                {
                                    label.ForeColor = ptrChartData.Option.Cursor1Color;
                                }

                                //Update Cursor 2
                                x = ptrChart.Annotations.FindByName(area.Name + "Cursor_2X");
                                if (x != null)
                                {
                                    x.LineDashStyle = ptrChartData.Option.Cursor2DashStyle;
                                    x.LineColor = ptrChartData.Option.Cursor2Color;
                                    x.LineWidth = ptrChartData.Option.Cursor2LineWidth;
                                }
                                y = ptrChart.Annotations.FindByName(area.Name + "Cursor_2Y");
                                if (y != null)
                                {
                                    y.LineDashStyle = ptrChartData.Option.Cursor2DashStyle;
                                    y.LineColor = ptrChartData.Option.Cursor2Color;
                                    y.LineWidth = ptrChartData.Option.Cursor2LineWidth;
                                }
                                label = ptrChart.Annotations.FindByName(area.Name + "cursor2_Label");
                                if (label != null)
                                {
                                    label.ForeColor = ptrChartData.Option.Cursor2Color;
                                }

                            }
                            if (ptrChartData.Option.Theme != null) ptrChart.AssignTheme(ptrChartData.Option.Theme);
                            ptrChart.Invalidate();
                        }
                    }
                    break;
                case "About...":
                    using (AboutDialog dlg = new AboutDialog("MSChart Extension"))
                    {
                        dlg.FacebookLink = "https://www.facebook.com/CodeArtEngineering";
                        dlg.WebsiteLink = "http://www.codearteng.com/2016/08/mschart-extension-v2.html";
                        dlg.GitHubLink = "https://github.com/Code-Artist/MSChartExtension";
                        dlg.ShowDialog();
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

        #region [ (PRIVATE) Chart Control State + Events ]

        private static readonly Dictionary<Chart, ChartData> ChartTool = new Dictionary<Chart, ChartData>();

        /// <summary>
        /// Programmatically Set Chart Tool Control state
        /// </summary>
        /// <param name="sender">Source Chart</param>
        /// <param name="state">Control State</param>
        public static void SetChartControlState(Chart sender, MSChartExtensionToolState state)
        {
            ChartTool[(Chart)sender].ToolState = state;
            UpdateChartControlState(sender);
        }

        private static void UpdateChartControlState(Chart sender)
        {
            ChartArea activeChartArea = ChartTool[sender].ActiveChartArea;
            if (activeChartArea != null)
            {
                activeChartArea.CursorX.IsUserEnabled = false;
                activeChartArea.CursorY.IsUserEnabled = false;
                if (IsChartAreaEmpty(sender, activeChartArea))
                {
                    sender.Cursor = Cursors.Arrow;
                    return;
                }
            }

            ChartData ptrChartData = ChartTool[(Chart)sender];
            if (ptrChartData.Enabled == false)
            {
                sender.Cursor = Cursors.Arrow;
                return;
            }

            switch (ptrChartData.ToolState)
            {
                case MSChartExtensionToolState.Select:
                case MSChartExtensionToolState.Select2:
                    sender.Cursor = Cursors.Cross;
                    break;
                case MSChartExtensionToolState.Zoom:
                case MSChartExtensionToolState.ZoomX:
                case MSChartExtensionToolState.ZoomY:
                    sender.Cursor = Cursors.Cross;
                    break;
                case MSChartExtensionToolState.Pan:
                    sender.Cursor = Cursors.Hand;
                    break;
            }

        }

        #endregion

        #region [ (PRIVATE) Chart - Mouse Events ]

        private static bool MouseDowned;
        private static double X2Start, Y2Start;

        private static ChartArea ChartAreaHitTest(object sender, Point cursorPos)
        {
            Chart ptrChart = (Chart)sender;
            //Chart Area Hit Test
            double xPos = (double)(cursorPos.X * 100) / ptrChart.Width;
            double yPos = (double)(cursorPos.Y * 100) / ptrChart.Height;
            foreach (ChartArea cArea in ptrChart.ChartAreas)
            {
                ElementPosition ePos = cArea.Position;
                if ((xPos < ePos.X) || (xPos > ePos.Right)) continue; //Cursor not inside chart area.
                if ((yPos < ePos.Y) || (yPos > ePos.Bottom)) continue;
                return cArea;
            }
            return null;
        }

        private static void ChartControl_MouseWheel(object sender, MouseEventArgs e)
        {
            Chart ptrChart = sender as Chart;
            ChartData ptrChartData = ChartTool[ptrChart];
            ChartArea ptrChartArea = ptrChartData.ActiveChartArea;

            if (ptrChartArea == null) return;

            if (Form.ModifierKeys == (Keys.Alt | Keys.Control))
            {
                //Mouse Wheel Zoom: X Axis
                ScaleViewZoom(ptrChartArea.AxisX, e.Delta);
                ScaleViewZoom(ptrChartArea.AxisX2, e.Delta);
            }
            else if (Form.ModifierKeys == Keys.Shift)
            {
                //PAN Along Axis X
                if (ptrChartArea.AxisY.ScaleView.IsZoomed)
                {
                    ScaleViewScroll(ptrChartArea.AxisY2, e.Delta);
                    ScaleViewScroll(ptrChartArea.AxisY, e.Delta);
                }
            }
            else if (Form.ModifierKeys == Keys.Control)
            {
                //Mouse Wheel Zoom: X and Y
                ScaleViewZoom(ptrChartArea.AxisX, e.Delta);
                ScaleViewZoom(ptrChartArea.AxisX2, e.Delta);
                ScaleViewZoom(ptrChartArea.AxisY, e.Delta);
                ScaleViewZoom(ptrChartArea.AxisY2, e.Delta);
            }
            else
            {
                if (ptrChartArea.AxisX.ScaleView.IsZoomed)
                {
                    ScaleViewScroll(ptrChartArea.AxisX, e.Delta);
                    ScaleViewScroll(ptrChartArea.AxisX2, e.Delta);
                }

            }
            ptrChartData.ZoomChangedCallback?.Invoke(ptrChart);
        }

        private static void ScaleViewScroll(Axis ptrAxis, int delta)
        {
            AxisScaleView ptrView = ptrAxis.ScaleView;
            double deltaPos = (ptrView.ViewMaximum - ptrView.ViewMinimum) * 0.10;
            if (delta < 0) deltaPos = -deltaPos;
            ptrView.Scroll(ptrView.Position + deltaPos);
        }

        private static void ScaleViewZoom(Axis ptrAxis, int delta)
        {
            AxisScaleView ptrView = ptrAxis.ScaleView;
            double deltaPos = (ptrView.ViewMaximum - ptrView.ViewMinimum) * 0.10;
            if (delta < 0) deltaPos = -deltaPos;
            double newStart = ptrView.ViewMinimum + deltaPos;
            if (newStart < ptrAxis.Minimum) newStart = ptrAxis.Minimum;
            double newEnd = ptrView.ViewMaximum - deltaPos;
            if (newEnd > ptrAxis.Maximum) newEnd = ptrAxis.Maximum;
            ptrView.Zoom(newStart, newEnd);
        }

        private static string FormatCursorValue(double value, ChartValueType valueType, string NumberLabelFormat)
        {
            switch (valueType)
            {
                case ChartValueType.Auto: return value.ToString();
                case ChartValueType.Double:
                case ChartValueType.Single:
                case ChartValueType.Int32:
                case ChartValueType.Int64:
                case ChartValueType.UInt32:
                case ChartValueType.UInt64:
                    return value.ToString(NumberLabelFormat);
                case ChartValueType.String:
                    return value.ToString(); //Not Supported as X Value always 0 if string label is used.
                case ChartValueType.DateTime:
                case ChartValueType.Date:
                case ChartValueType.Time:
                case ChartValueType.DateTimeOffset:
                    return DateTime.FromOADate(value).ToString();
            }
            return value.ToString();
        }

        private static bool IsChartAreaEmpty(Chart chart, ChartArea chartArea)
        {
            if (chartArea == null) return true;
            return (chart.Series.Where(x => x.ChartArea == chartArea.Name).Where(x => x.Points.Count > 0).Count() == 0);
        }

        private static void ChartControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Chart ptrChart = (Chart)sender;
            ChartData ptrChartData = ChartTool[ptrChart];
            ChartArea ptrChartArea = ChartTool[ptrChart].ActiveChartArea;

            //Check if all series of this chart have no data
            if (IsChartAreaEmpty(ptrChart, ptrChartArea)) { UpdateChartControlState(ptrChart); return; }

            if (ptrChartArea == null) return;
            if (!ptrChartData.Enabled) return;
            MouseDowned = true;

            //NOTE: Clicking on the chart in selection mode will draw a cross whether or not the following
            //  code is run (since Cursor.IsUserEnabled is true)

            // We must set the selection start because it doesn't seem to get
            //    reset automatically (remove the next two lines and zoom a few times to see)
            Point startAndEndPt = e.Location;
            const bool roundToBoundary = true;
            ptrChartArea.CursorX.SetSelectionPixelPosition(startAndEndPt, startAndEndPt, roundToBoundary);
            ptrChartArea.CursorY.SetSelectionPixelPosition(startAndEndPt, startAndEndPt, roundToBoundary);

            X2Start = ptrChartArea.AxisX2.PixelPositionToValue(e.Location.X);
            Y2Start = ptrChartArea.AxisY2.PixelPositionToValue(e.Location.Y);

            if (ptrChartData.ToolState == MSChartExtensionToolState.Select)
            {
                Color cursorColor = ptrChartData.Option.Cursor1Color;
                ChartDashStyle cursorDashStyle = ptrChartData.Option.Cursor1DashStyle;
                int lineWidth = ptrChartData.Option.Cursor1LineWidth;
                Series ptrSeries = ptrChartData.Cursor1.SelectedChartSeries;
                if (ptrSeries == null) ptrSeries = ptrChartData.Cursor1.SelectedChartSeries = ptrChart.Series.First(x => x.ChartArea == ptrChartArea.Name);
                if (ptrSeries != null)
                {
                    Axis ptrXAxis = ptrSeries.XAxisType == AxisType.Primary ? ptrChartArea.AxisX : ptrChartArea.AxisX2;
                    Axis ptrYAxis = ptrSeries.YAxisType == AxisType.Primary ? ptrChartArea.AxisY : ptrChartArea.AxisY2;
                    double XStart = ptrXAxis.PixelPositionToValue(e.Location.X);
                    double YStart = ptrYAxis.PixelPositionToValue(e.Location.Y);

                    //Sanity check - make sure XStart and YStart within limit
                    if (!SanityCheck(ptrXAxis, XStart)) return;
                    if (!SanityCheck(ptrYAxis, YStart)) return;

                    if (ptrChartArea.ChartAreaBoundaryTest(ptrXAxis, ptrYAxis, XStart, YStart))
                    {
                        if (ptrChartData.Option.SnapCursorToData) SnapToNearestData(ptrChart, ptrSeries, ptrXAxis, ptrYAxis, e, ref XStart, ref YStart);

                        RemoveAnnotation(ptrChart, ptrChartArea.Name + "cursor1_Label");

                        DrawVerticalLine(ptrChart, XStart, cursorColor, ptrChartArea.Name + "Cursor_1X", lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.XAxisType);
                        DrawHorizontalLine(ptrChart, YStart, cursorColor, ptrChartArea.Name + "Cursor_1Y", lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.YAxisType);
                        ptrChartData.Cursor1.X = XStart;
                        ptrChartData.Cursor1.Y = YStart;
                        ptrChartData.Cursor1.XFormattedString = FormatCursorValue(XStart, ptrSeries.XValueType, ptrChartData.Option.CursorLabelStringFormat);
                        ptrChartData.Cursor1.YFormattedString = FormatCursorValue(YStart, ptrSeries.YValueType, ptrChartData.Option.CursorLabelStringFormat);
                        ptrChartData.Cursor1.ChartArea = ptrChartArea;

                        if (ptrChartData.Option.ShowCursorValue)
                        {
                            //Add Cursor Value : X, Y
                            string cursorValue = ptrChartData.Cursor1.XFormattedString + "," + ptrChartData.Cursor1.YFormattedString;
                            AddText(ptrChart, cursorValue, XStart, YStart, cursorColor, ptrChartArea.Name + "cursor1_Label", TextStyle.Default, ptrChartArea, ptrSeries.XAxisType, ptrSeries.YAxisType);
                        }

                        ptrChartData.PositionChangedCallback?.Invoke(ptrChart, ptrChartData.Cursor1.Clone() as ChartCursor);
                    }
                }
            }
            else if (ptrChartData.ToolState == MSChartExtensionToolState.Select2)
            {
                Color cursorColor = ptrChartData.Option.Cursor2Color;
                ChartDashStyle cursorDashStyle = ptrChartData.Option.Cursor2DashStyle;
                int lineWidth = ptrChartData.Option.Cursor2LineWidth;
                Series ptrSeries = ptrChartData.Cursor2.SelectedChartSeries;
                if (ptrSeries == null) ptrSeries = ptrChartData.Cursor2.SelectedChartSeries = ptrChart.Series.First(x => x.ChartArea == ptrChartArea.Name);
                if (ptrSeries != null)
                {
                    Axis ptrXAxis = ptrSeries.XAxisType == AxisType.Primary ? ptrChartArea.AxisX : ptrChartArea.AxisX2;
                    Axis ptrYAxis = ptrSeries.YAxisType == AxisType.Primary ? ptrChartArea.AxisY : ptrChartArea.AxisY2;
                    double XStart = ptrXAxis.PixelPositionToValue(e.Location.X);
                    double YStart = ptrYAxis.PixelPositionToValue(e.Location.Y);

                    //Sanity check - make sure XStart and YStart within limit
                    if (!SanityCheck(ptrXAxis, XStart)) return;
                    if (!SanityCheck(ptrYAxis, YStart)) return;

                    if (ptrChartArea.ChartAreaBoundaryTest(ptrXAxis, ptrYAxis, XStart, YStart))
                    {
                        if (ptrChartData.Option.SnapCursorToData) SnapToNearestData(ptrChart, ptrSeries, ptrXAxis, ptrYAxis, e, ref XStart, ref YStart);

                        RemoveAnnotation(ptrChart, ptrChartArea.Name + "cursor2_Label");

                        DrawVerticalLine(ptrChart, XStart, cursorColor, ptrChartArea.Name + "Cursor_2X", lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.XAxisType);
                        DrawHorizontalLine(ptrChart, YStart, cursorColor, ptrChartArea.Name + "Cursor_2Y", lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.YAxisType);
                        ptrChartData.Cursor2.X = XStart;
                        ptrChartData.Cursor2.Y = YStart;
                        ptrChartData.Cursor2.XFormattedString = FormatCursorValue(XStart, ptrSeries.XValueType, ptrChartData.Option.CursorLabelStringFormat);
                        ptrChartData.Cursor2.YFormattedString = FormatCursorValue(YStart, ptrSeries.YValueType, ptrChartData.Option.CursorLabelStringFormat);
                        ptrChartData.Cursor2.ChartArea = ptrChartArea;

                        if (ptrChartData.Option.ShowCursorValue)
                        {
                            //Add Cursor Value : X, Y
                            string cursorValue = ptrChartData.Cursor2.XFormattedString + "," + ptrChartData.Cursor2.YFormattedString;
                            AddText(ptrChart, cursorValue, XStart, YStart, cursorColor, ptrChartArea.Name + "cursor2_Label", TextStyle.Default, ptrChartArea, ptrSeries.XAxisType, ptrSeries.YAxisType);
                        }

                        ptrChartData.PositionChangedCallback?.Invoke(ptrChart, ptrChartData.Cursor2.Clone() as ChartCursor);
                    }
                }
            }
        }

        private static bool SanityCheck(Axis axis, double value)
        {
            if ((value >= axis.Minimum) && (value <= axis.Maximum)) return true;
            return false;
        }


        private static void ChartControl_MouseMove(object sender, MouseEventArgs e)
        {
            Chart ptrChart = (Chart)sender;
            ChartData ptrChartData = ChartTool[ptrChart];

            #region [ ChartArea - Hit Test ]

            if (!MouseDowned)
            {
                ChartArea hitArea = ChartAreaHitTest(sender, e.Location);
                if (hitArea != ptrChartData.ActiveChartArea)
                {
                    //Mouse crossed border, verify and update 'passport'.
                    ptrChartData.ActiveChartArea = hitArea;
                    Debug.WriteLine("Active Chart Area: " + (ptrChartData.ActiveChartArea == null ? "NULL" : ptrChartData.ActiveChartArea.Name));

                    if (ptrChartData.ActiveChartArea != null)
                    {
                        if (!ptrChartData.SupportedChartArea.Contains(ptrChartData.ActiveChartArea))
                        {
                            Debug.WriteLine(ptrChartData.ActiveChartArea.Name + ": Zoom and Pan Control not supported.");
                            ptrChartData.Enabled = false;
                            UpdateChartControlState((Chart)sender);
                            return;
                        }
                        else if ((hitArea.AxisX.IsLogarithmic || hitArea.AxisY.IsLogarithmic ||
                            hitArea.AxisX2.IsLogarithmic || hitArea.AxisY2.IsLogarithmic))
                        {
                            //Disable Zoom and Pan Controls if any of the axis is LOG axis.
                            Debug.WriteLine(ptrChartData.ActiveChartArea.Name + ": Zoom and Pan Control not supported for Log Axis.");
                            ptrChartData.Enabled = false;
                            UpdateChartControlState((Chart)sender);
                            return;
                        }
                        else
                        {
                            ptrChartData.Enabled = true;
                            UpdateChartControlState((Chart)sender);
                        }
                    }
                    else ptrChartData.Enabled = false;
                }
            }

            #endregion

            ChartArea ptrChartArea = ptrChartData.ActiveChartArea;

            if (ptrChartArea == null)
            {
                ptrChartData.CursorMovedCallback?.Invoke(ptrChart, new ChartCursor() { X = Double.NaN, Y = Double.NaN });
                ptrChartData.Enabled = false;
                return;
            }

            //Check if all series of this chart have no data
            if (IsChartAreaEmpty(ptrChart, ptrChartArea)) { UpdateChartControlState(ptrChart); return; }

            double selX, selY, selX2, selY2;
            selX = selY = selX2 = selY2 = 0;
            try
            {
                selX = ptrChartArea.AxisX.PixelPositionToValue(e.Location.X);
                selY = ptrChartArea.AxisY.PixelPositionToValue(e.Location.Y);
                selX2 = ptrChartArea.AxisX2.PixelPositionToValue(e.Location.X);
                selY2 = ptrChartArea.AxisY2.PixelPositionToValue(e.Location.Y);

                //Debug.WriteLine(String.Format("Selection: {0}, {1}, {2}, {3}", selX, selX2, selY, selY2));

                if (!ptrChartArea.ChartAreaBoundaryTest(ptrChartArea.AxisX, ptrChartArea.AxisY, selX, selY)) return; //Pointer outside boundary.

                ChartValueType xValueType = ptrChart.Series.Where(x => x.ChartArea == ptrChartArea.Name).Where(x => x.XAxisType == AxisType.Primary).First().XValueType;
                ChartValueType yValueType = ptrChart.Series.Where(x => x.ChartArea == ptrChartArea.Name).Where(x => x.XAxisType == AxisType.Primary).First().XValueType;
                ChartTool[ptrChart].CursorMovedCallback?.Invoke(ptrChart,
                    new ChartCursor()
                    {
                        X = selX,
                        Y = selY,
                        ChartArea = ptrChartArea,
                        XFormattedString = FormatCursorValue(selX, xValueType, ptrChartData.Option.CursorLabelStringFormat),
                        YFormattedString = FormatCursorValue(selY, yValueType, ptrChartData.Option.CursorLabelStringFormat)
                    });
            }
            catch (Exception)
            {
                //Handle exception when scrolled out of range.
                //Set coordinate to NaN
                ChartTool[ptrChart].CursorMovedCallback?.Invoke(ptrChart, new ChartCursor() { X = Double.NaN, Y = Double.NaN });
                return;
            }

            switch (ChartTool[ptrChart].ToolState)
            {
                case MSChartExtensionToolState.Zoom:
                    #region [ Zoom Control ]
                    if (MouseDowned)
                    {
                        ptrChartArea.CursorX.SelectionEnd = selX;
                        ptrChartArea.CursorY.SelectionEnd = selY;
                    }
                    #endregion
                    break;

                case MSChartExtensionToolState.ZoomX:
                    if (MouseDowned)
                    {
                        ptrChartArea.CursorX.SelectionEnd = selX;
                    }
                    break;

                case MSChartExtensionToolState.ZoomY:
                    if (MouseDowned)
                    {
                        ptrChartArea.CursorY.SelectionEnd = selY;
                    }
                    break;

                case MSChartExtensionToolState.Pan:
                    #region [ Pan Control ]
                    if (MouseDowned)
                    {
                        //Pan Move - Valid only if view is zoomed
                        if (ptrChartArea.AxisX.ScaleView.IsZoomed ||
                            ptrChartArea.AxisY.ScaleView.IsZoomed)
                        {
                            double dx = -selX + ptrChartArea.CursorX.SelectionStart;
                            double dy = -selY + ptrChartArea.CursorY.SelectionStart;
                            double dx2 = -selX2 + X2Start;
                            double dy2 = -selY2 + Y2Start;

                            double newX = ptrChartArea.AxisX.ScaleView.Position + dx;
                            double newY = ptrChartArea.AxisY.ScaleView.Position + dy;
                            double newX2 = ptrChartArea.AxisX2.ScaleView.Position + dx2;
                            double newY2 = ptrChartArea.AxisY2.ScaleView.Position + dy2;

                            ptrChartArea.AxisX.ScaleView.Scroll(newX);
                            ptrChartArea.AxisY.ScaleView.Scroll(newY);
                            ptrChartArea.AxisX2.ScaleView.Scroll(newX2);
                            ptrChartArea.AxisY2.ScaleView.Scroll(newY2);
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
            ChartData ptrChartData = ChartTool[ptrChart];
            ChartArea ptrChartArea = ChartTool[ptrChart].ActiveChartArea;
            if (ptrChartArea == null) return;
            MSChartExtensionToolState state = ptrChartData.ToolState;
            switch (state)
            {
                case MSChartExtensionToolState.Zoom:
                case MSChartExtensionToolState.ZoomX:
                case MSChartExtensionToolState.ZoomY:

                    //Zoom area (Pixel)
                    double XStart = ptrChartArea.CursorX.SelectionStart;
                    double XEnd = ptrChartArea.CursorX.SelectionEnd;
                    double YStart = ptrChartArea.CursorY.SelectionStart;
                    double YEnd = ptrChartArea.CursorY.SelectionEnd;

                    if ((XStart == XEnd) && (YStart == YEnd)) return;

                    //Zoom area for X Axis
                    ChartOption option = ptrChartData.Option;
                    double left = Math.Min(XStart, XEnd);
                    double right = Math.Max(XStart, XEnd);
                    if (option.XAxisPrecision > 0)
                    {
                        //Precision Correction
                        left = Math.Round(left, option.XAxisPrecision);
                        right = Math.Round(right, option.XAxisPrecision);
                    }
                    double XMin = ptrChartArea.AxisX.ValueToPosition(left);
                    double XMax = ptrChartArea.AxisX.ValueToPosition(right);
                    //Zoom area for Y Axis
                    double bottom = Math.Min(YStart, YEnd);
                    double top = Math.Max(YStart, YEnd);
                    if (option.YAxisPrecision > 0)
                    {
                        //Precision Correction
                        top = Math.Round(top, option.YAxisPrecision);
                        bottom = Math.Round(bottom, option.YAxisPrecision);
                    }
                    double YMin = ptrChartArea.AxisY.ValueToPosition(bottom);
                    double YMax = ptrChartArea.AxisY.ValueToPosition(top);

                    // NOTE: left <= right, even if Axis.IsReversed
                    //X-Axis
                    if ((state == MSChartExtensionToolState.Zoom) || (state == MSChartExtensionToolState.ZoomX))
                    {
                        if ((XMin == XMax) || (left < ptrChartArea.AxisX.Minimum) || (right > ptrChartArea.AxisX.Maximum))
                        {
                            //Handle conditions which caused exception on zoom operation
                            ptrChartArea.CursorX.SetSelectionPosition(0, 0);
                            ptrChartArea.CursorY.SetSelectionPosition(0, 0);
                            return;
                        }

                        ptrChartArea.AxisX.ScaleView.Zoom(left, right);
                        ptrChartArea.AxisX2.ScaleView.Zoom(
                            ptrChartArea.AxisX2.PositionToValue(XMin),
                            ptrChartArea.AxisX2.PositionToValue(XMax));
                    }
                    //Y-Axis
                    if ((state == MSChartExtensionToolState.Zoom) || (state == MSChartExtensionToolState.ZoomY))
                    {
                        if ((YMin == YMax) || (bottom < ptrChartArea.AxisY.Minimum) || (top > ptrChartArea.AxisY.Maximum))
                        {
                            //Handle conditions which caused exception on zoom operation
                            ptrChartArea.CursorX.SetSelectionPosition(0, 0);
                            ptrChartArea.CursorY.SetSelectionPosition(0, 0);
                            return;
                        }

                        ptrChartArea.AxisY.ScaleView.Zoom(bottom, top);
                        ptrChartArea.AxisY2.ScaleView.Zoom(
                            ptrChartArea.AxisY2.PositionToValue(YMin),
                            ptrChartArea.AxisY2.PositionToValue(YMax));
                    }

                    //Clear selection (the following seem to be equivalent)
                    ptrChartArea.CursorX.SetSelectionPosition(0, 0);
                    ptrChartArea.CursorY.SetSelectionPosition(0, 0);


                    ptrChartData.ZoomChangedCallback?.Invoke(ptrChart);
                    break;

                case MSChartExtensionToolState.Pan:
                    ptrChartData.ZoomChangedCallback?.Invoke(ptrChart);
                    break;
            }
        }

        /// <summary>
        /// Snap Cursor to nearest data point
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="series">Selected Series</param>
        /// <param name="xAxis">Selected X-Axis</param>
        /// <param name="yAxis">Selected Y-Axis</param>
        /// <param name="e">Mouse Down event argument</param>
        /// <param name="XResult">Output: X Value</param>
        /// <param name="YResult">OUptut: Y Value</param>
        private static void SnapToNearestData(Chart chart, Series series, Axis xAxis, Axis yAxis, MouseEventArgs e,
            ref double XResult, ref double YResult)
        {
            if (series.Points.Count == 0) return;
            XResult = YResult = Double.MaxValue;

            ChartData ptrChartData = ChartTool[chart];
            ChartArea ptrChartArea = ChartTool[chart].ActiveChartArea;

            double xMin = xAxis.Minimum;
            double xMax = xAxis.Maximum;

            //Mouser Pointer Value
            double xTarget = xAxis.PixelPositionToValue(e.Location.X);
            double yTarget = yAxis.PixelPositionToValue(e.Location.Y);

            //Sort data point assending by X-Values
            DataPoint[] datas = series.Points.OrderBy(x => x.XValue).ToArray();

            //Get nearest data points
            int iLower, iUpper;
            iUpper = iLower = 0;
            int estIndex = (int)(datas.Length * (xTarget - xMin) / (xMax - xMin));

            //iLower --> XValue < xTarget
            //iUpper --> XValue > xTarget
            if (datas[estIndex].XValue > xTarget)
            {
                //Serch Down
                for (int x = estIndex; x > 0; x--)
                {
                    if (datas[x].XValue <= xTarget)
                    {
                        iLower = x;
                        iUpper = x + 1;
                        break;
                    }
                }
            }
            else //datas[estIndex].XValue < xTarget
            {
                //Search Up
                for (int x = estIndex; x < datas.Length; x++)
                {
                    if (datas[x].XValue >= xTarget)
                    {
                        iUpper = x;
                        iLower = x - 1;
                        break;
                    }
                }
            }

            //Search out of range.... iLower = iUpper

            //Distance = x^2 + y^2
            double distLower = Math.Pow(datas[iLower].XValue - xTarget, 2) + Math.Pow(datas[iLower].YValues[0] - yTarget, 2);
            double distUpper = Math.Pow(datas[iUpper].XValue - xTarget, 2) + Math.Pow(datas[iUpper].YValues[0] - yTarget, 2);

            if (distLower > distUpper)
            {
                XResult = datas[iUpper].XValue;
                YResult = datas[iUpper].YValues[0];
            }
            else
            {
                XResult = datas[iLower].XValue;
                YResult = datas[iLower].YValues[0];
            }
        }

        #endregion

        #region [ Clear Data ]

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
        /// Clear all series and Annotations
        /// </summary>
        /// <param name="sender"></param>
        public static void Clear(this Chart sender)
        {
            sender.SuspendLayout();
            foreach (Series s in sender.Series)
            {
                s.ClearPoints();
            }
            sender.Annotations.Clear();
            sender.ResumeLayout();
        }
        /// <summary>
        /// Clear all series and Annotations in selected chart area
        /// </summary>
        /// <param name="sender"></param>
        public static void Clear(this ChartArea sender)
        {
            Chart ptrChart = null;

            //Reverse search for Chart  
            foreach (KeyValuePair<Chart, ChartData> c in ChartTool)
            {
                if (c.Key.ChartAreas.Contains(sender))
                {
                    ptrChart = c.Key;
                    break;
                }
            }
            if (ptrChart == null) return; //null check

            foreach (Series s in ptrChart.Series)
            {
                if (s.ChartArea.Equals(sender.Name)) s.ClearPoints();
            }

            for (int x = 0; x < ptrChart.Annotations.Count; x++)
            {
                if (ptrChart.Annotations[x].ClipToChartArea.Equals(sender.Name))
                {
                    ptrChart.Annotations.RemoveAt(x--);
                }
            }
        }

        #endregion

        #region [ Annotations ]

        /// <summary>
        /// Remove Chart Annotation by Name
        /// </summary>
        /// <param name="sender">Source Chart.</param>
        /// <param name="annotationName">Annotation Name</param>
        public static void RemoveAnnotation(this Chart sender, string annotationName)
        {
            Annotation ptrAnnotation = sender.Annotations.FindByName(annotationName);
            if (ptrAnnotation != null) sender.Annotations.Remove(ptrAnnotation);
        }

        /// <summary>
        /// Draw a horizontal line on chart.
        /// </summary>
        /// <param name="sender">Source Chart.</param>
        /// <param name="y">YAxis value.</param>
        /// <param name="lineColor">Line color.</param>
        /// <param name="name">Annotation name.</param>
        /// <param name="lineWidth">Line width</param>
        /// <param name="lineStyle">Line style</param>
        /// <param name="chartArea">Target ChartArea where annotation should be displayed. Default to first ChartArea if not defined.</param>
        /// <param name="axis">Select Primary or Secondary Axis. Default value is Primary Axis.</param>
        public static Annotation DrawHorizontalLine(this Chart sender, double y,
            Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid, ChartArea chartArea = null, AxisType axis = AxisType.Primary)
        {
            HorizontalLineAnnotation horzLine = null;
            if (!string.IsNullOrEmpty(name)) horzLine = sender.Annotations.FindByName(name) as HorizontalLineAnnotation;

            if (horzLine == null)
            {
                horzLine = new HorizontalLineAnnotation();
                sender.Annotations.Add(horzLine);
                if (!string.IsNullOrEmpty(name)) horzLine.Name = name;
            }

            string chartAreaName = (chartArea == null) ? sender.ChartAreas[0].Name : chartArea.Name;
            horzLine.ClipToChartArea = chartAreaName;
            horzLine.AxisXName = chartAreaName + "\\rX";
            horzLine.YAxisName = chartAreaName + "\\rY";
            if (axis == AxisType.Secondary)
            {
                horzLine.AxisXName += "2";
                horzLine.YAxisName += "2";
            }

            horzLine.IsInfinitive = true;
            horzLine.IsSizeAlwaysRelative = false;

            horzLine.Y = y;
            horzLine.LineColor = lineColor;
            horzLine.LineWidth = lineWidth;
            horzLine.LineDashStyle = lineStyle;

            return horzLine;
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
        /// <param name="chartArea">Target ChartArea where annotation should be displayed. Default to first ChartArea if not defined.</param>
        /// <param name="axis">Select Primary or Secondary Axis. Default value is Primary Axis.</param>
        public static Annotation DrawVerticalLine(this Chart sender, double x,
            Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid, ChartArea chartArea = null, AxisType axis = AxisType.Primary)
        {
            VerticalLineAnnotation vertLine = null;
            if (!string.IsNullOrEmpty(name)) vertLine = sender.Annotations.FindByName(name) as VerticalLineAnnotation;

            if (vertLine == null)
            {
                vertLine = new VerticalLineAnnotation();
                sender.Annotations.Add(vertLine);
                if (!string.IsNullOrEmpty(name)) vertLine.Name = name;
            }

            string chartAreaName = (chartArea == null) ? sender.ChartAreas[0].Name : chartArea.Name;
            vertLine.ClipToChartArea = chartAreaName;
            vertLine.AxisXName = chartAreaName + "\\rX";
            vertLine.YAxisName = chartAreaName + "\\rY";
            if (axis == AxisType.Secondary)
            {
                vertLine.AxisXName += "2";
                vertLine.YAxisName += "2";
            }

            vertLine.IsInfinitive = true;
            vertLine.IsSizeAlwaysRelative = false;

            vertLine.X = x;
            vertLine.LineColor = lineColor;
            vertLine.LineWidth = lineWidth;
            vertLine.LineDashStyle = lineStyle;

            return vertLine;
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
        /// <param name="chartArea">Target ChartArea where annotation should be displayed. Default to first ChartArea if not defined.</param>
        /// <param name="xAxisType">Select Primary or Secondary for X Axis. Default value is Primary Axis.</param>
        /// <param name="yAxisType">Select Primary or Secondary for Y Axis. Default value is Primary Axis.</param>
        public static Annotation DrawRectangle(this Chart sender, double x, double y,
            double width, double height,
            Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid, ChartArea chartArea = null,
            AxisType xAxisType = AxisType.Primary, AxisType yAxisType = AxisType.Primary)
        {
            RectangleAnnotation rect = new RectangleAnnotation();
            ChartArea ptrChartArea = chartArea ?? sender.ChartAreas[0];
            string chartAreaName = ptrChartArea.Name;
            rect.ClipToChartArea = chartAreaName;
            rect.AxisXName = chartAreaName + "\\rX";
            rect.YAxisName = chartAreaName + "\\rY";
            if (xAxisType == AxisType.Secondary) rect.AxisXName += "2";
            if (yAxisType == AxisType.Secondary) rect.YAxisName += "2";
            rect.BackColor = Drawing.Color.Transparent;
            rect.ForeColor = Drawing.Color.Transparent;
            rect.IsSizeAlwaysRelative = false;

            rect.LineColor = lineColor;
            rect.LineWidth = lineWidth;
            rect.LineDashStyle = lineStyle;

            //Limit rectangle within chart area

            Axis ptrAxis = ptrChartArea.AxisX;
            if (x < ptrAxis.Minimum)
            {
                width -= (ptrAxis.Minimum - x);
                x = ptrAxis.Minimum;
            }
            else if (x > ptrAxis.Maximum)
            {
                width -= (x - ptrAxis.Maximum);
                x = ptrAxis.Maximum;
            }
            if ((x + width) > ptrAxis.Maximum) width = ptrAxis.Maximum - x;

            ptrAxis = ptrChartArea.AxisY;
            if (y < ptrAxis.Minimum)
            {
                height -= (ptrAxis.Minimum - y);
                y = ptrAxis.Minimum;
            }
            else if (y > ptrAxis.Maximum)
            {
                height -= (y - ptrAxis.Maximum);
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

            return rect;

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
        /// <param name="chartArea">Target ChartArea where annotation should be displayed. Default to first ChartArea if not defined.</param>
        /// <param name="xAxisType">Select Primary or Secondary for X Axis. Default value is Primary Axis.</param>
        /// <param name="yAxisType">Select Primary or Secondary for Y Axis. Default value is Primary Axis.</param>
        public static Annotation DrawLine(this Chart sender, double x0, double x1,
            double y0, double y1, Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid, ChartArea chartArea = null,
            AxisType xAxisType = AxisType.Primary, AxisType yAxisType = AxisType.Primary)
        {
            LineAnnotation line = new LineAnnotation();
            string chartAreaName = (chartArea == null) ? sender.ChartAreas[0].Name : chartArea.Name;
            line.ClipToChartArea = chartAreaName;
            line.AxisXName = chartAreaName + "\\rX";
            line.YAxisName = chartAreaName + "\\rY";
            if (xAxisType == AxisType.Secondary) line.AxisXName += "2";
            if (yAxisType == AxisType.Secondary) line.YAxisName += "2";
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

            return line;
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
        /// <param name="chartArea">Target ChartArea where annotation should be displayed. Default to first ChartArea if not defined.</param>
        /// <param name="xAxisType">Select Primary or Secondary for X Axis. Default value is Primary Axis.</param>
        /// <param name="yAxisType">Select Primary or Secondary for Y Axis. Default value is Primary Axis.</param>
        public static Annotation AddText(this Chart sender, string text,
            double x, double y,
            Drawing.Color textColor, string name = "",
            TextStyle textStyle = TextStyle.Default, ChartArea chartArea = null, AxisType xAxisType = AxisType.Primary, AxisType yAxisType = AxisType.Primary)
        {
            TextAnnotation textAnn = new TextAnnotation();
            string chartAreaName = (chartArea == null) ? sender.ChartAreas[0].Name : chartArea.Name;
            textAnn.ClipToChartArea = chartAreaName;
            textAnn.AxisXName = chartAreaName + "\\rX";
            textAnn.YAxisName = chartAreaName + "\\rY";
            if (xAxisType == AxisType.Secondary) textAnn.AxisXName += "2";
            if (yAxisType == AxisType.Secondary) textAnn.YAxisName += "2";
            textAnn.IsSizeAlwaysRelative = false;

            textAnn.Text = text;
            textAnn.ForeColor = textColor;
            textAnn.X = x;
            textAnn.Y = y;
            textAnn.TextStyle = textStyle;

            sender.Annotations.Add(textAnn);
            if (!string.IsNullOrEmpty(name)) textAnn.Name = name;

            return textAnn;
        }

        #endregion

        #region [ Chart Area - Boundaries ]

        /// <summary>
        /// Return the entire chart boundary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="xAxis">X Axis. Use Primary Axis if null</param>
        /// <param name="yAxis">Y Axis. Use Primary Axis if null</param>
        /// <returns></returns>
        public static RectangleF GetChartAreaBoundary(this ChartArea sender, Axis xAxis = null, Axis yAxis = null)
        {
            if (xAxis == null) xAxis = sender.AxisX;
            if (yAxis == null) yAxis = sender.AxisY;
            return GetChartAreaBoundary(xAxis, yAxis, visibleAreaOnly: false);
        }

        /// <summary>
        /// Return chart boundary for visible area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="xAxis">X Axis. Use Primary Axis if null</param>
        /// <param name="yAxis">Y Axis. Use Primary Axis if null</param>
        /// <returns></returns>
        public static RectangleF GetChartVisibleAreaBoundary(this ChartArea sender, Axis xAxis = null, Axis yAxis = null)
        {
            if (xAxis == null) xAxis = sender.AxisX;
            if (yAxis == null) yAxis = sender.AxisY;
            return GetChartAreaBoundary(xAxis, yAxis, visibleAreaOnly: true);
        }

        private static RectangleF GetChartAreaBoundary(Axis xAxis, Axis yAxis, bool visibleAreaOnly)
        {
            RectangleF result = new RectangleF();
            Axis ptrXAxis = xAxis;
            Axis ptrYAxis = yAxis;

            double xMin, xMax, yMin, yMax;
            if (visibleAreaOnly)
            {
                xMin = ptrXAxis.ScaleView.ViewMinimum;
                xMax = ptrXAxis.ScaleView.ViewMaximum;
                yMin = ptrYAxis.ScaleView.ViewMinimum;
                yMax = ptrYAxis.ScaleView.ViewMaximum;
            }
            else
            {
                xMin = ptrXAxis.Minimum;
                xMax = ptrXAxis.Maximum;
                yMin = ptrYAxis.Minimum;
                yMax = ptrYAxis.Maximum;
            }

            //Take into consideration reversed axis where [x/y]Min > [x/y]Max
            result.X = (float)(ptrXAxis.IsReversed ? xMax : xMin);
            result.Y = (float)(ptrYAxis.IsReversed ? yMin : yMax);
            result.Width = (float)Math.Abs(xMax - xMin);
            if (ptrXAxis.IsReversed) result.Width = -result.Width;
            result.Height = -(float)Math.Abs(yMax - yMin);
            if (ptrYAxis.IsReversed) result.Height = -result.Height;

            return result;
        }

        private static bool ChartAreaBoundaryTest(this ChartArea sender, Axis xAxis, Axis yAxis, double x, double y)
        {
            RectangleF area = GetChartVisibleAreaBoundary(sender, xAxis, yAxis);
            if ((x < area.Left) || (x > area.Right)) return false;
            if ((y < area.Bottom) || (y > area.Top)) return false;
            return true;
        }
        #endregion

        #region [ Utility Functions ]

        /// <summary>
        /// Check if any of the Chart Axes is zoomed
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsZoomed(this ChartArea sender)
        {
            foreach (Axis a in sender.Axes)
                if (a.ScaleView.IsZoomed) return true;
            return false;
        }

        #endregion
    }

}
