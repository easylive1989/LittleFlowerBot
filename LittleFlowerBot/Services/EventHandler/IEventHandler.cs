using System.Threading.Tasks;
using isRock.LineBot;

namespace LittleFlowerBot.Services.EventHandler
{
    public interface IEventHandler
    {
        Task Act(Event @event);
    }
}