namespace Keywords.Data;

public class IndexerEntity : BaseModel
{
    public IndexerState State { get; set; }
    public string IndexerId { get; set; }
    public string? KeyPhraseJobId { get; set; }
}