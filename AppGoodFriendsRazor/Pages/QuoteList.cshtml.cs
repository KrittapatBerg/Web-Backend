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

        //TODO: I want the same collapsable editable from AppStudies->InputModelListAdd 
        //    : Make it Input Model 

        public List<IFriend> QuotesList { get; set; } = new List<IFriend>();
        public async Task OnGet()
        {
            var quote = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);
            QuotesList = quote.ToList();
        }

        #region Constuctor
        public QuoteListModel(IFriendsService service, ILogger<QuoteListModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion

        #region Input Model
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

        #endregion
    }
}
