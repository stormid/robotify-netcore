using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Robotify.AspNetCore
{
    public class LoadFromAppSettingsRobotGroupProvider : IRobotifyRobotGroupProvider
    {
        private readonly IConfiguration configuration;

        public LoadFromAppSettingsRobotGroupProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<RobotGroup> Get()
        {
            var groups = configuration.GetSection("Robotify:Groups").Get<List<RobotGroup>>();
            return groups;
        }
    }
}
