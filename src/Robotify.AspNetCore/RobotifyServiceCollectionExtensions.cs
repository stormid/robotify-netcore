using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Robotify.AspNetCore.Config;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Robotify.AspNetCore.MetaTags;
using System;

namespace Robotify.AspNetCore
{
    public static class RobotifyServiceCollectionExtensions
    {
        public static RobotifyServiceConfigurer AddRobotGroupProvider<TAppender>(this RobotifyServiceConfigurer configurer)
            where TAppender : class, IRobotifyRobotGroupProvider
        {
            configurer.Services.AddScoped<IRobotifyRobotGroupProvider, TAppender>();
            return configurer;
        }

        /// <summary>
        /// Will attempt to read robot groups from the robotify appsettings configuration, from a node named "Groups" under "Robotify"
        /// </summary>
        /// <typeparam name="TAppender"></typeparam>
        /// <param name="configurer"></param>
        /// <returns></returns>
        public static RobotifyServiceConfigurer AddRobotGroupsFromAppSettings(this RobotifyServiceConfigurer configurer)
        {
            return configurer.AddRobotGroupProvider<LoadFromAppSettingsRobotGroupProvider>();
        }

        /// <summary>
        /// Will include a standard disallow all robot group
        /// </summary>
        /// <typeparam name="TAppender"></typeparam>
        /// <param name="configurer"></param>
        /// <returns></returns>
        public static RobotifyServiceConfigurer AddDisallowAllRobotGroupProvider(this RobotifyServiceConfigurer configurer)
        {
            return configurer.AddRobotGroupProvider<RobotifyDisallowAllRobotGroupProvider>();
        }

        private static IServiceCollection AddRobotifyServices<TContentWriter, TRobotGroupsResolver>(this IServiceCollection serviceCollection) where TContentWriter : class, IRobotifyContentWriter where TRobotGroupsResolver : class, IRobotifyRobotsGroupsResolver
        {
            serviceCollection.TryAddScoped<IRobotifyRobotsGroupsResolver, TRobotGroupsResolver>();
            serviceCollection.TryAddScoped<IRobotifyContentWriter, TContentWriter>();
            serviceCollection.TryAddSingleton<RobotifyMiddleware>();
            return serviceCollection;
        }

        public static IServiceCollection AddRobotify(this IServiceCollection serviceCollection, Action<RobotifyServiceConfigurer> configure)
        {
            return serviceCollection.AddRobotify<RobotifyContentWriter, RobotifyDefaultRobotsGroupResolver>(configure);
        }

        public static IServiceCollection AddRobotify(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddRobotify<RobotifyContentWriter, RobotifyDefaultRobotsGroupResolver>(null);
        }

        public static IServiceCollection AddRobotify<TContentWriter, TRobotGroupsResolver>(this IServiceCollection serviceCollection, Action<RobotifyServiceConfigurer> configure) where TContentWriter : class, IRobotifyContentWriter where TRobotGroupsResolver : class, IRobotifyRobotsGroupsResolver
        {
            serviceCollection.AddOptions();
            serviceCollection.TryAddSingleton<IConfigureOptions<RobotifyOptions>, ConfigureRobotifyOptions>();
            serviceCollection.Configure<RobotifyOptions>(_ => { });
            serviceCollection.AddSingleton<ITagHelperComponent, RobotifyMetaTagHelperComponent>();

            var configurer = new RobotifyServiceConfigurer(serviceCollection);
            configure?.Invoke(configurer);

            return serviceCollection.AddRobotifyServices<TContentWriter, TRobotGroupsResolver>();
        }

    }
}