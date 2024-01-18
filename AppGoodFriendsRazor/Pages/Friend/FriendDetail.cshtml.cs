using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class FriendDetailModel : PageModel
    {
        readonly IFriendsService service;
        readonly ILogger<FriendDetailModel> logger;
        loginUserSessionDto usr;

        public List<IFriend> FriendDetail { get; set; } = new List<IFriend>();
        public async Task<IActionResult> OnGetAsync()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid id))
            {
                //Read a friend
                var detail = await service.ReadFriendAsync(usr, id, false);

                if (detail != null)
                {
                    // Add the friend to the list
                    FriendDetail.Add(detail);
                }
                else
                {
                    // Handle the case where the friend is not found
                    ModelState.AddModelError(string.Empty, "Friend not found.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Whoops, friend does not exist.");
            }
            return Page();
        }

        #region Constructor and inject service
        public FriendDetailModel(IFriendsService service, ILogger<FriendDetailModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion
    }
}
