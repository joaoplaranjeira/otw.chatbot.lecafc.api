using System.Security.Cryptography.X509Certificates;

namespace otw.chatbot.lecafc.api.Models;

public class Member
{
    public string Name { get; set; } = "";
    public int MemberCode { get; set; } = 0;
    public string Description { get; set; } = "";
    public string Role { get; set; } = "";
    public string Department { get; set; } = "";
}