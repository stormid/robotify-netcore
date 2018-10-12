using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;
using Robotify.AspNetCore.Config;

namespace Robotify.AspNetCore
{
    public class RobotifyContentWriter : IRobotifyContentWriter
    {
        private readonly RobotifyOptions options;
        private readonly IEnumerable<IRobotifyRobotGroupProvider> providers;

        public RobotifyContentWriter(IOptions<RobotifyOptions> options, IEnumerable<IRobotifyRobotGroupProvider> providers)
        {
            this.options = options.Value;
            this.providers = providers;
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

            sb.AppendLine();

            var groups = providers.SelectMany(s => s.Get()).OrderBy(s => s.Order).Distinct().ToList() ?? Enumerable.Empty<RobotGroup>();
            foreach (var group in groups)
            {
                sb.AppendLine(GetRobotsGroup(group));
                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }

        protected virtual IEnumerable<string> GetFileHeaderComments()
        {
            yield return $"Generated with {GetType().GetTypeInfo().Assembly.GetName().Name} (v{GetType().GetTypeInfo().Assembly.GetName().Version})";
            yield return $"created: {DateTimeOffset.UtcNow:s}\n";
        }

        protected virtual string GetRobotsGroup(RobotGroup group)
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