namespace System.Windows.Forms.DataVisualization.Charting
{
    partial class ConfigurationDialog
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.chkAllowToHideSeries = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCursorLabelStringFormat = new System.Windows.Forms.TextBox();
            this.chkShowCursorValue = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btCursor1Color = new System.Windows.Forms.Button();
            this.btCursor2Color = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cbCursor1DashStyle = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbCursor2DashStyle = new System.Windows.Forms.ComboBox();
            this.cursor1LineWidth = new System.Windows.Forms.NumericUpDown();
            this.cursor2LineWidth = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.cbTheme = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SeriesGrid = new System.Windows.Forms.DataGridView();
            this.btCheckNone = new System.Windows.Forms.Button();
            this.btCheckAll = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.colSeriesEnable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colSeries = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSeriesColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSelectColor = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cursor1LineWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cursor2LineWidth)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SeriesGrid)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btCancel);
            this.panel1.Controls.Add(this.btOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 401);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(626, 42);
            this.panel1.TabIndex = 0;
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(561, 8);
            this.btCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(56, 24);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(500, 8);
            this.btOK.Margin = new System.Windows.Forms.Padding(2);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(56, 24);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.BtOK_Click);
            // 
            // chkAllowToHideSeries
            // 
            this.chkAllowToHideSeries.AutoSize = true;
            this.chkAllowToHideSeries.Location = new System.Drawing.Point(9, 17);
            this.chkAllowToHideSeries.Margin = new System.Windows.Forms.Padding(2);
            this.chkAllowToHideSeries.Name = "chkAllowToHideSeries";
            this.chkAllowToHideSeries.Size = new System.Drawing.Size(120, 17);
            this.chkAllowToHideSeries.TabIndex = 1;
            this.chkAllowToHideSeries.Text = "Allow to Hide Series";
            this.chkAllowToHideSeries.UseVisualStyleBackColor = true;
            this.chkAllowToHideSeries.CheckedChanged += new System.EventHandler(this.ChkAllowToHideSeries_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 61);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Curosr 1 Line Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 84);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Curosr 2 Line Width";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 179);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Cursor Label String Format";
            // 
            // txtCursorLabelStringFormat
            // 
            this.txtCursorLabelStringFormat.Location = new System.Drawing.Point(146, 176);
            this.txtCursorLabelStringFormat.Margin = new System.Windows.Forms.Padding(2);
            this.txtCursorLabelStringFormat.Name = "txtCursorLabelStringFormat";
            this.txtCursorLabelStringFormat.Size = new System.Drawing.Size(91, 20);
            this.txtCursorLabelStringFormat.TabIndex = 7;
            // 
            // chkShowCursorValue
            // 
            this.chkShowCursorValue.AutoSize = true;
            this.chkShowCursorValue.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkShowCursorValue.Location = new System.Drawing.Point(47, 154);
            this.chkShowCursorValue.Margin = new System.Windows.Forms.Padding(2);
            this.chkShowCursorValue.Name = "chkShowCursorValue";
            this.chkShowCursorValue.Size = new System.Drawing.Size(116, 17);
            this.chkShowCursorValue.TabIndex = 8;
            this.chkShowCursorValue.Text = "Show Cursor Value";
            this.chkShowCursorValue.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(67, 16);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Cursor 1 Color";
            // 
            // btCursor1Color
            // 
            this.btCursor1Color.BackColor = System.Drawing.SystemColors.Control;
            this.btCursor1Color.Location = new System.Drawing.Point(146, 13);
            this.btCursor1Color.Margin = new System.Windows.Forms.Padding(2);
            this.btCursor1Color.Name = "btCursor1Color";
            this.btCursor1Color.Size = new System.Drawing.Size(90, 20);
            this.btCursor1Color.TabIndex = 11;
            this.btCursor1Color.UseVisualStyleBackColor = false;
            this.btCursor1Color.Click += new System.EventHandler(this.BtCursor1Color_Click);
            // 
            // btCursor2Color
            // 
            this.btCursor2Color.BackColor = System.Drawing.SystemColors.Control;
            this.btCursor2Color.Location = new System.Drawing.Point(146, 34);
            this.btCursor2Color.Margin = new System.Windows.Forms.Padding(2);
            this.btCursor2Color.Name = "btCursor2Color";
            this.btCursor2Color.Size = new System.Drawing.Size(90, 20);
            this.btCursor2Color.TabIndex = 13;
            this.btCursor2Color.UseVisualStyleBackColor = false;
            this.btCursor2Color.Click += new System.EventHandler(this.BtCursor2Color_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(67, 37);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Cursor 2 Color";
            // 
            // cbCursor1DashStyle
            // 
            this.cbCursor1DashStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCursor1DashStyle.FormattingEnabled = true;
            this.cbCursor1DashStyle.Location = new System.Drawing.Point(145, 105);
            this.cbCursor1DashStyle.Margin = new System.Windows.Forms.Padding(2);
            this.cbCursor1DashStyle.Name = "cbCursor1DashStyle";
            this.cbCursor1DashStyle.Size = new System.Drawing.Size(92, 21);
            this.cbCursor1DashStyle.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 107);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Cursor 1 Dash Style";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(41, 132);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Cursor 2 Dash Style";
            // 
            // cbCursor2DashStyle
            // 
            this.cbCursor2DashStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCursor2DashStyle.FormattingEnabled = true;
            this.cbCursor2DashStyle.Location = new System.Drawing.Point(145, 129);
            this.cbCursor2DashStyle.Margin = new System.Windows.Forms.Padding(2);
            this.cbCursor2DashStyle.Name = "cbCursor2DashStyle";
            this.cbCursor2DashStyle.Size = new System.Drawing.Size(92, 21);
            this.cbCursor2DashStyle.TabIndex = 16;
            // 
            // cursor1LineWidth
            // 
            this.cursor1LineWidth.Location = new System.Drawing.Point(146, 59);
            this.cursor1LineWidth.Margin = new System.Windows.Forms.Padding(2);
            this.cursor1LineWidth.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.cursor1LineWidth.Name = "cursor1LineWidth";
            this.cursor1LineWidth.Size = new System.Drawing.Size(90, 20);
            this.cursor1LineWidth.TabIndex = 18;
            // 
            // cursor2LineWidth
            // 
            this.cursor2LineWidth.Location = new System.Drawing.Point(146, 82);
            this.cursor2LineWidth.Margin = new System.Windows.Forms.Padding(2);
            this.cursor2LineWidth.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.cursor2LineWidth.Name = "cursor2LineWidth";
            this.cursor2LineWidth.Size = new System.Drawing.Size(90, 20);
            this.cursor2LineWidth.TabIndex = 19;
            this.cursor2LineWidth.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 21);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Theme";
            // 
            // cbTheme
            // 
            this.cbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTheme.FormattingEnabled = true;
            this.cbTheme.Location = new System.Drawing.Point(65, 18);
            this.cbTheme.Margin = new System.Windows.Forms.Padding(2);
            this.cbTheme.Name = "cbTheme";
            this.cbTheme.Size = new System.Drawing.Size(173, 21);
            this.cbTheme.TabIndex = 21;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cursor2LineWidth);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cursor1LineWidth);
            this.groupBox1.Controls.Add(this.txtCursorLabelStringFormat);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.chkShowCursorValue);
            this.groupBox1.Controls.Add(this.cbCursor2DashStyle);
            this.groupBox1.Controls.Add(this.btCursor1Color);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cbCursor1DashStyle);
            this.groupBox1.Controls.Add(this.btCursor2Color);
            this.groupBox1.Location = new System.Drawing.Point(370, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 204);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cursors";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.SeriesGrid);
            this.groupBox2.Controls.Add(this.btCheckNone);
            this.groupBox2.Controls.Add(this.btCheckAll);
            this.groupBox2.Controls.Add(this.chkAllowToHideSeries);
            this.groupBox2.Location = new System.Drawing.Point(12, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(352, 387);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Series";
            // 
            // SeriesGrid
            // 
            this.SeriesGrid.AllowUserToAddRows = false;
            this.SeriesGrid.AllowUserToDeleteRows = false;
            this.SeriesGrid.AllowUserToResizeColumns = false;
            this.SeriesGrid.AllowUserToResizeRows = false;
            this.SeriesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SeriesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSeriesEnable,
            this.colSeries,
            this.colSeriesColor,
            this.colSelectColor});
            this.SeriesGrid.Location = new System.Drawing.Point(9, 41);
            this.SeriesGrid.Name = "SeriesGrid";
            this.SeriesGrid.RowHeadersVisible = false;
            this.SeriesGrid.Size = new System.Drawing.Size(337, 340);
            this.SeriesGrid.TabIndex = 25;
            this.SeriesGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SeriesGrid_CellContentClick);
            this.SeriesGrid.SelectionChanged += new System.EventHandler(this.SeriesGrid_SelectionChanged);
            // 
            // btCheckNone
            // 
            this.btCheckNone.Location = new System.Drawing.Point(271, 14);
            this.btCheckNone.Name = "btCheckNone";
            this.btCheckNone.Size = new System.Drawing.Size(75, 20);
            this.btCheckNone.TabIndex = 4;
            this.btCheckNone.Text = "NONE";
            this.btCheckNone.UseVisualStyleBackColor = true;
            this.btCheckNone.Click += new System.EventHandler(this.BtCheckNone_Click);
            // 
            // btCheckAll
            // 
            this.btCheckAll.Location = new System.Drawing.Point(193, 14);
            this.btCheckAll.Name = "btCheckAll";
            this.btCheckAll.Size = new System.Drawing.Size(75, 20);
            this.btCheckAll.TabIndex = 3;
            this.btCheckAll.Text = "ALL";
            this.btCheckAll.UseVisualStyleBackColor = true;
            this.btCheckAll.Click += new System.EventHandler(this.BtCheckAll_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbTheme);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(370, 220);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(251, 48);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Theme";
            // 
            // colSeriesEnable
            // 
            this.colSeriesEnable.HeaderText = "";
            this.colSeriesEnable.Name = "colSeriesEnable";
            this.colSeriesEnable.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSeriesEnable.Width = 20;
            // 
            // colSeries
            // 
            this.colSeries.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colSeries.HeaderText = "Series";
            this.colSeries.Name = "colSeries";
            this.colSeries.ReadOnly = true;
            this.colSeries.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSeries.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colSeriesColor
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSeriesColor.DefaultCellStyle = dataGridViewCellStyle1;
            this.colSeriesColor.HeaderText = "Color";
            this.colSeriesColor.Name = "colSeriesColor";
            this.colSeriesColor.ReadOnly = true;
            this.colSeriesColor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colSeriesColor.Width = 60;
            // 
            // colSelectColor
            // 
            this.colSelectColor.HeaderText = "";
            this.colSelectColor.Name = "colSelectColor";
            this.colSelectColor.ReadOnly = true;
            this.colSelectColor.Text = ">>";
            this.colSelectColor.Width = 30;
            // 
            // ConfigurationDialog
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(626, 443);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MSChart Settings";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cursor1LineWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cursor2LineWidth)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SeriesGrid)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Button btCancel;
        private Button btOK;
        private CheckBox chkAllowToHideSeries;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtCursorLabelStringFormat;
        private CheckBox chkShowCursorValue;
        private Label label4;
        private Button btCursor1Color;
        private Button btCursor2Color;
        private Label label5;
        private ComboBox cbCursor1DashStyle;
        private Label label6;
        private Label label7;
        private ComboBox cbCursor2DashStyle;
        private NumericUpDown cursor1LineWidth;
        private NumericUpDown cursor2LineWidth;
        private Label label8;
        private ComboBox cbTheme;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button btCheckNone;
        private Button btCheckAll;
        private GroupBox groupBox3;
        private DataGridView SeriesGrid;
        private DataGridViewCheckBoxColumn colSeriesEnable;
        private DataGridViewTextBoxColumn colSeries;
        private DataGridViewTextBoxColumn colSeriesColor;
        private DataGridViewButtonColumn colSelectColor;
    }
}