using System.Text.Json.Nodes;

namespace ApiServer.Models;

public class QuestionItem
{
    public int Id { get; set; }
    public int QId { get; set; }
    public string QString { get; set; } = string.Empty;
    public string QType { get; set; } = "Objetiva";
    public string ObjOptions { get; set; } = string.Empty;
    public string Quantity { get; set; } = "Unico";
    public string SubjSingleValue { get; set; } = string.Empty;
    public string SubjDoubleValue { get; set; } = ",,";
    
}
