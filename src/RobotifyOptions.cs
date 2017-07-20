using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;

namespace Robotify.AspNetCore
{
    public interface IRobotifyContentWriter
    {
        string Write();
    }
    
    public class RobotifyContentWriter : IRobotifyContentWriter
    {
        private readonly RobotifyOptions options;

        public RobotifyContentWriter(IOptions<RobotifyOptions> options)
        {
            this.options = options.Value;
        }
        
        public string Write()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"# Generated with {GetType().GetTypeInfo().Assembly.GetName().Name} (v{GetType().GetTypeInfo().Assembly.GetName().Version})");
            sb.AppendLine($"# created: {DateTimeOffset.UtcNow:s}\n");

            if (options.CrawlDelay.HasValue && options.CrawlDelay > 0)
            {
                sb.AppendLine($"crawl-delay: {options.CrawlDelay}");
            }

            if (options.SitemapUrl != null && options.SitemapUrl.IsAbsoluteUri)
            {
                sb.AppendLine($"sitemap: {options.SitemapUrl}");
            }

            if (options.HasGroups())
            {
                sb.AppendLine();
                foreach (var group in options.Groups)
                {
                    sb.AppendLine(group.ToString());
                }
            }

            return sb.ToString();
        }
    }

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