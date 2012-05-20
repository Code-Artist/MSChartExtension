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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnPlot = new System.Windows.Forms.ToolStripButton();
            this.btnClearDataFast = new System.Windows.Forms.ToolStripButton();
            this.btnClearDataSlow = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txtChartValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtChartSelect = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.item2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.ContextMenuStrip = this.contextMenuStrip1;
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
            this.chart1.Size = new System.Drawing.Size(841, 395);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPlot,
            this.btnClearDataFast,
            this.btnClearDataSlow});
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
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtChartValue,
            this.txtChartSelect});
            this.statusStrip1.Location = new System.Drawing.Point(0, 396);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(841, 24);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txtChartValue
            // 
            this.txtChartValue.AutoSize = false;
            this.txtChartValue.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.txtChartValue.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.txtChartValue.Name = "txtChartValue";
            this.txtChartValue.Size = new System.Drawing.Size(100, 19);
            this.txtChartValue.Text = "99.9999, 99.9999";
            this.txtChartValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtChartSelect
            // 
            this.txtChartSelect.AutoSize = false;
            this.txtChartSelect.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.txtChartSelect.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.txtChartSelect.Name = "txtChartSelect";
            this.txtChartSelect.Size = new System.Drawing.Size(100, 19);
            this.txtChartSelect.Text = "99.9999, 99.9999";
            this.txtChartSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemToolStripMenuItem,
            this.item2ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 48);
            // 
            // itemToolStripMenuItem
            // 
            this.itemToolStripMenuItem.Name = "itemToolStripMenuItem";
            this.itemToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.itemToolStripMenuItem.Text = "Item 1";
            // 
            // item2ToolStripMenuItem
            // 
            this.item2ToolStripMenuItem.Name = "item2ToolStripMenuItem";
            this.item2ToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.item2ToolStripMenuItem.Text = "Item 2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 420);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "MSChartExtension Demo";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
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
    }
}

