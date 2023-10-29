using EventHandlerSupport;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

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
        private const string Cursor1LabelName = "cursor1_Label";
        private const string Cursor1LabelBGName = "cursor1_Label_BG";
        private const string Cursor2LabelName = "cursor2_Label";
        private const string Cursor2LabelBGName = "cursor2_Label_BG";

        private const string Cursor1XName = "Cursor_1X";
        private const string Cursor1YName = "Cursor_1Y";
        private const string Cursor2XName = "Cursor_2X";
        private const string Cursor2YName = "Cursor_2Y";
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
        /// <param name="option">Additional user options <see cref="ChartOption"/></param>
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
                sender.Disposed += ChartDisposed;
                ptrChartData.Option = option ?? new ChartOption();
                ptrChartData.Backup();

                //Scan through series to identify valid ChartArea
                Chart ptrChart = sender;
                foreach (ChartArea cArea in ptrChart.ChartAreas)
                {
                    IEnumerable<Series> chartSeries = ptrChart.Series.Where(n => n.ChartArea == cArea.Name);
                    if (chartSeries.Count() == 0)
                    {
                        Trace.WriteLine(string.Format("WARNING: Chart {0}, Chart Area [{1}] does not contains any series.", sender.Name, cArea.Name));
                    }
                    else if (chartSeries.Where(n => UnsupportedChartType.Contains(n.ChartType)).Count() > 0)
                    {
                        Trace.WriteLine(string.Format("WARNING: Chart {0}, Chart Area [{1}] contains unsupported series.", sender.Name, cArea.Name));
                    }
                    else ptrChartData.SupportedChartArea.Add(cArea);
                }

                if (ptrChartData.SupportedChartArea.Count == 0)
                {
                    //No Supported Chart Area found, disable controls.
                    Trace.WriteLine("WARNING: Chart type not supported! Controls disabled!");
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
                ptrChart.KeyDown += ChartControl_KeyDown;
                ptrChart.PreviewKeyDown += ChartControl_PreviewKeyDown;
                ptrChart.AxisViewChanged += ChartControl_AxisViewChanged; //Event trigger when chart is scrolled
                ptrChart.SizeChanged += ChartControl_Resized;

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
                    ptrChartData.Option.Theme.AssignTheme(ptrChart);
                }

            }
        }

        private static void ChartControl_Resized(object sender, EventArgs e)
        {
            UpdateCursorLabelPosition(sender as Chart);
        }

        private static void UpdateCursorLabelPosition(Chart sender)
        {
            Chart ptrChart = sender as Chart;
            if (ptrChart == null) return;

            ChartData ptrChartData = ChartTool[ptrChart];
            if (ptrChartData == null) return;

            foreach (ChartArea ptrChartArea in ptrChart.ChartAreas)
            {
                TextAnnotation ptrCurosr1Label = ptrChart.Annotations.FindByName(ptrChartArea.Name + Cursor1LabelName) as TextAnnotation;
                if (ptrCurosr1Label != null)
                {
                    Annotation xCursor = ptrChart.Annotations.FindByName(ptrChartArea.Name + Cursor1XName);
                    Annotation yCursor = ptrChart.Annotations.FindByName(ptrChartArea.Name + Cursor1YName);
                    CheckAndUpdateTextAnnotationLocation(ptrChart, ptrCurosr1Label, xCursor.X, yCursor.Y);
                    AddTextBackground(ptrChart, ptrCurosr1Label.Name, ptrChartData.Option.Cursor1Color);
                }

                TextAnnotation ptrCursor2Label = ptrChart.Annotations.FindByName(ptrChartArea.Name + Cursor2LabelName) as TextAnnotation;
                if (ptrCursor2Label != null)
                {
                    Annotation xCursor = ptrChart.Annotations.FindByName(ptrChartArea.Name + Cursor2XName);
                    Annotation yCursor = ptrChart.Annotations.FindByName(ptrChartArea.Name + Cursor2YName);
                    CheckAndUpdateTextAnnotationLocation(ptrChart, ptrCursor2Label, xCursor.X, yCursor.Y);
                    AddTextBackground(ptrChart, ptrCursor2Label.Name, ptrChartData.Option.Cursor2Color);
                }
            }
        }

        //Auto clean up ChartTool when chart is disposed
        private static void ChartDisposed(object sender, EventArgs e)
        {
            Chart ptrChart = sender as Chart;
            ChartTool.Remove(ptrChart);
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
                ptrChart.MouseWheel -= ChartControl_MouseWheel;
                ptrChart.MouseDown -= ChartControl_MouseDown;
                ptrChart.MouseMove -= ChartControl_MouseMove;
                ptrChart.MouseUp -= ChartControl_MouseUp;
                ptrChart.KeyDown -= ChartControl_KeyDown;
                ptrChart.PreviewKeyDown -= ChartControl_PreviewKeyDown;
                ptrChart.AxisViewChanged -= ChartControl_AxisViewChanged;

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

        /// <summary>
        /// Get Chart option settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static ChartOption GetChartOption(this Chart sender) { return ChartTool[sender]?.Option; }

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

        private static void ZoomChangedCallback(Chart chart)
        {
            UpdateCursorLabelPosition(chart);
            ChartTool[chart].ZoomChangedCallback?.Invoke(chart);
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
                    ClearCursor1(ptrChartArea, ptrChartData);
                    ClearCursor2(ptrChartArea, ptrChartData);
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
                        ResetAxisIntervalForAllAxes(ptrChartArea);
                        ptrChartData.RepaintBufferedData();
                        WindowMessagesNativeMethods.ResumeDrawing(ptrChart);
                        ZoomChangedCallback(ptrChart);
                    }
                    break;
                case "Zoom Dialog...":
                    {
                        using (MSChartExtensionZoomDialog dlg = new Charting.MSChartExtensionZoomDialog(ptrChartArea))
                        {
                            if (dlg.ShowDialog() == DialogResult.OK)
                            {
                                ptrChartData.RepaintBufferedData();
                                ZoomChangedCallback(ptrChart);
                            }
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
                                Annotation x = ptrChart.Annotations.FindByName(area.Name + Cursor1XName);
                                if (x != null)
                                {
                                    x.LineDashStyle = ptrChartData.Option.Cursor1DashStyle;
                                    x.LineColor = ptrChartData.Option.Cursor1Color;
                                    x.LineWidth = ptrChartData.Option.Cursor1LineWidth;
                                }
                                Annotation y = ptrChart.Annotations.FindByName(area.Name + Cursor1YName);
                                if (y != null)
                                {
                                    y.LineDashStyle = ptrChartData.Option.Cursor1DashStyle;
                                    y.LineColor = ptrChartData.Option.Cursor1Color;
                                    y.LineWidth = ptrChartData.Option.Cursor1LineWidth;
                                }
                                Annotation label = ptrChart.Annotations.FindByName(area.Name + Cursor1LabelName);
                                if (label != null)
                                {
                                    label.ForeColor = ptrChartData.Option.Cursor1TextColor;
                                }
                                Annotation labelBg = ptrChart.Annotations.FindByName(area.Name + Cursor1LabelBGName);
                                if (labelBg != null)
                                {
                                    labelBg.BackColor = ptrChartData.Option.Cursor1Color;
                                }

                                //Update Cursor 2
                                x = ptrChart.Annotations.FindByName(area.Name + Cursor2XName);
                                if (x != null)
                                {
                                    x.LineDashStyle = ptrChartData.Option.Cursor2DashStyle;
                                    x.LineColor = ptrChartData.Option.Cursor2Color;
                                    x.LineWidth = ptrChartData.Option.Cursor2LineWidth;
                                }
                                y = ptrChart.Annotations.FindByName(area.Name + Cursor2YName);
                                if (y != null)
                                {
                                    y.LineDashStyle = ptrChartData.Option.Cursor2DashStyle;
                                    y.LineColor = ptrChartData.Option.Cursor2Color;
                                    y.LineWidth = ptrChartData.Option.Cursor2LineWidth;
                                }
                                label = ptrChart.Annotations.FindByName(area.Name + Cursor2LabelName);
                                if (label != null)
                                {
                                    label.ForeColor = ptrChartData.Option.Cursor2TextColor;
                                }
                                labelBg = ptrChart.Annotations.FindByName(area.Name + Cursor2LabelBGName);
                                if (labelBg != null)
                                {
                                    labelBg.BackColor = ptrChartData.Option.Cursor2Color;
                                }

                            }
                            if (ptrChartData.Option.Theme != null) ptrChartData.Option.Theme.AssignTheme(ptrChart);
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
            ChartTool[sender].ToolState = state;
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

            ChartData ptrChartData = ChartTool[sender];
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

        private static void ChartControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //IF KeyPreview property enabled in Parent form, left and right arrow key will be used to move chart cursor left / right
            if (e.KeyCode == Keys.Right) MoveCursor(sender as Chart, CursorDirection.Right);
            else if (e.KeyCode == Keys.Left) MoveCursor(sender as Chart, CursorDirection.Left);
        }

        private static void ChartControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemcomma) MoveCursor(sender as Chart, CursorDirection.Left);
            else if (e.KeyCode == Keys.OemPeriod) MoveCursor(sender as Chart, CursorDirection.Right);
        }

        private static void ChartControl_AxisViewChanged(object sender, ViewEventArgs e)
        {
            Chart ptrChart = sender as Chart;
            ChartData ptrChartData = ChartTool[ptrChart];
            AdjustAxisIntervalForAllAxes(e.ChartArea);
            ptrChartData.RepaintBufferedData();
            ZoomChangedCallback(ptrChart);
        }

        private enum CursorDirection { Right, Left }

        private static void MoveCursor(Chart chart, CursorDirection dir)
        {
            ChartData ptrChartData = ChartTool[chart];
            ChartCursor ptrCursor = ptrChartData.ToolState == MSChartExtensionToolState.Select ? ptrChartData.Cursor1 : ptrChartData.Cursor2;
            Series ptrSeries = ptrCursor.SelectedChartSeries;

            if (ptrCursor.DataIndex == -1) ptrCursor.DataIndex = 0;

            if (dir == CursorDirection.Left) ptrCursor.DataIndex--;
            else if (dir == CursorDirection.Right) ptrCursor.DataIndex++;

            if (ptrCursor.DataIndex <= 0) ptrCursor.DataIndex = 0;
            else if (ptrCursor.DataIndex >= ptrSeries.Points.Count()) ptrCursor.DataIndex = ptrSeries.Points.Count() - 1;

            DataPoint[] datas = ptrSeries.Points.OrderBy(x => x.XValue).ToArray();
            if (datas.Length == 0) return; //Skip the rest of the code when series have no valid data.

            ptrCursor.X = datas[ptrCursor.DataIndex].XValue;
            ptrCursor.Y = datas[ptrCursor.DataIndex].YValues.First();
            Trace.WriteLine("X = " + ptrCursor.X.ToString() + " Y = " + ptrCursor.Y.ToString());

            MoveCursor(chart, ptrCursor.X, ptrCursor.Y);
        }

        private static void MoveCursor(this Chart ptrChart, double newX, double newY)
        {
            ChartData ptrChartData = ChartTool[ptrChart];
            ChartArea ptrChartArea = ChartTool[ptrChart].ActiveChartArea;

            if (IsChartAreaEmpty(ptrChart, ptrChartArea)) { UpdateChartControlState(ptrChart); return; }
            if (ptrChartArea == null) return;
            if (!ptrChartData.Enabled) return;

            if (ptrChartData.ToolState == MSChartExtensionToolState.Select)
            {
                Color textColor = ptrChartData.Option.Cursor1TextColor;
                Color cursorColor = ptrChartData.Option.Cursor1Color;
                ChartDashStyle cursorDashStyle = ptrChartData.Option.Cursor1DashStyle;
                int lineWidth = ptrChartData.Option.Cursor1LineWidth;
                Series ptrSeries = ptrChartData.Cursor1.SelectedChartSeries;
                if (ptrSeries == null) ptrSeries = ptrChartData.Cursor1.SelectedChartSeries = ptrChart.Series.First(x => x.ChartArea == ptrChartArea.Name);
                if (ptrSeries != null)
                {
                    Axis ptrXAxis = ptrSeries.XAxisType == AxisType.Primary ? ptrChartArea.AxisX : ptrChartArea.AxisX2;
                    Axis ptrYAxis = ptrSeries.YAxisType == AxisType.Primary ? ptrChartArea.AxisY : ptrChartArea.AxisY2;
                    double XStart = newX;
                    double YStart = newY;

                    //Sanity check - make sure XStart and YStart within limit
                    if (!SanityCheck(ptrXAxis, XStart)) return;
                    if (!SanityCheck(ptrYAxis, YStart)) return;

                    ChartCursorLabel ptrLabelX = ptrSeries.XAxisType == AxisType.Primary ? ptrChartData.Option.CursorLabelFormatX1 : ptrChartData.Option.CursorLabelFormatX2;
                    ChartCursorLabel ptrLabelY = ptrSeries.YAxisType == AxisType.Primary ? ptrChartData.Option.CursorLabelFormatY1 : ptrChartData.Option.CursorLabelFormatY2;

                    if (ptrChartArea.ChartAreaBoundaryTest(ptrXAxis, ptrYAxis, XStart, YStart))
                    {
                        DrawVerticalLine(ptrChart, XStart, cursorColor, ptrChartArea.Name + Cursor1XName, lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.XAxisType);
                        DrawHorizontalLine(ptrChart, YStart, cursorColor, ptrChartArea.Name + Cursor1YName, lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.YAxisType);
                        ptrChartData.Cursor1.X = XStart;
                        ptrChartData.Cursor1.Y = YStart;
                        ptrChartData.Cursor1.XFormattedString = FormatCursorValue(XStart, ptrSeries.XValueType, ptrLabelX.StringFormat);
                        ptrChartData.Cursor1.YFormattedString = FormatCursorValue(YStart, ptrSeries.YValueType, ptrLabelY.StringFormat);
                        ptrChartData.Cursor1.ChartArea = ptrChartArea;

                        if (ptrChartData.Option.ShowCursorValue)
                        {
                            //Add Cursor Value : X, Y
                            string cursorValue = string.Empty;
                            if (ptrLabelX.Visible) cursorValue += ptrLabelX.Prefix + ptrChartData.Cursor1.XFormattedString + ptrLabelX.Postfix;
                            if (ptrLabelX.Visible && ptrLabelY.Visible) cursorValue += ",";
                            if (ptrLabelY.Visible) cursorValue += ptrLabelY.Prefix + ptrChartData.Cursor1.YFormattedString + ptrLabelY.Postfix;

                            TextAnnotation ptrTextAnnotation = AddText(ptrChart, cursorValue, XStart, YStart, textColor, ptrChartArea.Name + Cursor1LabelName,
                                                    TextStyle.Default, ptrChartArea, ptrSeries.XAxisType, ptrSeries.YAxisType) as TextAnnotation;
                            CheckAndUpdateTextAnnotationLocation(ptrChart, ptrTextAnnotation, XStart, YStart);
                            AddTextBackground(ptrChart, ptrTextAnnotation.Name, cursorColor);
                        }
                        ptrChartData.PositionChangedCallback?.Invoke(ptrChart, ptrChartData.Cursor1.Clone() as ChartCursor);
                    }
                }
            }
            else if (ptrChartData.ToolState == MSChartExtensionToolState.Select2)
            {
                Color textColor = ptrChartData.Option.Cursor2TextColor;
                Color cursorColor = ptrChartData.Option.Cursor2Color;
                ChartDashStyle cursorDashStyle = ptrChartData.Option.Cursor2DashStyle;
                int lineWidth = ptrChartData.Option.Cursor2LineWidth;
                Series ptrSeries = ptrChartData.Cursor2.SelectedChartSeries;
                if (ptrSeries == null) ptrSeries = ptrChartData.Cursor2.SelectedChartSeries = ptrChart.Series.First(x => x.ChartArea == ptrChartArea.Name);
                if (ptrSeries != null)
                {
                    Axis ptrXAxis = ptrSeries.XAxisType == AxisType.Primary ? ptrChartArea.AxisX : ptrChartArea.AxisX2;
                    Axis ptrYAxis = ptrSeries.YAxisType == AxisType.Primary ? ptrChartArea.AxisY : ptrChartArea.AxisY2;
                    double XStart = newX;
                    double YStart = newY;

                    ChartCursorLabel ptrLabelX = ptrSeries.XAxisType == AxisType.Primary ? ptrChartData.Option.CursorLabelFormatX1 : ptrChartData.Option.CursorLabelFormatX2;
                    ChartCursorLabel ptrLabelY = ptrSeries.YAxisType == AxisType.Primary ? ptrChartData.Option.CursorLabelFormatY1 : ptrChartData.Option.CursorLabelFormatY2;

                    //Sanity check - make sure XStart and YStart within limit
                    if (!SanityCheck(ptrXAxis, XStart)) return;
                    if (!SanityCheck(ptrYAxis, YStart)) return;

                    if (ptrChartArea.ChartAreaBoundaryTest(ptrXAxis, ptrYAxis, XStart, YStart))
                    {
                        DrawVerticalLine(ptrChart, XStart, cursorColor, ptrChartArea.Name + Cursor2XName, lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.XAxisType);
                        DrawHorizontalLine(ptrChart, YStart, cursorColor, ptrChartArea.Name + Cursor2YName, lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.YAxisType);
                        ptrChartData.Cursor2.X = XStart;
                        ptrChartData.Cursor2.Y = YStart;
                        ptrChartData.Cursor2.XFormattedString = FormatCursorValue(XStart, ptrSeries.XValueType, ptrLabelX.StringFormat);
                        ptrChartData.Cursor2.YFormattedString = FormatCursorValue(YStart, ptrSeries.YValueType, ptrLabelY.StringFormat);
                        ptrChartData.Cursor2.ChartArea = ptrChartArea;


                        if (ptrChartData.Option.ShowCursorValue)
                        {
                            //Add Cursor Value : X, Y
                            string cursorValue = string.Empty;
                            if (ptrLabelX.Visible) cursorValue += ptrLabelX.Prefix + ptrChartData.Cursor1.XFormattedString + ptrLabelX.Postfix;
                            if (ptrLabelX.Visible && ptrLabelY.Visible) cursorValue += ",";
                            if (ptrLabelY.Visible) cursorValue += ptrLabelY.Prefix + ptrChartData.Cursor1.YFormattedString + ptrLabelY.Postfix;

                            TextAnnotation ptrTextAnnotation = AddText(ptrChart, cursorValue, XStart, YStart, textColor, ptrChartArea.Name + Cursor2LabelName,
                                                TextStyle.Default, ptrChartArea, ptrSeries.XAxisType, ptrSeries.YAxisType) as TextAnnotation;
                            CheckAndUpdateTextAnnotationLocation(ptrChart, ptrTextAnnotation, XStart, YStart);
                            AddTextBackground(ptrChart, ptrTextAnnotation.Name, cursorColor);
                        }
                        ptrChartData.PositionChangedCallback?.Invoke(ptrChart, ptrChartData.Cursor2.Clone() as ChartCursor);
                    }
                }
            }

        }

        #endregion

        #region [ (PRIVATE) Chart - Mouse Events ]

        private static bool MouseDowned;
        private static double XStart, YStart, X2Start, Y2Start;

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
                else return;
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
                else return;
            }
            ptrChartArea.AdjustAxisIntervalForAllAxes();
            ptrChartData.RepaintBufferedData();
            ZoomChangedCallback(ptrChart);
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
            try
            {
                switch (valueType)
                {
                    case ChartValueType.Auto:
                    case ChartValueType.Double:
                    case ChartValueType.Single:
                    case ChartValueType.Int32:
                    case ChartValueType.Int64:
                    case ChartValueType.UInt32:
                    case ChartValueType.UInt64:
                        if (!string.IsNullOrEmpty(NumberLabelFormat))
                        {
                            if (NumberLabelFormat.ToUpper().StartsWith("X"))
                                return Convert.ToInt64(value).ToString(NumberLabelFormat);
                        }
                        return value.ToString(NumberLabelFormat);

                    case ChartValueType.String:
                        return value.ToString(); //Not Supported as X Value always 0 if string label is used.
                    case ChartValueType.DateTime:
                    case ChartValueType.Date:
                    case ChartValueType.Time:
                    case ChartValueType.DateTimeOffset:
                        return DateTime.FromOADate(value).ToString(NumberLabelFormat);
                }
                return value.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WARNING: Conversion Error " + ex.Message);
                return value.ToString();
            }
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

            ptrChartArea.CursorX.AxisType = !Double.IsNaN(ptrChartArea.AxisX.Minimum) ? AxisType.Primary : AxisType.Secondary;
            ptrChartArea.CursorY.AxisType = !Double.IsNaN(ptrChartArea.AxisY.Minimum) ? AxisType.Primary : AxisType.Secondary;
            ptrChartArea.CursorX.SetSelectionPixelPosition(startAndEndPt, startAndEndPt, roundToBoundary);
            ptrChartArea.CursorY.SetSelectionPixelPosition(startAndEndPt, startAndEndPt, roundToBoundary);

            //Value for PAN Control
            XStart = ptrChartArea.AxisX.PixelPositionToValue(e.Location.X);
            YStart = ptrChartArea.AxisY.PixelPositionToValue(e.Location.Y);
            X2Start = ptrChartArea.AxisX2.PixelPositionToValue(e.Location.X);
            Y2Start = ptrChartArea.AxisY2.PixelPositionToValue(e.Location.Y);

            int dataIndex = -1;
            if (ptrChartData.ToolState == MSChartExtensionToolState.Select)
            {
                Color textColor = ptrChartData.Option.Cursor1TextColor;
                Color cursorColor = ptrChartData.Option.Cursor1Color;
                ChartDashStyle cursorDashStyle = ptrChartData.Option.Cursor1DashStyle;
                int lineWidth = ptrChartData.Option.Cursor1LineWidth;
                Series ptrSeries = ptrChartData.Cursor1.SelectedChartSeries;
                if (ptrSeries == null) ptrSeries = ptrChartData.Cursor1.SelectedChartSeries = ptrChart.Series.First(x => x.ChartArea == ptrChartArea.Name);
                else if (!ptrChartData.Source.Series.Contains(ptrChartData.Cursor1.SelectedChartSeries))
                    ptrSeries = ptrChartData.Cursor1.SelectedChartSeries = ptrChart.Series.First(x => x.ChartArea == ptrChartArea.Name);

                if (ptrSeries != null)
                {
                    Axis ptrXAxis = ptrSeries.XAxisType == AxisType.Primary ? ptrChartArea.AxisX : ptrChartArea.AxisX2;
                    Axis ptrYAxis = ptrSeries.YAxisType == AxisType.Primary ? ptrChartArea.AxisY : ptrChartArea.AxisY2;
                    double XStart = ptrXAxis.PixelPositionToValue(e.Location.X);
                    double YStart = ptrYAxis.PixelPositionToValue(e.Location.Y);

                    //Sanity check - make sure XStart and YStart within limit
                    if (!SanityCheck(ptrXAxis, XStart)) return;
                    if (!SanityCheck(ptrYAxis, YStart)) return;

                    ChartCursorLabel ptrLabelX = ptrSeries.XAxisType == AxisType.Primary ? ptrChartData.Option.CursorLabelFormatX1 : ptrChartData.Option.CursorLabelFormatX2;
                    ChartCursorLabel ptrLabelY = ptrSeries.YAxisType == AxisType.Primary ? ptrChartData.Option.CursorLabelFormatY1 : ptrChartData.Option.CursorLabelFormatY2;

                    if (ptrChartArea.ChartAreaBoundaryTest(ptrXAxis, ptrYAxis, XStart, YStart))
                    {
                        if (ptrChartData.Option.SnapCursorToData) dataIndex = SnapToNearestData(ptrChart, ptrSeries, ptrXAxis, ptrYAxis, e, ref XStart, ref YStart);

                        DrawVerticalLine(ptrChart, XStart, cursorColor, ptrChartArea.Name + Cursor1XName, lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.XAxisType);
                        DrawHorizontalLine(ptrChart, YStart, cursorColor, ptrChartArea.Name + Cursor1YName, lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.YAxisType);
                        ptrChartData.Cursor1.X = XStart;
                        ptrChartData.Cursor1.Y = YStart;
                        ptrChartData.Cursor1.DataIndex = dataIndex;
                        ptrChartData.Cursor1.XFormattedString = FormatCursorValue(XStart, ptrSeries.XValueType, ptrLabelX.StringFormat);
                        ptrChartData.Cursor1.YFormattedString = FormatCursorValue(YStart, ptrSeries.YValueType, ptrLabelY.StringFormat);
                        ptrChartData.Cursor1.ChartArea = ptrChartArea;

                        if (ptrChartData.Option.ShowCursorValue)
                        {
                            //Add Cursor Value : X, Y
                            string cursorValue = string.Empty;
                            if (ptrLabelX.Visible) cursorValue += ptrLabelX.Prefix + ptrChartData.Cursor1.XFormattedString + ptrLabelX.Postfix;
                            if (ptrLabelX.Visible && ptrLabelY.Visible) cursorValue += ",";
                            if (ptrLabelY.Visible) cursorValue += ptrLabelY.Prefix + ptrChartData.Cursor1.YFormattedString + ptrLabelY.Postfix;

                            TextAnnotation ptrTextAnnotation = AddText(ptrChart, cursorValue, XStart, YStart, textColor, ptrChartArea.Name + Cursor1LabelName,
                                                                    TextStyle.Default, ptrChartArea, ptrSeries.XAxisType, ptrSeries.YAxisType) as TextAnnotation;
                            CheckAndUpdateTextAnnotationLocation(ptrChart, ptrTextAnnotation, XStart, YStart);
                            AddTextBackground(ptrChart, ptrTextAnnotation.Name, cursorColor);
                        }

                        ptrChartData.PositionChangedCallback?.Invoke(ptrChart, ptrChartData.Cursor1.Clone() as ChartCursor);
                    }
                }
                ptrChart.Focus();
            }
            else if (ptrChartData.ToolState == MSChartExtensionToolState.Select2)
            {
                Color textColor = ptrChartData.Option.Cursor2TextColor;
                Color cursorColor = ptrChartData.Option.Cursor2Color;
                ChartDashStyle cursorDashStyle = ptrChartData.Option.Cursor2DashStyle;
                int lineWidth = ptrChartData.Option.Cursor2LineWidth;
                Series ptrSeries = ptrChartData.Cursor2.SelectedChartSeries;
                if (ptrSeries == null) ptrSeries = ptrChartData.Cursor2.SelectedChartSeries = ptrChart.Series.First(x => x.ChartArea == ptrChartArea.Name);
                else if (!ptrChartData.Source.Series.Contains(ptrChartData.Cursor2.SelectedChartSeries))
                    ptrSeries = ptrChartData.Cursor2.SelectedChartSeries = ptrChart.Series.First(x => x.ChartArea == ptrChartArea.Name);

                if (ptrSeries != null)
                {
                    Axis ptrXAxis = ptrSeries.XAxisType == AxisType.Primary ? ptrChartArea.AxisX : ptrChartArea.AxisX2;
                    Axis ptrYAxis = ptrSeries.YAxisType == AxisType.Primary ? ptrChartArea.AxisY : ptrChartArea.AxisY2;
                    double XStart = ptrXAxis.PixelPositionToValue(e.Location.X);
                    double YStart = ptrYAxis.PixelPositionToValue(e.Location.Y);

                    //Sanity check - make sure XStart and YStart within limit
                    if (!SanityCheck(ptrXAxis, XStart)) return;
                    if (!SanityCheck(ptrYAxis, YStart)) return;

                    ChartCursorLabel ptrLabelX = ptrSeries.XAxisType == AxisType.Primary ? ptrChartData.Option.CursorLabelFormatX1 : ptrChartData.Option.CursorLabelFormatX2;
                    ChartCursorLabel ptrLabelY = ptrSeries.YAxisType == AxisType.Primary ? ptrChartData.Option.CursorLabelFormatY1 : ptrChartData.Option.CursorLabelFormatY2;

                    if (ptrChartArea.ChartAreaBoundaryTest(ptrXAxis, ptrYAxis, XStart, YStart))
                    {
                        if (ptrChartData.Option.SnapCursorToData) dataIndex = SnapToNearestData(ptrChart, ptrSeries, ptrXAxis, ptrYAxis, e, ref XStart, ref YStart);

                        DrawVerticalLine(ptrChart, XStart, cursorColor, ptrChartArea.Name + Cursor2XName, lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.XAxisType);
                        DrawHorizontalLine(ptrChart, YStart, cursorColor, ptrChartArea.Name + Cursor2YName, lineWidth, cursorDashStyle, ptrChartArea, ptrSeries.YAxisType);
                        ptrChartData.Cursor2.X = XStart;
                        ptrChartData.Cursor2.Y = YStart;
                        ptrChartData.Cursor2.DataIndex = dataIndex;
                        ptrChartData.Cursor1.XFormattedString = FormatCursorValue(XStart, ptrSeries.XValueType, ptrLabelX.StringFormat);
                        ptrChartData.Cursor1.YFormattedString = FormatCursorValue(YStart, ptrSeries.YValueType, ptrLabelY.StringFormat);
                        ptrChartData.Cursor2.ChartArea = ptrChartArea;

                        if (ptrChartData.Option.ShowCursorValue)
                        {
                            //Add Cursor Value : X, Y
                            string cursorValue = string.Empty;
                            if (ptrLabelX.Visible) cursorValue += ptrLabelX.Prefix + ptrChartData.Cursor1.XFormattedString + ptrLabelX.Postfix;
                            if (ptrLabelX.Visible && ptrLabelY.Visible) cursorValue += ",";
                            if (ptrLabelY.Visible) cursorValue += ptrLabelY.Prefix + ptrChartData.Cursor1.YFormattedString + ptrLabelY.Postfix;

                            TextAnnotation ptrTextAnnotation = AddText(ptrChart, cursorValue, XStart, YStart, textColor, ptrChartArea.Name + Cursor2LabelName,
                                                                TextStyle.Default, ptrChartArea, ptrSeries.XAxisType, ptrSeries.YAxisType) as TextAnnotation;
                            CheckAndUpdateTextAnnotationLocation(ptrChart, ptrTextAnnotation, XStart, YStart);
                            AddTextBackground(ptrChart, ptrTextAnnotation.Name, cursorColor);
                        }

                        ptrChartData.PositionChangedCallback?.Invoke(ptrChart, ptrChartData.Cursor2.Clone() as ChartCursor);
                    }
                }
                ptrChart.Focus();
            }
        }

        private static void CheckAndUpdateTextAnnotationLocation(Chart chart, TextAnnotation textAnn, double xStart, double yStart)
        {
            if (chart == null) throw new ArgumentNullException(nameof(chart));
            if (textAnn == null) throw new ArgumentNullException(nameof(textAnn));

            ChartArea ptrChartArea = chart.ChartAreas.FindByName(textAnn.ClipToChartArea);
            if (ptrChartArea == null) throw new ArgumentNullException("Chart are [" + textAnn.ClipToChartArea + "] not found!");
            Axis ptrXAxis = textAnn.AxisXName.EndsWith("2") ? ptrChartArea.AxisX2 : ptrChartArea.AxisX;
            Axis ptrYAxis = textAnn.YAxisName.EndsWith("2") ? ptrChartArea.AxisY2 : ptrChartArea.AxisY;

            textAnn.Text = textAnn.Text; //Update text to force recalculate text size and position
            Size textSize = TextRenderer.MeasureText(textAnn.Text, textAnn.Font);
            double xMin = ptrXAxis.ValueToPixelPosition(ptrXAxis.ScaleView.ViewMinimum);
            double xMax = ptrXAxis.ValueToPixelPosition(ptrXAxis.ScaleView.ViewMaximum);
            double yMin = ptrYAxis.ValueToPixelPosition(ptrYAxis.ScaleView.ViewMaximum);
            double yMax = ptrYAxis.ValueToPixelPosition(ptrYAxis.ScaleView.ViewMinimum);

            double xStartPixel = ptrXAxis.ValueToPixelPosition(xStart);
            double yStartPixel = ptrYAxis.ValueToPixelPosition(yStart);

            //Check if Text is out of sight
            if ((xStartPixel < xMin) || (xStartPixel > xMax)) return;
            if ((yStartPixel < yMin) || (yStartPixel > yMax)) return;


            if (((xStartPixel + textSize.Width) > xMax) &&
                ((yStartPixel - textSize.Height) < yMin))
            {
                double newXStart = ptrXAxis.PixelPositionToValue(xStartPixel - textSize.Width - 4);
                textAnn.X = newXStart;
                textAnn.Y = ptrYAxis.PixelPositionToValue(yStartPixel);
            }
            else if (((xStartPixel + textSize.Width) > xMax) ||
                ((yStartPixel + textSize.Height) > yMax))
            {
                double newXStart = ptrXAxis.PixelPositionToValue(xStartPixel - textSize.Width - 4);
                double newYStart = ptrYAxis.PixelPositionToValue(yStartPixel - textSize.Height - 4);
                textAnn.X = newXStart;
                textAnn.Y = newYStart;
            }
            else
            {
                textAnn.X = ptrXAxis.PixelPositionToValue(xStartPixel);
                textAnn.Y = ptrYAxis.PixelPositionToValue(yStartPixel);
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
                    //Debug.WriteLine("Active Chart Area: " + (ptrChartData.ActiveChartArea == null ? "NULL" : ptrChartData.ActiveChartArea.Name));

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

            double selX, selY, selX1, selY1, selX2, selY2;
            selX = selY = selX1 = selY1 = selX2 = selY2 = 0;
            Axis ptrXAxis, ptrYAxis;
            try
            {
                ptrXAxis = ptrChartArea.AxisX;
                ptrYAxis = ptrChartArea.AxisY;
                selX = selX1 = ptrChartArea.AxisX.PixelPositionToValue(e.Location.X);
                selY = selY1 = ptrChartArea.AxisY.PixelPositionToValue(e.Location.Y);
                selX2 = ptrChartArea.AxisX2.PixelPositionToValue(e.Location.X);
                selY2 = ptrChartArea.AxisY2.PixelPositionToValue(e.Location.Y);

                if (Double.IsNaN(ptrChartArea.AxisX.Minimum))
                {
                    selX = selX2;
                    ptrXAxis = ptrChartArea.AxisX2;
                }

                if (Double.IsNaN(ptrChartArea.AxisY.Minimum))
                {
                    selY = selY2;
                    ptrYAxis = ptrChartArea.AxisY2;
                }

                selY = !Double.IsNaN(ptrChartArea.AxisY.Minimum) ? selY1 : selY2;
                if (!ptrChartArea.ChartAreaBoundaryTest(ptrXAxis, ptrYAxis, selX, selY)) return; //Pointer outside boundary.

                ChartValueType xValueType = ptrChart.Series.Where(x => x.ChartArea == ptrChartArea.Name).Where(x => x.XAxisType == AxisType.Primary).First().XValueType;
                ChartValueType yValueType = ptrChart.Series.Where(x => x.ChartArea == ptrChartArea.Name).Where(x => x.XAxisType == AxisType.Primary).First().XValueType;
                ChartTool[ptrChart].CursorMovedCallback?.Invoke(ptrChart,
                    new ChartCursor()
                    {
                        X = selX,
                        Y = selY,
                        ChartArea = ptrChartArea,
                        XFormattedString = FormatCursorValue(selX, xValueType, ptrChartData.Option.CursorLabelFormatX1.StringFormat),
                        YFormattedString = FormatCursorValue(selY, yValueType, ptrChartData.Option.CursorLabelFormatY1.StringFormat)
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
                            double dx = -selX1 + XStart;
                            double dy = -selY1 + YStart;
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

                            ptrChartData.RepaintBufferedData();
                            AdjustAxisIntervalForAllAxes(ptrChartArea);
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

                    //Zoom Window (Axis Value)
                    double XStart = ptrChartArea.CursorX.SelectionStart;
                    double XEnd = ptrChartArea.CursorX.SelectionEnd;
                    double YStart = ptrChartArea.CursorY.SelectionStart;
                    double YEnd = ptrChartArea.CursorY.SelectionEnd;
                    if ((XStart == XEnd) && (YStart == YEnd)) return;

                    //Get Pixel Position based on Cursor Axis Type
                    double xStartPos, xEndPos, yStartPos, yEndPos;
                    if (ptrChartArea.CursorX.AxisType == AxisType.Primary)
                    {
                        xStartPos = ptrChartArea.AxisX.ValueToPixelPosition(XStart);
                        xEndPos = ptrChartArea.AxisX.ValueToPixelPosition(XEnd);
                    }
                    else
                    {
                        xStartPos = ptrChartArea.AxisX2.ValueToPixelPosition(XStart);
                        xEndPos = ptrChartArea.AxisX2.ValueToPixelPosition(XEnd);
                    }

                    if (ptrChartArea.CursorY.AxisType == AxisType.Primary)
                    {
                        yStartPos = ptrChartArea.AxisY.ValueToPixelPosition(YStart);
                        yEndPos = ptrChartArea.AxisY.ValueToPixelPosition(YEnd);
                    }
                    else
                    {
                        yStartPos = ptrChartArea.AxisY2.ValueToPixelPosition(YStart);
                        yEndPos = ptrChartArea.AxisY2.ValueToPixelPosition(YEnd);
                    }

                    //Primary Axis;
                    double x1Start = ptrChartArea.AxisX.PixelPositionToValue(xStartPos);
                    double x1End = ptrChartArea.AxisX.PixelPositionToValue(xEndPos);
                    double y1Start = ptrChartArea.AxisY.PixelPositionToValue(yStartPos);
                    double y1End = ptrChartArea.AxisY.PixelPositionToValue(yEndPos);
                    double x1Left = Math.Min(x1Start, x1End);
                    double x1Right = Math.Max(x1Start, x1End);
                    double y1Top = Math.Max(y1Start, y1End);
                    double y1Bottom = Math.Min(y1Start, y1End);

                    //Secondary Axis
                    double x2Start = ptrChartArea.AxisX2.PixelPositionToValue(xStartPos);
                    double x2End = ptrChartArea.AxisX2.PixelPositionToValue(xEndPos);
                    double y2Start = ptrChartArea.AxisY2.PixelPositionToValue(yStartPos);
                    double y2End = ptrChartArea.AxisY2.PixelPositionToValue(yEndPos);
                    double x2Left = Math.Min(x2Start, x2End);
                    double x2Right = Math.Max(x2Start, x2End);
                    double y2Top = Math.Max(y2Start, y2End);
                    double y2Bottom = Math.Min(y2Start, y2End);

                    //Precision Adjustment
                    ChartOption option = ptrChartData.Option;
                    if (option.XAxisPrecision > 0)
                    {
                        x1Left = Math.Round(x1Left, option.XAxisPrecision);
                        x1Right = Math.Round(x1Right, option.XAxisPrecision);
                        x2Left = Math.Round(x2Left, option.XAxisPrecision);
                        x2Right = Math.Round(x2Right, option.XAxisPrecision);
                    }
                    if (option.YAxisPrecision > 0)
                    {
                        y1Top = Math.Round(y1Top, option.XAxisPrecision);
                        y1Bottom = Math.Round(y1Bottom, option.XAxisPrecision);
                        y2Top = Math.Round(y2Top, option.XAxisPrecision);
                        y2Bottom = Math.Round(y2Bottom, option.XAxisPrecision);
                    }

                    // NOTE: left <= right, even if Axis.IsReversed
                    //Zomm X-Axis
                    if ((state == MSChartExtensionToolState.Zoom) || (state == MSChartExtensionToolState.ZoomX))
                    {
                        if ((x1Left >= ptrChartArea.AxisX.Minimum) && (x1Right <= ptrChartArea.AxisX.Maximum) && (x1Left != x1Right))
                        {
                            ptrChartArea.AxisX.ScaleView.Zoom(x1Left, x1Right);
                        }
                        if ((x2Left >= ptrChartArea.AxisX2.Minimum) && (x2Right <= ptrChartArea.AxisX2.Maximum) && (x2Left != x2Right))
                        {
                            ptrChartArea.AxisX2.ScaleView.Zoom(x2Left, x2Right);
                        }
                    }
                    //Y-Axis
                    if ((state == MSChartExtensionToolState.Zoom) || (state == MSChartExtensionToolState.ZoomY))
                    {
                        if ((y1Bottom >= ptrChartArea.AxisY.Minimum) && (y1Top <= ptrChartArea.AxisY.Maximum) && (y1Bottom != y1Top))
                        {
                            ptrChartArea.AxisY.ScaleView.Zoom(y1Bottom, y1Top);
                        }
                        if ((y2Bottom >= ptrChartArea.AxisY2.Minimum) && (y2Top <= ptrChartArea.AxisY2.Maximum) && (y2Bottom != y2Top))
                        {
                            ptrChartArea.AxisY2.ScaleView.Zoom(y2Bottom, y2Top);
                        }
                    }

                    //Clear selection (the following seem to be equivalent)
                    ptrChartArea.CursorX.SetSelectionPosition(0, 0);
                    ptrChartArea.CursorY.SetSelectionPosition(0, 0);

                    AdjustAxisIntervalForAllAxes(ptrChartArea);
                    ptrChartData.RepaintBufferedData();
                    ZoomChangedCallback(ptrChart);
                    SetChartControlState(ptrChart, MSChartExtensionToolState.Select);
                    break;

                case MSChartExtensionToolState.Pan:
                    //ptrChartData.ZoomChangedCallback?.Invoke(ptrChart);
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
        /// <returns> Return data index. -1 if not found.</returns>
        private static int SnapToNearestData(Chart chart, Series series, Axis xAxis, Axis yAxis, MouseEventArgs e,
            ref double XResult, ref double YResult)
        {
            if (series.Points.Count == 0) return -1;
            XResult = YResult = Double.MaxValue;

            ChartData ptrChartData = ChartTool[chart];
            ChartArea ptrChartArea = ChartTool[chart].ActiveChartArea;

            double xMin = xAxis.Minimum;
            double xMax = xAxis.Maximum;

            //Mouser Pointer Value
            double xTarget = xAxis.PixelPositionToValue(e.Location.X);
            double yTarget = yAxis.PixelPositionToValue(e.Location.Y);
            Debug.WriteLine("Target: " + xTarget + ", " + yTarget);

            // Get Incremental Ratio: Value / pixels
            double xRatio = (xAxis.ScaleView.ViewMaximum - xAxis.ScaleView.ViewMinimum) / (xAxis.ValueToPixelPosition(xAxis.ScaleView.ViewMaximum) - xAxis.ValueToPixelPosition(xAxis.ScaleView.ViewMinimum));
            double yRatio = (yAxis.ScaleView.ViewMaximum - yAxis.ScaleView.ViewMinimum) / (yAxis.ValueToPixelPosition(yAxis.ScaleView.ViewMaximum) - yAxis.ValueToPixelPosition(yAxis.ScaleView.ViewMinimum));
            Debug.WriteLine("Ratio: " + xRatio + ", " + yRatio);

            //Sort data point assending by X-Values
            DataPoint[] datas = series.Points.OrderBy(x => x.XValue).ToArray();

            //Get data point closed to mouse cursor x
            //Estimate x index of X position as search starting point. Assume that data count increase proportional to x values.
            int estIndex = (int)(datas.Length * (xTarget - xMin) / (xMax - xMin));
            Debug.WriteLine("Estimated index = " + estIndex);

            //Find crossing index of X Value.
            if (datas[estIndex].XValue > xTarget)
            {
                //Serch Down
                for (int x = estIndex; x > 0; x--)
                {
                    if (datas[x].XValue <= xTarget)
                    {
                        estIndex = x;
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
                        estIndex = x;
                        break;
                    }
                }
            }
            Debug.WriteLine("Calculated X index = " + estIndex);

            //Search out of range.... iLower = iUpper
            //Distance = x^2 + y^2

            //Calculate min distance within 20 data points
            int iLower = Math.Max(0, estIndex - 20);
            int iUppwer = Math.Min(estIndex + 20, datas.Length);

            double minDist = Double.MaxValue;

            //Get Data point with minimum distance within windowed data points
            double dist;
            int dataIndex = 0;
            for (int x = iLower; x < iUppwer; x++)
            {
                DataPoint ptrData = datas[x];
                //Calculate distance based on Axis ratio.
                double dX = (ptrData.XValue - xTarget) / xRatio;
                double dY = (ptrData.YValues[0] - yTarget) / yRatio;
                dist = Math.Pow(dX, 2) + Math.Pow(dY, 2);
                Debug.WriteLine(string.Format("[{0}] {1}, {2}, dX = {3}, dY = {4}, D = {5}", x, ptrData.XValue, ptrData.YValues[0], dX, dY, dist));

                if (dist < minDist)
                {
                    dataIndex = x;
                    minDist = dist;
                    XResult = ptrData.XValue;
                    YResult = ptrData.YValues[0];
                }
            }

            Trace.WriteLine("Snap data index = " + dataIndex);
            return dataIndex;
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
            if (sender.IsChartDataValid())
            {
                //Series is registered in MSChart Extension tool
                ClearPointsInt(sender, true);
                ChartArea ptrChartArea = sender.GetChartArea();
                ChartData ptrChartData = sender.GetChartData();
                if (ptrChartData.Cursor1.SelectedChartSeries == sender) ClearCursor1(ptrChartArea, ptrChartData);
                if (ptrChartData.Cursor2.SelectedChartSeries == sender) ClearCursor2(ptrChartArea, ptrChartData);
            }
            else ClearPointsInt(sender, false);
            sender.Points.ResumeUpdates();
        }

        private static void ClearCursor1(ChartArea chartArea, ChartData chartData)
        {
            Chart ptrChart = ChartTool.FirstOrDefault(x => x.Value == chartData).Key;
            ptrChart.RemoveAnnotation(chartArea.Name + Cursor1XName);
            ptrChart.RemoveAnnotation(chartArea.Name + Cursor1YName);
            ptrChart.RemoveAnnotation(chartArea.Name + Cursor1LabelName);
            ptrChart.RemoveAnnotation(chartArea.Name + Cursor1LabelBGName);
            chartData.Cursor1.X = double.NaN;
            chartData.Cursor1.Y = double.NaN;
            chartData.PositionChangedCallback?.Invoke(ptrChart, chartData.Cursor1);
        }

        private static void ClearCursor2(ChartArea chartArea, ChartData chartData)
        {
            Chart ptrChart = ChartTool.FirstOrDefault(x => x.Value == chartData).Key;
            ptrChart.RemoveAnnotation(chartArea.Name + Cursor2XName);
            ptrChart.RemoveAnnotation(chartArea.Name + Cursor2YName);
            ptrChart.RemoveAnnotation(chartArea.Name + Cursor2LabelName);
            ptrChart.RemoveAnnotation(chartArea.Name + Cursor2LabelBGName);
            chartData.Cursor2.X = double.NaN;
            chartData.Cursor2.Y = double.NaN;
            chartData.PositionChangedCallback?.Invoke(ptrChart, chartData.Cursor2);
        }

        /// <summary>
        /// Internal used implementation. <see cref="ClearPoints(Series)"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="clearDataBuffer">Delete buffered data for virtual mode</param>
        private static void ClearPointsInt(this Series sender, bool clearDataBuffer = true)
        {
            while (sender.Points.Count > 0)
                sender.Points.RemoveAt(sender.Points.Count - 1);

            if (clearDataBuffer)
            {
                if (sender.GetChartData().Option.BufferedMode)
                    sender.GetSeriesDataBuffer(true).Clear();
            }

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

        #region [ Add Data ]

        /// <summary>
        /// Add data point to data buffer. Used with <see cref="ChartOption.BufferedMode"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="xValue"></param>
        /// <param name="yValue"></param>
        public static void AddXYBuffered(this Series sender, double xValue, double yValue)
        {
            sender.GetSeriesDataBuffer(true)?.AddPoint(xValue, yValue);
        }

        private static void RepaintBufferedData(this ChartData sender)
        {
            if (!sender.Option.BufferedMode) return;

            //Cleanup old SeriesData if series had been removed from Chart
            for (int x = 0; x < sender.SeriesData.Count;)
            {
                if (!sender.Source.Series.Contains(sender.SeriesData[x].Series))
                    sender.SeriesData.RemoveAt(x);
                else x++;
            }

            foreach (SeriesDataBuffer s in sender.SeriesData)
            {
                if (s.DataBuffer.Count == 0) continue; //ToDo: Shall we allow mixture of series? Buffered and non-buffered series?
                if (s.Series.ChartArea == sender.ActiveChartArea.Name)
                    s.Series.PlotBufferedData();
            }
        }

        /// <summary>
        /// Plot chart using data buffer. Only applicable for <see cref="ChartOption.BufferedMode"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dynamicX">Enable / Disable dynamic X Value calculation</param>
        public static void PlotBufferedData(this Series sender, bool dynamicX = true)
        {
            ChartData ptrChartData = GetChartData(sender);
            if (!ptrChartData.Option.BufferedMode) throw new InvalidOperationException("Buffered mode not enabled.");

            ChartArea ptrChartArea = GetChartArea(sender);

            //Identify Series Axis
            Axis ptrXAxis = sender.XAxisType == AxisType.Primary ? ptrChartArea.AxisX : ptrChartArea.AxisX2;
            Axis ptrYAxis = sender.YAxisType == AxisType.Primary ? ptrChartArea.AxisY : ptrChartArea.AxisY2;

            //Get Visible Boundary
            RectangleF region = ptrChartArea.GetChartVisibleAreaBoundary(ptrXAxis, ptrYAxis);
            SeriesDataBuffer ptrDataBuffer = GetSeriesDataBuffer(sender);
            if (ptrDataBuffer == null) return;

            sender.ClearPointsInt(clearDataBuffer: false);
            IList<PointD> VisibleDatas = ptrDataBuffer.DataBuffer;
            if (VisibleDatas.Count == 0) return;

            if (ptrXAxis.ScaleView.IsZoomed || ptrYAxis.ScaleView.IsZoomed)
            {
                float width10p = (float)(region.Width * 0.20);

                float leftBoundary = (float)Math.Max(ptrDataBuffer.XMin, region.X - width10p);
                float leftWidth = region.X - leftBoundary; //Get widht increment on left, could be less than 10%
                region.X = leftBoundary;
                region.Width = (float)Math.Min(ptrDataBuffer.XMax - region.X, region.Width + leftWidth + width10p);

                //Get data points in visible area.
                if (ptrDataBuffer.DataBuffer.Count > 0)
                {
                    if (ptrXAxis.IsReversed)
                        VisibleDatas = ptrDataBuffer.DataBuffer.Where(n => ((n.X >= region.Right) && (n.X <= region.Left))).ToList();
                    else
                        VisibleDatas = ptrDataBuffer.DataBuffer.Where(n => ((n.X >= region.Left) && (n.X <= region.Right))).ToList();
                }
            }

            ChartOption ptrOption = GetChartData(sender).Option;
            if (VisibleDatas.Count > ptrOption.DisplayDataSize * 2)
            {
                VisibleDatas = DownSampling.DownsampleLTTB(VisibleDatas.ToArray(), ptrOption.DisplayDataSize, dynamicX);
            }

            //Plot Min Point if out of sight
            if (region.Left > ptrDataBuffer.XMin) sender.Points.AddXY(ptrDataBuffer.XMinPoint.X, ptrDataBuffer.XMinPoint.Y);

            //Plot Data
            foreach (PointD p in VisibleDatas)
            {
                sender.Points.AddXY(p.X, p.Y);
            }

            //Plot Max Point if out of sight
            if (region.Right < ptrDataBuffer.XMax) sender.Points.AddXY(ptrDataBuffer.XMaxPoint.X, ptrDataBuffer.XMaxPoint.Y);

        }

        private static ChartArea GetChartArea(this Series sender)
        {
            return GetChartData(sender).Source.ChartAreas.FirstOrDefault(x => x.Name == sender.ChartArea);
        }

        private static ChartData GetChartData(this Series sender)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            ChartData ptrChartData = ChartTool.FirstOrDefault(x => x.Key.Series.Contains(sender)).Value;
            if (ptrChartData == null) throw new ArgumentNullException(nameof(AddXYBuffered) + ": No matching series found!");
            return ptrChartData;
        }

        private static bool IsChartDataValid(this Series sender)
        {
            return !ChartTool.FirstOrDefault(x => x.Key.Series.Contains(sender)).Equals(new KeyValuePair<Chart, ChartData>());
        }

        private static SeriesDataBuffer GetSeriesDataBuffer(this Series sender, bool autoCreate = false)
        {
            ChartData ptrChartData = GetChartData(sender);
            SeriesDataBuffer result = ptrChartData.SeriesData.FirstOrDefault(x => x.Series == sender);
            if ((result == null) && (autoCreate == true))
            {
                result = new SeriesDataBuffer() { Series = sender };
                ptrChartData.SeriesData.Add(result);
            }
            return result;
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
            TextStyle textStyle = TextStyle.Default, ChartArea chartArea = null,
            AxisType xAxisType = AxisType.Primary, AxisType yAxisType = AxisType.Primary)
        {
            TextAnnotation textAnn = (string.IsNullOrEmpty(name)) ? null : (TextAnnotation)sender.Annotations.FindByName(name);
            if (textAnn == null)
            {
                textAnn = new TextAnnotation();
                if (!string.IsNullOrEmpty(name)) textAnn.Name = name;
                sender.Annotations.Add(textAnn);
            }

            if (chartArea == null) chartArea = sender.ChartAreas[0];
            string chartAreaName = chartArea.Name;
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

            return textAnn;
        }

        private static Annotation AddTextBackground(this Chart sender, string textAnnotationName, Drawing.Color color)
        {
            if (string.IsNullOrEmpty(textAnnotationName)) throw new ArgumentNullException(nameof(textAnnotationName));

            TextAnnotation ptrTextAnn = (TextAnnotation)sender.Annotations.FindByName(textAnnotationName);
            if (ptrTextAnn == null) throw new InvalidEnumArgumentException("AddTextBackground: Text annotation [" + textAnnotationName + "] not found!");

            RectangleAnnotation ann = (RectangleAnnotation)sender.Annotations.FindByName(textAnnotationName + "_BG");
            if (ann == null)
            {
                //Always add background before text
                ann = new RectangleAnnotation();
                ann.Name = textAnnotationName + "_BG";
                sender.RemoveAnnotation(ptrTextAnn.Name);
                sender.Annotations.Add(ann);
                sender.Annotations.Add(ptrTextAnn);
            }

            ChartArea ptrChartArea = sender.ChartAreas.FindByName(ptrTextAnn.ClipToChartArea);
            if (ptrChartArea == null) throw new ArgumentException("AddTextBackground: Invalid chart area [" + ptrTextAnn.ClipToChartArea + "]!");

            ann.ClipToChartArea = ptrTextAnn.ClipToChartArea;
            ann.AxisXName = ptrTextAnn.AxisXName;
            ann.YAxisName = ptrTextAnn.YAxisName;
            ann.IsSizeAlwaysRelative = false;
            ann.X = ptrTextAnn.X;
            ann.Y = ptrTextAnn.Y;
            ann.BackColor = color;
            Size textSize = TextRenderer.MeasureText(ptrTextAnn.Text, ptrTextAnn.Font);

            Axis ptrXAxis = ann.AxisXName.EndsWith("2") ? ptrChartArea.AxisX2 : ptrChartArea.AxisX;
            Axis ptrYAxis = ann.YAxisName.EndsWith("2") ? ptrChartArea.AxisY2 : ptrChartArea.AxisY;

            ann.Width = ptrXAxis.PixelPositionToValue(textSize.Width + 5) - ptrXAxis.PixelPositionToValue(0);
            ann.Height = ptrYAxis.PixelPositionToValue(textSize.Height + 5) - ptrYAxis.PixelPositionToValue(0);

            return ann;
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
            if (xAxis.IsReversed)
            {
                if ((x > area.Left) || (x < area.Right)) return false;      //Reversed Axis
            }
            else if ((x < area.Left) || (x > area.Right)) return false;     //Normal Axis

            if (yAxis.IsReversed)
            {
                if ((y > area.Bottom) || (y < area.Top)) return false;      //Reversed Axis
            }
            else if ((y < area.Bottom) || (y > area.Top)) return false;     //Normal Axis
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

        private static void ResetAxisIntervalForAllAxes(this ChartArea sender)
        {
            foreach (Axis a in sender.Axes)
            {
                a.Interval = 0;
                a.IntervalAutoMode = IntervalAutoMode.FixedCount;
                a.IntervalOffset = 0;
                a.IntervalOffsetType = DateTimeIntervalType.Auto;
                a.MajorTickMark.IntervalOffset = 0;
                a.MinorTickMark.IntervalOffset = 0;
            }
        }

        private static void AdjustAxisIntervalForAllAxes(this ChartArea sender)
        {
            foreach (Axis a in sender.Axes)
                AdjustAxisIntervalOffset(sender, a);
        }

        private static void AdjustAxisIntervalOffset(this ChartArea sender, Axis axis)
        {
            //Created by Shin-Hua Tseng <shtsenga@gmail.com>

            double[] unit_base = new double[] { 1.0, 2.0, 2.5, 5.0 };
            double unit = 1.0;
            double value = 0;
            double vmin, vmax;
            int max_count, scale;
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

            max_count = (((int)axis.AxisName) % 2 == 0) ? 10 : 10;

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

        #endregion
    }

}
