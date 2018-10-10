using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;

namespace Robotify.AspNetCore
{
    public class RobotifyContentWriter : IRobotifyContentWriter
    {
        private readonly RobotifyOptions options;

        public RobotifyContentWriter(IOptions<RobotifyOptions> options)
        {
            this.options = options.Value;
        }
        
        protected string GenerateSyntaxLine<T>(string key, T value)
        {
            return $"{key}: {value}";
        }

        public string Write()
        {
            var sb = new StringBuilder();

            if (!options.DisableFileHeaderComments)
            {
                var comments = GetFileHeaderComments();
                foreach (var comment in comments)
                {
                    sb.AppendLine($"# {comment}");
                }
            }

            if (options.CrawlDelay.HasValue && options.CrawlDelay > 0)
            {
                sb.AppendLine(GetCrawlDelay(options.CrawlDelay.Value));
            }

            if (options.SitemapUrl != null && options.SitemapUrl.IsAbsoluteUri)
            {
                sb.AppendLine(GetSitemapUrl(options.SitemapUrl));
            }

            if (options.HasGroups())
            {
                sb.AppendLine();
                foreach (var group in options.Groups)
                {
                    sb.AppendLine(GetRobotsGroup(group));
                }
            }

            return sb.ToString();
        }

        protected virtual IEnumerable<string> GetFileHeaderComments()
        {
            yield return $"Generated with {GetType().GetTypeInfo().Assembly.GetName().Name} (v{GetType().GetTypeInfo().Assembly.GetName().Version})";
            yield return $"created: {DateTimeOffset.UtcNow:s}\n";
        }

        protected virtual string GetRobotsGroup(RobotsGroup group)
        {
            return group.ToString();
        }

        protected virtual string GetSitemapUrl(Uri sitemapUrl)
        {
            return GenerateSyntaxLine("sitemap", sitemapUrl);
        }

        protected virtual string GetCrawlDelay(int crawlDelay)
        {
            return GenerateSyntaxLine("crawl-delay", crawlDelay);
        }
    }
}