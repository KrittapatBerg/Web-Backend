using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class FriendInCountryModel : PageModel
    {
        readonly IFriendsService service;
        loginUserSessionDto usr = null;

        public List<FriendsByCountry> FriendsByCountries = new List<FriendsByCountry>();
        public List<FriendPetByCity> FriendPetByCities = new List<FriendPetByCity>();

        public List<IAddress> Addresses { get; set; }
        public List<IFriend> Friends { get; set; }
        public Dictionary<string, int> FriendInCity { get; set; } = new Dictionary<string, int>();
        public async Task<IActionResult> OnGetAsync(string country)
        {

            Addresses = await service.ReadAddressesAsync(usr, true, false, country, 0, 100);
            Friends = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);

            return Page();
            /*
            var info = await service.InfoAsync;
            var countryList = info.Friends;
            var petList = info.Pets;

            FriendsByCountries = countryList
                .Where(x => x.Country != null && !string.IsNullOrEmpty(x.City))
                .DistinctBy(x => new { x.City, x.Country })
                .GroupBy(x => new { x.City, x.Country })
                .Select(group => new FriendsByCountry
                {
                    Countries = group.Key.Country,
                    Cities = group.Key.City,
                    TotalFriends = group.Sum(x => x.NrFriends),
                    TotalPets = petList
                        .Where(p => p.Country == group.Key.Country && p.City == group.Key.City)
                        .Sum(p => p.NrPets)
                }).ToList();

            return Page();
            */

            /*
             * this code kind of works but html + css is wierd 
            var petList = info.Pets;
            FriendsByCountries = countryList
                .GroupBy(x => new { x.Country, x.City })
                .Select(group => new FriendsByCountry
                {
                    Countries = group.Key.Country,
                    Cities = group.ToList(),
                    TotalFriends = group.Sum(x => x.NrFriends),
                    TotalCities = 1
                }).ToList();

            FriendPetByCities = petList
                .GroupBy(x => new { x.Country, x.City })
                .Select(group => new FriendPetByCity
                {
                    PetCities = $"{group.Key.City}, {group.Key.Country}",
                    NrPets = group.Sum(x => x.NrPets),
                    NrFriends = countryList
                        .Where(c => c.Country == group.Key.Country && c.City == group.Key.City)
                        .Sum(c => c.NrFriends),
                    Pets = group.ToList()
                }).ToList();

            return Page();
            */

            /* 
             * Let pause this block
            var gangByCountry = countryList.GroupBy(x => x.Country);
            foreach (var gang in gangByCountry)
            {
                var f = new FriendsByCountry();
                f.Countries = gang.Key;
                f.Cities = gang.Where(x => x.City != null).ToList();
                f.TotalFriends = f.Cities.Sum(x => x.NrFriends); //Correct
                f.TotalCities = f.Cities.Count(); //Correct 

                var petByCity = petList
                    .Where(p => f.Cities.Any(c => c.City == p.City))
                    .GroupBy(p => p.City);

                foreach (var petGroup in petByCity)
                {
                    var petInCity = new FriendPetByCity();
                    petInCity.PetCities = $"{petGroup.Key}, {f.Countries}";
                    petInCity.NrPets = petGroup.Sum(p => p.NrPets);
                    petInCity.NrFriends = f.Cities
                        .Where(c => c.City == petGroup.Key)
                        .Sum(c => c.NrFriends);
                    petInCity.Pets = petGroup.ToList();

                    FriendPetByCities.Add(petInCity);
                }

                FriendsByCountries.Add(f);
            }

            return Page();
            */

            /* 
            //From the first ChatGPT
            var dogCat = countryList
                         .GroupJoin(petList, f => f.City, p => p.City, (f, petGroup) => new
                         {
                             City = f.City,
                             NrFriends = f.NrFriends,
                             NrPets = petGroup.Count(), // Count the number of pets in the group
                             Pets = petGroup.ToList()
                         });
            var Catdog = dogCat.GroupBy(x => x.City);

            foreach (var pet in petList)
            {
                var hasPet = new FriendPetByCity
                {
                    PetCities = pet.City,
                    NrPets = pet.NrPets,
                };

                FriendPetByCities.Add(hasPet);
            }
             * something is wrong with this code 
            var petByCity = petList.GroupBy(x => new { x.Country, x.City });
            foreach (var group in petByCity)
            {
                var groupPetByCity = new FriendPetByCity();
                groupPetByCity.PetCities = $"{group.Key.City}, {group.Key.Country}";
                groupPetByCity.NrPets = group.Sum(x => x.NrPets); //.Count(x => x.NrPets);
                groupPetByCity.NrFriends = countryList
                    .Where(x => x.City == group.Key.City && x.Country == group.Key.Country)
                    .Sum(x => x.NrFriends);
                groupPetByCity.Pets = group.ToList();

                FriendPetByCities.Add(groupPetByCity);
            }
            return Page();
            */
            /* var groupPetByCity = petByCity.GroupBy(x => x.City);
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
            */

        }

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
        #region Constructor 
        public FriendInCountryModel(IFriendsService service)
        {
            this.service = service;
        }
        #endregion 
    }
}
