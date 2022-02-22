using System;
using System.Linq;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_shop.DataAccess.Data;
using Web_shop.Models;
using Web_shop.Utility;

namespace Web_shop.DataAccess.Initializer 
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            TryMigrate();
            if (_roleManager.RoleExistsAsync(WC.AdminRole).GetAwaiter().GetResult())
            {
                return;
            }

            CreateRoles();
            CreateAdminUser();
        }
        
        private void TryMigrate()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateRoles()
        {
            _roleManager.CreateAsync(new IdentityRole(WC.AdminRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(WC.CustomerRole)).GetAwaiter().GetResult();
        }

        private void CreateAdminUser()
        {
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                FullName = "Admin Tester",
                PhoneNumber = "111111111111"
            }, "Admin123*").GetAwaiter().GetResult();

            var user = _db.ApplicationUser.FirstOrDefault(u => u.Email == "admin@gmail.com");
            _userManager.AddToRoleAsync(user, WC.AdminRole).GetAwaiter().GetResult();
        }
    }
}
