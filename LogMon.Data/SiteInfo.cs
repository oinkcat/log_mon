using System;

namespace LogMon.Data
{
    /// <summary>
    /// IIS Site information
    /// </summary>
    public class SiteInfo
    {
        /// <summary>
        /// Site ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Site name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Site working state
        /// </summary>
        public bool IsStarted { get; set; }

        /// <summary>
        /// Get site name and Id in string
        /// </summary>
        /// <returns>Site name and Id</returns>
        public override string ToString()
        {
            string siteState = IsStarted ? "up" : "down";
            return $"{Name}:{Id} ({siteState})";
        }
    }
}
