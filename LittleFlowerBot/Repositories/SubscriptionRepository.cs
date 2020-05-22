using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LittleFlowerBot.DbContexts;
using LittleFlowerBot.Models.Subscribe;

namespace LittleFlowerBot.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly LittleFlowerBotContext _littleFlowerBotContext;

        public SubscriptionRepository(LittleFlowerBotContext littleFlowerBotContext)
        {
            _littleFlowerBotContext = littleFlowerBotContext;
        }
        
        public bool IsExist(string sender)
        {
            return _littleFlowerBotContext.Subscriptions.Any(subscription => subscription.Sender == sender);
        }

        public async Task AddAsync(string sender, string receiver)
        {
            await _littleFlowerBotContext.Subscriptions.AddAsync(new Subscription()
            {
                Sender = sender,
                Receiver = receiver
            });
            await _littleFlowerBotContext.SaveChangesAsync();
        }

        public Subscription Get(string sender)
        {
            return _littleFlowerBotContext.Subscriptions.Single(subscription => subscription.Sender == sender);
        }

        public List<Subscription> All()
        {
            return _littleFlowerBotContext.Subscriptions.ToList();
        }
    }
}