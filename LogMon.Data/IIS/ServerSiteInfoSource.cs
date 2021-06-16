using System;
using System.IO;
using System.Diagnostics;

namespace LogMon.Data.IIS
{
    /// <summary>
    /// Provides appcmd output about sites in XML format
    /// </summary>
    public class ServerSiteInfoSource : ISiteInfoSource
    {
        private const string AppCmdArgs = "list site /xml";

        private readonly string appCmdPath;

        public ServerSiteInfoSource(string appcmdPath) => this.appCmdPath = appcmdPath;

        /// <summary>
        /// Get APPCMD output for site info parsing
        /// </summary>
        /// <returns>APPCMD output reader</returns>
        public TextReader GetSiteInfoRawData()
        {
            var appcmdProcess = new Process()
            {
                StartInfo = new ProcessStartInfo(appCmdPath, AppCmdArgs)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            appcmdProcess.Start();

            return appcmdProcess.StandardOutput;
        }
    }
}
