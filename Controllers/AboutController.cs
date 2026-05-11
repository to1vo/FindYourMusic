using Microsoft.AspNetCore.Mvc;

namespace FindYourMusic.Controllers {
    public class AboutController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
