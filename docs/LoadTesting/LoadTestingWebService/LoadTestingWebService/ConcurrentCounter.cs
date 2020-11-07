namespace LoadTestingWebService
{
    public class ConcurrentCounter
    {
        private int counter = 1;
        private readonly object locker = new object();

        public int Next()
        {
            lock (locker)
            {
                var current = counter;
                counter++;
                return current;
            }
        }
    }
}