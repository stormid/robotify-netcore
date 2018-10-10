using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Robotify.AspNetCore.Config;

namespace Robotify.AspNetCore.MetaTags
{
    public class RobotifyMetaTagHelperComponent : TagHelperComponent
    {
        private readonly IOptions<RobotifyOptions> options;

        public RobotifyMetaTagHelperComponent(IOptions<RobotifyOptions> options)
        {
            this.options = options;
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (options.Value.Enabled && context.TagName.Equals("head", StringComparison.Ordinal))
            {
                var keys = ViewContext.ActionDescriptor.Properties.Keys.OfType<string>().Where(k => k.StartsWith(RobotifyMetaTagAttribute.Key));

                foreach (var key in keys)
                {
                    if (ViewContext.ActionDescriptor.Properties.TryGetValue(key, out object value) && value is RobotifyMetaTag metaTag)
                    {
                        var tag =
                            $"\t<meta name=\"{metaTag.UserAgent}\" content=\"{string.Join(",", metaTag.Directives)}\">\n";
                        output.PostContent.AppendHtml(tag);
                    }
                }
            }            
        }
    }
}
