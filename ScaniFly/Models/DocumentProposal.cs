namespace ScaniFly.Models;

public class DocumentProposal
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OriginalFilePath { get; set; } = string.Empty;
    public string SuggestedTitle { get; set; } = string.Empty;
    public string SuggestedDestinationFolder { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.Now;
    public bool IsAccepted { get; set; } = false;
}
