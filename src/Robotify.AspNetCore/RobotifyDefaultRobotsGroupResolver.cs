using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Robotify.AspNetCore
{
    public class RobotifyDefaultRobotsGroupResolver : IRobotifyRobotsGroupsResolver
    {
        private readonly IEnumerable<IRobotifyRobotGroupProvider> providers;

        public RobotifyDefaultRobotsGroupResolver(IEnumerable<IRobotifyRobotGroupProvider> providers)
        {
            this.providers = providers;
        }

        protected virtual IImmutableSet<RobotGroup> ResolveCore(IImmutableSet<RobotGroup> groups)
        {
            return groups;
        }

        public IImmutableSet<RobotGroup> Resolve()
        {
            var groups = providers.SelectMany(s => s.Get()).OrderBy(s => s.Order).ToImmutableHashSet();
            return ResolveCore(groups);
        }
    }
}