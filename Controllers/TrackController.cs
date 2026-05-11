using FindYourMusic.Data;
using FindYourMusic.Models;
using FindYourMusic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FindYourMusic.Controllers {
    public class TrackController : Controller {
        private readonly DatabaseContext _dbContext;
        private readonly TrackService _trackService;
        private readonly UserService _userService;

        public TrackController(DatabaseContext dbContext, TrackService trackService, UserService userService) {
            _dbContext = dbContext;
            _trackService = trackService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(string title, string artist, string gobackUrl) {
            try {
                //the track
                var track = await _trackService.GetTrack(title, artist);
                List<Category>? userCategories = null;
                bool? isBookmarked = null;
                if(track == null) {
                    return NotFound();
                }
                List<Track>? bookmarkedTracks = null;

                //user data
                if (User.Identity.IsAuthenticated) {
                    var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    userCategories = await _userService.GetUserTrackCategories(userId, track.id);
                    isBookmarked = _trackService.IsBookmarked(userId, track.id);
                    bookmarkedTracks = await _userService.GetAllBookmarks(userId);
                }

                var allUsedCategories = await _trackService.GetAllUsedCategories(track.id);
                var trackTop4Categories = allUsedCategories.Take(4).ToList();
                var similarTracks = await _trackService.GetSimilarTracks(track.id, allUsedCategories);
                
                TrackViewModel vm = new TrackViewModel(track, isBookmarked, userCategories, trackTop4Categories, allUsedCategories, similarTracks, bookmarkedTracks);
                TempData["gobackUrl"] = gobackUrl;
                return View(vm);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error-state"] = "Something went wrong retreiving track data";
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bookmark(string state, string trackId) {
            if (state == null || (state != "1" && state != "0") || trackId.IsNullOrEmpty()) {
                return BadRequest();
            }
            
            var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try {
                var trackIdInt = Int32.Parse(trackId);
                //check if track actually exists
                var track = _dbContext.Track.FirstOrDefault(t => t.id == trackIdInt);
                if(track == null) {
                    return BadRequest();
                }

                if (state == "1") {
                    var bookmark = _dbContext.Bookmark.FirstOrDefault(b => b.userId == userId && b.trackId == trackIdInt);
                    _dbContext.Bookmark.Remove(bookmark);
                } else {
                    _dbContext.Bookmark.Add(new Bookmark { userId = userId, trackId = trackIdInt, date = DateTime.Now });
                }
                await _dbContext.SaveChangesAsync();
                
                string newState = state == "1" ? "0" : "1";
                return Json(new { newState });
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookmarkRedirect(string state, string trackId, string returnUrl) {
            if (state == null || (state != "1" && state != "0") || trackId.IsNullOrEmpty() || returnUrl.IsNullOrEmpty()) {
                return RedirectToAction("Index", "User");
            }

            var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try {
                var trackIdInt = Int32.Parse(trackId);
                //check if track actually exists
                var track = await _trackService.GetTrackById(trackIdInt);
                if (track == null) {
                    return Redirect(returnUrl);
                }

                if (state == "1") {
                    var bookmark = _dbContext.Bookmark.FirstOrDefault(b => b.userId == userId && b.trackId == trackIdInt);
                    if(bookmark != null) {
                        _dbContext.Bookmark.Remove(bookmark);
                    } 
                } else {
                    _dbContext.Bookmark.Add(new Bookmark { userId = userId, trackId = trackIdInt, date = DateTime.Now });
                }
                await _dbContext.SaveChangesAsync();

                return Redirect(returnUrl);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error"] = "Failed to update bookmarking";
                return Redirect(returnUrl);
            }
        }
    }
}
