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

        public class csNrInCity
        {
            public string City { get; set; }
            public int NrOfFriends { get; set; }
            public int NrOfPets { get; set; }

        }
        public string Country { get; set; }
        public List<csNrInCity> Cities { get; set; }

        public async Task<IActionResult> OnGetAsync(string country)
        {
            Country = country;
            var info = await service.InfoAsync;

            var cityfriends = info.Friends.Where(x => x.Country == country && x.City != null).ToList();
            var citypets = info.Pets.Where(x => x.Country == country && x.City != null).ToList();

            Cities = cityfriends.Join(citypets, f => f.City, p => p.City,
                (f, p) => new csNrInCity() { City = f.City, NrOfFriends = f.NrFriends, NrOfPets = p.NrPets }).ToList();

            return Page();
        }

        #region Inject service 
        public FriendInCountryModel(IFriendsService service)
        {
            this.service = service;
        }
        #endregion
    }
}
