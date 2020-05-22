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
        private readonly GameStateCache _gameStateCache;

        public MgmtController(GameStateCache gameStateCache)
        {
            _gameStateCache = gameStateCache;
        }
        
        public async Task<ActionResult> ClearGame(string gameId)
        {
            await _gameStateCache.Remove(gameId);
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
                GameIdList = _gameStateCache.GetGameIdList()
            });
        }
        
    }
}