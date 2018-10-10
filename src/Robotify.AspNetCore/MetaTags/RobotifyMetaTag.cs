using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Primitives;

[assembly: InternalsVisibleTo("Robotify.AspNetCore.Tests")]

namespace Robotify.AspNetCore.MetaTags
{
    internal class RobotifyMetaTag : IEquatable<RobotifyMetaTag>
    {
        public string UserAgent { get; set; }
        public StringValues Directives { get; set; }

        public RobotifyMetaTag(string userAgent, StringValues directives)
        {
            UserAgent = userAgent;
            Directives = directives;
        }

        public bool Equals(RobotifyMetaTag other)
        {
            var userAgentEqual = UserAgent.Equals(other.UserAgent);
            var directivesIncluded = Directives.ToImmutableHashSet().IsSubsetOf(other.Directives.ToImmutableHashSet());

            return userAgentEqual && directivesIncluded;
        }

        public void MergeDirectives(StringValues directives)
        {
            Directives = new StringValues(Directives.Union(directives).ToArray());
        }
    }
}