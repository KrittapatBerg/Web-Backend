using AppStudies.SeidoHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.DTO;
using Services;
using System.ComponentModel.DataAnnotations;

namespace AppGoodFriendsRazor.Pages.Edit
{
    public class EditFriendDetailModel : PageModel
    {
        IFriendsService service = null;
        ILogger<EditFriendDetailModel> logger = null;
        loginUserSessionDto usr = null;

        [BindProperty]
        public csViewFriendIM FriendInput { get; set; }

        [BindProperty]
        public string PageHeader { get; set; }

        //For Validation
        public reModelValidationResult ValidationResult { get; set; } = new reModelValidationResult(false, null, null);

        public List<SelectListItem> Kind { get; set; } = new List<SelectListItem>().PopulateSelectList<enAnimalKind>();
        public List<SelectListItem> Mood { get; set; } = new List<SelectListItem>().PopulateSelectList<enAnimalMood>();


        #region HTTP request
        public async Task<IActionResult> OnGetAsync()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid id))
            {
                //Read a friend
                var friend = await service.ReadFriendAsync(usr, id, false);

                FriendInput = new csViewFriendIM(friend);
                PageHeader = "Edit details of a friend";

            }
            else
            {
                //create an empty friend 
                FriendInput = new csViewFriendIM();
                FriendInput.StatusIM = enStatusIM.Inserted;
                PageHeader = "Create a new a friend";

            }
            return Page();
        }


        public async Task<IActionResult> OnPostSave()
        {
            string[] keys = { "FriendInput.FirstName",
                              "FriendInput.LastName",
                              "FriendInput.Email"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                ValidationResult = validationResult;
                return Page();
            }

            //First, are we creating a new Friend or editing another
            if (FriendInput.StatusIM == enStatusIM.Inserted)
            {
                var fDto = new csFriendCUdto();

                //create the Friend in the database
                fDto.FirstName = FriendInput.FirstName;
                fDto.FirstName = FriendInput.LastName;
                fDto.Email = FriendInput.Email;

                // use AddressId from FriendInput.Address
                fDto.AddressId = FriendInput.Address.AddressId;


                var newF = await service.CreateFriendAsync(usr, fDto);
                //get the newly created FriendId
                FriendInput.FriendId = newF.FriendId;

            }
            // använd AddressId från FriendInput.Address
            var addressDto = new csAddressCUdto
            {
                AddressId = FriendInput.Address.AddressId,
                StreetAddress = FriendInput.Address.StreetAddress,
                ZipCode = FriendInput.Address.ZipCode,
                City = FriendInput.Address.City,
                Country = FriendInput.Address.Country
            };


            //Do all updates for Pets
            IFriend f = await SavePets();

            // Do all updates for Quotes
            //f = await SaveQuotes();

            //Finally, update the friend itself
            f.FirstName = FriendInput.FirstName;
            f.LastName = FriendInput.LastName;
            f.Email = FriendInput.Email;


            // använd AddressId från FriendInput.Address
            var address = new csAddress
            {
                AddressId = FriendInput.Address.AddressId,
                StreetAddress = FriendInput.Address.StreetAddress,
                ZipCode = FriendInput.Address.ZipCode,
                City = FriendInput.Address.City,
                Country = FriendInput.Address.Country
            };

            // Uppdatera befintlig adress
            var updatedAddress = await service.UpdateAddressAsync(usr, addressDto);
            f.Address = updatedAddress;


            var csFriendInstance = await service.UpdateFriendAsync(usr, new csFriendCUdto(f));

            if (csFriendInstance == null)
            {
                // Handle the case where UpdateFriendAsync does not return a csFriend
                throw new InvalidOperationException("UpdateFriendAsync did not return a csFriend instance.");
            }
            f = csFriendInstance;

            if (FriendInput.StatusIM == enStatusIM.Inserted)
            {
                return Redirect($"~/Friend/ListOfFriend");
            }

            return Redirect($"~/Edit/EditFriendDetail?id={FriendInput.FriendId}");
        }


        public IActionResult OnPostEditPet(Guid petId)
        {
            int idx = FriendInput.Pets.FindIndex(a => a.PetId == petId);
            string[] keys = { $"FriendInput.Pets[{idx}].editName",
                              $"FriendInput.Pets[{idx}].editKind",
                              $"FriendInput.Pets[{idx}].editMood"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                ValidationResult = validationResult;
                return Page();
            }

            //Set the pets as Modified, it will later be updated in the database
            var p = FriendInput.Pets.First(a => a.PetId == petId);

            if (p == null)
            {
                return NotFound();
            }
            if (p.StatusIM != enStatusIM.Inserted)
            {
                p.StatusIM = enStatusIM.Modified;
            }

            //Implement the changes
            p.Name = p.editName;
            p.Kind = (enAnimalKind)p.editKind;
            p.Mood = (enAnimalMood)p.editMood;
            return Page();
        }

        public IActionResult OnPostAddPet()
        {
            string[] keys = { "FriendInput.NewPet.Name",
                               "FriendInput.NewPet.Kind",
                               "FriendInput.NewPet.Mood"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                ValidationResult = validationResult;
                return Page();
            }

            //Set the pet as Inserted, it will later be inserted in the database
            FriendInput.NewPet.StatusIM = enStatusIM.Inserted;

            //Need to add a temp Guid so it can be deleted and editited in the form
            //A correct Guid will be created by the DTO when Inserted into the database
            FriendInput.NewPet.PetId = Guid.NewGuid();

            //Add it to the Input Models pets
            FriendInput.Pets.Add(new csPetIM(FriendInput.NewPet));

            //Clear the NewPet so another pet can be added
            FriendInput.NewPet = new csPetIM();

            return Page();
        }

        #endregion
        #region
        private async Task<IFriend> SavePets()
        {
            //Check if there are deleted Pets, if so simply remove them
            var deletedPets = FriendInput.Pets.FindAll(p => (p.StatusIM == enStatusIM.Deleted));

            //Check if there are any new pets added, if so create them in the database
            var newPets = FriendInput.Pets.FindAll(p => (p.StatusIM == enStatusIM.Inserted));
            var pf = await service.ReadFriendAsync(usr, FriendInput.FriendId, false);
            var dtopF = new csFriendCUdto(pf);
            foreach (var item in newPets)
            {
                //Create the corresposning model and CUdto objects
                var model = item.UpdateModel(new csPet());
                var cuDto = new csPetCUdto(model) { FriendId = FriendInput.FriendId };

                //Set the relationships of a newly created item and write to database
                cuDto.FriendId = FriendInput.FriendId;
                model = await service.CreatePetAsync(usr, cuDto);

                dtopF.PetsId.Add(model.PetId);

            }

            //To update modified and deleted pets, lets first read the original
            //Note that now the deleted pets will be removed and created pets will be nicely included
            var f = await service.UpdateFriendAsync(usr, dtopF);


            //Check if there are any modified pets , if so update them in the database
            var modifiedPets = FriendInput.Pets.FindAll(p => (p.StatusIM == enStatusIM.Modified));
            foreach (var item in modifiedPets)
            {
                var model = f.Pets.First(a => a.PetId == item.PetId);

                //Update the model from the InputModel
                model = item.UpdateModel(model);

                //Updatet the model in the database
                model = await service.UpdatePetAsync(usr, new csPetCUdto(model) { FriendId = FriendInput.FriendId });
            }

            f = await service.ReadFriendAsync(usr, FriendInput.FriendId, false);

            return f;
        }
        #endregion

        #region Constructor
        public EditFriendDetailModel(IFriendsService service, ILogger<EditFriendDetailModel> logger)
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

            //This is because I want to confirm modifications in PostEditAlbum
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
