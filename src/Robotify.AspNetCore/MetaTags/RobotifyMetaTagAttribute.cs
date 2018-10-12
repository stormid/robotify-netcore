using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Robotify.AspNetCore.MetaTags
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RobotifyMetaTagAttribute : ActionFilterAttribute
    {
        internal const string Key = "__robotify-metatags:";

        public RobotifyMetaTagAttribute(params string[] directives)
        {
            Directives = directives;
        }

        /// <summary>
        /// Gets or sets a <see cref="string"/> value of the useragent used when applying meta tags (defaults to 'robots')
        /// </summary>
        public string UserAgent { get;set; } = "robots";

        /// <summary>
        /// Gets or sets a <see cref="StringValues"/> value indicating which robots meta tag directives should be applied
        /// </summary>
        public StringValues Directives { get; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether meta tag attributes should inherit the directives from a parent (action -> controller)
        /// </summary>
        public bool Inherited { get; set; } = true;

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var k = $"{Key}{UserAgent}".ToLowerInvariant();

            if (context.ActionDescriptor.Properties.TryGetValue(k, out object value) && value is RobotifyMetaTag metaTag)
            {
                if (Inherited)
                {
                    metaTag.MergeDirectives(Directives);
                }
                else
                {
                    metaTag.Directives = Directives;
                }
            }
            else
            {
                context.ActionDescriptor.Properties.Add(k, new RobotifyMetaTag(UserAgent, Directives));
            }
        }
    }
}