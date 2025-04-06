namespace DigitalTherapyBackendApp.Application.Dtos.DailyTips
{
    public class DailyTipDto
    {
        public int Id { get; set; }
        public string TipKey { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBookmarked { get; set; }
        public DailyTipCategoryDto Category { get; set; }
    }
}