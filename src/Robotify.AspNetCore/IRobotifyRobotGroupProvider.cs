using System.Collections.Generic;

namespace Robotify.AspNetCore
{
    public interface IRobotifyRobotGroupProvider
    {
        IEnumerable<RobotGroup> Get();
    }
}