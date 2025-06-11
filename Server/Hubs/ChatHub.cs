using Microsoft.AspNetCore.SignalR;
using Server.Models.Message;
using Server.Services;
using Server.Services.Interface;
namespace Server.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IFileService _fileService;

    public ChatHub(IMessageService messageService, IFileService fileService)
    {
        _messageService = messageService;
        _fileService = fileService;
    }
    public async Task SendMessage(Guid chatId, Guid senderId, string message)
    {
        try
        {
            var savedMessage = _messageService.SaveMessage(chatId, senderId, message);

            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", savedMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в SendMessage: {ex}");
            throw new HubException(ex.Message);
        }
    }

    public async Task SendFile(Guid chatId, Guid senderId, string fileName, string fileType, byte[] fileData)
    {
        try
        {
            string fileUrl = await _fileService.SaveFileAsync(fileName, fileData);

            var savedMessageFile = _messageService.SaveMessageFile(chatId, senderId, fileUrl, fileName, fileType, fileData);

            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", savedMessageFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в SendFile: {ex}");
            throw new HubException(ex.Message);
        }
    }

    public async Task<byte[]> DownloadFile(string fileUrl)
    {
        try
        {
            var fileData = await _fileService.GetFileAsync(fileUrl);

            if (fileData == null || fileData.Length == 0)
            {
                throw new FileNotFoundException("Файл не найден или пуст");
            }

            return fileData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при скачивании файла: {ex}");
            throw new HubException($"Не удалось скачать файл: {ex.Message}");
        }
    }

    public async Task<List<MessageDTO>> RequestMessageHistory(Guid chatId, int skip = 0, int take = 50)
    {
        try
        {
            var messages = _messageService.GetMessagesForChat(chatId, skip, take);
            return messages ?? new List<MessageDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
            return new List<MessageDTO>();
        }
    }

    public async Task JoinChat(Guid chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }
}