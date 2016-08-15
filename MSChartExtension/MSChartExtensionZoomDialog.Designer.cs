namespace System.Windows.Forms.DataVisualization.Charting
{
    partial class MSChartExtensionZoomDialog
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
            this.txtXMin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtYMin = new System.Windows.Forms.TextBox();
            this.txtYMax = new System.Windows.Forms.TextBox();
            this.txtXMax = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtXLimit = new System.Windows.Forms.Label();
            this.txtYLimit = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.cbAxisType = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtXMin
            // 
            this.txtXMin.Location = new System.Drawing.Point(31, 62);
            this.txtXMin.Name = "txtXMin";
            this.txtXMin.Size = new System.Drawing.Size(69, 20);
            this.txtXMin.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Y";
            // 
            // txtYMin
            // 
            this.txtYMin.Location = new System.Drawing.Point(31, 88);
            this.txtYMin.Name = "txtYMin";
            this.txtYMin.Size = new System.Drawing.Size(69, 20);
            this.txtYMin.TabIndex = 3;
            // 
            // txtYMax
            // 
            this.txtYMax.Location = new System.Drawing.Point(106, 88);
            this.txtYMax.Name = "txtYMax";
            this.txtYMax.Size = new System.Drawing.Size(69, 20);
            this.txtYMax.TabIndex = 4;
            // 
            // txtXMax
            // 
            this.txtXMax.Location = new System.Drawing.Point(106, 62);
            this.txtXMax.Name = "txtXMax";
            this.txtXMax.Size = new System.Drawing.Size(69, 20);
            this.txtXMax.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Min";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(125, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Max";
            // 
            // txtXLimit
            // 
            this.txtXLimit.AutoSize = true;
            this.txtXLimit.Location = new System.Drawing.Point(181, 65);
            this.txtXLimit.Name = "txtXLimit";
            this.txtXLimit.Size = new System.Drawing.Size(68, 13);
            this.txtXLimit.TabIndex = 8;
            this.txtXLimit.Text = "[XAxis Limits]";
            // 
            // txtYLimit
            // 
            this.txtYLimit.AutoSize = true;
            this.txtYLimit.Location = new System.Drawing.Point(181, 91);
            this.txtYLimit.Name = "txtYLimit";
            this.txtYLimit.Size = new System.Drawing.Size(71, 13);
            this.txtYLimit.TabIndex = 9;
            this.txtYLimit.Text = "[YAxis Limits ]";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btCancel);
            this.panel1.Controls.Add(this.btOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 123);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(323, 38);
            this.panel1.TabIndex = 5;
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(239, 8);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.Location = new System.Drawing.Point(158, 8);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // cbAxisType
            // 
            this.cbAxisType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAxisType.FormattingEnabled = true;
            this.cbAxisType.Items.AddRange(new object[] {
            "Primary Axis",
            "Secondary Axis"});
            this.cbAxisType.Location = new System.Drawing.Point(14, 12);
            this.cbAxisType.Name = "cbAxisType";
            this.cbAxisType.Size = new System.Drawing.Size(161, 21);
            this.cbAxisType.TabIndex = 0;
            this.cbAxisType.SelectedIndexChanged += new System.EventHandler(this.cbAxisType_SelectedIndexChanged);
            // 
            // MSChartExtensionZoomDialog
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(323, 161);
            this.Controls.Add(this.cbAxisType);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtYLimit);
            this.Controls.Add(this.txtXLimit);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtYMax);
            this.Controls.Add(this.txtXMax);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtYMin);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtXMin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MSChartExtensionZoomDialog";
            this.Text = "Zoom Settings...";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox txtXMin;
        private Label label1;
        private Label label2;
        private TextBox txtYMin;
        private TextBox txtYMax;
        private TextBox txtXMax;
        private Label label3;
        private Label label4;
        private Label txtXLimit;
        private Label txtYLimit;
        private Panel panel1;
        private Button btCancel;
        private Button btOK;
        private ComboBox cbAxisType;
    }
}