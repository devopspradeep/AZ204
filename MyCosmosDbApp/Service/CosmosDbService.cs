using Microsoft.Azure.Cosmos;
using MyCosmosDbApp.IService;
using MyCosmosDbApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Azure.Cosmos;
//using Microsoft.Azure.Cosmos.Fluent;
//using Microsoft.Extensions.Configuration;

namespace MyCosmosDbApp.Service
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;
        public CosmosDbService(CosmosClient dbClient,string databaseName,string containerName)
        {
            this._container = dbClient.GetContainer(databaseName,containerName);    
        }
        public  async Task AddItemAsync(Item item)
        {
            await this._container.CreateItemAsync<Item>(item, new PartitionKey(item.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<Item>(id, new PartitionKey(id));
        }

        public async Task<Item> GetItemAsync(string id)
        {
            ItemResponse<Item> response = await this._container.ReadItemAsync<Item>(id, new PartitionKey(id));
            return response.Resource;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Item>(new QueryDefinition(queryString));
            List<Item> results = new List<Item>();
            while(query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, Item item)
        {
            await this._container.UpsertItemAsync<Item>(item, new PartitionKey(id));
        }
    }
}
