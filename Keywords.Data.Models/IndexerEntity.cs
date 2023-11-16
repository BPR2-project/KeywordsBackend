namespace Keywords.Data;

public class IndexerEntity : BaseModel
{
    public Guid VideoId { get; set; }
    public string State { get; set; }
    public string IndexerId { get; set; }
}