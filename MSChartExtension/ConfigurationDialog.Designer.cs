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
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cursor1LineWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cursor2LineWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btCancel);
            this.panel1.Controls.Add(this.btOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 305);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(324, 52);
            this.panel1.TabIndex = 0;
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(237, 10);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 30);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(156, 10);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 30);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.BtOK_Click);
            // 
            // chkAllowToHideSeries
            // 
            this.chkAllowToHideSeries.AutoSize = true;
            this.chkAllowToHideSeries.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAllowToHideSeries.Location = new System.Drawing.Point(53, 11);
            this.chkAllowToHideSeries.Name = "chkAllowToHideSeries";
            this.chkAllowToHideSeries.Size = new System.Drawing.Size(155, 21);
            this.chkAllowToHideSeries.TabIndex = 1;
            this.chkAllowToHideSeries.Text = "Allow to Hide Series";
            this.chkAllowToHideSeries.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Curosr 1 Line Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Curosr 2 Line Width";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 247);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Cursor Label String Format";
            // 
            // txtCursorLabelStringFormat
            // 
            this.txtCursorLabelStringFormat.Location = new System.Drawing.Point(191, 244);
            this.txtCursorLabelStringFormat.Name = "txtCursorLabelStringFormat";
            this.txtCursorLabelStringFormat.Size = new System.Drawing.Size(120, 22);
            this.txtCursorLabelStringFormat.TabIndex = 7;
            // 
            // chkShowCursorValue
            // 
            this.chkShowCursorValue.AutoSize = true;
            this.chkShowCursorValue.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkShowCursorValue.Location = new System.Drawing.Point(58, 217);
            this.chkShowCursorValue.Name = "chkShowCursorValue";
            this.chkShowCursorValue.Size = new System.Drawing.Size(150, 21);
            this.chkShowCursorValue.TabIndex = 8;
            this.chkShowCursorValue.Text = "Show Cursor Value";
            this.chkShowCursorValue.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(86, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Cursor 1 Color";
            // 
            // btCursor1Color
            // 
            this.btCursor1Color.BackColor = System.Drawing.SystemColors.Control;
            this.btCursor1Color.Location = new System.Drawing.Point(191, 38);
            this.btCursor1Color.Name = "btCursor1Color";
            this.btCursor1Color.Size = new System.Drawing.Size(120, 25);
            this.btCursor1Color.TabIndex = 11;
            this.btCursor1Color.UseVisualStyleBackColor = false;
            this.btCursor1Color.Click += new System.EventHandler(this.BtCursor1Color_Click);
            // 
            // btCursor2Color
            // 
            this.btCursor2Color.BackColor = System.Drawing.SystemColors.Control;
            this.btCursor2Color.Location = new System.Drawing.Point(191, 69);
            this.btCursor2Color.Name = "btCursor2Color";
            this.btCursor2Color.Size = new System.Drawing.Size(120, 25);
            this.btCursor2Color.TabIndex = 13;
            this.btCursor2Color.UseVisualStyleBackColor = false;
            this.btCursor2Color.Click += new System.EventHandler(this.BtCursor2Color_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(86, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "Cursor 2 Color";
            // 
            // cbCursor1DashStyle
            // 
            this.cbCursor1DashStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCursor1DashStyle.FormattingEnabled = true;
            this.cbCursor1DashStyle.Location = new System.Drawing.Point(190, 156);
            this.cbCursor1DashStyle.Name = "cbCursor1DashStyle";
            this.cbCursor1DashStyle.Size = new System.Drawing.Size(121, 24);
            this.cbCursor1DashStyle.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(50, 159);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(134, 17);
            this.label6.TabIndex = 15;
            this.label6.Text = "Cursor 1 Dash Style";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(50, 189);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(134, 17);
            this.label7.TabIndex = 17;
            this.label7.Text = "Cursor 2 Dash Style";
            // 
            // cbCursor2DashStyle
            // 
            this.cbCursor2DashStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCursor2DashStyle.FormattingEnabled = true;
            this.cbCursor2DashStyle.Location = new System.Drawing.Point(190, 186);
            this.cbCursor2DashStyle.Name = "cbCursor2DashStyle";
            this.cbCursor2DashStyle.Size = new System.Drawing.Size(121, 24);
            this.cbCursor2DashStyle.TabIndex = 16;
            // 
            // cursor1LineWidth
            // 
            this.cursor1LineWidth.Location = new System.Drawing.Point(191, 100);
            this.cursor1LineWidth.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.cursor1LineWidth.Name = "cursor1LineWidth";
            this.cursor1LineWidth.Size = new System.Drawing.Size(120, 22);
            this.cursor1LineWidth.TabIndex = 18;
            // 
            // cursor2LineWidth
            // 
            this.cursor2LineWidth.Location = new System.Drawing.Point(191, 128);
            this.cursor2LineWidth.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.cursor2LineWidth.Name = "cursor2LineWidth";
            this.cursor2LineWidth.Size = new System.Drawing.Size(120, 22);
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
            this.label8.Location = new System.Drawing.Point(132, 275);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 17);
            this.label8.TabIndex = 20;
            this.label8.Text = "Theme";
            // 
            // cbTheme
            // 
            this.cbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTheme.FormattingEnabled = true;
            this.cbTheme.Location = new System.Drawing.Point(190, 272);
            this.cbTheme.Name = "cbTheme";
            this.cbTheme.Size = new System.Drawing.Size(121, 24);
            this.cbTheme.TabIndex = 21;
            // 
            // ConfigurationDialog
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(324, 357);
            this.Controls.Add(this.cbTheme);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cursor2LineWidth);
            this.Controls.Add(this.cursor1LineWidth);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cbCursor2DashStyle);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbCursor1DashStyle);
            this.Controls.Add(this.btCursor2Color);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btCursor1Color);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkShowCursorValue);
            this.Controls.Add(this.txtCursorLabelStringFormat);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkAllowToHideSeries);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConfigurationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MSChart Settings";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cursor1LineWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cursor2LineWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}