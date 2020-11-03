using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LoadTestingWebService.Resources;

namespace LoadTestingWebService
{
    public class VirtualUserRegistrations
    {
        private readonly ConcurrentDictionary<int, UserResource> map = new ConcurrentDictionary<int, UserResource>();
        private readonly IEnumerable<Guid> userGlobalIds;

        public VirtualUserRegistrations(IEnumerable<Guid> userGlobalIds)
        {
            this.userGlobalIds = userGlobalIds;
        }

        public UserResource Get(int virtualUserId)
        {
            if (!map.ContainsKey(virtualUserId))
            {
                var nextIndex = 0;
                if (map.Values.Any())
                    nextIndex = map.Values.Max(x => x.Index) + 1;
                var globalId = userGlobalIds.ElementAt(nextIndex);
                map.TryAdd(virtualUserId, new UserResource
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