namespace otw.chatbot.lecafc.api.Models;

public class Team
{
    public string Name { get; set; } = "";
    public List<Match> Matches { get; set; } = new();
}