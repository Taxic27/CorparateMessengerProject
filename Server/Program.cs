using Server.Repository.Interface;
using Server.Repository;
using Server.Tools;
using Server.Services.Interface;
using Server.Services;
using Server.Hubs;
using System.Text.Json;
using Server.Models.User;
using Server.Models.Chat;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IChatService, ChatService>();

builder.Services.AddSingleton<IMainConnector>(connector => new MainConnector(builder.Configuration.GetValue<string>("ConnectionStrings:DefaultSQLConnection")));

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IMessageRepository, MessageRepository>();
builder.Services.AddSingleton<IChatRepository, ChatRepository>();

builder.Services.AddControllers();
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = null;
    options.DictionaryKeyPolicy = null;
});

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10 МБ (можно увеличить)
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

app.Run();
