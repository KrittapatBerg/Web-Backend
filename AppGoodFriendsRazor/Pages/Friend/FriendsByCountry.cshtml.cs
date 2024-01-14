using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class FriendsByCountryModel : PageModel
    {
        private readonly IFriendsService service;

        public List<FriendsByCountry> FriendsByCountries = new List<FriendsByCountry>();

        //public List<IFriend> Friends { get; set; } = new List<IFriend>();
        //public List<CountryForFriend> Countries { get; set; } = new List<CountryForFriend>();


        #region Inject service 
        public FriendsByCountryModel(IFriendsService service)
        {
            this.service = service;
        }
        #endregion

        public async Task<IActionResult> OnGetAsync() //(string country)
        {
            //Friends = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);

            var info = await service.InfoAsync;
            var countryList = info.Friends;

            var gangByCountry = countryList
                .Where(x => x.Country != null)
                .GroupBy(x => x.Country);
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

        public class FriendsByCountry
        {
            public string Countries { get; set; }
            public int TotalFriends { get; set; }
            public int TotalCities { get; set; }
            public List<gstusrInfoFriendsDto> Cities { get; set; }
        }
    }
}
