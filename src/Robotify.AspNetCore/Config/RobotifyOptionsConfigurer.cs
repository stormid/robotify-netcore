using System;

namespace Robotify.AspNetCore.Config
{
    public sealed class RobotifyOptionsConfigurer
    {
        private readonly RobotifyOptions options;

        internal RobotifyOptionsConfigurer(RobotifyOptions options)
        {
            this.options = options;
        }

        public RobotifyOptionsConfigurer Enabled(bool enable = true)
        {
            options.Enabled = enable;
            return this;
        }

        public RobotifyOptionsConfigurer WithSitemap(Uri absoluteSitemapUri)
        {
            if (!absoluteSitemapUri.IsAbsoluteUri)
            {
                throw new ArgumentOutOfRangeException(nameof(absoluteSitemapUri), "sitemap url must be absolute (https://www.example.com/sitemap.xml");
            }

            options.SitemapUrl = absoluteSitemapUri;
            
            return this;
        }

        public RobotifyOptionsConfigurer WithCrawlDelay(ushort delayInSeconds)
        {
            options.CrawlDelay = delayInSeconds;
            return this;
        }

        /// <summary>
        /// Will not output file header comments within the robots.txt output, by default a created date along with robotify version information will be added as comments
        /// </summary>
        /// <returns></returns>
        public RobotifyOptionsConfigurer DisableFileHeaderComments()
        {
            options.DisableFileHeaderComments = true;
            return this;
        }
    }
}