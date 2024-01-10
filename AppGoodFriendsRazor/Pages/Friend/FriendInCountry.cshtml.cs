using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class FriendInCountryModel : PageModel
    {
        readonly IFriendsService service;
        loginUserSessionDto usr = null;

        //public List<FriendsByCountry> FriendsByCountries = new List<FriendsByCountry>();
        //public List<FriendPetByCity> FriendPetByCities = new List<FriendPetByCity>();
        public class csNrInCity
        {
            public string City { get; set; }
            public int NrOfFriends { get; set; }
            public int NrOfPets { get; set; }

        }
        public string Country { get; set; }
        public List<csNrInCity> Cities { get; set; }
        //public List<IAddress> Addresses { get; set; }
        //public List<IFriend> Friends { get; set; }
        //public Dictionary<string, int> FriendInCity { get; set; } = new Dictionary<string, int>();
        public async Task<IActionResult> OnGetAsync(string country)
        {
            Country = country;
            var info = await service.InfoAsync;

            var cityfriends = info.Friends.Where(x => x.Country == country && x.City != null).ToList();
            var citypets = info.Pets.Where(x => x.Country == country && x.City != null).ToList();

            Cities = cityfriends.Join(citypets, f => f.City, p => p.City,
                (f, p) => new csNrInCity() { City = f.City, NrOfFriends = f.NrFriends, NrOfPets = p.NrPets }).ToList();

            //Addresses = await service.ReadAddressesAsync(usr, true, false, country, 0, 100);
            //Friends = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);

            return Page();

        }
        /*
         * Just in case
        public class FriendsByCountry
        {
            public string Countries { get; set; }
            public string Cities { get; set; }
            public int TotalFriends { get; set; }
            public int TotalPets { get; set; }
            public int TotalCities { get; set; }

            //public List<gstusrInfoFriendsDto> City { get; set; }
        }

        public class FriendPetByCity
        {
            public string PetCities { get; set; }
            public int NrPets { get; set; }
            public int NrFriends { get; set; }
            public List<gstusrInfoPetsDto> Pets { get; set; }
        }
        */

        #region Inject service 
        public FriendInCountryModel(IFriendsService service)
        {
            this.service = service;
        }
        #endregion
    }
}
