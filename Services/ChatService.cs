
using Microsoft.AspNetCore.SignalR;

namespace otw.chatbot.lecafc.api.Services;

public class ChatService
{
    private readonly List<string> _history;
    private readonly OpenAIService _openAi;
    private readonly CalendarService _calendarService;
    private readonly MemberService _memberService;

    public ChatService(OpenAIService openAi, CalendarService calendarService, MemberService memberService)
    {
        _history = new List<string>();
        _openAi = openAi;
        _calendarService = calendarService;
        _memberService = memberService;
    }

    public async Task<string> Answer(string userMessage)
    {
        var context = GetContext();
        var response = await _openAi.InterpretMessageAsync(userMessage, context);
        return response;
    }

    private string GetContext()
    {
        var context = _calendarService.GetContext();
        context = context + _memberService.GetContext();
        //context = context + getHistory();
        return context;
    }

    private string getHistory()
    {
        string history = "Histórico de perguntas:\n";
        foreach (var item in _history)
        {
            history += $"{item}\n";
        }
        return history;
    }
}