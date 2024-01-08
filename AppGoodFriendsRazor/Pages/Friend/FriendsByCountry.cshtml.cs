using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class FriendsByCountryModel : PageModel
    {
        readonly IFriendsService service;
        //public List<IFriend> Friends { get; set; } = new List<IFriend>();

        public List<FriendsByCountry> FriendsByCountries = new List<FriendsByCountry>();
        //public List<CountryForFriend> Countries { get; set; } = new List<CountryForFriend>();

        //public List<IGrouping<string, IFriend>> FriendByCountries { get; set; }   

        #region Constructor
        public FriendsByCountryModel(IFriendsService service)
        {
            this.service = service;
        }
        #endregion

        public async Task<IActionResult> OnGetAsync()  //string country   
        {
            //Friends = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);

            var info = await service.InfoAsync;
            var countryList = info.Friends;

            var gangByCountry = countryList.GroupBy(x => x.Country);
            foreach (var gang in gangByCountry)
            {
                var f = new FriendsByCountry();
                f.Countries = gang.Key;
                f.Cities = gang.Where(x => x.City != null).ToList();
                f.TotalFriends = f.Cities.Sum(x => x.NrFriends); //Correct
                f.TotalCities = f.Cities.Count(); //Correct 

                FriendsByCountries.Add(f);
            }

            return Page();
        }


        /* 
         * OLD CODE
        // Group by country for total count
        //Group friends by country based on addresses from the Friends collection
        Countries = Friends
               .Where(c => c.Address != null && !string.IsNullOrEmpty(c.Address.Country))
               .GroupBy(c => c.Address.Country)
               .Select(c => new CountryForFriend
               {
                   Land = c.Key,
                   TotalFriend = c.Count(),
                   TotalCity = c.Count()
               })
               .ToList();

        // I started with this block 
        FriendByCountries = Friends
                .Where(c => c.Address != null && !string.IsNullOrEmpty(c.Address.Country))
                .GroupBy(c => c.Address.Country)
                .ToList();
        foreach (var group in FriendByCountries)
        {
            Console.WriteLine($"Group: {group.Key}");
            //foreach (var friend in group)
            //{
            //    Console.WriteLine($" - {friend.}");
            //}
        }
        return Page();

    }

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
    // return Page(); 

    
        */

        public class FriendsByCountry
        {
            public string Countries { get; set; }
            public int TotalFriends { get; set; }
            public int TotalCities { get; set; }
            public List<gstusrInfoFriendsDto> Cities { get; set; }
        }

    }
}
