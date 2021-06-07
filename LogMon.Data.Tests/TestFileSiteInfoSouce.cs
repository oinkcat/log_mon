using System.IO;

namespace LogMon.Data.Tests
{
    /// <summary>
    /// Provides sites information from test xml file
    /// </summary>
    public class TestFileSiteInfoSouce : ISiteInfoSource
    {
        private const string TestFilePath = @"..\..\..\TestData\sites_test.xml";

        public TextReader GetSiteInfoRawData() => File.OpenText(TestFilePath);
    }
}