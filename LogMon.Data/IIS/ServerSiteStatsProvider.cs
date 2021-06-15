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
        private const string AppCmdRelPath = @"\inetsrv\appcmd.exe";
        private const string LogsRelPath = @"inetsrv\logs";

        private string AppCmdPath
        {
            get => Path.Combine(Environment.SystemDirectory, AppCmdRelPath);
        }

        private string LogsPath
        {
            get => Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), LogsPath);
        }

        /// <inheritdoc />
        public async Task<IList<SiteInfo>> GetSites()
        {
            CheckServerFilesExist();

            return new List<SiteInfo>();
        }

        /// <inheritdoc />
        public async Task<IList<SiteRequestStats>> GetSiteStats(SiteInfo site,
                                                                DateTime startDate,
                                                                DateTime endDate)
        {
            CheckServerFilesExist();

            return new List<SiteRequestStats>();
        }

        private void CheckServerFilesExist()
        {
            if(!File.Exists(AppCmdPath) || !Directory.Exists(LogsPath))
            {
                throw new FileNotFoundException("Server structues not found");
            }
        }
    }
}