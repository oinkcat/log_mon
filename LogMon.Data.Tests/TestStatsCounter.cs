using System;
using Xunit;

namespace LogMon.Data.Tests
{
    public class TestStatsCounter
    {
        private int TestSiteId = 10;

        private const string LogsDirName = @"..\..\..\TestData\logs";

        private const string StartDate = "2021-05-01";
        private const string EndDate = "2021-05-31";

        [Fact]
        public void TestCountSiteStats()
        {
            var testSiteInfo = new SiteInfo { Id = TestSiteId };
            var counter = new SiteStatsCounter(testSiteInfo, LogsDirName);

            var rangeStart = DateTime.Parse(StartDate);
            var rangeEnd = DateTime.Parse(EndDate);

            var statsByRange = counter.CountStats(rangeStart, rangeEnd);

            Assert.NotEmpty(statsByRange);
            Assert.NotEqual(0, statsByRange[0].TotalCount);
        }
    }
}