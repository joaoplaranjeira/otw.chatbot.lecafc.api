using System.Text;
using System.Text.Json;
using otw.chatbot.lecafc.api.Models;

namespace otw.chatbot.lecafc.api.Services;

public class CalendarService
{
    private readonly string _file;

    public CalendarService(string filePath)
    {
        _file = Path.Combine(Directory.GetCurrentDirectory(), filePath);
    }

    public string GetContext()
    {
        var json = File.ReadAllText(_file);
        var teams = JsonSerializer.Deserialize<List<Team>>(json)!;

        var sb = new StringBuilder();
        sb.AppendLine("Calendário atual:");

        foreach (var team in teams)
        {
            sb.AppendLine($"\n• {team.Name}:");
            foreach (var match in team.Matches.OrderBy(j => j.Date))
            {
                var date = match.Date.ToString("dd MMM yyyy às HH:mm");
                var homeOrAway = match.IsHomeMatch ? "em casa" : "fora";
                sb.AppendLine($" {match.TeamDesignation} vs {match.Opponent} ({date}, {match.Local})");
            }
        }

        return sb.ToString();
    }
}