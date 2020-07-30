using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TandemExercise.API;
using TandemExercise.Business.Entities;
using TandemExercise.Data;
using TandemExercise.Data.Cosmos;

namespace TandemExercise.IntegrationTest
{
    public class TestClientProvider : IDisposable
    {
        private TestServer testServer;
        public HttpClient Client { get; private set; }

        public TestClientProvider()
        {
            var configFile = "appsettings.test.json";
            var config = new ConfigurationBuilder()
                .AddJsonFile(configFile)
                .Build();

            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((builderContext, config) => { config.AddJsonFile(configFile); });

            testServer = new TestServer(webHostBuilder);

            
            webHostBuilder.ConfigureServices(services =>
            {
                var sp = services.BuildServiceProvider();
            });

            Task.Run(() => seedDatabase(config.GetSection("CosmosDb"))).Wait();

            Client = testServer.CreateClient();
        }

        private async Task seedDatabase(IConfigurationSection configurationSection)
        {
            var cosmosDbService = await CosmosDbService.InitializeCosmosClientInstanceAsync(configurationSection);

            // clear out database
            var users = await cosmosDbService.GetItemsAsync<User>("SELECT * FROM c");
            foreach (User u in users)
            {
                await cosmosDbService.DeleteItemAsync(u.id);
            }

            // populate database with fresh data
            await cosmosDbService.AddItemAsync<User>(new User()
            {
                firstName = "Mark",
                middleName = "Thomas",
                lastName = "Jewell",
                phoneNumber = "123-456-7890",
                emailAddress = "mark.jewell@tandemdiabetes.com"
            });

            await cosmosDbService.AddItemAsync<User>(new User()
            {
                firstName = "Nelson",
                middleName = "Jacob",
                lastName = "Packer",
                phoneNumber = "123-456-7890",
                emailAddress = "nelson.packer@tandemdiabetes.com"
            });

            await cosmosDbService.AddItemAsync<User>(new User()
            {
                firstName = "Bob",
                middleName = "George",
                lastName = "Smith",
                phoneNumber = "123-456-7890",
                emailAddress = "bob.smith@tandemdiabetes.com"
            });

            await cosmosDbService.AddItemAsync<User>(new User()
            {
                firstName = "Julie",
                middleName = "Ann",
                lastName = "Jeffries",
                phoneNumber = "123-456-7890",
                emailAddress = "julie.jeffries@tandemdiabetes.com"
            });

        }

        public void Dispose()
        {
            if (testServer != null)
            {
                testServer.Dispose();
                testServer = null;
            }
        }
    }
}
