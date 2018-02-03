using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Robotify.AspNetCore
{
    public sealed class RobotsGroup : IEquatable<RobotsGroup>
    {

        public string UserAgent { get; set; }
        public IList<string> Disallow { get; set; }
        public IList<string> Allow { get; set; }
        
        internal void MergePaths(StringValues disallowPaths, StringValues allowPaths)
        {
            Disallow = Disallow.Union(disallowPaths).ToArray();
            Allow = Allow.Union(allowPaths).ToArray();
        }

        internal void ResetPaths(StringValues disallowPaths, StringValues allowPaths)
        {
            Disallow = disallowPaths;
            Allow = allowPaths;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"user-agent: {UserAgent}");

            if (Disallow != null)
            {
                foreach (var disallowPath in Disallow)
                {
                    sb.AppendLine($"disallow: {disallowPath}");
                }
            }
           

            if (Allow != null)
            {
                foreach (var allowPath in Allow)
                {
                    sb.AppendLine($"allow: {allowPath}");
                }
            }

            return sb.ToString();
        }

        public bool Equals(RobotsGroup other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(UserAgent, other.UserAgent) && Disallow.Equals(other.Disallow) && Allow.Equals(other.Allow);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RobotsGroup) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (UserAgent != null ? UserAgent.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Disallow.GetHashCode();
                hashCode = (hashCode * 397) ^ Allow.GetHashCode();
                return hashCode;
            }
        }
    }
}
