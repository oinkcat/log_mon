using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogMon.Data.IIS
{
    /// <summary>
    /// Provides sites information and stats from IIS server
    /// </summary>
    public class ServerSiteStatsProvider : IStatsProvider
    {
        private const string AppCmdRelPath = @"inetsrv\appcmd.exe";
        private const string LogsRelPath = @"inetpub\logs\LogFiles";

        private string AppCmdPath
        {
            get => Path.Combine(Environment.SystemDirectory, AppCmdRelPath);
        }

        private string LogsPath
        {
            get => Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), LogsRelPath);
        }

        /// <inheritdoc />
        public async Task<IList<SiteInfo>> GetSites()
        {
            CheckServerComponentsExist();

            var siteInfoSource = new ServerSiteInfoSource(AppCmdPath);
            var siteLister = new SiteInfoReader(siteInfoSource);

            return await Task.Run(() => siteLister.GetSiteInfos());
        }

        /// <inheritdoc />
        public async Task<IList<SiteRequestStats>> GetSiteStats(SiteInfo site,
                                                                DateTime startDate,
                                                                DateTime endDate)
        {
            CheckServerComponentsExist();

            var logStatsCounter = new SiteStatsCounter(site, LogsPath);

            return await Task.Run(() => logStatsCounter.CountStats(startDate, endDate));
        }

        // Check for APPCMD and IIS logs directory, throw if doesn't exist
        private void CheckServerComponentsExist()
        {
            if(!File.Exists(AppCmdPath) || !Directory.Exists(LogsPath))
            {
                throw new FileNotFoundException("Server structues not found");
            }
        }
    }
}