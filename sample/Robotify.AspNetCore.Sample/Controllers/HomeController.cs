using Microsoft.AspNetCore.Mvc;
using Robotify.AspNetCore.MetaTags;

namespace Robotify.AspNetCore.Sample.Controllers
{
    [RobotifyMetaTag("noindex", "nofollow")]
    public class BaseController : Controller
    {

    }

    public class HomeController : BaseController
    {
        [RobotifyMetaTag("noarchive", UserAgent = "googlebot")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/{page}")]
        public IActionResult IndexTester(string page)
        {
            return View("Index");
        }

        [HttpGet("/defined")]
        [RobotifyMetaTag("nofollow", Inherited = false)]
        public IActionResult Defined(string page)
        {
            return View("Index");
        }
    }
}
