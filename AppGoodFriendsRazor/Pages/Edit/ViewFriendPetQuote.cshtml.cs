using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;
using System.ComponentModel.DataAnnotations;

namespace AppGoodFriendsRazor.Pages.Edit
{
    public class ViewAndEditModel : PageModel
    {
        IFriendsService service = null;
        ILogger<ViewAndEditModel> logger = null;
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

        //public async Task<IActionResult> OnPostAddFriend()
        //{
        //    ViewFriendIM.New
        //}

        #endregion

        #region Constructor
        public ViewAndEditModel(IFriendsService service, ILogger<ViewAndEditModel> logger)
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

            //Added properties to edit in the list with undo
            [Required(ErrorMessage = "You must provide firstname")]
            public string editFirstname { get; set; }

            [Required(ErrorMessage = "You must provide lastname")]
            public string editLastname { get; set; }
            public string editEmail { get; set; }
            public DateTime? editBirthday { get; set; }

            #region constructors and model update 
            public csViewFriendIM()
            {
                StatusIM = enStatusIM.Unchanged;
                AddressIM = new csAddressIM();
            }

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
            public IFriend UpdateModel(IFriend model)
            {
                model.FriendId = FriendId;
                model.FirstName = Firstname;
                model.LastName = Lastname;
                model.Email = Email;
                model.Birthday = Birthday;
                model.Address = AddressIM.UpdateModel(model.Address);
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

            //Added properties to edit in the list with undo
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
        }
        #endregion
    }
}
