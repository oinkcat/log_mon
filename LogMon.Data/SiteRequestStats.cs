using System;

namespace LogMon.Data
{
    /// <summary>
    /// Requests summary stats for site
    /// </summary>
    public class SiteRequestStats
    {
        /// <summary>
        /// Site which statistics is provided
        /// </summary>
        public SiteInfo Site { get; set; }

        /// <summary>
        /// Date of monitoring
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Count of requests ended with error
        /// </summary>
        public int ErrorsCount { get; set; }

        /// <summary>
        /// Count of GET requests for static files
        /// </summary>
        public int StaticCount { get; set; }

        /// <summary>
        /// Count of GET requests for MVC actions
        /// </summary>
        public int ActionCount { get; set; }

        /// <summary>
        /// Count of ASP.NET specific handler requests
        /// </summary>
        public int AspNetCount { get; set; }

        /// <summary>
        /// Count all rest requests
        /// </summary>
        public int NonGetCount { get; set; }

        /// <summary>
        /// Total count of requests by given day
        /// </summary>
        public int TotalCount => StaticCount + ActionCount + AspNetCount + NonGetCount;

        public SiteRequestStats(SiteInfo site, DateTime date)
        {
            Site = site;
            Date = date;
        }
    }
}