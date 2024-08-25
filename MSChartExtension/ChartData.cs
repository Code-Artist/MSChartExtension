using System.Linq;
using System.Collections.Generic;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Data Storage class for <see cref="MSChartExtension"/>
    /// </summary>
    internal class ChartData
    {
        //Store chart settings. Used to backup and restore chart settings.

        public Chart Source { get; private set; }
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
            ScrollBarXPositionInside = new bool[x];
            ScrollBarX2PositionInside = new bool[x];
            SupportedChartArea = new List<ChartArea>();

            Cursor1 = new ChartCursor() { CursorIndex = 1 };
            Cursor2 = new ChartCursor() { CursorIndex = 2 };
        }

        public MSChartExtensionToolState ToolState { get; set; }
        public CursorPositionChanged PositionChangedCallback;
        public CursorPositionChanged CursorMovedCallback;
        public ZoomChanged ZoomChangedCallback { get; set; }
        public ChartOption Option { get; set; }
        public List<ChartArea> SupportedChartArea { get; set; }
        public bool Enabled { get; set; } = true;
        public ChartArea ActiveChartArea { get; set; }
        /// <summary>
        /// Some series type have X-Axis on vertical bar. Positioning for these axis should be handle in different way.
        /// </summary>
        internal bool InvertedAxis { get; set; }
        public ChartCursor Cursor1 { get; private set; }
        public ChartCursor Cursor2 { get; private set; }
        public List<SeriesDataBuffer> SeriesData { get; set; } = new List<SeriesDataBuffer>();
        public List<ResourceSeries> ResourceSeries { get; set; } = null;
        public List<AxisSettings> AxisSettings { get; set; } = new List<AxisSettings>();
        private void CreateChartContextMenu()
        {
            ChartToolZoomOut = new ToolStripMenuItem("Zoom Out");
            ChartToolZoom = new ToolStripMenuItem("Zoom Window");
            ChartToolZoomX = new ToolStripMenuItem("Zoom XAxis");
            ChartToolZoomY = new ToolStripMenuItem("Zoom YAxis");
            ChartToolZoomDialog = new ToolStripMenuItem("Zoom Dialog...");
            ChartToolPan = new ToolStripMenuItem("Pan");
            ChartToolZoomOutSeparator = new ToolStripSeparator();
            ChartToolSelect = new ToolStripMenuItem("Select - Cursor 1") { Tag = this };
            ChartToolSelect2 = new ToolStripMenuItem("Select - Cursor 2") { Tag = this };
            ChartToolClearCursor = new ToolStripMenuItem("Clear Cursors...");
            AboutSeparator = new ToolStripSeparator() { Name = "AboutMenu" };
            Clear = new ToolStripMenuItem("Clear");
            Settings = new ToolStripMenuItem("Settings...");
            About = new ToolStripMenuItem("About...") { Name = "About" };
            About.Image = Properties.Resources.MSChartExtensionLogo;

            MenuItems = new List<ToolStripItem>
            {
                ChartToolZoomOut,
                ChartToolZoom,
                ChartToolZoomX,
                ChartToolZoomY,
                ChartToolZoomDialog,
                ChartToolPan,
                ChartToolZoomOutSeparator,
                ChartToolSelect,
                ChartToolSelect2,
                ChartToolClearCursor,
                AboutSeparator,
                Clear,
                Settings,
                About
            };

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

        public void ResetAxisSettings(Axis axis)
        {
            AxisSettings ptrSettings = AxisSettings.FirstOrDefault(n => n.Axis == axis);
            ptrSettings?.Restore();
        }

        /// <summary>
        /// Backtup properties for all Chart Area
        /// </summary>
        public void Backup()
        {
            ContextMenuStrip = Source.ContextMenuStrip;
            AxisSettings.Clear();
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
                ScrollBarXPositionInside[x] = ptrChartArea.AxisX.ScrollBar.IsPositionedInside;
                ScrollBarX2PositionInside[x] = ptrChartArea.AxisX2.ScrollBar.IsPositionedInside;
                x++;

                AxisSettings.Add(new AxisSettings(ptrChartArea.AxisX));
                AxisSettings.Add(new AxisSettings(ptrChartArea.AxisY));
                AxisSettings.Add(new AxisSettings(ptrChartArea.AxisX2));
                AxisSettings.Add(new AxisSettings(ptrChartArea.AxisY2));
            }

            foreach (AxisSettings s in AxisSettings) s.Backup();
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
                ptrChartArea.AxisX.ScrollBar.IsPositionedInside = ScrollBarXPositionInside[x];
                ptrChartArea.AxisX2.ScrollBar.IsPositionedInside = ScrollBarX2PositionInside[x];
                x++;
            }
            foreach (AxisSettings s in AxisSettings) s.Restore();
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
        private readonly bool[] CursorXUserEnabled;
        private readonly bool[] CursorYUserEnabled;
        private readonly Forms.Cursor[] Cursor;
        private readonly double[] CursorXInterval, CursorYInterval;
        private readonly bool[] CursorXAutoScroll, CursorYAutoScroll;
        private readonly bool[] ScrollBarX, ScrollBarX2, ScrollBarY, ScrollBarY2;
        private readonly bool[] ScrollBarXPositionInside, ScrollBarX2PositionInside;

        #endregion

        #region [ Extended Context Menu ]

        public List<ToolStripItem> MenuItems { get; private set; }
        public ToolStripMenuItem ChartToolSelect { get; private set; }
        public ToolStripMenuItem ChartToolSelect2 { get; private set; }
        public ToolStripMenuItem ChartToolClearCursor { get; private set; }
        public ToolStripMenuItem ChartToolZoom { get; private set; }
        public ToolStripMenuItem ChartToolZoomX { get; private set; }
        public ToolStripMenuItem ChartToolZoomY { get; private set; }
        public ToolStripMenuItem ChartToolZoomDialog { get; private set; }
        public ToolStripMenuItem ChartToolPan { get; private set; }
        public ToolStripMenuItem ChartToolZoomOut { get; private set; }
        public ToolStripSeparator ChartToolZoomOutSeparator { get; private set; }
        public ToolStripSeparator ChartContextSeparator { get; private set; }
        public ToolStripSeparator AboutSeparator { get; private set; }
        public ToolStripMenuItem Clear { get; private set; }
        public ToolStripMenuItem Settings { get; private set; }
        public ToolStripMenuItem About { get; private set; }
        private Dictionary<MSChartExtensionToolState, ToolStripMenuItem> StateMenu;

        #endregion

    }

    internal class AxisSettings
    {
        public Axis Axis { get; private set; }

        public double Interval { get; set; }
        public IntervalAutoMode IntervalAutoMode { get; set; }
        public double IntervalOffset { get; set; }
        public DateTimeIntervalType IntervalOffsetType { get; set; }
        public double MajorTickMarkOffset { get; set; }
        public double MinorTickMarkOffset { get; set; }

        public AxisSettings(Axis axis)
        {
            Axis = axis;
            Backup();
        }

        public void Backup()
        {
            Interval = Axis.Interval;
            IntervalAutoMode = Axis.IntervalAutoMode;
            IntervalOffset = Axis.IntervalOffset;
            IntervalOffsetType = Axis.IntervalOffsetType;
            MajorTickMarkOffset = Axis.MajorTickMark.IntervalOffset;
            MinorTickMarkOffset = Axis.MinorTickMark.IntervalOffset;
        }

        public void Restore()
        {
            Axis.Interval = Interval;
            Axis.IntervalAutoMode = IntervalAutoMode;
            Axis.IntervalOffset = IntervalOffset;
            Axis.IntervalOffsetType = IntervalOffsetType;
            Axis.MajorTickMark.IntervalOffset = MajorTickMarkOffset;
            Axis.MinorTickMark.IntervalOffset = MinorTickMarkOffset;
        }
    }

}
