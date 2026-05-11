using FindYourMusic.Data;
using FindYourMusic.Models;
using FindYourMusic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FindYourMusic.Controllers {
    public class DescribeController : Controller {
        private readonly APIService _APIService;
        private readonly DatabaseContext _dbContext;
        private readonly TrackService _trackService;
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;

        public DescribeController(APIService APIService, DatabaseContext dbcontext, TrackService trackService, CategoryService categoryService, UserService userService) {
            _APIService = APIService;
            _dbContext = dbcontext;
            _trackService = trackService;
            _categoryService = categoryService;
            _userService = userService;
        }

        [Authorize]
        public async Task<IActionResult> Index(string query) {
            if (string.IsNullOrWhiteSpace(query)) {
                return View();
            }
            
            try {
                var results = await _APIService.SearchTracks(query.Trim());
                var userAddedTracks = await _trackService.GetTracksAddedByUser(query);
                results.AddRange(userAddedTracks);
                return View(results);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error"] = "Something went wrong on search";
                return View();
            }
        }

        [Authorize]
        public async Task<IActionResult> Add(string track, string artist, string gobackUrl) {
            if(track.IsNullOrEmpty() || artist.IsNullOrEmpty()) {
                return RedirectToAction("Index", "Home");
            }
            
            var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try { 
                var categoryGroups = await _categoryService.GetAllCategoryGroups();
                var categories = await _categoryService.GetAllCategories();

                var trackInfo = await _trackService.GetTrack(track, artist);
                List<int> previouslyAddedCategories = [];
                
                if (trackInfo != null) {
                    previouslyAddedCategories = await _userService.GetUserTrackCategoryIds(userId, trackInfo.id);
                } else {
                    trackInfo = await _APIService.GetTrackInfo(track, artist);
                }

                //track not found from API or database
                if(trackInfo == null) {
                    return NotFound();
                }

                DescribeTrackViewModel vm = new DescribeTrackViewModel(trackInfo, categoryGroups, categories, previouslyAddedCategories);
                TempData["gobackUrl"] = gobackUrl;
                return View(vm);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error-state"] = "Something went wrong retreiving track information";
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(List<int> categories, string trackName, string artist, string duration, string album, string albumCoverImgUrl, string gobackUrl) {
            if(categories.Count == 0 || categories.Count > 4 || trackName.IsNullOrEmpty() || artist.IsNullOrEmpty()) {
                return RedirectToAction("Index", "Home");
            }

            var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            //get the track from db
            trackName = trackName.Trim();
            artist = artist.Trim();
            var track = await _trackService.GetTrack(trackName, artist);

            try {
                //add the track to database if it doesnt exist
                if(track == null) {
                    _dbContext.Track.Add(new Track { name = trackName, artist = artist, duration = duration, album = album, albumCoverImgUrl = albumCoverImgUrl });
                    await _dbContext.SaveChangesAsync();
                    track = await _trackService.GetTrack(trackName, artist);   
                }

                //remove previously added categorization if any
                var previouslyAddedCategories = await _dbContext.UserTrackCategory.Where(x => x.userId == userId && x.trackId == track.id).ToListAsync();
                _dbContext.UserTrackCategory.RemoveRange(previouslyAddedCategories);
             
                //add new categorization
                List<UserTrackCategory> newCategories = [];
                foreach(var id in categories) {
                    newCategories.Add(new UserTrackCategory { userId = userId, categoryId = id, trackId = track.id, date = DateTime.Now });
                }
                _dbContext.UserTrackCategory.AddRange(newCategories);
                
                await _dbContext.SaveChangesAsync();
                TempData["success"] = "Your description for this track was succesfully added!";
                return RedirectToAction(nameof(Add), new { track = trackName, artist, gobackUrl});
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error"] = "Something went wrong adding your description";
                return RedirectToAction(nameof(Add), new { track = trackName, artist, gobackUrl });
            }
        }

        [Authorize]
        public IActionResult AddManual() {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddManual(AddTrackViewModel vm) {
            if (!ModelState.IsValid) {
                return View(vm);
            }

            try {
                string title = vm.title.Trim();
                string artist = vm.artist.Trim();
                if (_trackService.TrackExists(title, artist)) {
                    TempData["error"] = "Track already exists";
                    return View();
                }

                string album = vm.album == null ? null : vm.album.Trim();
                string albumCoverImgUrl = vm.albumCoverImgUrl == null ? null : vm.albumCoverImgUrl.Trim();
                if(vm.year > DateTime.Now.Year || vm.year < 0) {
                    TempData["error"] = "Invalid value for year";
                    return View();
                }
                Track track = new Track { name = title, artist = artist, duration = vm.duration, year = vm.year, album = album, albumCoverImgUrl = albumCoverImgUrl, userAdded = true };
                _dbContext.Track.Add(track);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Add), new { track = track.name, artist = track.artist });
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error"] = "Something went wrong adding the track";
                return View();
            }
        }
    }
}
