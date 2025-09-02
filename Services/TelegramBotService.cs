using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace otw.chatbot.lecafc.api.Services;

public class TelegramBotService : IHostedService
{
    private readonly ITelegramBotClient _bot;
    private readonly OpenAIService _openAi;
    private readonly CalendarService _calendarService;

    public TelegramBotService(IConfiguration config, OpenAIService openAI, CalendarService calendarService)
    {
        // Usa a interface com a versão 18.x
        _bot = new TelegramBotClient(config["TelegramBotToken"]);
        _openAi = openAI;
        _calendarService = calendarService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // Recebe todos os tipos de mensagens
        };

        _bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        Console.WriteLine("🤖 Bot do Leça FC a correr...");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("🛑 Bot desligado.");
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message is not { } message || message.Text is null)
            return;

        var chatId = message.Chat.Id;
        var text = message.Text;
        var context = _calendarService.GetContext();

        Console.WriteLine($"📩 Mensagem recebida de {message.From?.Username ?? "Utilizador"}: {text}");

        if (text.StartsWith("/start"))
        {
            await _bot.SendTextMessageAsync(
                chatId,
                "👋 Olá! Sou o assistente do Leça Futebol Clube.\n\nPodes perguntar:\n• Quando é o próximo jogo?\n• Onde é o estádio?\n• Quem joga este fim de semana?\n• E muito mais...",
                cancellationToken: ct
            );
            return;
        }

        // Passa a mensagem ao modelo Mistral via OpenAI
        var resposta = await _openAi.InterpretMessageAsync(text, context);

        await _bot.SendTextMessageAsync(
            chatId,
            resposta,
            cancellationToken: ct
        );
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Erro da API do Telegram: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine($"❌ Erro: {errorMessage}");
        return Task.CompletedTask;
    }
}