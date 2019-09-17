using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityExample.Areas.Identity.Pages.Account.Administration.Users
{
    [Authorize(Policy = "PolicyAdmin")]
    public partial class AdminEditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminEditModel> _logger;

        public string UserId;
        public ApplicationUser UserDetails;

        public AdminEditModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AdminEditModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "E-mail")]
            public string Email { get; set; }

            [Required]
            [MinLength(2)]
            [MaxLength(32)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [MinLength(2)]
            [MaxLength(32)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "User has confirmed e-mail?")]
            public bool EmailConfirmed { get; set; }

            [Required]
            [Display(Name = "User locked out?")]
            public bool LockoutEnabled { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            string UserId = id; //UID from URI
            UserDetails = await _userManager.FindByIdAsync(UserId);
            if (UserDetails == null)
            {
                return NotFound($"Unable to load user from database.");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            string UserId = id; //UID from URI
            UserDetails = await _userManager.FindByIdAsync(UserId);
            if (UserDetails == null)
            {
                return NotFound($"Unable to load user from database.");
            }
            if (ModelState.IsValid)
            {
                UserDetails.UserName = Input.UserName;
                UserDetails.FirstName = Input.FirstName;
                UserDetails.LastName = Input.LastName;
                if (UserDetails.Email != Input.Email)
                {
                    var setEmail = await _userManager.SetEmailAsync(UserDetails, Input.Email);
                    if (!setEmail.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to update e-mail address");
                        return Page();
                    }
                }
                UserDetails.EmailConfirmed = Input.EmailConfirmed;
                UserDetails.LockoutEnabled = Input.LockoutEnabled;
                var result = await _userManager.UpdateAsync(UserDetails);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Administrator edited user account: {UserDetails.Id}.");
                    return RedirectToPage("../Index");
                }
                else
                {
                    ModelState.AddModelError(Input.UserName, "Failed to update database.");
                    return Page();
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}