using Microsoft.Extensions.DependencyInjection;

namespace Robotify.AspNetCore
{
    public class RobotifyServiceConfigurer
    {
        public IServiceCollection Services { get; }

        public RobotifyServiceConfigurer(IServiceCollection serviceCollection)
        {
            Services = serviceCollection;
        }
    }
}