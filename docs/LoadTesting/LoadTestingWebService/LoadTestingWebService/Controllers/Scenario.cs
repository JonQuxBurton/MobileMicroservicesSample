using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace LoadTestingWebService.Controllers
{
    public class Scenario
    {
        private readonly string name;
        private readonly IEnumerable<Guid> globalUserIds;

        private static readonly ConcurrentDictionary<int, int>
            VuIdToIndex0 = new ConcurrentDictionary<int, int>();
        //private static readonly ConcurrentDictionary<int, Guid>
        //    MapOfVuIdsToGlobalUserIds = new ConcurrentDictionary<int, Guid>();

        private int currentIndex;
        private readonly object currentIndexLock = new object();

        //public Scenario(string name, IEnumerable<Guid> globalUserIds)
        public Scenario(string name)
        {
            this.name = name;
            //this.globalUserIds = globalUserIds;
        }

        //public void AddGlobalUser(Guid globalUserId)
        //{
        //    //if (!MapOfVuIdsToGlobalUserIds.ContainsKey(globalUserId))
        //    //{
        //        lock (currentIndexLock)
        //        {
        //            MapOfVuIdsToGlobalUserIds.TryAdd(vuId, currentIndex);
        //            Console.WriteLine($"Scenario {name} VirtualUserId {vuId} set to index: {currentIndex}");
        //            currentIndex++;
        //        }
        //    //}
        //}

        //public Guid GetVirtualUserGlobalId(int vuId)
        //{
        //    if (!MapOfVuIdsToGlobalUserIds.ContainsKey(vuId))
        //    {
        //        lock (currentIndexLock)
        //        {
        //            MapOfVuIdsToGlobalUserIds.TryAdd(vuId, globalUserIds.ElementAt(currentIndex));
        //            Console.WriteLine($"Scenario {name} VirtualUserId {vuId} set to GlobalUserid: {globalUserIds.ElementAt(currentIndex)}");
        //            currentIndex++;
        //        }
        //    }

        //    return MapOfVuIdsToGlobalUserIds[vuId];
        //}

        public int GetIndex0(int vuId)
        {
            if (!VuIdToIndex0.ContainsKey(vuId))
            {
                lock (currentIndexLock)
                {
                    VuIdToIndex0.TryAdd(vuId, currentIndex);
                    Console.WriteLine($"Scenario {name} VirtualUserId {vuId} set to index: {currentIndex}");
                    currentIndex++;
                }
            }

            return VuIdToIndex0[vuId];
        }
    }
}