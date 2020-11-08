using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoadTestingWebService
{
    public class DataFileWriter : IDataFileWriter
    {
        private readonly TestDataSettings testDataSettings;

        public DataFileWriter(IOptions<TestDataSettings> testDataSettingsOptions)
        {
            testDataSettings = testDataSettingsOptions.Value;
        }

        public void WriteDataFile(Dictionary<string, List<UsersData>> allData)
        {
            var json = ConvertToJson(allData);
            File.WriteAllText(Path.Combine(testDataSettings.Path, testDataSettings.FileNameData), json);
        }

        private static string ConvertToJson(Dictionary<string, List<UsersData>> data)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
            return json;
        }
    }
}