using System;
using System.Collections.Concurrent;

namespace LoadTestingWebService.Controllers
{
    public class Scenario
    {
        private readonly string name;

        private static readonly ConcurrentDictionary<int, int>
            VuIdToIndex0 = new ConcurrentDictionary<int, int>();

        private int currentIndex;
        private readonly object currentIndexLock = new object();

        public Scenario(string name)
        {
            this.name = name;
        }

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