using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using isRock.LineBot;
using LittleFlowerBot.Extensions;
using LittleFlowerBot.Services.EventHandler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LittleFlowerBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineChatController : ControllerBase
    {
        private readonly IEnumerable<ILineEventHandler> _eventHandlers;
        private readonly ILogger<LineChatController> _logger;

        public LineChatController(IEnumerable<ILineEventHandler> eventHandlers, ILogger<LineChatController> logger)
        {
            _eventHandlers = eventHandlers;
            _logger = logger;
        }

        [HttpPost("Callback")]
        public async Task<ActionResult> ChatCallback()
        {
            var sw = Stopwatch.StartNew();
            var body = await ReadBodyAsync();
            _logger.LogWarning("[DEBUG] Webhook received, body length: {Length}, elapsed: {Elapsed}ms", body.Length, sw.ElapsedMilliseconds);

            var events = GetEvents(body);

            // Extract raw reply tokens from JSON to bypass SDK parsing issues
            PatchReplyTokensFromRawJson(body, events);

            _logger.LogWarning("[DEBUG] Events count: {Count}, types: [{Types}]",
                events.Count,
                string.Join(", ", events.Select(e => $"type={e.type}, replyToken={e.replyToken?[..Math.Min(8, e.replyToken?.Length ?? 0)]}...")));

            var validEvents = events.Where(@event => !string.IsNullOrEmpty(@event.Text())).ToList();
            _logger.LogWarning("[DEBUG] Valid text events: {Count}, elapsed: {Elapsed}ms", validEvents.Count, sw.ElapsedMilliseconds);

            foreach (var validEvent in validEvents)
            {
                _logger.LogWarning("[DEBUG] Processing event: text={Text}, replyToken={Token}, elapsed: {Elapsed}ms",
                    validEvent.Text(), validEvent.replyToken?[..Math.Min(8, validEvent.replyToken?.Length ?? 0)], sw.ElapsedMilliseconds);

                foreach (var eventHandler in _eventHandlers)
                {
                    await eventHandler.Act(validEvent);
                }

                _logger.LogWarning("[DEBUG] Event processed, elapsed: {Elapsed}ms", sw.ElapsedMilliseconds);
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

        private void PatchReplyTokensFromRawJson(string body, List<Event> events)
        {
            try
            {
                var json = JsonDocument.Parse(body);
                var rawEvents = json.RootElement.GetProperty("events");

                for (int i = 0; i < events.Count && i < rawEvents.GetArrayLength(); i++)
                {
                    var rawEvent = rawEvents[i];
                    if (rawEvent.TryGetProperty("replyToken", out var rawToken))
                    {
                        var rawTokenStr = rawToken.GetString();
                        var sdkToken = events[i].replyToken;

                        if (rawTokenStr != sdkToken)
                        {
                            _logger.LogWarning("[DEBUG] Reply token mismatch! SDK={SdkToken}, Raw={RawToken}", sdkToken, rawTokenStr);
                            events[i].replyToken = rawTokenStr;
                        }
                        else
                        {
                            _logger.LogWarning("[DEBUG] Reply token match confirmed: {Token}", rawTokenStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DEBUG] Failed to parse raw JSON for reply token extraction");
            }
        }
    }
}
