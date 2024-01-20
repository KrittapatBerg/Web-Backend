using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages
{
    public class FriendListModel : PageModel
    {
        IFriendsService service;
        ILogger<FriendListModel> logger;
        loginUserSessionDto usr = null;

        public List<IFriend> FriendsList { get; set; } = new List<IFriend>();

        //Pagination
        public int NrOfPages { get; set; }
        public int PageSize { get; } = 5;

        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;
        public int PresentPages { get; set; } = 0;

        //ModelBinding for the form
        [BindProperty]
        public string SearchFilter { get; set; } = null;

        #region HTTP request
        public async Task OnGet()
        {
            var friend = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);
            FriendsList = friend.ToList();
        }
        #endregion

        #region Constructor
        public FriendListModel(IFriendsService service, ILogger<FriendListModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion
    }
}