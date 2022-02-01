using System;
using System.Collections.Generic;
using System.Windows;
using LogMon.Data;

namespace LogMon.ViewModels
{
    /// <summary>
    /// Data for grid row displaying statistical info
    /// </summary>
    public class StatRowViewModel
    {
        private const int ColumnsCount = 6;

        private readonly SiteRequestStats requestStats;

        /// <summary>
        /// Stats row display date
        /// </summary>
        public DateTime Date => requestStats.Date;

        /// <summary>
        /// Total requests count
        /// </summary>
        public int TotalCount => requestStats.TotalCount;

        public GridLength[] ColumnSizes { get; }

        public string ErrorText => $"Error requests: {requestStats.StaticCount}";

        public string StaticText => $"Static requests: {requestStats.StaticCount}";

        public string ActionText => $"Action requests: {requestStats.ActionCount}";

        public string AspNetText => $"ASP.NET requests: {requestStats.AspNetCount}";

        public string NonGetText => $"Non GET requests: {requestStats.NonGetCount}";

        public StatRowViewModel(SiteRequestStats stats, int absoluteMaxValue)
        {
            requestStats = stats;
            ColumnSizes = new GridLength[ColumnsCount];

            ComputeColumnSizes(absoluteMaxValue);
        }

        private void ComputeColumnSizes(int absMax)
        {
            double totalRequests = (TotalCount > 0) ? TotalCount : 1;

            int[] statsData = {
                requestStats.ErrorsCount,
                requestStats.StaticCount,
                requestStats.ActionCount,
                requestStats.AspNetCount,
                requestStats.NonGetCount,
                absMax
            };

            for(int i = 0; i < ColumnsCount; i++)
            {
                double colSizePart = Math.Round(statsData[i] / totalRequests, 2);

                if(i == ColumnsCount - 1)
                {
                    colSizePart--;
                }

                ColumnSizes[i] = new GridLength(colSizePart, GridUnitType.Star);
            }
        }
    }
}
