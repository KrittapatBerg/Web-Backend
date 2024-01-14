using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages
{
    public class FriendListModel : PageModel
    {
        IFriendsService service;
        ILogger<FriendListModel> logger;
        loginUserSessionDto usr = null;

        public List<IFriend> FriendsList { get; set; } = new List<IFriend>();
        #region HTTP request
        public async Task OnGet()
        {
            var friend = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);
            FriendsList = friend.ToList();
        }
        #endregion

        #region Constructor
        public FriendListModel(IFriendsService service, ILogger<FriendListModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion
    }
}