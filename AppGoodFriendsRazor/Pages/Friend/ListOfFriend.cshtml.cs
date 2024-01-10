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
        loginUserSessionDto usr = null;


        public List<IAddress> Addresses { get; set; }
        public List<IFriend> FriendsList { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            FriendsList = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);

            //var friendList = info.Friends.ToList();
            return Page();
        }


        #region Inject service
        public ListOfFriendModel(IFriendsService service)
        {
            this.service = service;
        }
        #endregion
    }
}
