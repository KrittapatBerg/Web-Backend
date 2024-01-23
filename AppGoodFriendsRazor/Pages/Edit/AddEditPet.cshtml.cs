using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;
using System.ComponentModel.DataAnnotations;
using static AppGoodFriendsRazor.Pages.Edit.EditFriendDetailModel;

namespace AppGoodFriendsRazor.Pages.Edit
{
    public class AddEditPetModel : PageModel
    {
        IFriendsService service;
        ILogger<AddEditPetModel> logger;
        loginUserSessionDto usr;

        [BindProperty]
        public csViewFriendIM FriendInput { get; set; }

        [BindProperty]
        public csPetIM PetIM { get; set; }

        public Guid FriendId { get; set; }

        public List<csPetIM> PetIMs { get; set; } = new List<csPetIM>();

        public List<csPetCUdto> Pets { get; set; }

        public string ErrorMessage { get; set; } = null;

        [BindProperty]
        public string PageHeader { get; set; }

        //For Server Side Validation set by IsValid()
        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }


        #region HTTP request
        public async Task<IActionResult> OnGet(Guid Id, Guid friendId)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    if (friendId == Guid.Empty)
                        throw new ArgumentNullException(nameof(friendId));

                    //Create an empty InputModel
                    PetIM = new csPetIM();
                    PetIM.StatusIM = enStatusIM.Inserted;
                    PetIM.FriendId = friendId;

                    PageHeader = "Create a new pet";

                    return Page();
                }

                PetIM = new csPetIM(await service.ReadPetAsync(usr, Id, false));
                PageHeader = "Edit details of a pet";
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            return Page();
        }

        #region Save pet
        public async Task<IActionResult> OnPostSave(Guid petId)
        {

            string[] validationKeys = new string[]
            {
               "PetIM.PetId", "PetIM.Kind", "PetIM.Mood", "PetIM.Name"
            };

            if (!IsValid(validationKeys))
            {
                return Page();
            }

            var p = await service.ReadPetAsync(usr, petId, false);
            p = PetIM.UpdateModel(p);
            var dto = new csPetCUdto(p);  //update

            p = await service.UpdatePetAsync(usr, dto);


            PageHeader = "Pet has been saved";
            return Redirect($"~/Friend/FriendDetail?id={p.Friend.FriendId}");
        }
        #endregion

        #region Add pet
        public async Task<IActionResult> OnPostAddPet()
        {
            var newPet = await service.CreatePetAsync(usr, new csPetCUdto()
            {
                Name = PetIM.Name,
                Mood = PetIM.Mood,
                Kind = PetIM.Kind,
                FriendId = PetIM.FriendId
            });

            if (newPet is null)
                throw new Exception("Failed to create a new pet.");


            return Redirect($"~/Friend/FriendDetail?id={newPet.Friend.FriendId}");

        }
        #endregion

        #endregion


        #region Constructor
        public AddEditPetModel(IFriendsService service, ILogger<AddEditPetModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion


        #region Input Model
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

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

            public Guid FriendId { get; set; }
            public IPet UpdateModel(IPet model)
            {
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
                FriendId = original.FriendId;
            }
            public csPetIM(IPet model)
            {
                StatusIM = enStatusIM.Unchanged;
                PetId = model.PetId;
                Kind = model.Kind;
                Mood = model.Mood;
                Name = editName = model.Name;
                FriendId = model.Friend?.FriendId ?? Guid.Empty;

            }
        }

        #endregion

        private bool IsValid(string[] validateOnlyKeys = null)
        {
            InvalidKeys = ModelState
                .Where(s => s.Value.ValidationState == ModelValidationState.Invalid);

            if (validateOnlyKeys != null)
            {
                InvalidKeys = InvalidKeys.Where(s => validateOnlyKeys.Any(vk => vk == s.Key));
            }

            ValidationErrorMsgs = InvalidKeys.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage);
            HasValidationErrors = InvalidKeys.Count() != 0;

            return !HasValidationErrors;
        }

    }
}
