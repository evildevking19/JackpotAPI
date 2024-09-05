namespace ApiServer.Models;

public class Award
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "JOGO";
    public string Code { get; set; } = string.Empty;
    public string BannerURL { get; set; } = string.Empty;
    public string BannerMimeType { get; set; } = string.Empty;
    public string Status { get; set; } = "LIVRE";
    public bool Active { get; set; } = true;
    public string CreateDate { get; set; } = string.Empty;
}
