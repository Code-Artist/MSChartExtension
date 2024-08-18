using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// DataPointCollection Extension
    /// </summary>
    public static class DataPointCollectionExtension
    {
        /// <summary>
        /// Add a <see cref="DataPoint"/> to object and assign object to Tag properties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataObject"></param>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <returns></returns>
        public static DataPoint AddXYWithObject(this DataPointCollection sender, object dataObject, object xValues, params object[] yValues)
        {
            int i = sender.AddXY(xValues, yValues);
            DataPoint p = sender[i];
            p.Tag = dataObject;
            return p;
        }

    }
}
