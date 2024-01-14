using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages
{
    public class FriendListModel : PageModel
    {
        IFriendsService service = null;
        ILogger<FriendListModel> logger = null;
        loginUserSessionDto usr = null;
        csFriendCUdto item = null;

        [BindProperty]
        public List<csFriendIM> FriendsIM { get; set; }

        [BindProperty]
        public csFriendIM NewFriendIM { get; set; } = new csFriendIM();

        #region HTTP request
        public IActionResult OnGet()
        {
            FriendsIM = service.ReadFriends(usr, true, false, "", 0, 100).Select(f => new csFriendIM(f)).ToList();
            return Page();
        }

        public IActionResult OnPostDelete(Guid friendId)
        {
            //Set the Quote as deleted, it will not be rendered
            FriendsIM.First(q => q.FriendId == friendId).StatusIM = enStatusIM.Deleted;
            return Page();
        }

        public IActionResult OnPostEdit(Guid friendId)
        {
            //Set the Quote as Modified, it will later be updated in the database
            var f = FriendsIM.First(q => q.FriendId == friendId);
            f.StatusIM = enStatusIM.Modified;

            //Implement the changes
            f.FirstName = f.editFirstName;
            f.LastName = f.editLastName;
            f.Email = f.editEmail;
            f.Birthday = f.editBirthday;
            return Page();
        }
        public IActionResult OnPostAdd()
        {
            //Set the Artist as Inserted, it will later be inserted in the database
            NewFriendIM.StatusIM = enStatusIM.Inserted;

            //Need to add a temp Guid so it can be deleted and editited in the form
            //A correct Guid will be created by the DTO when Inserted into the database
            NewFriendIM.FriendId = Guid.NewGuid();

            //Add it to the Input Models artists
            FriendsIM.Add(new csFriendIM(NewFriendIM));

            //Clear the NewArtist so another album can be added
            NewFriendIM = new csFriendIM();

            return Page();
        }

        public IActionResult OnPostUndo()
        {
            FriendsIM = service.ReadFriends(usr, true, false, "", 0, 100).Select(f => new csFriendIM(f)).ToList();
            return Page();
        }

        public IActionResult OnPostSave()
        {
            //Check if there are deleted quotes, if so simply remove them
            var deletes = FriendsIM.FindAll(q => (q.StatusIM == enStatusIM.Deleted));
            foreach (var item in deletes)
            {
                //Remove from the database
                service.DeleteFriendAsync(usr, item.FriendId);
            }

            #region Add quotes
            //Check if there are any new quotes added, if so create them in the database
            var newies = FriendsIM.FindAll(q => (q.StatusIM == enStatusIM.Inserted));
            foreach (var item1 in newies)
            {
                //Create the corresposning model
                var model = item1.UpdateModel(new csFriend());

                //create in the database
                //model = service.CreateFriendAsync(usr, item);
            }
            #endregion

            //Check if there are any modified quotes , if so update them in the database
            var _modyfies = FriendsIM.FindAll(a => (a.StatusIM == enStatusIM.Modified));
            foreach (var item in _modyfies)
            {
                //get model
                var model = service.ReadFriendAsync(usr, item.FriendId, false);

                //update the changes and save
                //model = item.UpdateModel(model);
                //model = service.UpdateFriendAsync(model);
            }

            //Reload the InputModel
            FriendsIM = service.ReadFriends(usr, true, false, "", 0, 100).Select(f => new csFriendIM(f)).ToList();
            return Page();
        }

        #endregion

        #region Constructors
        //Inject services just like in WebApi
        public FriendListModel(IFriendsService service, ILogger<FriendListModel> logger, csFriendCUdto item, loginUserSessionDto usr)
        {
            this.service = service;
            this.logger = logger;
            this.item = item;
            this.usr = usr;
        }
        #endregion

        #region Input Model
        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        //These classes are in center of ModelBinding and Validation
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

        public class csFriendIM
        {
            //Status of InputModel
            public enStatusIM StatusIM { get; set; }

            //Properties from Model which is to be edited in the <form>
            public Guid FriendId { get; set; } = Guid.NewGuid();
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public DateTime? Birthday { get; set; }


            //Added properites to edit in the list
            public string editFirstName { get; set; }
            public string editLastName { get; set; }

            public string editEmail { get; set; }
            public DateTime? editBirthday { get; set; }

            #region constructors and model update
            public csFriendIM() { StatusIM = enStatusIM.Unchanged; }

            //Copy constructor
            public csFriendIM(csFriendIM original)
            {
                StatusIM = original.StatusIM;

                FriendId = original.FriendId;
                FirstName = original.FirstName;
                LastName = original.LastName;
                Email = original.Email;
                Birthday = original.Birthday;

                editFirstName = original.editFirstName;
                editLastName = original.editLastName;
                editEmail = original.editEmail;
                editBirthday = original.editBirthday;
            }

            //Model => InputModel constructor
            public csFriendIM(IFriend original)
            {
                StatusIM = enStatusIM.Unchanged;
                FriendId = original.FriendId;
                FirstName = editFirstName = original.FirstName;
                LastName = editLastName = original.LastName;
                Email = editEmail = original.Email;
                Birthday = editBirthday = original.Birthday;
            }

            //InputModel => Model
            public IFriend UpdateModel(IFriend model)
            {
                model.FriendId = FriendId;
                model.FirstName = FirstName;
                model.LastName = LastName;
                model.Email = Email;
                model.Birthday = Birthday;
                return model;
            }
            #endregion

        }
        #endregion
    }
}
