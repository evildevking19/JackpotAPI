using System.Text.Json.Nodes;

namespace ApiServer.Models;

public class Champion
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string BannerURL { get; set; } = string.Empty;
    public string BannerMimeType { get; set; } = string.Empty;
    public bool Status { get; set; } = true;
    public string CreateDate { get; set; } = string.Empty;
}
