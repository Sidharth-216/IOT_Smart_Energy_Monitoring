namespace SmartEnergy.Web.Models
{
    public class Suggestion
    {
        public int SuggestionId { get; set; }
        public string Text { get; set; }
        public string Source { get; set; } // AI / Static
        public DateTime CreatedAt { get; set; }
        public int? Rating { get; set; } // thumbs up/down
    }
}
