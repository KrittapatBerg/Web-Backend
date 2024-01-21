using AppStudies.SeidoHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.DTO;
using Services;
using System.ComponentModel.DataAnnotations;
using static AppGoodFriendsRazor.Pages.Edit.EditFriendDetailModel;

namespace AppGoodFriendsRazor.Pages.Edit
{
    public class EditPetModel : PageModel
    {
        IFriendsService service;
        ILogger<EditPetModel> logger;
        loginUserSessionDto usr;

        [BindProperty]
        public csViewFriendIM FriendInput { get; set; }

        [BindProperty]
        public string PageHeader { get; set; }

        //For Validation
        public reModelValidationResult ValidationResult { get; set; } = new reModelValidationResult(false, null, null);

        public List<SelectListItem> KindList { get; set; } = new List<SelectListItem>().PopulateSelectList<enAnimalKind>();
        public List<SelectListItem> MoodList { get; set; } = new List<SelectListItem>().PopulateSelectList<enAnimalMood>();


        #region HTTP request
        public async Task<IActionResult> OnGetAsync()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid id))
            {
                //Read a friend
                var friend = await service.ReadFriendAsync(usr, id, false);

                FriendInput = new csViewFriendIM(friend);

                PageHeader = "Add or Edit Pets of a Friend";

            }
            else
            {
                //create an empty pet 
                FriendInput = new csViewFriendIM();
                FriendInput.StatusIM = enStatusIM.Inserted;
                FriendInput.Pets = new List<csPetIM> { new csPetIM() };

                PageHeader = "Create a new pet";

            }
            return Page();
        }


        #region Add pet
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

            //return Page();
            return Redirect($"~/Friend/FriendDetail?id={FriendInput.FriendId}");

        }
        #endregion

        #region Edit pet 
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
            var p = FriendInput.Pets.FirstOrDefault(a => a.PetId == petId);

            if (p == null)
            {
                return NotFound();
            }
            if (p.StatusIM != enStatusIM.Inserted)
            {
                p.StatusIM = enStatusIM.Modified;
            }

            // Check for null before accessing properties
            //Implement the changes
            if (p.editName != null)
            {
                p.Name = p.editName;
            }
            if (p.editKind != null)
            {
                p.Kind = (enAnimalKind)p.editKind;
            }
            if (p.editMood != null)
            {
                p.Mood = (enAnimalMood)p.editMood;
            }

            //return Page();
            return Redirect($"~/Friend/FriendDetail?id={FriendInput.FriendId}");

        }
        #endregion

        #region Save pet
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

        #endregion

        #region Constructor
        public EditPetModel(IFriendsService service, ILogger<EditPetModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion


        #region Input Model
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
            //public List<csQuoteIM> Quotes { get; set; } = new List<csQuoteIM>();
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
                //Quotes = model.Quotes?.Select(m => new csQuoteIM(m)).ToList() ?? new List<csQuoteIM>();
            }

            public csPetIM NewPet { get; set; } = new csPetIM();

            //public csQuoteIM NewQuote { get; set; } = new csQuoteIM();
        }

        #region Pet
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
                model.PetId = PetId;
                model.Kind = Kind;
                model.Mood = Mood;
                model.Name = Name;

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
                Kind = model.Kind;
                Mood = model.Mood;
                Name = editName = model.Name;

            }
        }
        #endregion

        #endregion

    }
}
