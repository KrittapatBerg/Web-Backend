using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class FriendListModel : PageModel
    {
        readonly IFriendsService service;
        //readonly loginUserSessionDto usr;

        public List<IFriend> Friends { get; set; } = new List<IFriend>();
        public List<FriendsByCountry> FriendsByCountries = new List<FriendsByCountry>();

        public async Task<IActionResult> OnGetAsync(string country)
        {
            //var info = await service.InfoAsync;
            //var friends = info.Friends;
            ////Friends = await service.ReadFriendsAsync(usr, true, false, "", 0, 50);
            //var friendsByCountry = friends.GroupBy(f => f.Country);
            //foreach (var item in friendsByCountry)
            //{
            //    //We start with this and build on it 
            //    //var country = item.Key;
            //    //var friendsInCountry = item.Where(f => f.City != null).ToList();

            //    var f = new FriendsByCountry();
            //    f.Countries = item.Key;
            //    f.Cities = item.Where(f => f.City != null).ToList();

            //    FriendsByCountries.Add(f);

            //}
            return Page();
        }

        public class FriendsByCountry
        {
            public string Countries { get; set; }
            public List<gstusrInfoFriendsDto> Cities { get; set; }
        }
        public FriendListModel(IFriendsService service)
        {
            this.service = service;
        }
    }
}
