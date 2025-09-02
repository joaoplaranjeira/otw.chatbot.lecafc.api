using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using otw.chatbot.lecafc.api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddHttpClient();
builder.Services.AddSingleton<OpenAIService>();
builder.Services.AddSingleton(sp => new CalendarService("Data/calendar.json"));
builder.Services.AddSingleton(sp => new MemberService("Data/members.json"));
builder.Services.AddHostedService<TelegramBotService>();
builder.Services.AddScoped<ChatService>();

// 👇 Força o host a escutar em todas as interfaces
builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await app.RunAsync();