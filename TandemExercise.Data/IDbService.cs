using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TandemExercise.Business.Entities;

namespace TandemExercise.Data
{
    public interface IDbService
    {
        Task<IEnumerable<T>> GetItemsAsync<T>(string query);
        Task<T> GetItemAsync<T>(string id);
        Task<T> AddItemAsync<T>(T item) where T : EntityBase;
        Task<T> UpdateItemAsync<T>(string id, T item) where T : EntityBase;
        Task DeleteItemAsync(string id);
    }
}
