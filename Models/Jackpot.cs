using System.Text.Json.Nodes;

namespace ApiServer.Models;

public class Jackpot
{
    public int Id { get; set; }
    public int ChampionId { get; set; }
    public int TeamId { get; set; }
    public int QuestionId { get; set; }
    public string AwardType { get; set; } = string.Empty;
    public string PrizeCategory { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public double PotValue { get; set; } =  0;
    public double PotBalance { get; set; } = 0;
    public string BannerURL { get; set; } = string.Empty;
    public string BannerMimeType { get; set; } = string.Empty;
    public bool BannerStatus { get; set; } = true;
    public string CreateDate { get; set; } = string.Empty;
    public string LotteryDate { get; set; } = string.Empty;
    public string Status { get; set; } = "Ativo";
    public string RoundName { get; set; } = string.Empty;
    public string HomeTeamNames { get; set; } = string.Empty;
    public string TicketGames { get; set; } = string.Empty;
    public string TicketSales { get; set; } = string.Empty;
    public IEnumerable<Event> Events { get; set; } = [];
}
