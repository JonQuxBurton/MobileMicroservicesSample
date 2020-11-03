using System;
using LoadTestingWebService.Resources;

namespace LoadTestingWebService
{
    public interface IScenariosService
    {
        UserResource GetUserId(string scenarioKey, int virtualUserId);
        void GenerateData();
    }
}