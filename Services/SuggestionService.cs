using SmartEnergy.Web.Models;
using System.Collections.Generic;
using System.Linq;
using SmartEnergy.Web.Data;  

namespace SmartEnergy.Web.Services
{
    public class SuggestionService : ISuggestionService
    {
        private readonly ApplicationDbContext _context;

        public SuggestionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Suggestion> GetAllSuggestions()
        {
            return _context.Suggestions
                .OrderByDescending(s => s.CreatedAt)
                .ToList();
        }

        public void RateSuggestion(int id, int rating)
        {
            var suggestion = _context.Suggestions.FirstOrDefault(s => s.SuggestionId == id);
            if (suggestion != null)
            {
                suggestion.Rating = (suggestion.Rating ?? 0) + rating;
                _context.SaveChanges();
            }
        }

        public void AddSuggestion(Suggestion suggestion)
        {
            _context.Suggestions.Add(suggestion);
            _context.SaveChanges();
        }
    }

}
