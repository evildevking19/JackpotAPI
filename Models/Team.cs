using System.Text.Json.Nodes;

namespace ApiServer.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BannerURL { get; set; } = string.Empty;
    public string BannerMimeType { get; set; } = string.Empty;
    public string CreateDate { get; set; } = string.Empty;
}
