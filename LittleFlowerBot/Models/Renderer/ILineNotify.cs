using System.Threading.Tasks;

namespace LittleFlowerBot.Models.Renderer
{
    public interface ILineNotify
    {
        Task SaveToken(string senderId, string code);
        
        string GenerateLink(string guid);

        bool IsRegistered(string senderId);
    }
}