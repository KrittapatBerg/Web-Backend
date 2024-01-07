using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class FriendsByCountryModel : PageModel
    {
        readonly IFriendsService service;
        readonly loginUserSessionDto usr;
        public List<IFriend> Friends { get; set; } = new List<IFriend>();
        public List<IAddress> Addresses { get; set; } = new List<IAddress>();

        public List<FriendsByCountry> FriendsByCountries = new List<FriendsByCountry>();
        public List<CountryForFriend> Countries { get; set; } = new List<CountryForFriend>();

        //public List<IGrouping<string, IFriend>> FriendByCountries { get; set; }

        public async Task<IActionResult> OnGetAsync(string country) //string country
        {
            var info = await service.InfoAsync;
            var friends = info.Friends;

            var friendsByCountry = friends.GroupBy(f => f.Country);
            foreach (var item in friendsByCountry)
            {
                /*We start with this and build on it 
                var place = item.Key;
                var friendsInCountry = item.Where(f => f.City != null).ToList(); */

                //do if() here to take away null cities/ country 
                var f = new FriendsByCountry();
                f.Countries = item.Key;
                f.Cities = item.Where(f => f.City != null).ToList();

                FriendsByCountries.Add(f);

            }

            //Friends = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);

            //Group friends by country based on addresses from the Friends collection
            //var friendInCountry = friends
            var lala = await service.ReadAddressesAsync(usr, true, false, "", 0, 100);

            /*   Countries = lala
                   .Where(c => c.Address != null && !string.IsNullOrEmpty(c.Address.Country))
                   .GroupBy(c => c.Address.Country)
                   .Select(c => new CountryForFriend
                   {
                       Land = c.Key,
                       TotalFriend = c.Count(),
                       TotalCity = c.Count()
                   })
                   .ToList();
               */

            /* I started with this block 
            FriendByCountries = friendByCountry;
            foreach (var group in friendByCountry)
            {
                Console.WriteLine($"Group: {group.Key}");
                foreach (var friend in group)
                {
                    Console.WriteLine($" - {friend.FirstName}");
                }
            }*/

            return Page();

            //FriendByCountries = await service.ReadAddressesAsync(usr, true, false, "", 0, 100);
            //var groupByCountry = FriendByCountries.GroupBy(f => f.Country).ToList();
            //Friends = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);
            //var friendByCountry = Friends.Where(c => c.Address != null && !string.IsNullOrEmpty(c.Address.Country))
            //                             .GroupBy(c => c.Address.Country).ToList();
            //foreach (var item in groupByCountry)
            //{
            //    Console.WriteLine(item);
            //}


            //        .Where(f => f.Address != null && !string.IsNullOrEmpty(f.Address.Country))
            //        .GroupBy(f => f.Address.Country)
            //.Select(c => new CountryFriendCount
            //    //{
            //    //    Country = c.Key,
            //    //    FriendCount = c.Count()
            //    //})
            //.ToList();
        }

        public class CountryForFriend()
        {
            public required string Land { get; set; }
            public int TotalCity { get; set; }
            public int TotalFriend { get; set; }

        }
        public class FriendsByCountry
        {
            public string Countries { get; set; }
            public List<gstusrInfoFriendsDto> Cities { get; set; }
        }

        #region Constructor
        public FriendsByCountryModel(IFriendsService service)
        {
            this.service = service;
        }
        #endregion
    }
}
