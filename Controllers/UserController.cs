using FindYourMusic.Models;
using FindYourMusic.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FindYourMusic.Controllers {
    public class UserController : Controller {
        private readonly UserService _userService;

        public UserController(UserService userService) {
            _userService = userService;
        }

        [Authorize]
        [Route("/Profile")]
        public async Task<IActionResult> Index() {
            var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try {
                var tracksDescribedAmount = _userService.tracksDescribedAmount(userId);
                var recentDescriptions = await _userService.GetRecentTrackDescriptions(userId);
                var recentBookmarks = await _userService.GetRecentBookmarks(userId);
                var favoriteCategories = await _userService.GetFavoriteCategories(userId);
                var bookmarksAmount = await _userService.GetAllBookmarks(userId);

                UserProfileViewModel vm = new UserProfileViewModel(tracksDescribedAmount, recentDescriptions, recentBookmarks, favoriteCategories, bookmarksAmount.Count);
                return View(vm);
            } catch(Exception ex) {
                Console.WriteLine(ex);
                TempData["error-state"] = "Something went wrong retreiving user data";
                return View();
            }
        }

        [Authorize]
        [Route("/Profile/Bookmarked")]
        public async Task<IActionResult> Bookmarked(int? pageNumber) {
            var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try {
                var tracksDescribed = _userService.tracksDescribedAmount(userId);
                var bookmarkedTracks = await _userService.GetAllBookmarks(userId);
                
                UserBookmarksViewModel vm = new UserBookmarksViewModel(tracksDescribed, bookmarkedTracks, pageNumber);
                return View(vm);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error-state"] = "Something went wrong retreiving user data";
                return View();
            }
        }

        [Authorize]
        [Route("/Profile/Described")]
        public async Task<IActionResult> Described(int? pageNumber) {
            var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try {
                var tracksDescribed = _userService.tracksDescribedAmount(userId);
                var userTrackDescriptions = await _userService.GetAllTrackDescriptions(userId);

                UserDescribedViewModel vm = new UserDescribedViewModel(tracksDescribed, userTrackDescriptions, pageNumber);
                return View(vm);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error-state"] = "Something went wrong retreiving user data";
                return View();
            }
        }

        [Authorize]
        public IActionResult Edit() {
            return View(new UserEditViewModel(User.Identity.Name));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel vm) {
            if (!ModelState.IsValid) {
                return View(vm);
            }

            if(vm.username == User.Identity.Name) {
                return RedirectToAction(nameof(Index));
            }

            var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try {
                bool usernameExists = await _userService.UsernameExists(vm.username.Trim());
                if ((bool)usernameExists) {
                    TempData["error"] = "Username already exists";
                    return View();
                }
                if(!await _userService.UpdateUser(userId, vm.username)) {
                    TempData["error"] = "Something went wrong";
                    return View();
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, vm.username), new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction(nameof(Index));
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error"] = "Something went wrong";
                return View();
            }
        }
    }
}
