﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using LogMon.Data;
using LogMon.Data.IIS;

namespace LogMon.ViewModels
{
    /// <summary>
    /// Statistics main view
    /// </summary>
    public class LogStatsViewModel : INotifyPropertyChanged
    {
        private Dictionary<int, IList<StatRowViewModel>> dailyStats;

        private SiteInfo currentSite;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Available sites for statistics overview
        /// </summary>
        public List<SiteInfo> Sites { get; private set; }

        /// <summary>
        /// Selected site
        /// </summary>
        public SiteInfo CurrentSite
        {
            get => currentSite;
            set
            {
                currentSite = value;
                UpdateStatsView();
            }
        }

        /// <summary>
        /// Selected site's request statistics
        /// </summary>
        public IList<StatRowViewModel> CurrentSiteStats { get; private set; }

        /// <summary>
        /// Date interval for statistics
        /// </summary>
        public string DateInterval { get; private set; }

        /// <summary>
        /// The data is fully loaded
        /// </summary>
        public bool Ready { get; private set; }

        public LogStatsViewModel()
        {
            Sites = new List<SiteInfo>();

            var endDate = DateTime.Now.Date;
            var startDate = endDate.AddMonths(-1);

            LoadStatistics(startDate, endDate).ContinueWith(completedTask =>
            {
                if(completedTask.Status == TaskStatus.RanToCompletion)
                {
                    // Update UI
                    CurrentSite = Sites.FirstOrDefault();

                    DateInterval = String.Concat(startDate.ToShortDateString(),
                                                 " - ",
                                                 endDate.ToShortDateString());
                    Ready = true;
                }
                else
                {
                    AlertError(completedTask.Exception.InnerException);
                }
            });
        }

        private void AlertError(Exception e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var alerter = Application.Current.MainWindow as IUserAlerter;
                alerter.ShowErrorAndExit(e);
            });
        }

        private async Task LoadStatistics(DateTime start, DateTime end)
        {
            IStatsProvider statsProvider = new ServerSiteStatsProvider();
            Sites.AddRange(await statsProvider.GetSites());

            dailyStats = new Dictionary<int, IList<StatRowViewModel>>();

            foreach(var site in Sites)
            {
                var siteDailyStats = await statsProvider.GetSiteStats(site, start, end);
                int maxTotalRequests = siteDailyStats.Max(stat => stat.TotalCount);

                var siteStatsRow = siteDailyStats
                    .Select(stat => new StatRowViewModel(stat, maxTotalRequests))
                    .OrderByDescending(stat => stat.Date)
                    .ToList();

                dailyStats.Add(site.Id, siteStatsRow);
            }
        }

        private void UpdateStatsView()
        {
            CurrentSiteStats = dailyStats[CurrentSite.Id];

            var propChangeArg = new PropertyChangedEventArgs("CurrentSiteStats");
            PropertyChanged?.Invoke(this, propChangeArg);
        }
    }
}
