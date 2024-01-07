using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services;
namespace AppGoodFriendsRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IFriendsService _service;

        public List<FriendsByCountry> FriendsByCountries = new List<FriendsByCountry>();
        public List<FriendPetByCity> FriendPetByCities = new List<FriendPetByCity>();

        public async Task OnGet()
        {
            var info = await _service.InfoAsync;
            var friends = info.Friends;
            var pets = info.Pets;

            var friendsByCountry = friends.GroupBy(f => f.Country);
            foreach (var item in friendsByCountry)
            {
                //We start with this and build on it 
                //var country = item.Key;
                //var friendsInCountry = item.Where(f => f.City != null).ToList();

                var f = new FriendsByCountry();
                f.Countries = item.Key;
                f.Cities = item.Where(f => f.City != null).ToList();

                FriendsByCountries.Add(f);

            }

            //From ChatGPT
            var petList = friends
                         .GroupJoin(pets, f => f.City, p => p.City, (f, petGroup) => new
                         {
                             City = f.City,
                             NrFriends = f.NrFriends,
                             NrPets = petGroup.Count(), // Count the number of pets in the group
                             Pets = petGroup.ToList()
                         });

            foreach (var pet in petList)
            {
                var hasPet = new FriendPetByCity
                {
                    City = pet.City,
                    NrFriends = pet.NrFriends,
                    NrPets = pet.NrPets,
                    Pets = pet.Pets
                };

                FriendPetByCities.Add(hasPet);
            }

            /*var petList = friends.Join(pets, f => f.City, p => p.City, (f, p)
                            => new FriendPetByCity()
                            {
                                City = f.City,
                                NrFriends = f.NrFriends,
                                NrPets = p.NrPets
                            });
            foreach (var pet in petList)
            {
                var hasPet = new FriendPetByCity();
                hasPet.City = pet.City;
                hasPet.Pets = pet.Count(hasPet => hasPet.City != null).ToList();

                FriendPetByCities.Add(hasPet);
            }*/
        }

        public class FriendsByCountry
        {
            public string Countries { get; set; }
            public List<gstusrInfoFriendsDto> Cities { get; set; }
        }

        public class FriendPetByCity
        {
            public string City { get; set; }
            public int NrPets { get; set; }
            public int NrFriends { get; set; }
            public List<gstusrInfoPetsDto> Pets { get; set; }
        }
        public IndexModel(ILogger<IndexModel> logger, IFriendsService service)
        {
            _logger = logger;
            _service = service;
        }
    }
}
