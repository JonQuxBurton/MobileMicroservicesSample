using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoadTestingSetupApp
{
    internal class Program
    {
        private static string GetPhoneNumber(int counter)
        {
            return $"07001{counter.ToString().PadLeft(6, '0')}";
        }

        private static string GetContactPhoneNumber(int counter)
        {
            return $"0114{counter.ToString().PadLeft(6, '0')}";
        }

        private static string GetContactName(int counter)
        {
            return $"Neil Armstrong-{counter}";
        }

        private static string GetActivationCode(int counter)
        {
            return $"AAA{counter.ToString().PadLeft(3, '0')}";
        }

        private static void Main(string[] args)
        {
            var config = new Config()
            {
                ConnectionString = "Server=JQB1-2020;Initial Catalog=Mobile;Integrated Security=True"
            };
            var dataStore = new DataStore(config);

            var orderMobileRawData = new[]
            {
                new
                {
                    CustomerId = "C5C04D13-25B2-4EC2-97E0-99737673287F"
                },
                new
                {
                    CustomerId = "C5C04D13-25B2-4EC2-97E0-99737673287F"
                }
            };
            var completeProvisionRawData = new[]
            {
                new
                {
                    CustomerId = "C5C04D13-25B2-4EC2-97E0-99737673287F",
                    MobileId = "0D070AAD-2897-4B2D-B03C-7D7894777856",
                    ProvisionOrderId = "EE918282-2940-4453-9298-EE361FEDFB1B"
                },
                new
                {
                    CustomerId = "C5C04D13-25B2-4EC2-97E0-99737673287F",
                    MobileId = "08027DE6-C655-474C-8BD7-08D4A9186225",
                    ProvisionOrderId = "A5463012-862C-490A-AC32-D52B63531328"
                }
            };
            var activateMobileRawData = new[]
            {
                new
                {
                    CustomerId = "C5C04D13-25B2-4EC2-97E0-99737673287F",
                    MobileId ="A12F2B7E-170E-408E-9451-85DC796A9C07",
                    MobileOrderId = "A12F2B7E-170E-408E-9451-85DC796A9C07"
                },
                new
                {
                    CustomerId = "C5C04D13-25B2-4EC2-97E0-99737673287F",
                    MobileId ="66606739-E37A-452A-A6A5-831D960C4AD8",
                    MobileOrderId =""
                }
            };
            var completeActivateRawData = new[]
            {
                new
                {
                    CustomerId = "C5C04D13-25B2-4EC2-97E0-99737673287F",
                    MobileId = "422DD8B1-BD6E-4594-8958-D8DC60D495B1",
                    ActivateOrderId = "499FA140-310A-433A-ACF5-B3BAAD206553"
                },
                new
                {
                    CustomerId = "C5C04D13-25B2-4EC2-97E0-99737673287F",
                    MobileId = "9A479AB1-FD6C-4004-B0AA-A7D6BCF98344",
                    ActivateOrderId = "2A894D82-F14A-489C-8E30-983A1FA7A358"
                }
            };

            var path = @"D:\Projects\GitHub\MobileMicroservicesSample\docs\LoadTesting\data.json";

            var orderMobiles = new List<OrderMobileData>();
            var completeProvisions = new List<CompleteProvisionData>();
            var activateMobiles = new List<ActivateMobileData>();
            var completeActivates = new List<CompleteActivateData>();

            var globalCounter = 1;

            foreach (var orderMobileRaw in orderMobileRawData)
            {
                orderMobiles.Add(new OrderMobileData
                {
                    CustomerId = orderMobileRaw.CustomerId,
                    PhoneNumber = GetPhoneNumber(globalCounter),
                    ContactName = GetContactName(globalCounter),
                    ContactPhoneNumber = GetContactPhoneNumber(globalCounter)
                });
                globalCounter++;
            }

            foreach (var completeProvisionRaw in completeProvisionRawData)
            {
                var phoneNumber = GetPhoneNumber(globalCounter);
                var contactName = GetContactName(globalCounter);
                dataStore.SetupDataForCompleteProvision(completeProvisionRaw.CustomerId, completeProvisionRaw.MobileId, completeProvisionRaw.ProvisionOrderId, phoneNumber, contactName);
                completeProvisions.Add(new CompleteProvisionData
                {
                    MobileId = completeProvisionRaw.MobileId,
                    ProvisionOrderId = completeProvisionRaw.ProvisionOrderId,
                    PhoneNumber = phoneNumber,
                    ContactName = contactName
                });
                globalCounter++;
            }

            foreach (var activateMobileRaw in activateMobileRawData)
            {
                var phoneNumber = GetPhoneNumber(globalCounter);
                var activationCode = GetActivationCode(globalCounter);
                dataStore.SetupDataForActivate(activateMobileRaw.CustomerId, activateMobileRaw.MobileId, phoneNumber, activationCode);
                activateMobiles.Add(new ActivateMobileData
                {
                    MobileId = activateMobileRaw.MobileId,
                    ActivationCode = activationCode
                });
                globalCounter++;
            }

            foreach (var completeActivateRaw in completeActivateRawData)
            {
                var phoneNumber = GetPhoneNumber(globalCounter);
                dataStore.SetupDataForCompleteActivate(completeActivateRaw.CustomerId, completeActivateRaw.MobileId, completeActivateRaw.ActivateOrderId, phoneNumber);
                completeActivates.Add(new CompleteActivateData
                {
                    MobileId = completeActivateRaw.MobileId,
                    ActivateOrderId = completeActivateRaw.ActivateOrderId
                });
                globalCounter++;
            }

            var data = new Data
            {
                OrderMobile = orderMobiles.ToArray(),
                CompleteProvision = completeProvisions.ToArray(),
                ActivateMobile = activateMobiles.ToArray(),
                CompleteActivate = completeActivates.ToArray()
            };

            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            Console.WriteLine($"Writing to file {path}...");
            File.WriteAllText(path, json);

            Console.WriteLine("Done!");
        }
    }
}