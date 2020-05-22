using System.Collections.Generic;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Subscribe;

namespace LittleFlowerBot.Repositories
{
    public interface ISubscriptionRepository
    {
        bool IsExist(string sender);

        Task AddAsync(string sender, string receiver);

        Subscription Get(string sender);

        List<Subscription> All();
    }
}