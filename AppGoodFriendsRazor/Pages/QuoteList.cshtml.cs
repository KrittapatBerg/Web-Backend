using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages
{
    public class QuoteListModel : PageModel
    {
        IFriendsService service;
        ILogger<QuoteListModel> logger;
        loginUserSessionDto usr;

        public List<IFriend> QuotesList { get; set; } = new List<IFriend>();

        #region HTTP Requests 
        public async Task OnGetAsync()
        {
            var quote = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);
            QuotesList = quote.ToList();

        }
        #endregion


        #region Constuctor
        public QuoteListModel(IFriendsService service, ILogger<QuoteListModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion

    }
}
