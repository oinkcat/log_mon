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
        private readonly SiteRequestStats requestStats;

        /// <summary>
        /// Statistic item's data
        /// </summary>
        public DateTime Date => requestStats.Date;

        /// <summary>
        /// Total requests count
        /// </summary>
        public int TotalCount => requestStats.TotalCount;

        public GridLength ErrorColLength { get; private set; }

        public GridLength StaticColLength { get; private set; }

        public GridLength ActionColLength { get; private set; }

        public GridLength AspNetColLength { get; private set; }

        public GridLength NonGetColLength { get; private set; }

        public GridLength BlankColLength { get; private set; }

        public string ErrorText => $"Error requests: {requestStats.StaticCount}";

        public string StaticText => $"Static requests: {requestStats.StaticCount}";

        public string ActionText => $"Action requests: {requestStats.ActionCount}";

        public string AspNetText => $"ASP.NET requests: {requestStats.AspNetCount}";

        public string NonGetText => $"Non GET requests: {requestStats.NonGetCount}";

        public StatRowViewModel(SiteRequestStats stats, int absoluteMaxValue)
        {
            requestStats = stats;

            ComputeRates(absoluteMaxValue);
        }

        private void ComputeRates(int absMax)
        {
            double totalRequests = (TotalCount > 0) ? TotalCount : 1;

            double er = Math.Round(requestStats.ErrorsCount / totalRequests, 2);
            ErrorColLength = new GridLength(er, GridUnitType.Star);

            double sr = Math.Round(requestStats.StaticCount / totalRequests, 2);
            StaticColLength = new GridLength(sr, GridUnitType.Star);

            double ar = Math.Round(requestStats.ActionCount / totalRequests, 2);
            ActionColLength = new GridLength(ar, GridUnitType.Star);

            double anr = Math.Round(requestStats.AspNetCount / totalRequests, 2);
            AspNetColLength = new GridLength(anr, GridUnitType.Star);

            double ngr = Math.Round(requestStats.NonGetCount / totalRequests, 2);
            NonGetColLength = new GridLength(ngr, GridUnitType.Star);

            double r = Math.Round(absMax / totalRequests, 2) - 1;
            BlankColLength = new GridLength(r, GridUnitType.Star);
        }
    }
}
