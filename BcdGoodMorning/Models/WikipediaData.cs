using Newtonsoft.Json;

namespace BcdGoodMorning.Models;

public class Namespace
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class Titles
    {
        [JsonProperty("canonical")]
        public string Canonical { get; set; }

        [JsonProperty("normalized")]
        public string Normalized { get; set; }

        [JsonProperty("display")]
        public string Display { get; set; }
    }

    public class Thumbnail
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class OriginalImage
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class ContentUrls
    {
        [JsonProperty("desktop")]
        public Urls Desktop { get; set; }

        [JsonProperty("mobile")]
        public Urls Mobile { get; set; }
    }

    public class Urls
    {
        [JsonProperty("page")]
        public string Page { get; set; }

        [JsonProperty("revisions")]
        public string Revisions { get; set; }

        [JsonProperty("edit")]
        public string Edit { get; set; }

        [JsonProperty("talk")]
        public string Talk { get; set; }
    }

    public class WikipediaData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("displaytitle")]
        public string DisplayTitle { get; set; }

        [JsonProperty("namespace")]
        public Namespace Namespace { get; set; }

        [JsonProperty("wikibase_item")]
        public string WikibaseItem { get; set; }

        [JsonProperty("titles")]
        public Titles Titles { get; set; }

        [JsonProperty("pageid")]
        public int PageId { get; set; }

        [JsonProperty("thumbnail")]
        public Thumbnail Thumbnail { get; set; }

        [JsonProperty("originalimage")]
        public OriginalImage OriginalImage { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("dir")]
        public string Dir { get; set; }

        [JsonProperty("revision")]
        public string Revision { get; set; }

        [JsonProperty("tid")]
        public string Tid { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("description_source")]
        public string DescriptionSource { get; set; }

        [JsonProperty("content_urls")]
        public ContentUrls ContentUrls { get; set; }

        [JsonProperty("extract")]
        public string Extract { get; set; }

        [JsonProperty("extract_html")]
        public string ExtractHtml { get; set; }
    }