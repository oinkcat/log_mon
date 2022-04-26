using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogMon.Data;

namespace LogMon.DataProviders
{
    /// <summary>
    /// Provides sample statistics data
    /// </summary>
    public class SampleStatsProvider : IStatsProvider
    {
        private const int MaxRequestsCount = 1000;

        private const int ErrorRate = 10;

        private readonly SiteInfo[] sampleData = {
            new SiteInfo { Id = 1, Name = "Sample site", IsStarted = true },
            new SiteInfo { Id = 2, Name = "Web site" },
            new SiteInfo { Id = 5, Name = "Internal site", IsStarted = true },
            new SiteInfo { Id = 7, Name = "Admin web app" },
        };

        /// <inheritdoc />
        public Task<IList<SiteInfo>> GetSites()
        {
            return Task.FromResult(sampleData as IList<SiteInfo>);
        }

        /// <inheritdoc />
        public Task<IList<SiteRequestStats>> GetSiteStats(SiteInfo site, 
                                                          DateTime startDate,
                                                          DateTime endDate)
        {
            var rng = new Random();
            int dateRangeLength = (int)endDate.Subtract(startDate).TotalDays + 1;

            var randomStatData = Enumerable.Range(0, dateRangeLength)
                .Select(d => new SiteRequestStats(site, startDate.AddDays(d))
                {
                    ErrorsCount = rng.Next(MaxRequestsCount) / ErrorRate,
                    StaticCount = rng.Next(MaxRequestsCount),
                    ActionCount = rng.Next(MaxRequestsCount),
                    AspNetCount = rng.Next(MaxRequestsCount),
                    NonGetCount = rng.Next(MaxRequestsCount)
                })
                .ToList();

            return Task.FromResult(randomStatData as IList<SiteRequestStats>);
        }
    }
}
