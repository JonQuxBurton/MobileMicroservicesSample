using System;

namespace LoadTestingWebService
{
    public interface IScenariosService
    {
        User GetUserId(string scenarioKey, int virtualUserId);
    }
}