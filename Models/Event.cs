using System.Text.Json.Nodes;

namespace ApiServer.Models;

public class Event
{
    public int Id { get; set; }
    public int JackpotId { get; set; }
    public int HomeTeamId { get; set; }
    public int VisitorTeamId { get; set; }
    public string TicketSaleName { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Complement { get; set; } = string.Empty;
    public double Number { get; set; } = 0;
    public string Producer { get; set; } = string.Empty;
    public string PublicPlace { get; set; } = string.Empty;
    public string Neighbor { get; set; } = string.Empty;
}
