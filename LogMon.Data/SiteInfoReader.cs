using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace LogMon.Data
{
    /// <summary>
    /// Reads site information from some source
    /// </summary>
    public class SiteInfoReader
    {
        private ISiteInfoSource rawDataSource;

        public SiteInfoReader(ISiteInfoSource source) => rawDataSource = source;

        /// <summary>
        /// Get IIS site information from appcmd output
        /// </summary>
        /// <returns>IIS sites basic information</returns>
        public IList<SiteInfo> GetSiteInfos()
        {
            return XDocument.Load(rawDataSource.GetSiteInfoRawData())
                .Root
                .Elements(XName.Get("SITE"))
                .Select(elem => new SiteInfo {
                    Id = int.Parse(elem.Attribute(XName.Get("SITE.ID")).Value),
                    Name = elem.Attribute(XName.Get("SITE.NAME")).Value,
                    IsStarted = elem.Attribute(XName.Get("state")).Value.Equals("Started")
                })
                .ToList();
        }
    }
}