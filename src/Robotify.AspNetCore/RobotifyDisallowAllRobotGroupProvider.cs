using System.Collections.Generic;

namespace Robotify.AspNetCore
{
    public class RobotifyDisallowAllRobotGroupProvider : IRobotifyRobotGroupProvider
    {
        public IEnumerable<RobotGroup> Get()
        {
            yield return RobotGroup.DisallowAll();
        }
    }
}
