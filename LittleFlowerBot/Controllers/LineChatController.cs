using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using isRock.LineBot;
using LittleFlowerBot.Extensions;
using LittleFlowerBot.Models.Requests;
using LittleFlowerBot.Services.EventHandler;
using Microsoft.AspNetCore.Mvc;

namespace LittleFlowerBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineChatController : ControllerBase
    {
        private readonly IEnumerable<IEventHandler> _eventHandlers;
        
        public LineChatController(IEnumerable<IEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers;
        }

        [HttpPost("Callback")]
        public async Task<ActionResult> ChatCallback()
        {
            var body = ReadBody();

            var events = GetEvents(body);

            var validEvents = events.Where(@event => !string.IsNullOrEmpty(@event.Text())).ToList();
            foreach (var validEvent in validEvents)
            {
                foreach (var eventHandler in _eventHandlers)
                {
                    await eventHandler.Act(validEvent);
                }
            }

            return Ok();
        }
        
        [HttpPost("Notify")]
        public async Task<ActionResult> NotifyCallback([FromForm]LineNotifyRequest request)
        {
            var eventHandler = _eventHandlers.First(handler => handler is RegistrationHandler);
            await ((RegistrationHandler)eventHandler).Subscribe(request.Code, request.State);
            return Ok();
        }

        private string ReadBody()
        {
            string body;
            using (StreamReader reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8))
            {
                body = reader.ReadToEndAsync().Result;
            }

            return body;
        }

        private List<Event> GetEvents(string callback)
        {
            var receivedMessage = Utility.Parsing(callback);
            return receivedMessage.events;
        }
    }
}