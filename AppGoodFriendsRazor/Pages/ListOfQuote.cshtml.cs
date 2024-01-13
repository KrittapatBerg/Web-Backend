using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages
{
    public class ListOfQuoteModel : PageModel
    {
        IFriendsService service = null;
        ILogger<ListOfQuoteModel> logger = null;
        loginUserSessionDto usr = null;

        public List<IQuote> QuoteList { get; set; } // = new List<IFriend>();

        //[BindProperty]
        //public List<csFriendQuoteIM> QuotesIM { get; set; }
        //[BindProperty]
        //public csFriendQuoteIM NewQuoteIM { get; set; } = new csFriendQuoteIM();





        #region HTTP Requests 
        public async Task<IActionResult> OnGetAsync()
        {
            QuoteList = await service.ReadQuotesAsync(usr, true, false, "", 0, 100);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteQuote(Guid id)
        {
            var qDelete = await service.DeleteQuoteAsync(usr, id);
            QuoteList = await service.ReadQuotesAsync(usr, true, false, "", 0, 100);
            return Page();
        }
        #endregion

        #region Constructor
        //Inject services just like in WebApi
        public ListOfQuoteModel(IFriendsService service, ILogger<ListOfQuoteModel> logger)
        {
            this.logger = logger;
            this.service = service;
        }
        #endregion

        #region Input Model
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

        public class csFriendQuoteIM
        {
            //Status of InputModel
            public enStatusIM StatusIM { get; set; }

            //Properties from Model which is to be edited in the <form>
            public Guid QuoteId { get; set; } = Guid.NewGuid();
            public string Quote { get; set; }
            public string Author { get; set; }

            //Added properites to edit in the list with undo
            public string editQuote { get; set; }
            public string editAuthor { get; set; }

            #region constructors and model update
            public csFriendQuoteIM() { StatusIM = enStatusIM.Unchanged; }

            //Copy constructor
            public csFriendQuoteIM(csFriendQuoteIM original)
            {
                StatusIM = original.StatusIM;

                QuoteId = original.QuoteId;
                Quote = original.Quote;
                Author = original.Author;

                editQuote = original.editQuote;
                editAuthor = original.editAuthor;
            }

            //Model => InputModel constructor
            public csFriendQuoteIM(IQuote original)
            {
                StatusIM = enStatusIM.Unchanged;
                QuoteId = original.QuoteId;
                Quote = editQuote = original.Quote;
                Author = editAuthor = original.Author;
            }

            //InputModel => Model
            public IQuote UpdateModel(IQuote model)
            {
                model.QuoteId = QuoteId;
                model.Quote = Quote;
                model.Author = Author;
                return model;
            }
            #endregion

        }
        #endregion
    }
}
