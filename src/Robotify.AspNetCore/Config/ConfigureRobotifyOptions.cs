using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Robotify.AspNetCore.Config
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
            //var groups = configuration.GetSection("Robotify:Groups").Get<HashSet<RobotGroup>>();
            //options.Groups = groups ?? new HashSet<RobotGroup>();
        }
    }
}