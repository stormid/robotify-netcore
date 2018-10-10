using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Robotify.AspNetCore.Config;
using Xunit;

namespace Robotify.AspNetCore.Tests
{
    public class RobotifyContentWriterTests : IDisposable
    {
        private RobotifyOptions options;
        private readonly RobotifyOptionsConfigurer configure;

        public RobotifyContentWriterTests()
        {
            options = new RobotifyOptions();
            configure = new RobotifyOptionsConfigurer(options);
        }

        //[Fact]
        //public void ShouldNotOutputFileHeaderCommentsWhenDisabled()
        //{
        //    var resolver = new Mock<IRobotifyRobotsGroupsResolver>();
        //    resolver.Setup(s => s.Resolve()).Returns(new HashSet<RobotGroup>());

        //    configure.DisableFileHeaderComments();
        //    var writer = new RobotifyContentWriter(new OptionsWrapper<RobotifyOptions>(options), resolver.Object);

        //    var robots = writer.Write();

        //    robots.Should().NotContain("# Generated with");
        //    robots.Should().NotContain("# created: ");
        //}

        //[Fact]
        //public void Empty()
        //{
        //    var resolver = new Mock<IRobotifyRobotsGroupsResolver>();
        //    resolver.Setup(s => s.Resolve()).Returns(new HashSet<RobotGroup>());

        //    configure.DisableFileHeaderComments();
        //    var writer = new RobotifyContentWriter(new OptionsWrapper<RobotifyOptions>(options), resolver.Object);

        //    var robots = writer.Write();

        //    robots.Should().BeNullOrEmpty();
        //}

        //[Fact]
        //public void DenyAll()
        //{
        //    var resolver = new Mock<IRobotifyRobotsGroupsResolver>();
        //    resolver.Setup(s => s.Resolve()).Returns(new HashSet<RobotGroup>());

        //    configure.DenyAll();
        //    var writer = new RobotifyContentWriter(new OptionsWrapper<RobotifyOptions>(options), resolver.Object);

        //    var robots = writer.Write();

        //    robots.Should().Contain("user-agent: *\r\ndisallow: /");
        //}

        //[Fact]
        //public void AllowAll()
        //{
        //    var resolver = new Mock<IRobotifyRobotsGroupsResolver>();
        //    resolver.Setup(s => s.Resolve()).Returns(new HashSet<RobotGroup>());

        //    configure.AllowAll();
        //    var writer = new RobotifyContentWriter(new OptionsWrapper<RobotifyOptions>(options), resolver.Object);

        //    var robots = writer.Write();

        //    robots.Should().Contain("user-agent: *\r\ndisallow:");
        //}

        //[Theory]
        //[InlineData("googlebot", "/")]
        //[InlineData("googlebot", "/" ,"/admin")]
        //[InlineData("bingbot", "/" ,"/admin", "/admin")]
        //public void AllowSpecificUserAgent(string userAgent, params string[] paths)
        //{
        //    var resolver = new Mock<IRobotifyRobotsGroupsResolver>();
        //    resolver.Setup(s => s.Resolve()).Returns(new HashSet<RobotGroup>());

        //    configure.DisableFileHeaderComments();
        //    configure.Allow(userAgent, paths);
        //    var writer = new RobotifyContentWriter(new OptionsWrapper<RobotifyOptions>(options), resolver.Object);

        //    var robots = writer.Write();

        //    robots.Should().Contain($"user-agent: {userAgent}");
        //    foreach (var path in paths)
        //    {
        //        robots.Should().Contain($"allow: {path}");
        //    }
        //}

        //[Theory]
        //[InlineData("googlebot", "/")]
        //[InlineData("googlebot", "/" ,"/admin")]
        //[InlineData("bingbot", "/" ,"/admin", "/admin")]
        //public void DisallowSpecificUserAgent(string userAgent, params string[] paths)
        //{
        //    var resolver = new Mock<IRobotifyRobotsGroupsResolver>();
        //    resolver.Setup(s => s.Resolve()).Returns(new HashSet<RobotGroup>());

        //    configure.DisableFileHeaderComments();
        //    configure.Disallow(userAgent, paths);
        //    var writer = new RobotifyContentWriter(new OptionsWrapper<RobotifyOptions>(options), resolver.Object);

        //    var robots = writer.Write();

        //    robots.Should().Contain($"user-agent: {userAgent}");
        //    foreach (var path in paths)
        //    {
        //        robots.Should().Contain($"disallow: {path}");
        //    }
        //}

        //[Theory]
        //[InlineData(0, "")]
        //[InlineData(10, "crawl-delay: 10")]
        //public void CrawlDelay(ushort seconds, string expected)
        //{
        //    var resolver = new Mock<IRobotifyRobotsGroupsResolver>();
        //    resolver.Setup(s => s.Resolve()).Returns(new HashSet<RobotGroup>());

        //    configure.DisableFileHeaderComments();
        //    configure.WithCrawlDelay(seconds);
        //    var writer = new RobotifyContentWriter(new OptionsWrapper<RobotifyOptions>(options), resolver.Object);

        //    var robots = writer.Write();

        //    robots.Should().Equals(expected);
        //}

        //[Theory]
        //[InlineData("https://www.example.com/sitemap.xml")]
        //public void Sitemap(string uri)
        //{
        //    var resolver = new Mock<IRobotifyRobotsGroupsResolver>();
        //    resolver.Setup(s => s.Resolve()).Returns(new HashSet<RobotGroup>());

        //    configure.DisableFileHeaderComments();
        //    configure.WithSitemap(new Uri(uri));
        //    var writer = new RobotifyContentWriter(new OptionsWrapper<RobotifyOptions>(options), resolver.Object);

        //    var robots = writer.Write();

        //    robots.Should().Equals($"sitemap: {uri}");
        //}

        public void Dispose()
        {
            options = null;
        }
    }
}