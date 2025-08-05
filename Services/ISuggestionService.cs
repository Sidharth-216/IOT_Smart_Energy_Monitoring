using SmartEnergy.Web.Models;
using System.Collections.Generic;

public interface ISuggestionService
{
    IEnumerable<Suggestion> GetAllSuggestions();
    void RateSuggestion(int id, int rating); // +1 / -1
    void AddSuggestion(Suggestion suggestion);
}
