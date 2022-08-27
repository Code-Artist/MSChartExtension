using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Chart cursor label format
    /// </summary>
    public class ChartCursorLabel
    {
        /// <summary>
        /// Label value format
        /// </summary>
        /// <remarks>More details regarding string format at https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings </remarks>
        public string StringFormat { get; set; }
        /// <summary>
        /// Label prefix
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// Label postfix
        /// </summary>
        public string Postfix { get; set; }
        /// <summary>
        /// Show / hide labels
        /// </summary>
        public bool Visible { get; set; } = true;
    }
}
