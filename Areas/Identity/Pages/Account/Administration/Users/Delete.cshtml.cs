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
    public partial class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DeleteModel> _logger;

        public string UserId;
        public ApplicationUser UserDetails;

        public DeleteModel(UserManager<ApplicationUser> userManager, ILogger<DeleteModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

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
            var result = await _userManager.DeleteAsync(UserDetails);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Administrator deleted user account: {UserDetails.Id}.");
                return RedirectToPage("../Index");
            }
            else
            {
                StatusMessage = "Delete operation failed at database.";
                return RedirectToPage("../Index");
            }
        }
    }
}