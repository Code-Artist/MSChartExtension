namespace MSChartExtensionDemo
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.item11ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.item12ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.item13ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.item14ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.item2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnPlot = new System.Windows.Forms.ToolStripButton();
            this.btnClearDataFast = new System.Windows.Forms.ToolStripButton();
            this.btnClearDataSlow = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txtChartValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtChartSelect = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblZoomExtents = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart1.Location = new System.Drawing.Point(0, 25);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Red;
            series1.MarkerBorderColor = System.Drawing.Color.Red;
            series1.MarkerColor = System.Drawing.Color.Red;
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Square;
            series1.Name = "Series1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Blue;
            series2.MarkerBorderColor = System.Drawing.Color.Blue;
            series2.MarkerColor = System.Drawing.Color.Blue;
            series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series2.Name = "Series2";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(841, 371);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemToolStripMenuItem,
            this.item2ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 48);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // itemToolStripMenuItem
            // 
            this.itemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.item11ToolStripMenuItem,
            this.item12ToolStripMenuItem,
            this.toolStripSeparator1,
            this.item13ToolStripMenuItem,
            this.item14ToolStripMenuItem});
            this.itemToolStripMenuItem.Name = "itemToolStripMenuItem";
            this.itemToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.itemToolStripMenuItem.Text = "Item 1";
            // 
            // item11ToolStripMenuItem
            // 
            this.item11ToolStripMenuItem.Name = "item11ToolStripMenuItem";
            this.item11ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.item11ToolStripMenuItem.Text = "Item 1.1";
            this.item11ToolStripMenuItem.Click += new System.EventHandler(this.item11ToolStripMenuItem_Click);
            // 
            // item12ToolStripMenuItem
            // 
            this.item12ToolStripMenuItem.Name = "item12ToolStripMenuItem";
            this.item12ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.item12ToolStripMenuItem.Text = "Item 1.2";
            this.item12ToolStripMenuItem.Click += new System.EventHandler(this.item12ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(113, 6);
            // 
            // item13ToolStripMenuItem
            // 
            this.item13ToolStripMenuItem.Name = "item13ToolStripMenuItem";
            this.item13ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.item13ToolStripMenuItem.Text = "Item 1.3";
            this.item13ToolStripMenuItem.Click += new System.EventHandler(this.item13ToolStripMenuItem_Click);
            // 
            // item14ToolStripMenuItem
            // 
            this.item14ToolStripMenuItem.Name = "item14ToolStripMenuItem";
            this.item14ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.item14ToolStripMenuItem.Text = "Item 1.4";
            this.item14ToolStripMenuItem.Click += new System.EventHandler(this.item14ToolStripMenuItem_Click);
            // 
            // item2ToolStripMenuItem
            // 
            this.item2ToolStripMenuItem.Name = "item2ToolStripMenuItem";
            this.item2ToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.item2ToolStripMenuItem.Text = "Item 2";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPlot,
            this.btnClearDataFast,
            this.btnClearDataSlow,
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(841, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnPlot
            // 
            this.btnPlot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPlot.Image = ((System.Drawing.Image)(resources.GetObject("btnPlot.Image")));
            this.btnPlot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlot.Name = "btnPlot";
            this.btnPlot.Size = new System.Drawing.Size(59, 22);
            this.btnPlot.Text = "Plot Data";
            this.btnPlot.Click += new System.EventHandler(this.btnPlot_Click);
            // 
            // btnClearDataFast
            // 
            this.btnClearDataFast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClearDataFast.Image = ((System.Drawing.Image)(resources.GetObject("btnClearDataFast.Image")));
            this.btnClearDataFast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearDataFast.Name = "btnClearDataFast";
            this.btnClearDataFast.Size = new System.Drawing.Size(97, 22);
            this.btnClearDataFast.Text = "Clear Data (Fast)";
            this.btnClearDataFast.Click += new System.EventHandler(this.btnClearDataFast_Click);
            // 
            // btnClearDataSlow
            // 
            this.btnClearDataSlow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClearDataSlow.Image = ((System.Drawing.Image)(resources.GetObject("btnClearDataSlow.Image")));
            this.btnClearDataSlow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearDataSlow.Name = "btnClearDataSlow";
            this.btnClearDataSlow.Size = new System.Drawing.Size(101, 22);
            this.btnClearDataSlow.Text = "Clear Data (Slow)";
            this.btnClearDataSlow.Click += new System.EventHandler(this.btnClearDataSlow_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(96, 22);
            this.toolStripButton1.Text = "Annotation Test";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(106, 22);
            this.toolStripButton2.Text = "Clear Annotations";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(53, 22);
            this.toolStripButton3.Text = "LogTest";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtChartValue,
            this.txtChartSelect,
            this.lblZoomExtents});
            this.statusStrip1.Location = new System.Drawing.Point(0, 396);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(841, 24);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txtChartValue
            // 
            this.txtChartValue.AutoSize = false;
            this.txtChartValue.AutoToolTip = true;
            this.txtChartValue.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.txtChartValue.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.txtChartValue.Name = "txtChartValue";
            this.txtChartValue.Size = new System.Drawing.Size(173, 19);
            this.txtChartValue.Text = "Hover mouse to see point here";
            this.txtChartValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtChartSelect
            // 
            this.txtChartSelect.AutoSize = false;
            this.txtChartSelect.AutoToolTip = true;
            this.txtChartSelect.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.txtChartSelect.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.txtChartSelect.Name = "txtChartSelect";
            this.txtChartSelect.Size = new System.Drawing.Size(133, 19);
            this.txtChartSelect.Text = "Select to see point here";
            this.txtChartSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblZoomExtents
            // 
            this.lblZoomExtents.AutoSize = false;
            this.lblZoomExtents.AutoToolTip = true;
            this.lblZoomExtents.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblZoomExtents.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.lblZoomExtents.Name = "lblZoomExtents";
            this.lblZoomExtents.Size = new System.Drawing.Size(180, 19);
            this.lblZoomExtents.Text = "Zoom to see extents here";
            this.lblZoomExtents.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 420);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "MSChartExtension Demo";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnPlot;
        private System.Windows.Forms.ToolStripButton btnClearDataSlow;
        private System.Windows.Forms.ToolStripButton btnClearDataFast;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel txtChartValue;
        private System.Windows.Forms.ToolStripStatusLabel txtChartSelect;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem itemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem item2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem item11ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem item12ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem item13ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem item14ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripStatusLabel lblZoomExtents;
    }
}

