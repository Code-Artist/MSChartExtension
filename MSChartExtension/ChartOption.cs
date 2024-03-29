﻿using System.Drawing;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Configuration options for <see cref="MSChartExtension"/> 
    /// </summary>
    [Serializable]
    public class ChartOption
    {
        /// <summary>
        /// Enable / Disable controls in ContextMenu to show / hide series
        /// </summary>
        public bool ContextMenuAllowToHideSeries { get; set; } = true;
        /// <summary>
        /// Round value on XAxis to number of decimal place
        /// </summary>
        public int XAxisPrecision { get; set; } = -1;
        /// <summary>
        /// Round value on YAxis to number of decimal place
        /// </summary>
        public int YAxisPrecision { get; set; } = -1;
        /// <summary>
        /// Cursor 1 Color, default is <see cref="Color.Red"/> 
        /// </summary>
        public Color Cursor1Color { get; set; } = Color.Red;
        /// <summary>
        /// Cursor 1 text color, default is <see cref="Color.White"/>
        /// </summary>
        public Color Cursor1TextColor { get; set; } = Color.White;
        /// <summary>
        /// Cursor 2 Color, default is <see cref="Color.Green"/> 
        /// </summary>
        public Color Cursor2Color { get; set; } = Color.Green;
        /// <summary>
        /// Cursor 2 text color, default is <see cref="Color.White"/>
        /// </summary>
        public Color Cursor2TextColor { get; set; } = Color.White;
        /// <summary>
        /// Cursor 1 Dash Style, default is <see cref="ChartDashStyle.Dash"/> 
        /// </summary>
        public ChartDashStyle Cursor1DashStyle { get; set; } = ChartDashStyle.Dash;
        /// <summary>
        /// Cursor 2 Dash Style, default is <see cref="ChartDashStyle.Dash"/> 
        /// </summary>
        public ChartDashStyle Cursor2DashStyle { get; set; } = ChartDashStyle.Dash;
        /// <summary>
        /// Cursor 1 Line Width, default = 1
        /// </summary>
        public int Cursor1LineWidth { get; set; } = 1;
        /// <summary>
        /// Cursor 2 Line Width, default = 1
        /// </summary>
        public int Cursor2LineWidth { get; set; } = 1;
        /// <summary>
        /// Enable / Disable snap cursor to nearest data points.
        /// This feature is enabled by default.
        /// </summary>
        public bool SnapCursorToData { get; set; } = true;
        
        /// <summary>
        /// Label format for X1
        /// </summary>
        public ChartCursorLabel CursorLabelFormatX1 { get; set; } = new ChartCursorLabel();
        /// <summary>
        /// Label format for X2
        /// </summary>
        public ChartCursorLabel CursorLabelFormatX2 { get; set; } = new ChartCursorLabel();
        /// <summary>
        /// Label format for Y1
        /// </summary>
        public ChartCursorLabel CursorLabelFormatY1 { get; set; } = new ChartCursorLabel();
        /// <summary>
        /// Label format for Y2
        /// </summary>
        public ChartCursorLabel CursorLabelFormatY2 { get; set; } = new ChartCursorLabel();

        /// <summary>
        /// Define string format of cursor value. Default is "F4", 4 digits fixed decimal.
        /// <see cref="double.ToString(string)"/>
        /// Properties split into X1, X2, Y1, Y2 from Version 3.2.0 onwards. Writting to this properties update all 4 properties listed below.
        /// Read from this properties return value from <see cref="CursorLabelStringFormatY1"/>
        /// For cursor moved callback, <see cref="CursorLabelStringFormatX1"/> and <see cref="CursorLabelStringFormatY1"/> will be use to format label string.
        /// <para><see cref="CursorLabelStringFormatX1"/></para>
        /// <para><see cref="CursorLabelStringFormatX2"/></para>
        /// <para><see cref="CursorLabelStringFormatY1"/></para>
        /// <para><see cref="CursorLabelStringFormatY2"/></para>
        /// </summary>
        /// <remarks>More details regarding string format at https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings </remarks>
        [Obsolete("Replaced by CursorLabelFormatX1, CursorLabelFormatX2, CursorLabelFormatY1, CursorLabelFormatY2")]
        public string CursorLabelStringFormat
        {
            get => CursorLabelFormatX1.StringFormat;
            set
            {
                CursorLabelFormatX1.StringFormat = CursorLabelFormatX2.StringFormat = value;
                CursorLabelFormatY1.StringFormat = CursorLabelFormatY2.StringFormat = value;
            }
        }
        /// <summary>
        /// Define string format for cursor value which use X primary axis.
        /// </summary>
        /// <remarks>More details regarding string format at https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings </remarks>
        [Obsolete("Replaced by CursorLabelFormatX1")]
        public string CursorLabelStringFormatX1 { get; set; }
        /// <summary>
        /// Define string format for cursor value which use X secondary axis.
        /// </summary>
        /// <remarks>More details regarding string format at https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings </remarks>
        [Obsolete("Replaced by CursorLabelFormatX2")]
        public string CursorLabelStringFormatX2 { get; set; }
        /// <summary>
        /// Define string format for cursor value which use Y primary axis.
        /// </summary>
        /// <remarks>More details regarding string format at https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings </remarks>
        [Obsolete("Replaced by CursorLabelFormatY1")]
        public string CursorLabelStringFormatY1 { get; set; }
        /// <summary>
        /// Define string format for cursor value which use Y secondary axis.
        /// </summary>
        /// <remarks>More details regarding string format at https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings </remarks>
        [Obsolete("Replaced by CursorLabelFormatY2")]
        public string CursorLabelStringFormatY2 { get; set; }

        /// <summary>
        /// Assign prefix on label string, default is empty
        /// </summary>
        [Obsolete("Replaced by CursorLabelFormatX1")]
        public string CursorLabelPrefixX1 { get; set; }
        /// <summary>
        /// Assign prefix on label string, default is empty
        /// </summary>
        [Obsolete("Replaced by CursorLabelFormatX2")]
        public string CursorLabelPrefixX2 { get; set; }
        /// <summary>
        /// Assign prefix on label string, default is empty
        /// </summary>
        [Obsolete("Replaced by CursorLabelFormatY1")]
        public string CursorLabelPrefixY1 { get; set; }
        /// <summary>
        /// Assign prefix on label string, default is empty
        /// </summary>
        [Obsolete("Replaced by CursorLabelFormatY2")]
        public string CursorLabelPrefixY2 { get; set; }

        /// <summary>
        /// Assign postfix on label string, default is empty
        /// </summary>
        [Obsolete("Replaced by CursorLabelFormatX1")]
        public string CursorLabelPostfixX1 { get; set; }
        /// <summary>
        /// Assign postfix on label string, default is empty
        /// </summary>
        [Obsolete("Replaced by CursorLabelFormatX2")]
        public string CursorLabelPostfixX2 { get; set; }
        /// <summary>
        /// Assign postfix on label string, default is empty
        /// </summary>
        [Obsolete("Replaced by CursorLabelFormatY1")]
        public string CursorLabelPostfixY1 { get; set; }
        /// <summary>
        /// Assign postfix on label string, default is empty
        /// </summary>
        [Obsolete("Replaced by CursorLabelFormatY2")]
        public string CursorLabelPostfixY2 { get; set; }

        /// <summary>
        /// Display cursor value on chart
        /// </summary>
        public bool ShowCursorValue { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public ThemeBase Theme { get; set; } = null;

        /// <summary>
        /// Virtual Display Mode
        /// </summary>
        public bool BufferedMode { get; set; } = false;
        /// <summary>
        /// Number of data to display in <see cref="BufferedMode"/>
        /// Minimum value is 10. Default Value is 500
        /// </summary>
        public int DisplayDataSize
        {
            get => DispDataSize; set
            {
                DispDataSize = value;
                if (DispDataSize < 10) DispDataSize = 10;
            }
        }
        private int DispDataSize = 1000;
    }

}
