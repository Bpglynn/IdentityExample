using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityExample.Areas.Identity.Pages.Account.Administration.Roles
{
    [Authorize(Policy = "PolicyAdmin")]
    public partial class DetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public string RoleId;
        public IdentityRole RoleDetails;
        public List<ApplicationUser> UserList;

        public DetailsModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            string RoleId = id; //UID from URI
            RoleDetails = await _roleManager.FindByIdAsync(RoleId);
            
            if (RoleDetails == null)
            {
                return NotFound($"Unable to load role from database.");
            }
            UserList = await GetUsersForRole(RoleDetails); // Populate user roles
            return Page();
        }
        public async Task<List<ApplicationUser>> GetUsersForRole(IdentityRole role)
        {
            // Takes an IdentityRole object and returns list of ApplicationUser objects assigned to the role
            List<ApplicationUser> UserList = new List<ApplicationUser>();
            foreach (var user in _userManager.Users)
            {
                if (user != null && await _userManager.IsInRoleAsync(user, role.Name))
                    UserList.Add(user);
            }
            return UserList;
        }
    }
}