/*using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Services;

public class SuggestionsController : Controller
{
    private readonly ISuggestionService _suggestionService;

    public SuggestionsController(ISuggestionService suggestionService)
    {
        _suggestionService = suggestionService;
    }

    public IActionResult Index()
    {
        var suggestions = _suggestionService.GetAllSuggestions();
        return View(suggestions);
    }

    public IActionResult Like(int id)
    {
        _suggestionService.RateSuggestion(id, 1);
        return RedirectToAction("Index");
    }

    public IActionResult Dislike(int id)
    {
        _suggestionService.RateSuggestion(id, -1);
        return RedirectToAction("Index");
    }
}*/

//main code below

using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Models;

namespace SmartEnergy.Web.Controllers
{
    public class SuggestionController : Controller
    {
        private static List<Suggestion> _suggestions = new List<Suggestion>(); // Temporary storage

        // GET: /Suggestions
        public IActionResult Index()
        {
            return View(_suggestions.OrderByDescending(s => s.CreatedAt).ToList());
        }

        // GET: /Suggestions/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Suggestions/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Suggestion suggestion)
        {
            if (ModelState.IsValid)
            {
                suggestion.SuggestionId = _suggestions.Count + 1;
                suggestion.CreatedAt = DateTime.Now;
                suggestion.Source = "User"; // or AI / Static if required
                _suggestions.Add(suggestion);
                return RedirectToAction("Index");
            }

            return View(suggestion);
        }
    }
}
