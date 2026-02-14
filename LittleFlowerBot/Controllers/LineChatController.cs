using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using isRock.LineBot;
using LittleFlowerBot.Extensions;
using LittleFlowerBot.Services.EventHandler;
using Microsoft.AspNetCore.Mvc;

namespace LittleFlowerBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineChatController : ControllerBase
    {
        private readonly IEnumerable<ILineEventHandler> _eventHandlers;

        public LineChatController(IEnumerable<ILineEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers;
        }

        [HttpPost("Callback")]
        public async Task<ActionResult> ChatCallback()
        {
            var body = await ReadBodyAsync();
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

        private async Task<string> ReadBodyAsync()
        {
            using var reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        private List<Event> GetEvents(string callback)
        {
            var receivedMessage = Utility.Parsing(callback);
            return receivedMessage.events;
        }
    }
}
