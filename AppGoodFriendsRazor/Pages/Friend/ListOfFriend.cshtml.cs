using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class ListOfFriendModel : PageModel
    {
        readonly IFriendsService service;
        readonly ILogger<ListOfFriendModel> logger;
        loginUserSessionDto usr = null;

        public List<IFriend> FriendsList { get; set; } = new List<IFriend>();

        public async Task<IActionResult> OnGetAsync()
        {
            var idList = Request.Query["idList"].ToString();

            var friendsList = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);
            var inCity = friendsList.Where(x => x.Address != null).ToList();
            var friendInCity = inCity.Where(x => x.Address.City == idList).ToList();
            FriendsList = friendInCity;

            return Page();
        }

        #region Inject service
        public ListOfFriendModel(IFriendsService service, ILogger<ListOfFriendModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion
    }
}
