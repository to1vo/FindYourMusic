using FindYourMusic.Data;
using FindYourMusic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FindYourMusic.Services;

namespace FindYourMusic.Controllers {
    public class AuthenticationController : Controller {
        private readonly UserService _userService;
        
        public AuthenticationController(UserService userService) {
            _userService = userService;
        }

        public IActionResult Index(string? view) {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Home");
            }
            if(view != null && view == "signup") {
                TempData["partial"] = "signup";
            }
            return View();
        }

        [HttpGet]
        [Route("/_Login")]
        public IActionResult _Login() {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Home");
            }
            return PartialView();
        }

        [HttpPost]
        [Route("/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm) {
            if (!ModelState.IsValid) {
                return PartialView("_Login");
            }

            try {
                var user = await _userService.GetExistingUser(vm.username);
                if(user == null) {
                    TempData["error"] = "Username or password is incorrect";
                    return PartialView("_Login");
                }

                var passwordsMatch = new PasswordHasher<User>().VerifyHashedPassword(user, user.hashedPassword, vm.password);
                if(passwordsMatch == PasswordVerificationResult.Failed) {
                    TempData["error"] = "Username or password is incorrect";
                    return PartialView("_Login");
                }

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.username), new Claim(ClaimTypes.NameIdentifier, user.id.ToString()) };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return Json(new { redirectUrl = Url.Action("Index", "Home") });
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error"] = "Login failed";
                return PartialView("_Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("/_Signup")]
        public IActionResult _SignUp() {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Home");
            }
            return PartialView();
        }

        [HttpPost]
        [Route("/Signup")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel vm) {
            if (!ModelState.IsValid) {
                return PartialView("_SignUp");
            }

            try {
                var username = vm.username.Trim();
                var password = vm.password.Trim();
                if (await _userService.UsernameExists(username)) {
                    TempData["error"] = "Username already exists";
                    return PartialView("_SignUp");
                }

                await _userService.CreateUser(username, password);
                TempData["success"] = "User created succesfully";
                return Json(new { redirectUrl = Url.Action("Index") });
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
                TempData["error"] = "Something went wrong signing up";
                return PartialView("_SignUp");
            }
        }
    }
}
