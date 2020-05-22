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
        private readonly ILineNotify _lineNotify;

        public RegistrationHandler(IMessage message,
            RegistrationCache registrationCache, ILineNotify lineNotify)
        {
            _message = message;
            _registrationCache = registrationCache;
            _lineNotify = lineNotify;
        }

        public Task Act(Event @event)
        {
            if (@event.Text().Equals("我要註冊"))
            {
                if (_lineNotify.IsRegistered(@event.SenderId()))
                {
                    _message.Reply(@event.replyToken, "你已經註冊了");
                }
                else
                {
                    var guid = GenerateGuid();
                    _registrationCache.Add(guid, @event.SenderId());
                    _message.Reply(@event.replyToken, _lineNotify.GenerateLink(guid));
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
                if (!_lineNotify.IsRegistered(senderId))
                {
                    await _lineNotify.SaveToken(senderId, code);
                    _registrationCache.Remove(guid);;
                }
            }
        }
    }
}