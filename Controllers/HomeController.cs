using System.Diagnostics;
using FindYourMusic.Models;
using Microsoft.AspNetCore.Mvc;
using FindYourMusic.Services;
using System.Security.Claims;

namespace FindYourMusic.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly TrackService _trackService;
        private readonly UserService _userService;

        public HomeController(ILogger<HomeController> logger, TrackService trackService, UserService userService) {
            _logger = logger;
            _trackService = trackService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(){
            try {
                List<Track> recentlyDescribedTracks = await _trackService.GetRecentlyDescribedTracks();
                List<Track>? recommendedTracks = null;
                if (User.Identity.IsAuthenticated) {
                    var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var favoriteCategories = await _userService.GetFavoriteCategories(userId);
                    var bookmarkedTracks = await _userService.GetAllBookmarks(userId);
                    recommendedTracks = await _trackService.GetRecommendedTracks(userId, favoriteCategories, bookmarkedTracks);
                }
                HomeViewModel vm = new HomeViewModel(recentlyDescribedTracks, recommendedTracks);
                return View(vm);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error-state"] = "Something went wrong retreiving data";
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(){
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
