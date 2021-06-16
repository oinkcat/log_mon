using System;
using System.IO;
using System.Collections.Generic;

namespace LogMon.Data
{
    /// <summary>
    /// Counts site request statistics
    /// </summary>
    public class SiteStatsCounter
    {
        private const int MethodFieldMapIndex = 0;
        private const int UriStemFieldMapIndex = 1;

        private readonly int[] fieldsMap;

        private readonly string siteLogsDir;

        /// <summary>
        /// Information of site for stats
        /// </summary>
        public SiteInfo Site { get; }

        public SiteStatsCounter(SiteInfo site, string iisLogsDir)
        {
            Site = site;
            fieldsMap = new int[UriStemFieldMapIndex + 1];
            siteLogsDir = Path.Combine(iisLogsDir, $"W3SVC{site.Id}");
        }

        /// <summary>
        /// Count site request statistics
        /// </summary>
        /// <param name="start">Stats interval start date</param>
        /// <param name="end">Stats interval end date</param>
        /// <returns>Site request statistics by days</returns>
        public IList<SiteRequestStats> CountStats(DateTime start, DateTime end)
        {
            var requestStats = new List<SiteRequestStats>();

            for(var date = start.Date; date <= end.Date; date = date.AddDays(1))
            {
                var dailyStats = new SiteRequestStats(Site, date);
                CountDailyStats(dailyStats);

                requestStats.Add(dailyStats);
            }

            return requestStats;
        }

        private void CountDailyStats(SiteRequestStats stats)
        {
            string logFileName = $"u_ex{stats.Date:yyMMdd}.log";
            string logPath = Path.Combine(siteLogsDir, logFileName);

            if(File.Exists(logPath))
            {
                using (var logStream = new FileStream(logPath, 
                                                      FileMode.Open, 
                                                      FileAccess.Read, 
                                                      FileShare.ReadWrite))
                using (var lineReader = new StreamReader(logStream))
                {
                    while(!lineReader.EndOfStream)
                    {
                        string logLine = lineReader.ReadLine();

                        if (logLine.StartsWith("#"))
                        {
                            ParseComment(logLine.Substring(1));
                        }
                        else if (!String.IsNullOrWhiteSpace(logLine))
                        {
                            ParseRequestInfo(logLine, stats);
                        }
                    }
                }
            }
        }

        private void ParseComment(string commentLine)
        {
            if(commentLine.StartsWith("Fields"))
            {
                string[] fieldNames = commentLine.Split();

                for(int i = 1; i < fieldNames.Length; i++)
                {
                    string fieldName = fieldNames[i];

                    if(fieldName.Equals("cs-method"))
                    {
                        fieldsMap[MethodFieldMapIndex] = i - 1;
                    }
                    else if(fieldName.Equals("cs-uri-stem"))
                    {
                        fieldsMap[UriStemFieldMapIndex] = i - 1;
                    }
                }
            }
        }

        private void ParseRequestInfo(string infoLine, SiteRequestStats stats)
        {
            string[] infoFields = infoLine.Split();

            if(infoFields[fieldsMap[MethodFieldMapIndex]].Equals("GET"))
            {
                string requestUriStem = infoFields[fieldsMap[UriStemFieldMapIndex]];

                int lastDotPos = requestUriStem.LastIndexOf('.');
                int lastSlashPos = requestUriStem.LastIndexOf('/');

                if(lastDotPos > lastSlashPos)
                {
                    stats.StaticCount++;
                }
                else
                {
                    stats.ActionCount++;
                }
            }
            else
            {
                stats.NonGetCount++;
            }
        }
    }
}