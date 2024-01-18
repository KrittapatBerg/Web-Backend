using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace AppGoodFriendsRazor.Pages.Edit
{
    public enum enAnimalKind { Dog, Cat, Rabbit, Fish, Bird };
    public enum enAnimalMood { Happy, Hungry, Lazy, Sulky, Buzy, Sleepy };
    public class EditPetModel : PageModel
    {
        IFriendsService service;
        ILogger<EditPetModel> logger;

        [BindProperty]
        public enAnimalKind SelectedKind { get; set; }

        [BindProperty]
        public enAnimalMood SelectedMood { get; set; }

        [BindProperty]
        public List<enAnimalKind> KindList { get; set; } = new List<enAnimalKind>();

        [BindProperty]
        public List<enAnimalMood> MoodList { get; set; } = new List<enAnimalMood> { };

        public IActionResult OnGet()
        {
            return Page();
        }

    }
}
