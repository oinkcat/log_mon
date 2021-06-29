using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using LogMon.Data;
using LogMon.Data.IIS;
using LogMon.DataProviders;

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
        public ObservableCollection<SiteInfo> Sites { get; private set; }

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

        /// <summary>
        /// The data is currently loading
        /// </summary>
        public bool IsLoading => !Ready;

        private IWindowHelpers UIHelpers => (IWindowHelpers)Application.Current.MainWindow;

        public LogStatsViewModel()
        {
            Sites = new ObservableCollection<SiteInfo>();

            var endDate = DateTime.Now.Date;
            var startDate = endDate.AddMonths(-1);

            DateInterval = String.Concat(startDate.ToShortDateString(),
                                         " - ",
                                         endDate.ToShortDateString());

            SetLoadingUiState(true);

            LoadStatistics(startDate, endDate).ContinueWith(completedTask =>
            {
                SetLoadingUiState(false);

                if(completedTask.Status == TaskStatus.RanToCompletion)
                {
                    // Update UI
                    CurrentSite = Sites.FirstOrDefault();
                    NotifyPropChange(nameof(CurrentSite));

                    Ready = true;
                    NotifyPropChange(nameof(Ready));
                    NotifyPropChange(nameof(IsLoading));
                }
                else
                {
                    AlertError(completedTask.Exception.InnerException);
                }
            });
        }

        private void SetLoadingUiState(bool loading)
        {
            Application.Current.Dispatcher.Invoke(() => UIHelpers?.ToggleLoadingState(loading));
        }

        private void AlertError(Exception e)
        {
            Application.Current.Dispatcher.Invoke(() => UIHelpers?.ShowErrorAndExit(e));
        }

        private async Task LoadStatistics(DateTime start, DateTime end)
        {
            var statsProvider = (UIHelpers == null)
                ? new SampleStatsProvider() as IStatsProvider
                : new ServerSiteStatsProvider();

            var allSites = await statsProvider.GetSites();

            dailyStats = new Dictionary<int, IList<StatRowViewModel>>();

            foreach(var site in allSites)
            {
                Sites.Add(site);

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
            NotifyPropChange(nameof(CurrentSiteStats));
        }


        private void NotifyPropChange(string propName)
        {
            var propChangeArg = new PropertyChangedEventArgs(propName);
            PropertyChanged?.Invoke(this, propChangeArg);
        }
    }
}
