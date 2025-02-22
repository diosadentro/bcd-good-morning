using Newtonsoft.Json;

public class Pagination
{
    [JsonProperty("total")] public int Total { get; set; }
    [JsonProperty("limit")] public int Limit { get; set; }
    [JsonProperty("offset")] public int Offset { get; set; }
    [JsonProperty("total_pages")] public int TotalPages { get; set; }
    [JsonProperty("current_page")] public int CurrentPage { get; set; }
    [JsonProperty("next_url")] public string NextUrl { get; set; }
}

public class Thumbnail
{
    [JsonProperty("lqip")] public string Lqip { get; set; }
    [JsonProperty("width")] public int? Width { get; set; }
    [JsonProperty("height")] public int? Height { get; set; }
    [JsonProperty("alt_text")] public string AltText { get; set; }
}

public class Color
{
    [JsonProperty("h")] public int? H { get; set; }
    [JsonProperty("l")] public int? L { get; set; }
    [JsonProperty("s")] public int? S { get; set; }
    [JsonProperty("percentage")] public double? Percentage { get; set; }
    [JsonProperty("population")] public int? Population { get; set; }
}

public class Artwork
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("artist_display")] public string ArtistDisplay { get; set; }
    [JsonProperty("place_of_origin")] public string PlaceOfOrigin { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("dimensions")] public string Dimensions { get; set; }
    [JsonProperty("medium_display")] public string MediumDisplay { get; set; }
    [JsonProperty("credit_line")] public string CreditLine { get; set; }
    [JsonProperty("is_public_domain")] public bool? IsPublicDomain { get; set; }
    [JsonProperty("is_zoomable")] public bool? IsZoomable { get; set; }
    [JsonProperty("artwork_type_title")] public string ArtworkTypeTitle { get; set; }
    [JsonProperty("department_title")] public string DepartmentTitle { get; set; }
    [JsonProperty("artist_id")] public int? ArtistId { get; set; }
    [JsonProperty("artist_title")] public string ArtistTitle { get; set; }
    [JsonProperty("category_titles")] public List<string> CategoryTitles { get; set; }
    [JsonProperty("style_titles")] public List<string> StyleTitles { get; set; }
    [JsonProperty("classification_titles")] public List<string> ClassificationTitles { get; set; }
    [JsonProperty("subject_titles")] public List<string> SubjectTitles { get; set; }
    [JsonProperty("material_titles")] public List<string> MaterialTitles { get; set; }
    [JsonProperty("technique_titles")] public List<string> TechniqueTitles { get; set; }
    [JsonProperty("thumbnail")] public Thumbnail Thumbnail { get; set; }
    [JsonProperty("image_id")] public string ImageId { get; set; }
}

public class Info
{
    [JsonProperty("license_text")] public string LicenseText { get; set; }
    [JsonProperty("license_links")] public List<string> LicenseLinks { get; set; }
    [JsonProperty("version")] public string Version { get; set; }
}

public class Config
{
    [JsonProperty("iiif_url")] public string IiifUrl { get; set; }
    [JsonProperty("website_url")] public string WebsiteUrl { get; set; }
}

public class ArtModel
{
    [JsonProperty("pagination")] public Pagination Pagination { get; set; }
    [JsonProperty("data")] public List<Artwork> Data { get; set; }
    [JsonProperty("info")] public Info Info { get; set; }
    [JsonProperty("config")] public Config Config { get; set; }
}
