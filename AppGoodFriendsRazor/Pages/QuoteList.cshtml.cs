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
        csQuoteCUdto item;

        //TODO: I want the same collapsable editable from AppStudies->InputModelListAdd 
        //    : Make it Input Model 
        //[BindProperty]
        //public List<csQuoteIM> QuotesIM { get; set; }

        //[BindProperty]
        //public csQuoteIM NewQuoteIM { get; set; } = new csQuoteIM();
        public List<IFriend> QuotesList { get; set; } = new List<IFriend>();

        #region HTTP Requests 
        public async Task OnGetAsync()
        {
            var quote = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);
            QuotesList = quote.ToList();

        }


        /*
        public IActionResult OnPostEdit(Guid quoteId)
        {
            //Set the Quote as Modified, it will later be updated in the database
            var q = QuotesIM.First(q => q.QuoteId == quoteId);
            q.StatusIM = enStatusIM.Modified;

            //Implement the changes
            q.Author = q.editAuthor;
            q.Quote = q.editQuote;
            return Page();
        }

        public IActionResult OnPostAdd()
        {
            //Set the Artist as Inserted, it will later be inserted in the database
            NewQuoteIM.StatusIM = enStatusIM.Inserted;

            //Need to add a temp Guid so it can be deleted and editited in the form
            //A correct Guid will be created by the DTO when Inserted into the database
            NewQuoteIM.QuoteId = Guid.NewGuid();

            //Add it to the Input Models artists
            QuotesIM.Add(new csQuoteIM(NewQuoteIM));

            //Clear the NewArtist so another album can be added
            NewQuoteIM = new csQuoteIM();

            return Page();
        }

        public IActionResult OnPostSave()
        {
            //Check if there are deleted quotes, if so simply remove them
            var deletes = QuotesIM.FindAll(q => (q.StatusIM == enStatusIM.Deleted));
            foreach (var item in deletes)
            {
                //Remove from the database
                service.DeleteQuoteAsync(usr, item.QuoteId);
            }

            #region Add quotes
            //Check if there are any new quotes added, if so create them in the database
            var newbies = QuotesIM.FindAll(q => (q.StatusIM == enStatusIM.Inserted));
            foreach (var product in newbies)
            {
                //Create the corresposning model
                var model = product.UpdateModel(new csQuoteCUdto());

                //create in the database
                //model = service.CreateQuoteAsync(usr, item);
            }
            #endregion

            //Check if there are any modified quotes , if so update them in the database
            var modifies = QuotesIM.FindAll(a => (a.StatusIM == enStatusIM.Modified));
            foreach (var product in modifies)
            {
                //get model
                var model = service.ReadQuoteAsync(usr, product.QuoteId, false);

                //update the changes and save
                //model = product.UpdateModel(model);
                model = service.UpdateQuoteAsync(usr, item);
            }

            return Page();
        }
        */
        #endregion


        #region Constuctor
        public QuoteListModel(IFriendsService service, ILogger<QuoteListModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion

        #region Input Model
        /* public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

         public class csQuoteIM
         {
             //Status of InputModel
             public enStatusIM StatusIM { get; set; }

             //Properties from Model which is to be edited in the <form>
             public Guid QuoteId { get; set; } = Guid.NewGuid();
             public Guid FriendId { get; set; }
             public string FirstName { get; set; }
             public string LastName { get; set; }
             public string Quote { get; set; }
             public string Author { get; set; }

             //Added properites to edit in the list with undo
             public string editQuote { get; set; }
             public string editAuthor { get; set; }

             #region constructors and model update
             public csQuoteIM() { StatusIM = enStatusIM.Unchanged; }

             //Copy constructor
             public csQuoteIM(csQuoteIM original)
             {
                 StatusIM = original.StatusIM;

                 QuoteId = original.QuoteId;
                 Quote = original.Quote;
                 Author = original.Author;

                 editQuote = original.editQuote;
                 editAuthor = original.editAuthor;
             }

             //Model => InputModel constructor
             public csQuoteIM(IQuote original)  //or use the Dto one 
             {
                 StatusIM = enStatusIM.Unchanged;
                 QuoteId = original.QuoteId;
                 Quote = editQuote = original.Quote;
                 Author = editAuthor = original.Author;
             }

             //InputModel => Model
             public csQuoteCUdto UpdateModel(csQuoteCUdto model)
             {
                 model.QuoteId = QuoteId;
                 model.Quote = Quote;
                 model.Author = Author;
                 return model;
             }
             #endregion
         }
         */
        #endregion
    }
}
