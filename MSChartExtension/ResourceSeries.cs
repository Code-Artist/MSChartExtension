using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Forms.DataVisualization.Charting
{
    /// <summary>
    /// Resource utilization chart series
    /// </summary>
    public class ResourceSeries
    {
        /// <summary>
        /// Series used for this Resource Chart.
        /// </summary>
        public Series BaseSeries { get; private set; }
        /// <summary>
        /// Chart area where this series is belongs to.
        /// </summary>
        public ChartArea ChartArea { get; private set; }
        internal ResourceSeries(ChartArea chartArea, Series series)
        {
            ChartArea = chartArea;
            BaseSeries = series;
            ChartArea.AxisX.LabelStyle.Interval = 1;
        }

        private List<ResourceRow> Rows { get; set; } = new List<ResourceRow>();

        /// <summary>
        /// Get or add Row data.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ResourceRow Row(string name)
        {
            ResourceRow result = Rows.FirstOrDefault(n => n.Name == name);
            if (result == null)
            {
                result = new ResourceRow(name, BaseSeries, Rows.Count);
                Rows.Add(result);
            }
            return result;
        }

        /// <summary>
        /// Clear all data points.
        /// </summary>
        public void Clear()
        {
            BaseSeries.ClearPoints();
            Rows.Clear();
        }
    }

    /// <summary>
    /// Resource series data row, one row for each X index from 0 to N
    /// </summary>
    public class ResourceRow
    {
        /// <summary>
        /// Data Row name
        /// </summary>
        public string Name { get; private set; }
        public int RowIndex { get; private set; }
        internal Series Series { get; private set; }
        internal ResourceRow(string name, Series series, int rowIndex)
        {
            Name = name;
            Series = series;
            RowIndex = rowIndex;
        }

        /// <summary>
        /// Add data to current data row.
        /// </summary>
        /// <param name="blocks"></param>
        public void AddTimeBlocks(params TimeBlock[] blocks)
        {
            foreach (TimeBlock t in blocks)
            {
                int d = Series.Points.AddXY(RowIndex, t.StartTime, t.EndTime);
                Series.Points[d].AxisLabel = Name;
                
            }
        }

    }
}
