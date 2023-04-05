using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhotoGallery.Data;
using WebPhotoGallery.Helper;
using WebPhotoGallery.Models;
using WebPhotoGallery.Service;

namespace WebPhotoGallery.Controllers
{
    [Authorize]
    //[Route("Administrator")]
    //[Route("[Administrator]")]
    public class AdministratorController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministratorController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this._roleManager = roleManager;
            _UserManager = userManager;
        }
   
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index(string SearchString, string SortOrder)
        {
            if (String.IsNullOrEmpty(SortOrder))
                SortOrder = "Name_desc";
            else
                SortOrder = "";


            ViewBag.NameSortParam = SortOrder;
            ViewBag.CurrentSortOrder = SortOrder;
            ViewBag.CurrentFilter = SearchString;

            var users = await _UserManager.GetUsersInRoleAsync(Constant.User);

            var userslist = users.AsQueryable();
           
            if (!String.IsNullOrEmpty(SearchString))
            {
                userslist = userslist.Where(u => u.UserName.Contains(SearchString));
              
            }
            //sort logic
            switch (SortOrder)
            {
                case "Name_desc":
                    userslist = userslist.OrderByDescending(b => b.FirstName);
                    break;
                default:
                    userslist = userslist.OrderBy(b => b.FirstName);
                    break;
            }
            var result = new PagedResult<ApplicationUser>
            {
                Data = userslist.AsNoTracking().ToList(),
                TotalItems = _UserManager.Users.Count(),
                //TotalItems = 0,// modelCount,
            PageNumber = 0,// pageNumber,
                PageSize =0,// pageSize,
            };

            return View(result);
        }
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserStatusVM model/*string id, bool status*/)
        {
            var user = await _UserManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.Message = Constant.UserNotFound;

            }
            user.EmailConfirmed = model.Status;
            var result = await _UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                EmailService.SendEmail(user.Email, model.Status);
                ViewBag.Messagesuccess = Constant.Messagesuccess;
                return RedirectToAction("Index", "Administrator");
            }
            return View(user);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
