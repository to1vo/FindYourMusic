using FindYourMusic.Models;
using FindYourMusic.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FindYourMusic.Controllers {
    public class SearchController : Controller {
        private readonly CategoryService _categoryService;
        private readonly TrackService _trackService;
        private readonly UserService _userService;

        public SearchController(CategoryService categoryService, TrackService trackService, UserService userService) {
            _categoryService = categoryService;
            _trackService = trackService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(List<int> categories) {
            try {
                var categoryGroups = await _categoryService.GetAllCategoryGroups();
                var allCategories = await _categoryService.GetAllCategories();
                
                if (categories == null || categories.Count() > 4) {
                    categories = [];
                }

                SearchViewModel vm = new SearchViewModel(categoryGroups, allCategories, categories);
                return View(vm);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error-state"] = "Something went wrong retreiving data";
                return View();
            }
        }

        public async Task<IActionResult> Results(List<int> categories, int? pageNumber) {
            if(categories == null || categories.Count() == 0 || categories.Count() > 4) {
                return RedirectToAction(nameof(Index));
            }

            try {
                List<TrackDescription> results = await _trackService.GetTracksThatFitCategories(categories);
                List<Category> selectedCategories = await _categoryService.GetCategoriesByIds(categories);
                TempData["goBackUrl"] = "/Search"+Request.QueryString;
                if (User.Identity.IsAuthenticated) {
                    var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var trackIds = results.Select(r => r.track.id).ToList();
                    var userBookmarkedTracks = await _userService.GetBookmarkedTracksFromIds(userId, trackIds);
                    SearchResultsViewModel vm = new SearchResultsViewModel(selectedCategories, results, pageNumber, userBookmarkedTracks);
                    return View(vm);
                } else {
                    SearchResultsViewModel vm = new SearchResultsViewModel(selectedCategories, results, pageNumber, null);
                    return View(vm);
                }
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error-state"] = "Something went wrong";
                return View();
            }
        }
    }
}
