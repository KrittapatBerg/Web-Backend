using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class FriendInCountryModel : PageModel
    {
        readonly IFriendsService service;

        public List<FriendsByCountry> FriendsByCountries = new List<FriendsByCountry>();
        public List<FriendPetByCity> FriendPetByCities = new List<FriendPetByCity>();

        //public List<IFriend> Friends { get; set; } = new List<IFriend>();

        public async Task<IActionResult> OnGetAsync(string country)
        {
            var info = await service.InfoAsync;
            var countryList = info.Friends;
            var pet = info.Pets;

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

            var petByCity = countryList.Join(
                pet,
                c => c.City,
                p => p.City,
                (c, p) => new
                {
                    City = c.City,
                    NrFriends = c.NrFriends,
                    NrPets = p.NrPets
                });

            var groupPetByCity = petByCity.GroupBy(x => x.City);
            foreach (var group in groupPetByCity)
            {
                var petList = new FriendPetByCity();
                petList.PetCities = group.Key;
                petList.NrFriends = group.First().NrFriends;
                petList.NrPets = group.Sum(x => x.NrPets);
                petList.Pets = pet.Where(x => x.City == group.Key).ToList();

                FriendPetByCities.Add(petList);
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

        public class FriendPetByCity
        {
            public string PetCities { get; set; }
            public int NrPets { get; set; }
            public int NrFriends { get; set; }
            public List<gstusrInfoPetsDto> Pets { get; set; }
        }
        #region Constructor 
        public FriendInCountryModel(IFriendsService service)
        {
            this.service = service;
        }
        #endregion 
    }
}
