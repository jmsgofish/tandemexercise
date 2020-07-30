using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TandemExercise.Business.Entities;

namespace TandemExercise.Data.Cosmos
{
    public class CosmosDbService : IDbService
    {
        private Container container;

        public static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;
            Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);

            CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);

            Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);

            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id" );

            return cosmosDbService;
        }

        public CosmosDbService(CosmosClient dbClient, string databaseName, string containerName)
        {
            this.container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<T> AddItemAsync<T>(T item) where T : EntityBase
        {
            item.id ??= Guid.NewGuid().ToString();
            ItemResponse<T> response = await this.container.CreateItemAsync<T>(item, new PartitionKey(item.id));

            return response.Resource;
        }

        public async Task DeleteItemAsync(string id)
        {
            await this.container.DeleteItemAsync<EntityBase>(id, new PartitionKey(id));
        }

        public async Task<EntityBase> GetItemAsync<EntityBase>(string id)
        {
            try
            {
                ItemResponse<EntityBase> response = await this.container.ReadItemAsync<EntityBase>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return default(EntityBase);
            }

        }

        public async Task<IEnumerable<EntityBase>> GetItemsAsync<EntityBase>(string queryString)
        {
            var query = this.container.GetItemQueryIterator<EntityBase>(new QueryDefinition(queryString));
            List<EntityBase> results = new List<EntityBase>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<T> UpdateItemAsync<T>(string id, T item) where T : EntityBase
        {
            ItemResponse<T> response = await this.container.UpsertItemAsync<T>(item, new PartitionKey(id));

            return response.Resource;
        }
    }
}
