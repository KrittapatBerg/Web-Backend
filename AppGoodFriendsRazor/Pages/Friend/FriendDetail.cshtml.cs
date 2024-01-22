using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace AppGoodFriendsRazor.Pages.Friend
{
    public class FriendDetailModel : PageModel
    {
        readonly IFriendsService service;
        readonly ILogger<FriendDetailModel> logger;
        loginUserSessionDto usr;

        public string ErrorMessage { get; set; } = null;
        public IFriend FriendDetail { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //Read a QueryParameter
                var idQuery = Request.Query["id"];
                if (string.IsNullOrEmpty(idQuery))
                {
                    ErrorMessage = "ID query parameter is missing.";
                    return Page();
                }

                Guid _id = Guid.Parse(idQuery);

                //Use the Service
                FriendDetail = await service.ReadFriendAsync(usr, _id, false);
                if (FriendDetail == null)
                {
                    ErrorMessage = "No friend found with the provided ID.";
                    return Page();
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            return Page();
            //if (Guid.TryParse(Request.Query["id"], out Guid id))
            //{
            //    //Read a friend
            //    FriendDetail = await service.ReadFriendAsync(usr, id, false)
            //        ?? throw new Exception("Friend not found.");
            //}
            //else
            //{
            //    ModelState.AddModelError(string.Empty, "Whoops, friend does not exist.");
            //}
            //return Page();
        }

        #region Constructor and inject service
        public FriendDetailModel(IFriendsService service, ILogger<FriendDetailModel> logger)
        {
            this.service = service;
            this.logger = logger;
        }
        #endregion
    }
}
