using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TandemExercise.Business.Entities;
using TandemExercise.Data.Exceptions;

namespace TandemExercise.Data.Cosmos
{
    public class UserRepository : IUserRepository
    {
        private IDbService dbService;

        public UserRepository(IDbService dbService)
        {
            this.dbService = dbService;
        }

        public async Task<User> Create(User user)
        {
            // ensure no duplicate emails. Need to figure out if this can be done by the database
            User existing = await Get(user.emailAddress);

            if (existing != null) throw new DuplicateEmailException();

            return await dbService.AddItemAsync(user);
        }

        public async Task<User> Update(User user)
        {
            // ensure no duplicate emails. Need to figure out if this can be done by the database
            var users = await getUsersByEmail(user.emailAddress);

            foreach (var u in users)
            {
                if (u.id != user.id)
                    throw new DuplicateEmailException();
            }

            return await dbService.UpdateItemAsync(user.id, user);
        }

        public async Task<IEnumerable<User>> Get()
        {
            return await dbService.GetItemsAsync<User>("SELECT * FROM c");
        }

        public async Task<User> Get(string emailAddress)
        {
            var users = await this.getUsersByEmail(emailAddress);
            return users.FirstOrDefault();
        }

        private async Task<IEnumerable<User>> getUsersByEmail(string emailAddress)
        {
            // should figure out how to parameterize the query, but in interest of time,
            // just protect against sql injection using string replace
            emailAddress = emailAddress.Replace("--", "").Replace("'", "").Replace(";", "").Replace("xp_", "").Replace("/*", "");

            return await dbService.GetItemsAsync<User>($"SELECT * FROM c WHERE c.emailAddress = '{emailAddress}'");
        }
    }
}