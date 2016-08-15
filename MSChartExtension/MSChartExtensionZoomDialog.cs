using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace System.Windows.Forms.DataVisualization.Charting
{
    internal partial class MSChartExtensionZoomDialog : Form
    {
        private ChartArea ptrChartArea;
        private Axis ptrXAxis, ptrYAxis;
        public MSChartExtensionZoomDialog(ChartArea sender)
        {
            InitializeComponent();
            ptrChartArea = sender;
            cbAxisType.SelectedIndex = 0;
            cbAxisType_SelectedIndexChanged(this, null);
        }

        private void cbAxisType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbAxisType.SelectedIndex)
            {
                case 0: //Primary Axis
                    ptrXAxis = ptrChartArea.AxisX;
                    ptrYAxis = ptrChartArea.AxisY;
                    break;

                case 1:  //Secondary Axis
                    ptrXAxis = ptrChartArea.AxisX2;
                    ptrYAxis = ptrChartArea.AxisY2;
                    break;
            }
            txtXLimit.Text = string.Format("[{0} to {1}]",
                FormatDouble(ptrXAxis.Minimum),
                FormatDouble(ptrXAxis.Maximum));

            txtYLimit.Text = string.Format("[{0} to {1}]",
                FormatDouble(ptrYAxis.Minimum),
                FormatDouble(ptrYAxis.Maximum));

            txtXMin.Text = ptrXAxis.ScaleView.ViewMinimum.ToString();
            txtXMax.Text = ptrXAxis.ScaleView.ViewMaximum.ToString();
            txtYMin.Text = ptrYAxis.ScaleView.ViewMinimum.ToString();
            txtYMax.Text = ptrYAxis.ScaleView.ViewMaximum.ToString();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            bool inputValid = true;

            //Sanity Check
            if (!ValidateInput(txtXMin)) { inputValid = false; }
            if (!ValidateInput(txtXMax)) { inputValid = false; }
            if (!ValidateInput(txtYMin)) { inputValid = false; }
            if (!ValidateInput(txtYMax)) { inputValid = false; }
            if (!inputValid)
            {
                MessageBox.Show("Invalid input values!", this.Text,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Limit Check
            double xStart = Convert.ToDouble(txtXMin.Text);
            double xEnd = Convert.ToDouble(txtXMax.Text);
            double yStart = Convert.ToDouble(txtYMin.Text);
            double yEnd = Convert.ToDouble(txtYMax.Text);

            //Perform ZOOM
            double XMin = ptrXAxis.ValueToPixelPosition(xStart);
            double XMax = ptrXAxis.ValueToPixelPosition(xEnd);
            double YMin = ptrYAxis.ValueToPixelPosition(yStart);
            double YMax = ptrYAxis.ValueToPixelPosition(yEnd);

            ptrXAxis.ScaleView.Zoom(xStart, xEnd);
            ptrYAxis.ScaleView.Zoom(yStart, yEnd);

            //Swtich to next axis
            ptrXAxis = (ptrXAxis == ptrChartArea.AxisX) ? ptrChartArea.AxisX2 : ptrChartArea.AxisX;
            ptrYAxis = (ptrYAxis == ptrChartArea.AxisY) ? ptrChartArea.AxisY2 : ptrChartArea.AxisY;
            ptrXAxis.ScaleView.Zoom(ptrXAxis.PixelPositionToValue(XMin), ptrXAxis.PixelPositionToValue(XMax));
            ptrYAxis.ScaleView.Zoom(ptrYAxis.PixelPositionToValue(YMin), ptrYAxis.PixelPositionToValue(YMax));

            DialogResult = DialogResult.OK;
        }

        private bool ValidateInput(TextBox sender)
        {
            double result;
            bool valid = double.TryParse(sender.Text, out result);
            sender.BackColor = valid ? Color.FromKnownColor(KnownColor.Window) : Color.FromArgb(255, 192, 192);
            return valid;
        }

        private string FormatDouble(double number)
        {
            double numberRange = Math.Abs(number);
            if (numberRange < 0)
                return number.ToString("0.0000");
            else if (numberRange < 10)
                return number.ToString("0.0000");
            else if (numberRange < 100)
                return number.ToString("0.00");
            else if (numberRange < 1000)
                return number.ToString("0.0");
            else
                return number.ToString("0");
        }
    }
}
