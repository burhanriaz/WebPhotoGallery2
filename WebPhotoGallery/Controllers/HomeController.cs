using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebPhotoGallery.Data;
using WebPhotoGallery.Models;

namespace WebPhotoGallery.Controllers
{
    [Authorize]
    //[Route("Home")]
    //[Route("[/Home]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;

        public HomeController(UserManager<ApplicationUser> userManager, AppDbContext dbContext, ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public  async Task< IActionResult> Index()
        {
            var HomepageDetails = new HomePageDetailsVM()
            {
                TotalRegisterUser = _userManager.Users.Count(),
                TodayRegisterUser = _userManager.Users.Where(x => x.RegDate.Date == DateTime.Today.Date).Count(),
                TodayUploadPicture = _dbContext.Pictures.Where(x => x.CreateDate.Date == DateTime.Today.Date).Count(),
                TotalPicture = _dbContext.Pictures.Count()
            };
            return View(HomepageDetails);
        }

        [Authorize]
        public IActionResult ContactUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}