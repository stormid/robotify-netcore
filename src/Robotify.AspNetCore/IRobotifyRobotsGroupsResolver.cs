using System.Collections.Generic;
using System.Collections.Immutable;

namespace Robotify.AspNetCore
{
    public interface IRobotifyRobotsGroupsResolver
    {
        IImmutableSet<RobotGroup> Resolve();
    }
}