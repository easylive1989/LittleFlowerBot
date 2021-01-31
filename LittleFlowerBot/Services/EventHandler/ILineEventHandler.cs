using System.Threading.Tasks;
using isRock.LineBot;

namespace LittleFlowerBot.Services.EventHandler
{
    public interface ILineEventHandler
    {
        Task Act(Event @event);
    }
}