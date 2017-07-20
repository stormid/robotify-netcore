using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Robotify.AspNetCore
{
    public static class RobotifyApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRobotify(this IApplicationBuilder app, Action<RobotifyOptionsConfigurer> configureOptions = null)
        {
            var evt = new EventId(1, "Robotify");
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<RobotifyMiddleware>();

            var options = app.ApplicationServices.GetService<IOptions<RobotifyOptions>>();
            var configurer = new RobotifyOptionsConfigurer(options.Value);

            if (configureOptions == null && !options.Value.HasGroups())
            {
                configureOptions = r => r.DenyAll();
                logger.LogInformation(evt, "[Robotify] - you have no rules configured, adding default deny all just to keep you safe!");
            }

            try
            {
                configureOptions?.Invoke(configurer);
            }
            catch (Exception exception)
            {
                logger.LogCritical(evt, exception, "Failed to configure robotify middleware");
            }
            
            if (options.Value.Enabled)
            {
                if (!string.IsNullOrWhiteSpace(options.Value.RobotsFilePath) && Uri.IsWellFormedUriString(options.Value.RobotsFilePath, UriKind.Relative))
                {
                    logger.LogInformation(evt, "[Robotify] - Registered robotify middleware to respond to {path}", new {path = options.Value.RobotsFilePath});
                    app.Map(options.Value.RobotsFilePath, robotsApp =>
                    {
                        robotsApp.UseMiddleware<RobotifyMiddleware>();
                    });
                }
                else
                {
                    logger.LogError(evt, "[Robotify] - Unable to configure robotify, middleware response path is invalid {path}", new {path = options.Value.RobotsFilePath});
                }
            }
            return app;
        }
    }
}