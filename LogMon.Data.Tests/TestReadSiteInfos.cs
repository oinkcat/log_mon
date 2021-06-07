using System;
using Xunit;

namespace LogMon.Data.Tests
{
    public class TestReadSiteInfos
    {
        [Fact]
        public void TestGetSiteInfosFromFile()
        {
            var infoSource = new TestFileSiteInfoSouce();
            var infoReader = new SiteInfoReader(infoSource);

            var siteInfos = infoReader.GetSiteInfos();

            Assert.NotEmpty(siteInfos);
            Assert.False(siteInfos[0].IsStarted);
        }
    }
}
