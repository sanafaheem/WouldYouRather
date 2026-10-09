using Microsoft.SemanticKernel;
using MongoDB.Driver;
using Microsoft.SemanticKernel.Connectors.Google;
using Scalar.AspNetCore;
using WouldYouRather.API.Features.Question.Models;
using WouldYouRather.API.Features.Question.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();

builder.Services.AddOpenApi();


// MongoDB
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration["MongoDB:ConnectionString"]));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(builder.Configuration["MongoDB:DatabaseName"]);
});

// Registered as a singleton, not scoped: IMongoCollection<T> is a thread-safe, stateless handle
// onto the "questions" collection, not a connection itself, so it's safe to share across requests.
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoDatabase>().GetCollection<Question>("questions"));

// Semantic Kernel
var kernelBuilder = Kernel.CreateBuilder();

if (builder.Environment.IsDevelopment())
{
    // Some local networks can't complete the OCSP/CRL check for Google's cert, causing
    // RevocationStatusUnknown TLS failures. Skip revocation checking in dev only.
    var handler = new HttpClientHandler
    {
        CheckCertificateRevocationList = false
    };
    kernelBuilder.AddGoogleAIGeminiChatCompletion(
        modelId: "gemini-3.8-flash",
        apiKey: builder.Configuration["Gemini:ApiKey"]!,
        httpClient: new HttpClient(handler));
}
else
{
    kernelBuilder.AddGoogleAIGeminiChatCompletion(
        modelId: "gemini-3.8-flash",
        apiKey: builder.Configuration["Gemini:ApiKey"]!);
}

builder.Services.AddSingleton(kernelBuilder.Build());

// Features
builder.Services.AddSingleton<IQuestionContentGenerator, QuestionContentGenerator>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── Pipeline ───────────────────────────────────────────────────

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowClient");
app.UseAuthorization();
app.MapControllers();
app.Run();
