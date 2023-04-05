using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebPhotoGallery.Data;
using WebPhotoGallery.Models;

namespace WebPhotoGallery.Seeding
{
    public class DatabaseSeeding : IDatabaseSeeding
    {

        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSeeding(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //creates an admin role and  Admin
        public async void DataabseSeed()
        {
            //create database schema if none exists
            // _context.Database.EnsureCreated();
            var role = CreateAdminRole().GetAwaiter().GetResult();
            if (role != null)
            {
                var superadmin = CreateAdmin(role.Id).GetAwaiter().GetResult();
            }
            var userRole = CreateUserRole().GetAwaiter().GetResult();
            await _context.SaveChangesAsync();
        }
        public async Task<string> CreateAdmin(string roleId)
        {
            //var hasher = new PasswordHasher<ApplicationUser>();
            var admin = new ApplicationUser
            {
                FirstName= "Admin",
                LastName= "Admin",
                RegDate= DateTime.Now,
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "Admin@gmai.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                
            };
            var userResult=await _userManager.CreateAsync(admin, "Admin@123");                  
            var rolename = await _roleManager.FindByIdAsync(roleId);
            //Adding user to the role
            var roleResult = await _userManager.AddToRoleAsync(admin, rolename.Name);
            if (roleResult.Succeeded)
                return admin.Id;
            return null;
        }
        public async Task<IdentityRole> CreateAdminRole()
        {
            //If there is already an Admin role, abort
            if (_context.Roles.Any(r => r.Name == "Admin"))
                return null;
            //Create the Admin Role
            var adminRole = new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()              
            
            };
            await _roleManager.CreateAsync(adminRole);
            return adminRole;
        }
        public async Task<IdentityRole> CreateUserRole()
        {
            if (_context.Roles.Any(r => r.Name == "User"))
                return null;
           
             var   userRole = new IdentityRole
                {
                    Name = "User",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    NormalizedName  = "USER"
                };
                //save  role
                await _roleManager.CreateAsync(userRole);             
                return userRole;
           
        }
    }
}
