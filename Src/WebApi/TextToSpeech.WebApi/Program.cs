using Microsoft.AspNetCore.Http.HttpResults;
using Scalar.AspNetCore;
using TextToSpeech.WebApi.AiServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/Audio/TextToSpeech", async (string text) =>
{
    AiService aiService = new AiService(builder.Configuration);
    var result = await aiService.TextToSpeech(text);

    return Results.Created();
});

app.MapPost("/api/Audio/TextToSpeechAvanegar/", async (string text) =>
{
    AiService aiService = new AiService(builder.Configuration);
    var url = await aiService.TextToSpeechAvanegar(text);
    // return Results.Ok("https://"+url.data.data.filePath);
    return Results.Text("https://" + url.data.data.filePath);
});

app.Run();

