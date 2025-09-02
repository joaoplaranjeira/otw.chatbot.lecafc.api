using otw.chatbot.lecafc.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace otw.chatbot.lecafc.apis.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ChatRequest request)
    {
        var reply = await _chatService.Answer(request.Message);
        return Ok(new ChatResponse { Reply = reply });
    }
}

public class ChatRequest
{
    public string Message { get; set; } = "";
}

public class ChatResponse
{
    public string Reply { get; set; } = "";
}