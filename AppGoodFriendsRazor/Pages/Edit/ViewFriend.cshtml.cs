using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace AppGoodFriendsRazor.Pages.Edit
{
    public class ViewAndEditModel : PageModel
    {
        IFriendsService service = null;
        ILogger<ViewAndEditModel> logger = null;

        [BindProperty]
        public List<csViewAndEditEverythingIM> ViewAndEditEverythingIMs { get; set; }

        #region HTTP request
        public IActionResult OnGet()
        {
            //ViewAndEditEverythingIMs = service.ReadFriendAsync().Select(e => new csViewAndEditEverythingIM(e)).ToList();
            return Page();
        }
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

        public class csViewAndEditEverythingIM
        {
            //Status of InputModel
            public enStatusIM StatusIM { get; set; }

            //properties from Model which is to be edited in the <form>
            public Guid FriendId { get; init; } = Guid.NewGuid();
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string Email { get; set; }

            //Added properties to edit in the list with undo
            public string editFirstname { get; set; }
            public string editLastname { get; set; }
            public string editEmail { get; set; }

            #region constructors and model update 
            public csViewAndEditEverythingIM() { StatusIM = enStatusIM.Unchanged; }

            //copy constructor
            public csViewAndEditEverythingIM(csViewAndEditEverythingIM original)
            {
                StatusIM = original.StatusIM;
                FriendId = original.FriendId;
                Firstname = original.Firstname;
                Lastname = original.Lastname;
                Email = original.Email;

                editFirstname = original.editFirstname;
                editLastname = original.editLastname;
                editEmail = original.editEmail;
            }

            //Model => InputModel constructor
            public csViewAndEditEverythingIM(csFriend original)
            {
                StatusIM = enStatusIM.Unchanged;
                FriendId = original.FriendId;
                Firstname = editFirstname = original.FirstName;
                Lastname = editLastname = original.LastName;
                Email = editEmail = original.Email;
            }

            //InputModel => Model 
            public csFriend UpdateModel(csFriend model)
            {
                model.FriendId = FriendId;
                model.FirstName = Firstname;
                model.LastName = Lastname;
                model.Email = Email;
                return model;
            }
            #endregion
        }
        #endregion
    }
}
