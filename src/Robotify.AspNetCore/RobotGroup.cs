using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Robotify.AspNetCore
{
    public sealed class RobotGroup : IEquatable<RobotGroup>
    {
        public int Order { get; set; }

        public string UserAgent { get; set; }
        public IList<string> Disallow { get; set; } = Enumerable.Empty<string>().ToList();
        public IList<string> Allow { get; set; } = Enumerable.Empty<string>().ToList();
        
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

        public static RobotGroup DisallowAll()
        {
            return new RobotGroup()
            {
                Order = -9999,
                UserAgent = "*",
                Disallow = new[] {"/"}
            };
        }

        public static RobotGroup DisallowPaths(string userAgent = "*", params string[] paths)
        {
            return new RobotGroup()
            {
                Order = -9999,
                UserAgent = userAgent,
                Disallow = paths
            };
        }

        public static RobotGroup AllowPaths(string userAgent = "*", params string[] paths)
        {
            return new RobotGroup()
            {
                Order = -9999,
                UserAgent = userAgent,
                Allow = paths
            };
        }

        public static RobotGroup AllowAll()
        {
            return new RobotGroup()
            {
                Order = -9999,
                UserAgent = "*",
                Allow = new[] {"/"}
            };
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

            return sb.ToString().TrimEnd();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var userAgentHashCode = (UserAgent != null ? UserAgent.GetHashCode() : 0);
                var allowHashCode = GetSequenceHashCode(Allow);
                var disallowHashCode = GetSequenceHashCode(Disallow);

                return userAgentHashCode ^ allowHashCode ^ disallowHashCode;
            }
        }

        private static int GetSequenceHashCode<T>(IList<T> sequence)
        {
            const int seed = 487;
            const int modifier = 31;

            unchecked
            {
                return sequence.Aggregate(seed, (current, item) =>
                    (current ^ modifier) ^ item.GetHashCode());
            }            
        }

        public bool Equals(RobotGroup other)
        {
            var myHash = GetHashCode();
            var otherHash = other.GetHashCode();
            return GetHashCode().Equals(other.GetHashCode());
        }
    }
}
