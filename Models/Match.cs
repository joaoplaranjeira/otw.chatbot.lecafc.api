namespace otw.chatbot.lecafc.api.Models;

public class Match
{
    public string TeamDesignation { get; set; } = "";
    public string Opponent { get; set; } = "";
    public DateTime Date { get; set; }
    public string Local { get; set; } = "";
    public bool IsHomeMatch { get; set; }
}