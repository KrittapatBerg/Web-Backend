using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;
using System.ComponentModel.DataAnnotations;

namespace AppGoodFriendsRazor.Pages.Edit
{
    public class ViewFriendPetQuoteModel : PageModel
    {
        IFriendsService service = null;
        ILogger<ViewFriendPetQuoteModel> logger = null;
        loginUserSessionDto usr = null;

        /* 
         * maybe it doesn't have to be a list
        [BindProperty]
        public List<csViewAndEditIM> ViewAndEditIMs { get; set; }    
        */

        [BindProperty]
        public csViewFriendIM ViewFriendIM { get; set; }

        [BindProperty]
        public string PageHeader { get; set; }

        #region HTTP request
        public async Task<IActionResult> OnGetAsync()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid id))
            {
                //Read a friend 
                var friend = await service.ReadFriendAsync(usr, id, false);

                ViewFriendIM = new csViewFriendIM(friend);
                PageHeader = "Edit detail of a friend";
            }
            else
            {
                //create an empty friend 
                ViewFriendIM = new csViewFriendIM();
                ViewFriendIM.StatusIM = enStatusIM.Inserted;
                PageHeader = "Create a new friend";

            }
            return Page();
            // ViewAndEditEverythingIMs = await service.ReadFriendAsync(usr, id, false).Select(e => new csViewAndEditEverythingIM(e)).ToList();
        }

        public IActionResult OnPostEditPet(Guid petId)
        {
            int petIdx = ViewFriendIM.PetIM.FindIndex(a => a.PetId == petId);
            string[] keys = { $"ViewFriendIM.PetIM[{petIdx}].editName",
                            $"ViewFriendIM.PetIM[{petIdx}].editKind",
                            $"ViewFriendIM.PetIM[{petIdx}].editMood"};

            //Set the Album as Modified, it will later be updated in the database
            var a = ViewFriendIM.PetIM.First(a => a.PetId == petId);
            if (a.StatusIM != enStatusIM.Inserted)
            {
                a.StatusIM = enStatusIM.Modified;
            }

            //Implement the changes
            a.Name = a.editName;
            a.Kind = a.editKind;
            a.Mood = a.editMood;

            return Page();
        }

        #endregion

        #region Constructor
        public ViewFriendPetQuoteModel(IFriendsService service, ILogger<ViewFriendPetQuoteModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion

        #region Input Model 
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

        public class csViewFriendIM
        {
            //Status of InputModel
            public enStatusIM StatusIM { get; set; }

            //properties from Model which is to be edited in the <form>
            public Guid FriendId { get; init; } = Guid.NewGuid();

            [Required(ErrorMessage = "You must provide firstname")]
            public string Firstname { get; set; }

            [Required(ErrorMessage = "You must provide lastname")]
            public string Lastname { get; set; }
            public string Email { get; set; }
            public DateTime? Birthday { get; set; }
            public csAddressIM AddressIM { get; set; }

            //Added properties to edit in the list
            [Required(ErrorMessage = "You must provide firstname")]
            public string editFirstname { get; set; }

            [Required(ErrorMessage = "You must provide lastname")]
            public string editLastname { get; set; }
            public string editEmail { get; set; }
            public DateTime? editBirthday { get; set; }

            public List<csPetIM> PetIM { get; set; } = new List<csPetIM>();
            public List<csQuoteIM> QuoteIM { get; set; } = new List<csQuoteIM>();

            #region constructors and model update 
            //public csViewFriendIM() { }
            public csViewFriendIM()
            {
                StatusIM = enStatusIM.Unchanged;
                AddressIM = new csAddressIM();
                PetIM = new List<csPetIM>();
                QuoteIM = new List<csQuoteIM>();
            }

            /*
            public csViewFriendIM(IFriend model)
            {
                StatusIM = enStatusIM.Unchanged;
                FriendId = model.FriendId;
                Firstname = model.FirstName;
                Lastname = model.LastName;
                Email = model.Email;
                Birthday = model.Birthday;

                PetIM = model.Pets?.Select(p => new csPetIM(p)).ToList();
                QuoteIM = model.Quotes?.Select(q => new csQuoteIM(q)).ToList();
            }
            */

            public csPetIM NewPet { get; set; } = new csPetIM();
            public csQuoteIM NewQuote { get; set; } = new csQuoteIM();

            //copy constructor
            public csViewFriendIM(csViewFriendIM original)
            {
                StatusIM = original.StatusIM;
                FriendId = original.FriendId;
                Firstname = original.Firstname;
                Lastname = original.Lastname;
                Email = original.Email;
                Birthday = original.Birthday;
                AddressIM = original.AddressIM;

                editFirstname = original.editFirstname;
                editLastname = original.editLastname;
                editEmail = original.editEmail;
                editBirthday = original.editBirthday;

            }

            //Model => InputModel constructor
            public csViewFriendIM(IFriend original)
            {
                StatusIM = enStatusIM.Unchanged;
                FriendId = original.FriendId;
                Firstname = editFirstname = original.FirstName;
                Lastname = editLastname = original.LastName;
                Email = editEmail = original.Email;
                Birthday = editBirthday = original.Birthday;
                AddressIM = new csAddressIM(original.Address);

            }

            //InputModel => Model 
            public IFriend UpdateFriendModel(IFriend model)
            {
                model.FriendId = FriendId;
                model.FirstName = Firstname;
                model.LastName = Lastname;
                model.Email = Email;
                model.Birthday = Birthday;
                model.Address = AddressIM.UpdateModel(model.Address);

                model.Pets = PetIM.Select(p => p.UpdatePetModel(new csPet())).ToList();
                model.Quotes = QuoteIM.Select(q => q.UpdateQuoteModel(new csQuote())).ToList();
                return model;
            }
            #endregion
        }
        #endregion

        #region csAddressIM
        public class csAddressIM
        {
            public enStatusIM StatusIM { get; set; }

            //properties from Model which is to be edited in the <form>
            public Guid AddressId { get; set; } = Guid.NewGuid();

            [Required(ErrorMessage = "You must provide streetaddress")]
            public string StreetAddress { get; set; }

            [Required(ErrorMessage = "You must provide zipcode")]
            public int ZipCode { get; set; }

            [Required(ErrorMessage = "You type provide city")]
            public string City { get; set; }

            [Required(ErrorMessage = "You type provide country")]
            public string Country { get; set; }

            public override string ToString() => $"{StreetAddress}, {ZipCode} {City}, {Country}";

            //Added properties to edit in the list
            [Required(ErrorMessage = "You must provide streetaddress")]
            public string editStreetAddress { get; set; }

            [Required(ErrorMessage = "You must provide zipcode")]
            public int editZipcode { get; set; }

            [Required(ErrorMessage = "You type provide city")]
            public string editCity { get; set; }

            [Required(ErrorMessage = "You type provide country")]
            public string editCountry { get; set; }

            //Model => InputModel constructor
            public csAddressIM()
            {
                StatusIM = new enStatusIM();
            }

            //copy constructor
            public csAddressIM(csAddressIM original)
            {
                StatusIM = original.StatusIM;
                AddressId = original.AddressId;
                StreetAddress = original.StreetAddress;
                ZipCode = original.ZipCode;
                City = original.City;
                Country = original.Country;

                editStreetAddress = original.editStreetAddress;
                editZipcode = original.editZipcode;
                editCity = original.editCity;
                editCountry = original.editCountry;

            }

            //Model => InputModel constructor
            public csAddressIM(IAddress original)
            {
                StatusIM = enStatusIM.Unchanged;
                AddressId = original.AddressId;
                StreetAddress = editStreetAddress = original.StreetAddress;
                ZipCode = editZipcode = original.ZipCode;
                City = editCity = original.City;
                Country = editCountry = original.Country;
            }

            //InputModel => Model 
            public IAddress UpdateModel(IAddress model)
            {
                model.AddressId = AddressId;
                model.StreetAddress = StreetAddress;
                model.ZipCode = ZipCode;
                model.City = City;
                model.Country = Country;

                return model;
            }
        }
        #endregion

        #region csPetIM
        public class csPetIM
        {
            public enStatusIM StatusIM { get; set; }
            public Guid PetId { get; set; }

            [Required(ErrorMessage = "The pet must have a name")]
            public string Name { get; set; }

            [Required(ErrorMessage = "You must choose a pet's kind")]
            public enAnimalKind Kind { get; set; }
            public enAnimalMood Mood { get; set; }  //optional

            //Added properties to edit in the list
            [Required(ErrorMessage = "The pet must have a name")]
            public string editName { get; set; }

            [Required(ErrorMessage = "You must choose a pet's kind")]
            public enAnimalKind editKind { get; set; }

            public enAnimalMood editMood { get; set; }


            //Model => InputModel constructor
            public csPetIM()
            {
                StatusIM = new enStatusIM();
            }

            //copy constructor
            public csPetIM(csPetIM original)
            {
                StatusIM = original.StatusIM;
                PetId = original.PetId;
                Name = original.Name;
                Kind = original.Kind;
                Mood = original.Mood;

                editName = original.editName;
                editKind = original.editKind;
                editMood = original.editMood;
            }

            //Model => InputModel constructor
            public csPetIM(IPet original)
            {
                StatusIM = enStatusIM.Unchanged;
                PetId = original.PetId;
                Name = editName = original.Name;
                Kind = editKind = original.Kind;
                Mood = editMood = original.Mood;
            }

            //InputModel => Model 
            public IPet UpdatePetModel(IPet model)
            {
                model.PetId = PetId;
                model.Name = Name;
                model.Kind = Kind;
                model.Mood = Mood;

                return model;
            }
        }
        #endregion

        #region csQuoteIM
        public class csQuoteIM
        {
            public enStatusIM StatusIM { get; set; }
            public Guid QuoteId { get; set; }

            [Required(ErrorMessage = "You must provide a quote")]
            public string Quote { get; set; }

            [Required(ErrorMessage = "You must provide a name for the author, otherwise write Unknown")]
            public string Author { get; set; }

            //Added properties to edit in the list
            [Required(ErrorMessage = "You must provide a quote")]
            public string editQuote { get; set; }

            [Required(ErrorMessage = "You must provide a name for the author, otherwise write Unknown")]
            public string editAuthor { get; set; }


            //Model => InputModel constructor
            public csQuoteIM()
            {
                StatusIM = new enStatusIM();
            }


            //copy constructor
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
            public csQuoteIM(IQuote original)
            {
                StatusIM = enStatusIM.Unchanged;
                QuoteId = original.QuoteId;
                Quote = editQuote = original.Quote;
                Author = editAuthor = original.Author;
            }


            //InputModel => Model 
            public IQuote UpdateQuoteModel(IQuote model)
            {
                model.QuoteId = QuoteId;
                model.Quote = Quote;
                model.Author = Author;

                return model;
            }

        }
        #endregion
    }
}
