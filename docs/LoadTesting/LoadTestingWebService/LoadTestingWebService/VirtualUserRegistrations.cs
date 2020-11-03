using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace LoadTestingWebService
{
    public class VirtualUserRegistrations
    {
        private readonly ConcurrentDictionary<int, User> map = new ConcurrentDictionary<int, User>();
        private readonly IEnumerable<Guid> userGlobalIds;

        public VirtualUserRegistrations(IEnumerable<Guid> userGlobalIds)
        {
            this.userGlobalIds = userGlobalIds;
        }

        public User Get(int virtualUserId)
        {
            if (!map.ContainsKey(virtualUserId))
            {
                var nextIndex = 0;
                if (map.Values.Any())
                    nextIndex = map.Values.Max(x => x.Index) + 1;
                var globalId = userGlobalIds.ElementAt(nextIndex);
                map.TryAdd(virtualUserId, new User
                {
                    GlobalId = globalId,
                    Index = nextIndex,
                    VirtualUserId = virtualUserId
                });
            }

            return map[virtualUserId];
        }
    }
}