using Bulky.DataAccess.Data;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager ,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _db=db;
        }
        public void Initialize()
        {
            //migartions if they aren't applied
            try {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
                    }
            catch(Exception ex) { }

            //create roles if they are not created

            if (!_roleManager.RoleExistsAsync(StaticDetails.Customer_Role).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Customer_Role)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Company_Role)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Employee_Role)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Admin_Role)).GetAwaiter().GetResult();

                //if roles aren't created , then we will create admin user as well

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "IbrahimAdmin@gmail.com",
                    Email = "IbrahimAdmin@gmail.com",
                    Name = "Ebrahim Shaban",
                    PhoneNumber = "01287832221",
                    Street = "Shawa",
                    City = "Mansoura",
                    Country = "Egypt"

                }, "Admin301002").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "IbrahimAdmin@gmail.com");
                _userManager.AddToRoleAsync(user, StaticDetails.Admin_Role).GetAwaiter().GetResult();

            }
            return;
        }
    }
}
