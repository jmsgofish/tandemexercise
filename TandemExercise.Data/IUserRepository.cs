using System.Collections.Generic;
using System.Threading.Tasks;
using TandemExercise.Business.Entities;

namespace TandemExercise.Data
{
    public interface IUserRepository
    {
        Task<User> Create(User user);

        Task<User> Update(User user);

        Task<IEnumerable<User>> Get();

        Task<User> Get(string emailAddress);
    }
}