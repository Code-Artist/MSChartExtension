namespace System.Windows.Forms.DataVisualization.Charting
{
    partial class LabelFormatControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkVisible = new System.Windows.Forms.CheckBox();
            this.LbName = new System.Windows.Forms.Label();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.txtFormat = new System.Windows.Forms.TextBox();
            this.txtPostfix = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // chkVisible
            // 
            this.chkVisible.AutoSize = true;
            this.chkVisible.Location = new System.Drawing.Point(239, 6);
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.Size = new System.Drawing.Size(56, 17);
            this.chkVisible.TabIndex = 0;
            this.chkVisible.Text = "Visible";
            this.chkVisible.UseVisualStyleBackColor = true;
            // 
            // LbName
            // 
            this.LbName.AutoSize = true;
            this.LbName.Location = new System.Drawing.Point(3, 6);
            this.LbName.Name = "LbName";
            this.LbName.Size = new System.Drawing.Size(20, 13);
            this.LbName.TabIndex = 1;
            this.LbName.Text = "X1";
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(29, 3);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(46, 20);
            this.txtPrefix.TabIndex = 3;
            // 
            // txtFormat
            // 
            this.txtFormat.Location = new System.Drawing.Point(78, 3);
            this.txtFormat.Name = "txtFormat";
            this.txtFormat.Size = new System.Drawing.Size(106, 20);
            this.txtFormat.TabIndex = 4;
            // 
            // txtPostfix
            // 
            this.txtPostfix.Location = new System.Drawing.Point(187, 3);
            this.txtPostfix.Name = "txtPostfix";
            this.txtPostfix.Size = new System.Drawing.Size(46, 20);
            this.txtPostfix.TabIndex = 5;
            // 
            // LabelFormatControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.txtPostfix);
            this.Controls.Add(this.txtFormat);
            this.Controls.Add(this.txtPrefix);
            this.Controls.Add(this.LbName);
            this.Controls.Add(this.chkVisible);
            this.Name = "LabelFormatControl";
            this.Size = new System.Drawing.Size(298, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox chkVisible;
        private Label LbName;
        private TextBox txtPrefix;
        private TextBox txtFormat;
        private TextBox txtPostfix;
    }
}
