using AppStudies.SeidoHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;
using System.ComponentModel.DataAnnotations;

namespace AppGoodFriendsRazor.Pages
{
    public class AIQuoteListModel : PageModel
    {
        IFriendsService service;
        ILogger<AIQuoteListModel> logger;
        loginUserSessionDto usr;

        public csViewFriendIM QuoteInput { get; set; }
        public List<IFriend> QuotesList { get; set; } = new List<IFriend>();

        //For Validation
        public reModelValidationResult ValidationResult { get; set; } = new reModelValidationResult(false, null, null);

        #region HTTP Requests 
        //public async Task OnGetAsync()
        //{
        //    var quote = await service.ReadFriendsAsync(usr, true, false, "", 0, 100);
        //    QuotesList = quote.ToList();
        //}

        public async Task<IActionResult> OnGetAsync()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid id))
            {
                //Read a friend
                var friend = await service.ReadFriendAsync(usr, id, false);

                QuoteInput = new csViewFriendIM(friend);

            }
            else
            {
                //create an empty friend 
                QuoteInput = new csViewFriendIM();
                QuoteInput.StatusIM = enStatusIM.Inserted;
                QuoteInput.Pets = new List<csPetIM> { new csPetIM() };
                QuoteInput.Quotes = new List<csQuoteIM> { new csQuoteIM() };

            }
            return Page();
        }

        public IActionResult OnPostAddQuote()
        {
            string[] keys = { "QuoteInput.NewQuote.Quote",
                               "QuoteInput.NewQuote.Author"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                ValidationResult = validationResult;
                return Page();
            }

            //Set the pet as Inserted, it will later be inserted in the database
            QuoteInput.NewQuote.StatusIM = enStatusIM.Inserted;

            //Need to add a temp Guid so it can be deleted and editited in the form
            //A correct Guid will be created by the DTO when Inserted into the database
            QuoteInput.NewQuote.QuoteId = Guid.NewGuid();

            //Add it to the Input Models pets
            QuoteInput.Quotes.Add(new csQuoteIM(QuoteInput.NewQuote));

            //Clear the NewPet so another pet can be added
            QuoteInput.NewQuote = new csQuoteIM();

            return Redirect($"~/Friend/FriendDetail?id={QuoteInput.FriendId}");
        }

        #endregion


        #region Constuctor
        public AIQuoteListModel(IFriendsService service, ILogger<AIQuoteListModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion

        #region Input Model
        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        //These classes are in center of ModelBinding and Validation
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }
        public class csViewFriendIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid FriendId { get; set; }

            [Required(ErrorMessage = "You must provide a first name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "You must provide a last name")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "You must provide a Email")]
            public string Email { get; set; }

            //This is because I want to confirm modifications in PostEditFriend
            [Required(ErrorMessage = "You must provide a first name")]
            public string editFirstName { get; set; }

            [Required(ErrorMessage = "You must provide a last name")]
            public string editLastName { get; set; }

            [Required(ErrorMessage = "You must provide a Email")]
            public string editEmail { get; set; }

            public csAddressIM Address { get; set; }

            public List<csPetIM> Pets { get; set; } = new List<csPetIM>();
            public List<csQuoteIM> Quotes { get; set; } = new List<csQuoteIM>();
            public IFriend UpdateModel(IFriend model)
            {
                model.FriendId = this.FriendId;
                model.FirstName = this.FirstName;
                model.LastName = this.LastName;
                model.Email = this.Email;
                model.Address = Address.UpdateModel(model.Address);
                return model;
            }

            public csViewFriendIM()
            {
                StatusIM = enStatusIM.Unchanged;
                Address = new csAddressIM();
            }
            public csViewFriendIM(csViewFriendIM original)
            {
                StatusIM = original.StatusIM;
                FriendId = original.FriendId;
                FirstName = original.FirstName;
                LastName = original.LastName;
                Email = original.Email;

                editFirstName = original.editFirstName;
                editLastName = original.editLastName;
                editEmail = original.editEmail;


                Address = original.Address;
            }
            public csViewFriendIM(IFriend model)
            {
                StatusIM = enStatusIM.Unchanged;
                FriendId = model.FriendId;
                FirstName = editFirstName = model.FirstName;
                LastName = editLastName = model.LastName;
                Email = editEmail = model.Email;

                Address = new csAddressIM(model.Address);

                Pets = model.Pets?.Select(m => new csPetIM(m)).ToList() ?? new List<csPetIM>();
                Quotes = model.Quotes?.Select(m => new csQuoteIM(m)).ToList() ?? new List<csQuoteIM>();
            }

            public csPetIM NewPet { get; set; } = new csPetIM();

            public csQuoteIM NewQuote { get; set; } = new csQuoteIM();
        }

        public class csAddressIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid AddressId { get; set; }

            [Required(ErrorMessage = "You must enter an StreetAddress")]
            public string StreetAddress { get; set; }

            [Required(ErrorMessage = "You must enter an ZipCode")]
            public int ZipCode { get; set; }


            [Required(ErrorMessage = "You must enter an City")]
            public string City { get; set; }


            [Required(ErrorMessage = "You must enter an Country")]
            public string Country { get; set; }


            [Required(ErrorMessage = "You must enter an StreetAddress")]
            public string editStreetAddress { get; set; }


            [Required(ErrorMessage = "You must enter an ZipCode")]
            public int editZipCode { get; set; }

            [Required(ErrorMessage = "You must enter an City")]
            public string editCity { get; set; }

            [Required(ErrorMessage = "You must enter an Country")]
            public string editCountry { get; set; }


            public IAddress UpdateModel(IAddress model)
            {
                model.AddressId = this.AddressId;
                model.StreetAddress = this.StreetAddress;
                model.ZipCode = this.ZipCode;
                model.City = this.City;
                model.Country = this.Country;

                return model;
            }

            public csAddressIM() { }
            public csAddressIM(csAddressIM original)
            {
                StatusIM = original.StatusIM;
                AddressId = original.AddressId;
                StreetAddress = original.StreetAddress;
                ZipCode = original.ZipCode;
                City = original.City;
                Country = original.Country;

                editStreetAddress = original.editStreetAddress;
                editZipCode = original.editZipCode;
                editCity = original.editCity;
                editCountry = original.editCountry;

            }
            public csAddressIM(IAddress model)
            {
                StatusIM = enStatusIM.Unchanged;
                AddressId = model.AddressId;
                StreetAddress = editStreetAddress = model.StreetAddress;
                ZipCode = editZipCode = model.ZipCode;
                City = editCity = model.City;
                StreetAddress = editStreetAddress = model.StreetAddress;
                Country = editStreetAddress = model.Country;

            }
        }

        public class csPetIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid PetId { get; set; }

            [Required(ErrorMessage = "You must enter an Animal Kind")]
            public enAnimalKind Kind { get; set; }

            [Required(ErrorMessage = "You must enter an Animal Mood")]
            public enAnimalMood Mood { get; set; }


            [Required(ErrorMessage = "You must enter an Name")]
            public string Name { get; set; }

            [Required(ErrorMessage = "You must select a pet Kind")]
            public enAnimalKind? editKind { get; set; }

            [Required(ErrorMessage = "You must enter a pet Mood")]
            public enAnimalMood? editMood { get; set; }

            [Required(ErrorMessage = "You must enter an Name")]
            public string editName { get; set; }


            public IPet UpdateModel(IPet model)
            {
                model.PetId = this.PetId;
                model.Kind = (Models.enAnimalKind)this.Kind;
                model.Mood = (Models.enAnimalMood)this.Mood;
                model.Name = this.Name;

                return model;
            }

            public csPetIM() { }
            public csPetIM(csPetIM original)
            {
                StatusIM = original.StatusIM;
                PetId = original.PetId;
                Name = original.Name;

                editName = original.editName;

            }
            public csPetIM(IPet model)
            {
                StatusIM = enStatusIM.Unchanged;
                PetId = model.PetId;
                Kind = (enAnimalKind)model.Kind;
                Mood = (enAnimalMood)model.Mood;
                Name = editName = model.Name;

            }
        }

        public class csQuoteIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid QuoteId { get; set; }

            [Required(ErrorMessage = "You must enter an Quote")]
            public string Quote { get; set; }

            [Required(ErrorMessage = "You must enter an Author")]
            public string Author { get; set; }



            [Required(ErrorMessage = "You must enter an Quote")]
            public string editQuote { get; set; }

            [Required(ErrorMessage = "You must enter an Author")]
            public string editAuthor { get; set; }


            public IQuote UpdateModel(IQuote model)
            {
                model.QuoteId = this.QuoteId;
                model.Quote = this.Quote;
                model.Author = this.Author;

                return model;
            }

            public csQuoteIM() { }
            public csQuoteIM(csQuoteIM original)
            {
                StatusIM = original.StatusIM;
                QuoteId = original.QuoteId;
                Quote = original.Quote;
                Author = original.Author;

                editQuote = original.editQuote;
                editAuthor = original.editAuthor;

            }
            public csQuoteIM(IQuote model)
            {
                StatusIM = enStatusIM.Unchanged;
                QuoteId = model.QuoteId;
                Quote = editQuote = model.Quote;
                Author = editAuthor = model.Author;
            }
        }

        #endregion
    }
}
