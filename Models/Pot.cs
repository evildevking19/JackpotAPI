using System.Text.Json.Nodes;

namespace ApiServer.Models;

public class Pot
{
    public int Id { get; set; }
    public int ChampionId { get; set; }
    public string ChampionName { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public double InitValue { get; set; }
    public double CurValue { get; set; } = 0;
    public bool Status { get; set; } = true;
    public string CreateDate { get; set; } = string.Empty;
}
