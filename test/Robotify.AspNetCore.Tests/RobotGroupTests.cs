using System;
using FluentAssertions;
using Xunit;

namespace Robotify.AspNetCore.Tests
{
    public class RobotGroupTests : IDisposable
    {
        public RobotGroupTests()
        {
            
        }

        [Fact]
        public void RobotGroupEquality()
        {
            var lhs = new RobotGroup();
            var rhs = new RobotGroup();

            lhs.Equals(rhs).Should().BeTrue();
        }
        
        [Theory]
        [InlineData("*", "", "", true)]
        [InlineData("*", "/", "/", true)]
        [InlineData("*", "", "/", false)]
        [InlineData("*", "/admin;/support", "/admin;/support", true)]
        [InlineData("*", "/admin;/support", "/support;/admin", true)]
        [InlineData("*", "/admin;/support", "/support;/admin;/logout", false)]
        [InlineData("*", "/admin;/support;/logout", "/support;/admin", false)]
        public void RobotGroupEqualityAllows(string userAgent, string lhsPaths, string rhsPaths, bool expected)
        {
            var lhs = new RobotGroup()
            {
                UserAgent = userAgent,
                Allow = lhsPaths.Split(";")
            };
            var rhs = new RobotGroup()
            {
                UserAgent = userAgent,
                Allow = rhsPaths.Split(";")
            };

            lhs.Equals(rhs).Should().Be(expected);
        }

        [Theory]
        [InlineData("*", "", "", true)]
        [InlineData("*", "/", "/", true)]
        [InlineData("*", "", "/", false)]
        [InlineData("*", "/admin;/support", "/admin;/support", true)]
        [InlineData("*", "/admin;/support", "/support;/admin", true)]
        [InlineData("*", "/admin;/support", "/support;/admin;/logout", false)]
        [InlineData("*", "/admin;/support;/logout", "/support;/admin", false)]
        public void RobotGroupEqualityDisallows(string userAgent, string lhsPaths, string rhsPaths, bool expected)
        {
            var lhs = new RobotGroup()
            {
                UserAgent = userAgent,
                Disallow = lhsPaths.Split(";")
            };
            var rhs = new RobotGroup()
            {
                UserAgent = userAgent,
                Disallow = rhsPaths.Split(";")
            };

            lhs.Equals(rhs).Should().Be(expected);
        }

        [Theory]
        [InlineData("*", "", "", true)]
        [InlineData("*", "/", "/", true)]
        [InlineData("*", "", "/", false)]
        [InlineData("*", "/admin;/support", "/admin;/support", true)]
        [InlineData("*", "/admin;/support", "/support;/admin", true)]
        [InlineData("*", "/admin;/support", "/support;/admin;/logout", false)]
        [InlineData("*", "/admin;/support;/logout", "/support;/admin", false)]
        public void RobotGroupEqualityPaths(string userAgent, string lhsPaths, string rhsPaths, bool expected)
        {
            var lhs = new RobotGroup()
            {
                UserAgent = userAgent,
                Allow = lhsPaths.Split(";"),
                Disallow = lhsPaths.Split(";")
            };
            var rhs = new RobotGroup()
            {
                UserAgent = userAgent,
                Allow = lhsPaths.Split(";"),
                Disallow = rhsPaths.Split(";")
            };

            lhs.Equals(rhs).Should().Be(expected);
        }

        [Theory]
        [InlineData("user-agent: googlebot\r\nallow: /", "googlebot", "/")]
        [InlineData("user-agent: googlebot\r\nallow: /\r\nallow: /admin", "googlebot", "/", "/admin")]
        public void AllowRobotGroupToString(string expected, string userAgent, params string[] allowPaths)
        {
            var group = new RobotGroup
            {
                UserAgent = userAgent,
                Allow = allowPaths
            };

            group.ToString().Should().Be(expected);
        }

        [Theory]
        [InlineData("user-agent: *\r\ndisallow: /", "*", "/")]
        [InlineData("user-agent: googlebot\r\ndisallow: /", "googlebot", "/")]
        [InlineData("user-agent: googlebot\r\ndisallow: /\r\ndisallow: /admin", "googlebot", "/", "/admin")]
        public void DisallowRobotGroupToString(string expected, string userAgent, params string[] disallowPaths)
        {
            var group = new RobotGroup
            {
                UserAgent = userAgent,
                Disallow = disallowPaths
            };

            group.ToString().Should().Be(expected);
        }


        public void Dispose()
        {
        }
    }
}
