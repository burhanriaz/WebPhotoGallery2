using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using WebPhotoGallery.Data;
using WebPhotoGallery.Helper;
using WebPhotoGallery.Models;

namespace WebPhotoGallery.Controllers
{
    [Authorize]
    //[Route("Picture")]
    //[Route("[Picture]")]
    public class PictureController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PictureController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> UserManager, AppDbContext dbContext, IWebHostEnvironment hostingEnvironment)
        {
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
            _UserManager = UserManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pictures = await _dbContext.Pictures.ToListAsync();
            return View(pictures);
        }

        [HttpGet("UploadPicture")]
        public IActionResult UploadPicture()
        {
            return View();
        }

        [HttpPost("UploadPicture")]
        public async Task<IActionResult> UploadPicture(PictureVM pictures)
        {
            if (pictures.Name == null )
            {
                if (!ModelState.IsValid)
                    return View();
            }
            var user = await _UserManager.GetUserAsync(User);
            var rolecheck = await _UserManager.IsInRoleAsync(user,Constant.User);
                var count = await _dbContext.Pictures.Where(x => x.ApplicationUserId == user.Id).CountAsync();
            if (rolecheck && count>5)
            {
                ViewBag.LimitExceeded = "Your upload Limit Exceeded";
                return View();
            }
            var pic = new Pictures()
            {
                Name = pictures.Name,
                Description = pictures.Description,
                CreateDate = DateTime.Now.Date,
                ApplicationUserId = user.Id,
                ApplicationUser = user,
            };
            var path = await UploadImage();
            pic.ImageUrl = path;

            await _dbContext.Pictures.AddAsync(pic);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Picture");
        }
        private async Task<string> UploadImage()
        {
            var webrootpath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            string RelativeImagePath = null;
            if (files.Count != 0)
            {
                Directory.CreateDirectory(webrootpath + @"\imgs\");
                var Imagepath = @"\imgs\";
                var Extention = Path.GetExtension(files[0].FileName);
                Guid obj = Guid.NewGuid();
                RelativeImagePath = Imagepath + obj.ToString() + Extention;
                var AbsPath = webrootpath + RelativeImagePath;
                using (var stream = new FileStream(AbsPath, FileMode.Create))
                {
                    await files[0].CopyToAsync(stream);
                    ViewBag.Message = Constant.ImageUploaded;

                }
            }
            return RelativeImagePath;

        }

        [HttpGet("EditPicture")]
        public async Task<IActionResult> EditPicture(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }
            var pic = await _dbContext.Pictures.SingleOrDefaultAsync(m => m.Id == id);

            if (pic == null)
            {
                return NotFound();
            }
            return View(pic);
        }
        [HttpPost("EditPicture")]
        public async Task<IActionResult> EditPicture(PictureVM pictures)
        {
            if (pictures.Name == null)
            {
                if (!ModelState.IsValid)
                    return View();
            }
            var pic = await _dbContext.Pictures.FirstOrDefaultAsync(x => x.Id == pictures.Id);
            var user = await _UserManager.GetUserAsync(User);

            pic.Name = pictures.Name;
            pic.Description = pictures.Description;
            var path = await UploadImage();
            if (path != null)
            {
                var webrootpath = _hostingEnvironment.WebRootPath;
                var imageFolderPath = "imgs";
                var imageFileName = Path.GetFileName(pic.ImageUrl);
                var existingPath = Path.Combine(webrootpath, imageFolderPath, imageFileName);
                if (System.IO.File.Exists(existingPath))
                {
                    System.IO.File.Delete(existingPath);
                }

                pic.ImageUrl = path;
            }

            _dbContext.Pictures.Update(pic);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Picture");


        }

        [HttpPost("DeletePicture")]
        public async Task<IActionResult> DeletePicture(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pic = await _dbContext.Pictures.FirstOrDefaultAsync(x => x.Id == id);
            var user = await _UserManager.GetUserAsync(User);
            if (pic.ApplicationUserId == user.Id || User.IsInRole(Constant.Admin))
            {
                var webrootpath = _hostingEnvironment.WebRootPath;
                var imageFolderPath = "imgs";
                var imageFileName = Path.GetFileName(pic.ImageUrl);
                var existingPath = Path.Combine(webrootpath, imageFolderPath, imageFileName);
                if (System.IO.File.Exists(existingPath))
                {
                    System.IO.File.Delete(existingPath);
                }
                _dbContext.Pictures.Remove(pic);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Picture");


        }
        [AllowAnonymous]
        [Route("AboutSite")]
        public IActionResult AboutSite()
        {
            return View();
        }

        [HttpGet("ViewfullPicture")]
        public IActionResult ViewfullPicture(int id)
        {
            var picture = _dbContext.Pictures.Find(id);
            return View(picture);
        }



    }
}
