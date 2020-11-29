using System.Linq;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleFlowerBot.Controllers
{
    public class MgmtController : Controller
    {
        private readonly IGameBoardCache _gameBoardCache;

        public MgmtController(IGameBoardCache gameBoardCache)
        {
            _gameBoardCache = gameBoardCache;
        }
        
        public async Task<ActionResult> ClearGame(string gameId)
        {
            await _gameBoardCache.Remove(gameId);
            return Content("OK");
        }
        
        public ActionResult Index()
        {
            return View(new MgmtIndexModel());
        }
        
        public ActionResult CacheMonitor()
        {
            return View(new CacheMonitorModel()
            {
                GameIdList = _gameBoardCache.GetGameIdList()
            });
        }
        
    }
}