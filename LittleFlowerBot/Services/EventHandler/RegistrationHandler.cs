using System;
using System.Threading.Tasks;
using isRock.LineBot;
using LittleFlowerBot.Extensions;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.Message;
using LittleFlowerBot.Models.Renderer;

namespace LittleFlowerBot.Services.EventHandler
{
    public class RegistrationHandler : IEventHandler
    {
        private readonly IMessage _message;
        private readonly RegistrationCache _registrationCache;
        private readonly ILineNotifySubscription _lineNotifySubscription;

        public RegistrationHandler(IMessage message,
            RegistrationCache registrationCache, ILineNotifySubscription lineNotifySubscription)
        {
            _message = message;
            _registrationCache = registrationCache;
            _lineNotifySubscription = lineNotifySubscription;
        }

        public Task Act(Event @event)
        {
            if (@event.Text().Equals("我要註冊"))
            {
                if (_lineNotifySubscription.IsRegistered(@event.SenderId()))
                {
                    _message.Reply(@event.replyToken, "你已經註冊了");
                }
                else
                {
                    var guid = GenerateGuid();
                    _registrationCache.Add(guid, @event.SenderId());
                    _message.Reply(@event.replyToken, _lineNotifySubscription.GenerateLink(guid));
                }
            }
            
            return Task.CompletedTask;
        }

        protected virtual string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task Subscribe(string code, string guid)
        {
            var senderId = _registrationCache.GetSenderId(guid);
            if (senderId != null)
            {
                if (!_lineNotifySubscription.IsRegistered(senderId))
                {
                    await _lineNotifySubscription.SaveToken(senderId, code);
                    _registrationCache.Remove(guid);;
                }
            }
        }
    }
}