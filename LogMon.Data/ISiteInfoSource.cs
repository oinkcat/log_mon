using System.IO;

namespace LogMon.Data
{
    /// <summary>
    /// Provides raw sites info
    /// </summary>
    public interface ISiteInfoSource
    {
        /// <summary>
        /// Get sites info for parsing
        /// </summary>
        /// <returns>Sites information reader</returns>
         TextReader GetSiteInfoRawData();
    }
}