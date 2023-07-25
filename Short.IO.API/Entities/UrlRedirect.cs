namespace Short.IO.API;
public class UrlRedirect
{
    public Guid Id { get; set; }
    public string ShortUrl { get; set; } = string.Empty;
    public string LongUrl { get; set; } = string.Empty;
    public int Clicks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}
