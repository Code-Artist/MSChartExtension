using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using EventHandlerSupport;
using System.Diagnostics;

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
    /// Chart Cursor Position 
    /// </summary>
    public class ChartCursor : ICloneable
    {
        /// <summary>
        /// X Value based on primary Axis
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Y Value based on primary Axis
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// ChartArea where the cursor is located.
        /// </summary>
        public ChartArea ChartArea { get; set; } = null;
        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new ChartCursor() { X = this.X, Y = this.Y, ChartArea = this.ChartArea };
        }
    }

    /// <summary>
    /// Data Storage class for <see cref="MSChartExtension"/>
    /// </summary>
    internal class ChartData
    {
        //Store chart settings. Used to backup and restore chart settings.

        private Chart Source;
        public ChartData(Chart sender)
        {
            Source = sender;
            CreateChartContextMenu();

            int x = sender.ChartAreas.Count;
            CursorXUserEnabled = new bool[x];
            CursorYUserEnabled = new bool[x];
            Cursor = new Forms.Cursor[x];
            CursorXInterval = new double[x];
            CursorYInterval = new double[x];
            CursorXAutoScroll = new bool[x];
            CursorYAutoScroll = new bool[x];
            ScrollBarX = new bool[x];
            ScrollBarX2 = new bool[x];
            ScrollBarY = new bool[x];
            ScrollBarY2 = new bool[x];
            SupportedChartArea = new List<ChartArea>();

            Cursor1 = new ChartCursor();
            Cursor2 = new ChartCursor();
        }

        public MSChartExtensionToolState ToolState { get; set; }
        public CursorPositionChanged PositionChangedCallback;
        public CursorPositionChanged CursorMovedCallback;
        public ZoomChanged ZoomChangedCallback { get; set; }
        public ChartOption Option { get; set; }
        public List<ChartArea> SupportedChartArea;
        public bool Enabled { get; set; } = true;
        public ChartArea ActiveChartArea { get; set; }
        public ChartCursor Cursor1 { get; private set; }
        public ChartCursor Cursor2 { get; private set; }

        private void CreateChartContextMenu()
        {
            ChartToolZoomOut = new ToolStripMenuItem("Zoom Out");
            ChartToolZoom = new ToolStripMenuItem("Zoom Window");
            ChartToolZoomX = new ToolStripMenuItem("Zoom XAxis");
            ChartToolZoomY = new ToolStripMenuItem("Zoom YAxis");
            ChartToolZoomDialog = new ToolStripMenuItem("Zoom Dialog...");
            ChartToolZoomOutSeparator = new ToolStripSeparator();
            ChartToolSelect = new ToolStripMenuItem("Select - Cursor 1");
            ChartToolSelect2 = new ToolStripMenuItem("Select - Cursor 2");
            ChartToolPan = new ToolStripMenuItem("Pan");
            AboutSeparator = new ToolStripSeparator();
            About = new ToolStripMenuItem("About...");
            About.Image = Properties.Resources.MSChartExtensionLogo;
            ChartContextSeparator = new ToolStripSeparator();

            MenuItems = new List<ToolStripItem>();
            MenuItems.Add(ChartToolZoomOut);
            MenuItems.Add(ChartToolZoom);
            MenuItems.Add(ChartToolZoomX);
            MenuItems.Add(ChartToolZoomY);
            MenuItems.Add(ChartToolZoomDialog);
            MenuItems.Add(ChartToolZoomOutSeparator);
            MenuItems.Add(ChartToolSelect);
            MenuItems.Add(ChartToolSelect2);
            MenuItems.Add(ChartToolPan);
            MenuItems.Add(AboutSeparator);
            MenuItems.Add(About);
            MenuItems.Add(ChartContextSeparator);

            StateMenu = new Dictionary<MSChartExtensionToolState, ToolStripMenuItem>
                {
                    {MSChartExtensionToolState.Select, ChartToolSelect},
                    {MSChartExtensionToolState.Select2, ChartToolSelect2},
                    {MSChartExtensionToolState.Pan, ChartToolPan},
                    {MSChartExtensionToolState.Zoom, ChartToolZoom},
                    {MSChartExtensionToolState.ZoomX, ChartToolZoomX},
                    {MSChartExtensionToolState.ZoomY, ChartToolZoomY},
                    {MSChartExtensionToolState.ZoomDialog, ChartToolZoomDialog }

                };
        }

        /// <summary>
        /// Backtup properties for all Chart Area
        /// </summary>
        public void Backup()
        {
            ContextMenuStrip = Source.ContextMenuStrip;
            int x = 0;
            foreach (ChartArea ptrChartArea in Source.ChartAreas)
            {
                CursorXUserEnabled[x] = ptrChartArea.CursorX.IsUserEnabled;
                CursorYUserEnabled[x] = ptrChartArea.CursorY.IsUserEnabled;
                Cursor[x] = Source.Cursor;
                CursorXInterval[x] = ptrChartArea.CursorX.Interval;
                CursorYInterval[x] = ptrChartArea.CursorY.Interval;
                CursorXAutoScroll[x] = ptrChartArea.CursorX.AutoScroll;
                CursorYAutoScroll[x] = ptrChartArea.CursorY.AutoScroll;
                ScrollBarX[x] = ptrChartArea.AxisX.ScrollBar.Enabled;
                ScrollBarX2[x] = ptrChartArea.AxisX2.ScrollBar.Enabled;
                ScrollBarY[x] = ptrChartArea.AxisY.ScrollBar.Enabled;
                ScrollBarY2[x] = ptrChartArea.AxisY2.ScrollBar.Enabled;
                x++;
            }
        }

        /// <summary>
        /// Restore properties for all Chart Area
        /// </summary>
        public void Restore()
        {
            Source.ContextMenuStrip = ContextMenuStrip;
            int x = 0;
            foreach (ChartArea ptrChartArea in Source.ChartAreas)
            {
                ptrChartArea.CursorX.IsUserEnabled = CursorXUserEnabled[x];
                ptrChartArea.CursorY.IsUserEnabled = CursorYUserEnabled[x];
                Source.Cursor = Cursor[x];
                ptrChartArea.CursorX.Interval = CursorXInterval[x];
                ptrChartArea.CursorY.Interval = CursorYInterval[x];
                ptrChartArea.CursorX.AutoScroll = CursorXAutoScroll[x];
                ptrChartArea.CursorY.AutoScroll = CursorYAutoScroll[x];
                ptrChartArea.AxisX.ScrollBar.Enabled = ScrollBarX[x];
                ptrChartArea.AxisX2.ScrollBar.Enabled = ScrollBarX2[x];
                ptrChartArea.AxisY.ScrollBar.Enabled = ScrollBarY[x];
                ptrChartArea.AxisY2.ScrollBar.Enabled = ScrollBarY2[x];
                x++;
            }
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
        private bool[] CursorXUserEnabled;
        private bool[] CursorYUserEnabled;
        private System.Windows.Forms.Cursor[] Cursor;
        private double[] CursorXInterval, CursorYInterval;
        private bool[] CursorXAutoScroll, CursorYAutoScroll;
        private bool[] ScrollBarX, ScrollBarX2, ScrollBarY, ScrollBarY2;

        #endregion

        #region [ Extended Context Menu ]

        public List<ToolStripItem> MenuItems { get; private set; }
        public ToolStripMenuItem ChartToolSelect { get; private set; }
        public ToolStripMenuItem ChartToolSelect2 { get; private set; }
        public ToolStripMenuItem ChartToolZoom { get; private set; }
        public ToolStripMenuItem ChartToolZoomX { get; private set; }
        public ToolStripMenuItem ChartToolZoomY { get; private set; }
        public ToolStripMenuItem ChartToolZoomDialog { get; private set; }
        public ToolStripMenuItem ChartToolPan { get; private set; }
        public ToolStripMenuItem ChartToolZoomOut { get; private set; }
        public ToolStripSeparator ChartToolZoomOutSeparator { get; private set; }
        public ToolStripSeparator ChartContextSeparator { get; private set; }
        public ToolStripSeparator AboutSeparator { get; private set; }
        public ToolStripMenuItem About { get; private set; }
        private Dictionary<MSChartExtensionToolState, ToolStripMenuItem> StateMenu;

        #endregion

    }

    /// <summary>
    /// Configuration options for <see cref="MSChartExtension"/> 
    /// </summary>
    public class ChartOption
    {
        /// <summary>
        /// Enable / Disable controls in ContextMenu to show / hide series
        /// </summary>
        public bool ContextMenuAllowToHideSeries { get; set; } = true;
        /// <summary>
        /// Round value on XAxis to number of decimal place
        /// </summary>
        public int XAxisPrecision { get; set; } = -999;
        /// <summary>
        /// Round value on YAxis to number of decimal place
        /// </summary>
        public int YAxisPrecision { get; set; } = -999;
        /// <summary>
        /// Cursor 1 Color, default is <see cref="Color.Red"/> 
        /// </summary>
        public Color Cursor1Color { get; set; } = Color.Red;
        /// <summary>
        /// Cursor 2 Color, default is <see cref="Color.Green"/> 
        /// </summary>
        public Color Cursor2Color { get; set; } = Color.Green;
        /// <summary>
        /// Cursor 1 Dash Style, default is <see cref="ChartDashStyle.Solid"/> 
        /// </summary>
        public ChartDashStyle Cursor1DashStyle { get; set; } = ChartDashStyle.Solid;
        /// <summary>
        /// Cursor 2 Dash Style, default is <see cref="ChartDashStyle.Solid"/> 
        /// </summary>
        public ChartDashStyle Cursor2DashStyle { get; set; } = ChartDashStyle.Solid;
        /// <summary>
        /// Cursor 1 Line Width, default = 1
        /// </summary>
        public int Cursor1LineWidth { get; set; } = 1;
        /// <summary>
        /// Cursor 2 Line Width, default = 1
        /// </summary>
        public int Cursor2LineWidth { get; set; } = 1;
    }

    /// <summary>
    /// Extension class for MSChart
    /// </summary>
    public static class MSChartExtension
    {
        private static SeriesChartType[] UnsupportedChartType =
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
                ChartTool[sender] = new ChartData(sender);
                ChartData ptrChartData = ChartTool[sender];
                ptrChartData.Option = (option == null) ? new ChartOption() : option;
                ptrChartData.Backup();
                ptrChartData.PositionChangedCallback = selectionChanged;
                ptrChartData.CursorMovedCallback = cursorMoved;
                ptrChartData.ZoomChangedCallback = zoomChanged;

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
                    ptrChartArea.CursorX.AutoScroll = false;
                    ptrChartArea.CursorX.Interval = 1e-06;
                    ptrChartArea.CursorY.AutoScroll = false;
                    ptrChartArea.CursorY.Interval = 1e-06;

                    ptrChartArea.AxisX.ScrollBar.Enabled = false;
                    ptrChartArea.AxisX2.ScrollBar.Enabled = false;
                    ptrChartArea.AxisY.ScrollBar.Enabled = false;
                    ptrChartArea.AxisY2.ScrollBar.Enabled = false;
                }
                SetChartControlState(sender, MSChartExtensionToolState.Select);
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

            if (ptrChartData.Option.ContextMenuAllowToHideSeries) //Option to show / hide series controls
            {
                SeriesCollection chartSeries = ((Chart)menuStrip.SourceControl).Series;
                if (ptrChartData.ActiveChartArea != null)
                {
                    ToolStripSeparator separator = new ToolStripSeparator();
                    menuStrip.Items.Add(separator);
                    separator.Tag = "Series";

                    foreach (Series ptrSeries in chartSeries)
                    {
                        if (ptrSeries.ChartArea != ptrChartData.ActiveChartArea.Name) continue;
                        ToolStripItem ptrItem = menuStrip.Items.Add(ptrSeries.Name);
                        ToolStripMenuItem ptrMenuItem = (ToolStripMenuItem)ptrItem;
                        ptrMenuItem.Checked = ptrSeries.Enabled;
                        ptrItem.Tag = "Series";
                    }
                }
            }

            if (!ptrChartData.Enabled)
            {
                //Disable Zoom and Pan Controls
                foreach (ToolStripItem item in ptrChartData.MenuItems)
                    item.Enabled = false;
                return;
            }
            else
            {
                foreach (ToolStripItem item in ptrChartData.MenuItems)
                    item.Enabled = true;
            }

            //Check Zoomed state
            if (activeChartArea.AxisX.ScaleView.IsZoomed ||
                activeChartArea.AxisY.ScaleView.IsZoomed ||
                activeChartArea.AxisY2.ScaleView.IsZoomed)
            {
                ptrChartData.ChartToolZoomOut.Visible = true;
            }
            else
            {
                ptrChartData.ChartToolZoomOut.Visible = false;
            }

            //Get Chart Control State
            if (!ChartTool.ContainsKey(senderChart))
            {
                //Initialize Chart Tool
                SetChartControlState(senderChart, MSChartExtensionToolState.Select);
            }

            //Update menu (uncheck all, check current) based on current state.
            ptrChartData.UpdateState();

        }

        private static void ChartContext_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip ptrMenuStrip = (ContextMenuStrip)sender;
            Chart ptrChart = (Chart)ptrMenuStrip.SourceControl;
            ChartArea ptrChartArea = ChartTool[ptrChart].ActiveChartArea;
            if (ptrChartArea == null) return;

            switch (e.ClickedItem.Text)
            {
                case "Select - Cursor 1":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.Select);
                    break;
                case "Select - Cursor 2":
                    SetChartControlState(ptrChart, MSChartExtensionToolState.Select2);
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

        private static Dictionary<Chart, ChartData> ChartTool = new Dictionary<Chart, ChartData>();

        private static void SetChartControlState(Chart sender, MSChartExtensionToolState state)
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

                    //Disabled User Interface, manually drawn.
                    //if (activeChartArea != null)
                    //{
                    //    activeChartArea.CursorX.IsUserEnabled = true;
                    //    activeChartArea.CursorY.IsUserEnabled = true;
                    //}
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
            //Debug.WriteLine(string.Format("{0}, {1}", xPos.ToString(), yPos.ToString()));
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

            Debug.WriteLine("Mouse Wheel Click = " + e.Delta.ToString());
            if (Form.ModifierKeys == Keys.Alt)
            {
                return;
            }
            else if (Form.ModifierKeys == Keys.Shift)
            {
                Debug.WriteLine("SHIFT KEY"); //PAN Along Axis X
                if (ptrChartArea.AxisY.ScaleView.IsZoomed)
                {
                    ScaleViewScroll(ptrChartArea.AxisY2, e.Delta);
                    ScaleViewScroll(ptrChartArea.AxisY, e.Delta);
                }
            }
            else if (Form.ModifierKeys == Keys.Control)
            {
                Debug.WriteLine("CTRL KEY");
                //ToDo: Mouse Wheel Zoom
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
            Debug.WriteLine(string.Format("{0}, {1}", newStart, newEnd));
            ptrView.Zoom(newStart, newEnd);
        }

        private static void ChartControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Chart ptrChart = (Chart)sender;
            ChartData ptrChartData = ChartTool[ptrChart];
            ChartArea ptrChartArea = ChartTool[ptrChart].ActiveChartArea;

            if (ptrChartArea == null) return;
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

                double XStart = ptrChartArea.AxisX.PixelPositionToValue(e.Location.X);
                double YStart = ptrChartArea.AxisY.PixelPositionToValue(e.Location.Y);
                DrawVerticalLine(ptrChart, XStart, cursorColor, "Cursor_1X", 1, ChartDashStyle.Dash, ptrChartArea);
                DrawHorizontalLine(ptrChart, YStart, cursorColor, "Cursor_1Y", 1, ChartDashStyle.Dash, ptrChartArea);
                ptrChartData.Cursor1.X = XStart;
                ptrChartData.Cursor1.Y = YStart;
                ptrChartData.Cursor1.ChartArea = ptrChartArea;

                ptrChartData.PositionChangedCallback?.Invoke(ptrChart, ptrChartData.Cursor1.Clone() as ChartCursor);
            }
            else if (ptrChartData.ToolState == MSChartExtensionToolState.Select2)
            {
                Color cursorColor = ptrChartData.Option.Cursor2Color;

                double XStart = ptrChartArea.AxisX.PixelPositionToValue(e.Location.X);
                double YStart = ptrChartArea.AxisY.PixelPositionToValue(e.Location.Y);
                DrawVerticalLine(ptrChart, XStart, cursorColor, "Cursor_2X", 1, ChartDashStyle.Dash, ptrChartArea);
                DrawHorizontalLine(ptrChart, YStart, cursorColor, "Cursor_2Y", 1, ChartDashStyle.Dash, ptrChartArea);
                ptrChartData.Cursor2.X = XStart;
                ptrChartData.Cursor2.Y = YStart;
                ptrChartData.Cursor2.ChartArea = ptrChartArea;

                ptrChartData.PositionChangedCallback?.Invoke(ptrChart, ptrChartData.Cursor2.Clone() as ChartCursor);
            }
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

            double selX, selY, selX2, selY2;
            selX = selY = selX2 = selY2 = 0;
            try
            {
                selX = ptrChartArea.AxisX.PixelPositionToValue(e.Location.X);
                selY = ptrChartArea.AxisY.PixelPositionToValue(e.Location.Y);
                selX2 = ptrChartArea.AxisX2.PixelPositionToValue(e.Location.X);
                selY2 = ptrChartArea.AxisY2.PixelPositionToValue(e.Location.Y);

                ChartTool[ptrChart].CursorMovedCallback?.Invoke(ptrChart, new ChartCursor() { X = selX, Y = selY, ChartArea = ptrChartArea });
            }
            catch (Exception)
            {
                //Handle exception when scrolled out of range.
                //Set coordinate to 0,0
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
                    if (option.XAxisPrecision != -1)
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
                    if (option.YAxisPrecision != -1)
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
                        ptrChartArea.AxisX.ScaleView.Zoom(left, right);
                        ptrChartArea.AxisX2.ScaleView.Zoom(
                            ptrChartArea.AxisX2.PositionToValue(XMin),
                            ptrChartArea.AxisX2.PositionToValue(XMax));
                    }
                    //Y-Axis
                    if ((state == MSChartExtensionToolState.Zoom) || (state == MSChartExtensionToolState.ZoomY))
                    {
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

        #endregion

        #region [ Series ]

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
        /// <param name="chartArea">Target ChartArea where annotation should be displayed. Default to first ChartArea if not defined.</param>
        public static Annotation DrawHorizontalLine(this Chart sender, double y,
            Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid, ChartArea chartArea = null)
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
        public static Annotation DrawVerticalLine(this Chart sender, double x,
            Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid, ChartArea chartArea = null)
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
        public static Annotation DrawRectangle(this Chart sender, double x, double y,
            double width, double height,
            Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid, ChartArea chartArea = null)
        {
            RectangleAnnotation rect = new RectangleAnnotation();
            ChartArea ptrChartArea = (chartArea == null) ? sender.ChartAreas[0] : chartArea;
            string chartAreaName = ptrChartArea.Name;
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

            Axis ptrAxis = ptrChartArea.AxisX;
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

            ptrAxis = ptrChartArea.AxisY;
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
        public static Annotation DrawLine(this Chart sender, double x0, double x1,
            double y0, double y1, Drawing.Color lineColor, string name = "",
            int lineWidth = 1, ChartDashStyle lineStyle = ChartDashStyle.Solid, ChartArea chartArea = null)
        {
            LineAnnotation line = new LineAnnotation();
            string chartAreaName = (chartArea == null) ? sender.ChartAreas[0].Name : chartArea.Name;
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
        public static Annotation AddText(this Chart sender, string text,
            double x, double y,
            Drawing.Color textColor, string name = "",
            TextStyle textStyle = TextStyle.Default, ChartArea chartArea = null)
        {
            TextAnnotation textAnn = new TextAnnotation();
            string chartAreaName = (chartArea == null) ? sender.ChartAreas[0].Name : chartArea.Name;
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

            return textAnn;
        }

        #endregion

        #region [ Chart Area - Boundaries ]

        /// <summary>
        /// Return the entire chart boundary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="primaryAxis"></param>
        /// <seealso cref="GetChartVisibleAreaBoundary(ChartArea, bool)"/>
        /// <returns></returns>
        public static RectangleF GetChartAreaBoundary(this ChartArea sender, bool primaryAxis = true)
        {
            return GetChartAreaBoundary(sender, primaryAxis, visibleAreaOnly: false);
        }

        /// <summary>
        /// Return chart boundary for visible area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="primaryAxis"></param>
        /// <seealso cref="GetChartAreaBoundary(ChartArea, bool)"/>
        /// <returns></returns>
        public static RectangleF GetChartVisibleAreaBoundary(this ChartArea sender, bool primaryAxis = true)
        {
            return GetChartAreaBoundary(sender, primaryAxis, visibleAreaOnly: true);
        }

        private static RectangleF GetChartAreaBoundary(ChartArea sender, bool primaryAxis, bool visibleAreaOnly)
        {
            RectangleF result = new RectangleF();
            Axis ptrXAxis = primaryAxis ? sender.AxisX : sender.AxisX2;
            Axis ptrYAxis = primaryAxis ? sender.AxisY : sender.AxisY2;

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

        #endregion
    }

}
