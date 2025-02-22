namespace BcdGoodMorning.Models;

public class NewsArticle
{
    public int Weight { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public string Guid { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public DateTime PubDate { get; set; }
    public string Source { get; set; }
    public string Summary { get; set; }
    public string OriginalSummary { get; set; }
    public NewsArticleParse ParseData { get; set; }
    public string RawHtml { get; set; }
}