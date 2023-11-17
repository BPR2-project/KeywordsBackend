namespace Keywords.Data;

public class KeywordEntity : BaseModel
{
    public string Content { get; set; }
    public bool IsPublished { get; set; }
    public Guid VideoId { get; set; }
    public string Language { get; set; }
    public string AudioLink { get; set; }
}