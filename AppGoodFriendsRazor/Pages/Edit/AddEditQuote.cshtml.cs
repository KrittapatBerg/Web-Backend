using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;
using System.ComponentModel.DataAnnotations;

namespace AppGoodFriendsRazor.Pages.Edit
{
    public class EditQuoteModel : PageModel
    {
        private readonly IFriendsService _service;
        private readonly ILogger<EditQuoteModel> _logger;
        private readonly loginUserSessionDto _usr;

        [BindProperty]
        public Guid FriendId { get; set; }

        [BindProperty]
        public csQuoteIM QuoteIM { get; set; }

        public string PageHeader { get; set; }

        //public member becomes part of the Model in the Razor page
        public string ErrorMessage { get; set; } = null;

        public List<IQuote> Quotes { get; set; } = new List<IQuote>();


        // Constructor with dependency injection
        public EditQuoteModel(IFriendsService service, ILogger<EditQuoteModel> logger)
        {
            _service = service;
            _logger = logger;
        }

        //For Server Side Validation set by IsValid()
        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }

        public async Task<IActionResult> OnGet(Guid quoteId, Guid friendId)
        {
            FriendId = friendId;

            if (quoteId == Guid.Empty)
                return Page();

            QuoteIM = new csQuoteIM(await _service.ReadQuoteAsync(null, quoteId, false));
            PageHeader = "Edit details of a Friends";

            return Page();

        }
        public async Task<IActionResult> OnPostEdit()
        {
            // Load the existing quote
            var Quote = await _service.ReadQuoteAsync(null, QuoteIM.QuoteId, false);

            if (Quote == null)
            {
                // Handle the case where the quote with the given quoteId is not found
                return NotFound();
            }

            // Update the existing quote with the new values
            Quote.Quote = QuoteIM.Quote;
            Quote.Author = QuoteIM.Author;

            // Save the changes to the database
            var updatedQuote = new csQuoteCUdto(Quote);

            Quote = await _service.UpdateQuoteAsync(null, updatedQuote);

            QuoteIM = new csQuoteIM(Quote);

            if (FriendId == Guid.Empty)
                return RedirectToPage("FriendsInCountrys");

            // Redirect to the "ViewFriendDetails" page
            return Redirect($"~/Friend/FriendDetail?id={FriendId}");
            //return RedirectToPage("FriendView", new { id = FriendId });
        }

        public async Task<IActionResult> OnPostSave()
        {
            //PageHeader is stored in TempData which has to be set after a Post
            PageHeader = (QuoteIM.StatusIM == enStatusIM.Inserted) ?
                "Create a new quote" : "Edit details of a quote";

            IQuote createdQuote = await _service.CreateQuoteAsync(null, new csQuoteCUdto()
            {
                Quote = QuoteIM.Quote,
                Author = QuoteIM.Author
            }) ?? throw new Exception("Failed to create new quote.");


            if (createdQuote is null)
                throw new Exception("Failed to create new Quote.");

            if (FriendId == Guid.Empty)
                return RedirectToPage("FriendsInCountrys");


            csFriendCUdto friend = new(await _service.ReadFriendAsync(null, FriendId, false)
                ?? throw new Exception("Incorrect friendId"));

            friend.QuotesId.Add(createdQuote.QuoteId);

            friend = new(await _service.UpdateFriendAsync(null, friend)
                ?? throw new Exception("Failed to add new quote to friend."));

            // Redirect to the same page to refresh the list
            //return RedirectToPage("FriendView", new { id = friend.FriendId });
            return Redirect($"~/Friend/FriendDetail?id={friend.FriendId}");

        }


        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

        public class csQuoteIM
        {


            // Properties for Quote
            public enStatusIM StatusIM { get; set; } = enStatusIM.Unchanged;
            public Guid QuoteId { get; set; } = Guid.NewGuid();


            [Required(ErrorMessage = "You must enter a quote")]
            public string Quote { get; set; }

            [Required(ErrorMessage = "You must enter the author's name")]
            public string Author { get; set; }


            public Guid FriendId { get; set; }

            // Constructors
            public csQuoteIM() { }

            public csQuoteIM(csQuoteIM original)
            {
                StatusIM = original.StatusIM;

                QuoteId = original.QuoteId;
                Quote = original.Quote;
                Author = original.Author;

                FriendId = original.FriendId;
            }

            // Model => InputModel constructor
            public csQuoteIM(IQuote original)
            {
                StatusIM = enStatusIM.Unchanged;
                QuoteId = original.QuoteId;
                Quote = original.Quote;
                Author = original.Author;
                FriendId = original.Friends?.First().FriendId ?? Guid.Empty;
            }

            // InputModel => Model
            public IQuote UpdateModel(IQuote model)
            {
                model.QuoteId = QuoteId;
                model.Quote = Quote;
                model.Author = Author;
                return model;
            }


        }

    }
}

