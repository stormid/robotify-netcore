using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Robotify.AspNetCore
{
    public sealed class RobotifyOptions
    {
        public bool Enabled { get; set; } = true;
        public string RobotsFilePath { get; set; } = "/robots.txt";

        public IList<RobotsGroup> Groups { get; set; } = new List<RobotsGroup>();

        public int? CrawlDelay { get; set; }
        
        public Uri SitemapUrl { get; set; }
         
        public RobotifyOptions Add(RobotsGroup group)
        {
            Groups.Add(group);
            return this;
        }

        public bool HasGroups()
        {
            return Groups != null && Groups.Any();
        }
    }
}