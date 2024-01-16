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

        [BindProperty]
        public csViewFriendIM ViewFriendIM { get; set; }

        public csFriend ViewFriend { get; set; }

        #region HTTP request
        public async Task<IActionResult> OnGetAsync()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid id))
            {
                //Read a friend
                var friend = await service.ReadFriendAsync(usr, id, false);

                ViewFriend = new csFriend()
                {

                    FriendId = id,
                    FirstName = friend.FirstName,
                    LastName = friend.LastName,
                    Email = friend.Email,
                    Birthday = friend.Birthday,
                    Address = friend.Address,
                    Pets = friend.Pets,
                    Quotes = friend.Quotes
                };

                ViewFriendIM = new csViewFriendIM(friend);

            }
            else
            {
                //create an empty friend 
                ViewFriendIM = new csViewFriendIM();
                ViewFriendIM.StatusIM = enStatusIM.Inserted;
            }
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
        //YUP

        public class csViewFriendIM
        {
            //Status of InputModel
            public enStatusIM StatusIM { get; set; }

            //properties from Model which is to be edited in the <form>
            public Guid FriendId { get; set; } //init; } = Guid.NewGuid();

            [Required(ErrorMessage = "You must provide firstname")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "You must provide lastname")]
            public string LastName { get; set; }
            public string Email { get; set; }
            public DateTime? Birthday { get; set; }
            public csAddressIM AddressIM { get; set; }

            //Added properties to edit in the list
            [Required(ErrorMessage = "You must provide firstname")]
            public string editFirstName { get; set; }

            [Required(ErrorMessage = "You must provide lastname")]
            public string editLastname { get; set; }
            public string editEmail { get; set; }
            public DateTime? editBirthday { get; set; }

            public List<csPetIM> PetIM { get; set; } = new List<csPetIM>();
            public List<csQuoteIM> QuoteIM { get; set; } = new List<csQuoteIM>();

            //YUP 

            #region constructors and model update 
            //public csViewFriendIM() { }
            public csViewFriendIM()
            {
                StatusIM = enStatusIM.Unchanged;
                AddressIM = new csAddressIM();
                //Pets = new List<csPetIM>();
                //QuoteIM = new List<csQuoteIM>();
            }
            //YUP

            //public csPetIM NewPet { get; set; } = new csPetIM();
            //public csQuoteIM NewQuote { get; set; } = new csQuoteIM();

            //copy constructor
            public csViewFriendIM(csViewFriendIM original)
            {
                StatusIM = original.StatusIM;
                FriendId = original.FriendId;
                FirstName = original.FirstName;
                LastName = original.LastName;
                Email = original.Email;
                Birthday = original.Birthday;
                AddressIM = original.AddressIM;

                editFirstName = original.editFirstName;
                editLastname = original.editLastname;
                editEmail = original.editEmail;
                editBirthday = original.editBirthday;

            }  //YUP

            ////Model => InputModel constructor
            //public csViewFriendIM(IFriend original)
            //{
            //    StatusIM = enStatusIM.Unchanged;
            //    FriendId = original.FriendId;
            //    FirstName = editFirstName = original.FirstName;
            //    Lastname = editLastname = original.LastName;
            //    Email = editEmail = original.Email;
            //    Birthday = editBirthday = original.Birthday;
            //    AddressIM = new csAddressIM(original.Address);

            //}

            //InputModel => Model 
            public IFriend UpdateModel(IFriend model)
            {
                model.FriendId = FriendId;
                model.FirstName = FirstName;
                model.LastName = LastName;
                model.Email = Email;
                model.Birthday = Birthday;
                model.Address = AddressIM.UpdateAddressModel(model.Address);

                //model.Pets = Pets.Select(p => p.UpdatePetModel(new csPet())).ToList();
                //model.Quotes = QuoteIM.Select(q => q.UpdateQuoteModel(new csQuote())).ToList();
                return model;
            } //YUP
            #endregion

            public csViewFriendIM(IFriend model)
            {
                StatusIM = enStatusIM.Unchanged;
                FriendId = model.FriendId;
                FirstName = editFirstName = model.FirstName;
                LastName = editLastname = model.LastName;
                Email = editEmail = model.Email;
                Birthday = editBirthday = model.Birthday;

                AddressIM = new csAddressIM(model.Address);

                PetIM = model.Pets?.Select(p => new csPetIM(p)).ToList() ?? new List<csPetIM>();
                QuoteIM = model.Quotes?.Select(o => new csQuoteIM(o)).ToList() ?? new List<csQuoteIM>();
            }

            public csPetIM NewPetIM { get; set; } = new csPetIM();
            public csQuoteIM NewQuoteIM { get; set; } = new csQuoteIM();
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
            public csAddressIM(IAddress model)
            {
                StatusIM = enStatusIM.Unchanged;
                AddressId = model.AddressId;
                StreetAddress = editStreetAddress = model.StreetAddress;
                ZipCode = editZipcode = model.ZipCode;
                City = editCity = model.City;
                Country = editCountry = model.Country;
            }

            public csAddressIM() { }

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

            //InputModel => Model 
            public IAddress UpdateAddressModel(IAddress model)
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

            [Required(ErrorMessage = "You must choose a pet's mood")]
            public enAnimalMood Mood { get; set; }  //optional

            //Added properties to edit in the list
            [Required(ErrorMessage = "The pet must have a name")]
            public string editName { get; set; }

            [Required(ErrorMessage = "You must choose a pet's kind")]
            public enAnimalKind editKind { get; set; }

            [Required(ErrorMessage = "You must choose a pet's mood")]
            public enAnimalMood editMood { get; set; }


            //Model => InputModel constructor
            public csPetIM() { }

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

            //Model => InputModel constructor..... do I need this block ? 
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
            public csQuoteIM(IQuote model)
            {
                StatusIM = enStatusIM.Unchanged;
                QuoteId = model.QuoteId;
                Quote = editQuote = model.Quote;
                Author = editAuthor = model.Author;
            }


            //InputModel => Model 
            public IQuote UpdateModel(IQuote model)
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
