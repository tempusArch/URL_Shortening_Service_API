namespace ShortenUrlApi.Models;

public class ShortUrlModel {
    public int Id {get; set;}
    public string OriginalUrl {get; set;}
    public string ShortCode {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    public int AccessCount {get; set;}
}