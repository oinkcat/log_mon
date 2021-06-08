using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogMon.Data
{
    /// <summary>
    /// Provides statistics information for IIS sites
    /// </summary>
    public interface IStatsProvider
    {
        /// <summary>
        /// Get basic information about IIS sites
        /// </summary>
        /// <returns>Site information list</returns>
        Task<IList<SiteInfo>> GetSites();

        /// <summary>
        /// Get site request statistics for given range
        /// </summary>
        /// <param name="site">Site information for statistics</param>
        /// <param name="startDate">Range start date</param>
        /// <param name="endDate">Range end date</param>
        /// <returns>Site daily request information</returns>
        Task<IList<SiteRequestStats>> GetSiteStats(SiteInfo site,
                                                   DateTime startDate,
                                                   DateTime endDate);
    }
}
