using System.Text.Json.Nodes;

namespace ApiServer.Models;

public class Question
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = "VALOR DO POTE";
    public int PotLevel { get; set; }
    public bool Status { get; set; }
    public string CreateDate { get; set; } = string.Empty;
    public IEnumerable<QuestionItem> Items { get; set; } = [];
}
