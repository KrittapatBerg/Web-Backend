using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class ListOfFriendModel : PageModel
    {
        readonly IFriendsService service;
        readonly ILogger<ListOfFriendModel> logger;
        loginUserSessionDto usr = null;

        public List<IFriend> FriendsList { get; set; } = new List<IFriend>();
        public List<IAddress> AddressList { get; set; } = new List<IAddress>();

        //public List<csFriendListInCity> ListOfFriends { get; set; }
        //public List<csFriendListInCity> ListNrQuotes { get; set; }
        //public List<IAddress> AddressList { get; set; }
        //public List<IPet> PetList { get; set; }
        //public List<IQuote> QuoteList { get; set; }
        //public List<csFriendListInCity> QuoteList { get; set; }
        //public class csFriendListInCity
        //{
        //    public string Cities { get; set; }
        //    public int NrOfQuotes { get; set; }
        //    public int NrOfPets { get; set; }

        //}
        //public string Country { get; set; }
        //public string City { get; set; }

        public async Task<IActionResult> OnGet() //string country, string city
        {
            //Country = country;
            //City = city;

            //var info = await service.InfoAsync;
            //var friendInfo = info.Friends;
            //var petInfo = info.Pets;

            //var friendInfo = info.Friends.Where(x => x.City == city && x.City != null).ToList();
            //var petInfo = info.Pets.Where(x => x.City == city && x.City != null).ToList();

            var idList = Request.Query["idList"].ToString();

            var friendsList = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);
            var inCity = friendsList.Where(x => x.Address != null).ToList();
            var friendInCity = inCity.Where(x => x.Address.City == idList).ToList();
            FriendsList = friendInCity;

            return Page();
            //FriendsList = friendInfo;
            //foreach (var item in friendInfo)
            //{
            //    var f = new csFriendListInCity();
            //    f.Cities = item.
            //    f.NrOfPets = f.NrOfPets;
            //    f.NrOfQuotes = f.NrOfQuotes;

            //    FriendsList.Add(f);
            //}

            //PetList = await service.ReadPetsAsync(null, true, false, "", 0, 100);

            //var friendInfo = info.Friends
            //                    .Where(x => x.Country == country && x.City == city
            //                    && x.City != null).ToList();
            ////.GroupBy(x => x.City != null)

            //var petInfo = info.Pets
            //                    .Where(x => x.Country == country && x.City != null)
            //                    .GroupBy(x => x.City != null).ToList();


            //AddressList = await service.ReadAddressesAsync(usr, true, false, "", 0, 100);
            //PetList = await service.ReadPetsAsync(usr, true, false, country, 0, 100);
            //QuoteList = await service.ReadQuotesAsync(usr, true, false, " ", 0, 100);
            //petpet = PetList.Where()
        }

        //TODO: Work on search and Pagination if there's time.
        //Dont forget about the <a href> tag  
        #region Inject service
        public ListOfFriendModel(IFriendsService service, ILogger<ListOfFriendModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion
    }
}
