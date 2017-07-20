using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Robotify.AspNetCore
{
    public static class RobotifyServiceCollectionExtensions
    {
        public static IServiceCollection AddRobotify(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOptions();
            serviceCollection.Configure<RobotifyOptions>(robotifyOptions =>
            {
                configuration.GetSection("Robotify").Bind(robotifyOptions);
                var groups = configuration.GetSection("Robotify:Groups").Get<IList<RobotsGroup>>();
                robotifyOptions.Groups = groups ?? new List<RobotsGroup>();
            });

            serviceCollection.TryAddSingleton<IRobotifyContentWriter, RobotifyContentWriter>();
            serviceCollection.TryAddSingleton<RobotifyMiddleware>();
            
            return serviceCollection;
        }
    }
}