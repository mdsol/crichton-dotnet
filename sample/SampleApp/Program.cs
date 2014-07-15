using System;
using System.Net.Http;
using Crichton.Client;
using Crichton.Representors.Serializers;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestCrichtonClient(args[0], args[1]);
        }

        private static void TestCrichtonClient(string baseUrl, string relativeUrl)
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri(baseUrl) };

            var serializer = new JsonSerializer();
            var crichtonClient = new CrichtonClient(httpClient, serializer);
            var query = crichtonClient.CreateQuery().WithUrl(relativeUrl);
            var representor = crichtonClient.ExecuteQueryAsync(query).Result;

            //
            // get data from the representor.
            // 
            
            Console.WriteLine(serializer.Serialize(representor));
        }
    }
}