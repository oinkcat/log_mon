using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace LogMon.Data
{
    /// <summary>
    /// Counts site request statistics
    /// </summary>
    public class SiteStatsCounter
    {
        private const int MaxDaysInMonth = 31;

        private const int MaxLogReadingThreads = 4;

        private const int MethodFieldMapIndex = 0;
        private const int UriStemFieldMapIndex = 1;
        private const int StatusFieldMapIndex = 2;

        private readonly int[] fieldsMap;

        private readonly string siteLogsDir;

        private readonly HashSet<string> aspNetExts = new HashSet<string>(new [] {
            ".aspx",
            ".asmx",
            ".ashx",
            ".axd"
        });

        /// <summary>
        /// Information of site for stats
        /// </summary>
        public SiteInfo Site { get; }

        public SiteStatsCounter(SiteInfo site, string iisLogsDir)
        {
            Site = site;
            fieldsMap = new int[StatusFieldMapIndex + 1];
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
            var requestStats = new ConcurrentBag<SiteRequestStats>();

            var statDates = Enumerable.Range(0, MaxDaysInMonth + 1)
                .Select(d => start.AddDays(d))
                .TakeWhile(date => date <= end.Date);

            var parOptions = new ParallelOptions { 
                MaxDegreeOfParallelism = MaxLogReadingThreads
            };

            Parallel.ForEach(statDates, parOptions, date => {
                var dailyStats = new SiteRequestStats(Site, date);
                CountDailyStats(dailyStats);

                requestStats.Add(dailyStats);
            });

            return requestStats.ToList();
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
                    else if(fieldName.Equals("sc-status"))
                    {
                        fieldsMap[StatusFieldMapIndex] = i - 1;
                    }
                }
            }
        }

        private void ParseRequestInfo(string infoLine, SiteRequestStats stats)
        {
            string[] infoFields = infoLine.Split();

            string requestUriStem = infoFields[fieldsMap[UriStemFieldMapIndex]];
            string extension = Path.GetExtension(requestUriStem).ToLower();
            bool isGetMethod = infoFields[fieldsMap[MethodFieldMapIndex]].Equals("GET");

            int status = int.Parse(infoFields[fieldsMap[StatusFieldMapIndex]]);

            if(status >= 400)
            {
                stats.ErrorsCount++;
            }
            else if(aspNetExts.Contains(extension))
            {
                stats.AspNetCount++;
            }
            else if(isGetMethod && extension.Equals(String.Empty))
            {
                stats.ActionCount++;
            }
            else if(isGetMethod)
            {
                stats.StaticCount++;
            }
            else
            {
                stats.NonGetCount++;
            }
        }
    }
}