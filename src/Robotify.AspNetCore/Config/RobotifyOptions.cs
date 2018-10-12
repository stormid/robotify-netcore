using System;

namespace Robotify.AspNetCore.Config
{
    public sealed class RobotifyOptions
    {
        public bool Enabled { get; set; } = true;
        public string RobotsFilePath { get; set; } = "/robots.txt";
        public int? CrawlDelay { get; set; }
        public Uri SitemapUrl { get; set; }
        public bool DisableFileHeaderComments { get; set; }
    }
}