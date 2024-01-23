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
            //A push start from Martin 

            var info = await _service.InfoAsync;
            var friends = info.Friends;
            var pets = info.Pets;

            var friendsByCountry = friends.GroupBy(f => f.Country);
            foreach (var item in friendsByCountry)
            {

                var f = new FriendsByCountry();
                f.Countries = item.Key;
                f.Cities = item.Where(f => f.City != null).ToList();

                FriendsByCountries.Add(f);
            }
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
