using System.Threading.Tasks;

namespace LittleFlowerBot.Models.Message
{
    public interface ILineUserService
    {
        Task<bool> IsFollower(string userId);
    }
}
