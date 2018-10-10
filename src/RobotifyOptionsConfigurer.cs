using System;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Robotify.AspNetCore
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

        public RobotifyOptionsConfigurer AllowAll(string userAgent = "*")
        {
            return Replace(userAgent, "", new StringValues());
        }
        
        public RobotifyOptionsConfigurer DenyAll(string userAgent = "*")
        {
            var denyAll = new RobotsGroup { UserAgent = userAgent, Disallow = new [] { "/" }};
            var matchingGroups = options.Groups.Where(s => s.UserAgent.Equals(userAgent));

            foreach (var group in matchingGroups)
            {
                options.Groups.Remove(group);
            }
            
            if (!options.Groups.Contains(denyAll))
            {
                options.Add(denyAll);
            }
            return this;
        }

        public RobotifyOptionsConfigurer Replace(string userAgent, StringValues disallowPaths, StringValues allowPaths)
        {
            var existingGroup = options.Groups.FirstOrDefault(x => x.UserAgent.Equals(userAgent, StringComparison.CurrentCultureIgnoreCase));
            if (existingGroup == null)
            {
                var group = new RobotsGroup {UserAgent = userAgent, Disallow = disallowPaths, Allow = allowPaths};
                options.Add(group);
            }
            else
            {
                existingGroup.ResetPaths(disallowPaths, allowPaths);
            }
            return this;
        }
        
        public RobotifyOptionsConfigurer Add(string userAgent, StringValues disallowPaths, StringValues allowPaths)
        {
            var existingGroup = options.Groups.FirstOrDefault(x => x.UserAgent.Equals(userAgent, StringComparison.CurrentCultureIgnoreCase));
            if (existingGroup == null)
            {
                var group = new RobotsGroup { UserAgent = userAgent, Disallow = disallowPaths, Allow = allowPaths };
                options.Add(group);
            }
            else
            {
                existingGroup.MergePaths(disallowPaths, allowPaths);
            }
            return this;
        }
        
        public RobotifyOptionsConfigurer Disallow(string userAgent, params string[] paths)
        {
            return Add(userAgent, paths, new StringValues());
        }

        public RobotifyOptionsConfigurer Allow(string userAgent, params string[] paths)
        {
            return Add(userAgent, new StringValues(), paths);
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