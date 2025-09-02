using System.Text;
using System.Text.Json;
using otw.chatbot.lecafc.api.Models;

namespace otw.chatbot.lecafc.api.Services;

public class MemberService
{
    private readonly string _file;

    public MemberService(string filePath)
    {
        _file = Path.Combine(Directory.GetCurrentDirectory(), filePath);
    }

    public string GetContext()
    {
        var json = File.ReadAllText(_file);
        var members = JsonSerializer.Deserialize<List<Member>>(json)!;

        var sb = new StringBuilder();
        sb.AppendLine("Membros:");

        foreach (var member in members)
        {
            sb.AppendLine($"\n• {member.Name} ({member.MemberCode}):");
            sb.AppendLine($"  Cargo: {member.Role}");
        }

        return sb.ToString();
    }
}