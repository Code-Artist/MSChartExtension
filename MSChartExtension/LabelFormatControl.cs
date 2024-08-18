namespace System.Windows.Forms.DataVisualization.Charting
{
    internal partial class LabelFormatControl : UserControl
    {
        public LabelFormatControl()
        {
            InitializeComponent();
        }

        public string LabelName { get => LbName.Text; set => LbName.Text = value; }

        public void SetContent(ChartCursorLabel sender)
        {
            txtPrefix.Text = sender.Prefix;
            txtFormat.Text = sender.StringFormat;
            txtPostfix.Text = sender.Postfix;
            chkVisible.Checked = sender.Visible;
        }

        public ChartCursorLabel GetLabelContent()
        {
            ChartCursorLabel result = new ChartCursorLabel();
            result.Prefix = txtPrefix.Text;
            result.StringFormat = txtFormat.Text;
            result.Postfix = txtPostfix.Text;
            result.Visible = chkVisible.Checked;
            return result;
        }
    }
}
