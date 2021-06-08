using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using LogMon.Data;
using LogMon.SampleData;

namespace LogMon.ViewModels
{
    /// <summary>
    /// Statistics main view
    /// </summary>
    public class LogStatsViewModel : INotifyPropertyChanged
    {
        private Dictionary<int, IList<SiteRequestStats>> dailyStats;

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
        public IList<SiteRequestStats> CurrentSiteStats { get; private set; }

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

            LoadStatistics(startDate, endDate).ContinueWith(_ =>
            {
                // Update UI
                CurrentSite = Sites.FirstOrDefault();

                DateInterval = String.Concat(startDate.ToShortDateString(),
                                             " - ",
                                             endDate.ToShortDateString());
                Ready = true;
            });
        }

        private async Task LoadStatistics(DateTime start, DateTime end)
        {

            IStatsProvider statsProvider = new SampleStatsProvider();
            Sites.AddRange(await statsProvider.GetSites());

            dailyStats = new Dictionary<int, IList<SiteRequestStats>>();

            foreach(var site in Sites)
            {
                var siteStats = await statsProvider.GetSiteStats(site, start, end);
                dailyStats.Add(site.Id, siteStats);
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
