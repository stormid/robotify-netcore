using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Robotify.AspNetCore
{
    public static class RobotifyServiceCollectionExtensions
    {
        private static IServiceCollection AddRobotifyServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IRobotifyContentWriter, RobotifyContentWriter>();
            serviceCollection.TryAddSingleton<RobotifyMiddleware>();
            return serviceCollection;
        }

        public static IServiceCollection AddRobotify(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddOptions();
            serviceCollection.TryAddSingleton<IConfigureOptions<RobotifyOptions>, ConfigureRobotifyOptions>();
            serviceCollection.Configure<RobotifyOptions>(_ => { });

            return serviceCollection.AddRobotifyServices();
        }

        [Obsolete("Use AddRobotify() instead, this will be removed in a future version")]        
        public static IServiceCollection AddRobotify(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOptions();
            serviceCollection.Configure<RobotifyOptions>(robotifyOptions =>
            {
                var configure = new ConfigureRobotifyOptions(configuration);
                configure.Configure(robotifyOptions);
            });

            return serviceCollection.AddRobotifyServices();
        }
    }
}