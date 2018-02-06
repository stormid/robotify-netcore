using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Robotify.AspNetCore
{
    public class ConfigureRobotifyOptions : IConfigureOptions<RobotifyOptions>
    {
        private readonly IConfiguration configuration;

        public ConfigureRobotifyOptions(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public void Configure(RobotifyOptions options)
        {
            configuration.GetSection("Robotify").Bind(options);
            var groups = configuration.GetSection("Robotify:Groups").Get<IList<RobotsGroup>>();
            options.Groups = groups ?? new List<RobotsGroup>();
        }
    }
}