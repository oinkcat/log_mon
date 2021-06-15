using System.IO;
using System.Threading.Tasks;
using Xunit;
using LogMon.Data;
using LogMon.Data.IIS;

namespace LogMon.Data.Tests
{
    /// <summary>
    /// IIS server based stats provider tests
    /// </summary>
    public class TestServerSiteStatsProvider
    {
        [Fact]
        public async Task TestOnNonServerSystem()
        {
            IStatsProvider serverStatsProvider = new ServerSiteStatsProvider();

            await Assert.ThrowsAsync<FileNotFoundException>(async() => {
                await serverStatsProvider.GetSites();
            });
        }
    }
}