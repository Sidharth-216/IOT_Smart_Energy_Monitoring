using Microsoft.AspNetCore.Mvc;
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
}
