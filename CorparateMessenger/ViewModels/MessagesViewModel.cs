using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CorparateMessenger.Tools;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using Server.Models.Chat;
using Server.Models.Message;
using Server.Models.User;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CorparateMessenger.ViewModels
{
    public partial class MessagesViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoadingMessages;

        [ObservableProperty]
        private string _messageText = string.Empty;

        private UserDB _currentUser;

        public ObservableCollection<MessageModel> Messages { get; } = new();

        private int _loadedMessagesCount = 100;
        private const int MessagesBatchSize = 20;

        private HubConnection _hubConnection;

        [ObservableProperty]
        private ChatDB? _currentChat;

        public MessagesViewModel(HubConnection hubConnection, UserDB currentUser)
        {
            _hubConnection = hubConnection;
            _currentUser = currentUser;
            InitializeHubMethods();
        }

        private void InitializeHubMethods()
        {
            _hubConnection.On<MessageDTO>("ReceiveMessage", (message) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var newMessage = new MessageModel
                    {
                        Id = message.Id,
                        Role = message.Username == _currentUser.Username ? ChatRoleType.Sender : ChatRoleType.Receiver,
                        Username = message.Username,
                        SenderName = message.SenderName,
                        Text = message.Text,
                        SentAt = message.SentAt,
                        FileUrl = message.FileUrl,
                        FileName = message.FileName,
                        FileType = message.FileType,
                        FileSize = message.FileSize
                    };

                    var lastMessage = Messages.LastOrDefault();
                    newMessage.IsSenderInfoVisible = lastMessage == null || lastMessage.SenderName != newMessage.SenderName;

                    WeakReferenceMessenger.Default.Send(new UpdateLastMessage(CurrentChat.Id));

                    Messages.Add(newMessage);
                });
            });
        }

        public async Task SetCurrentChat(ChatDB chat)
        {
            CurrentChat = chat;
            Messages.Clear();
            _loadedMessagesCount = 0;

            if (chat != null)
            {
                await _hubConnection.InvokeAsync("JoinChat", chat.Id);
                await LoadChatMessages();
            }
        }

        private async Task LoadChatMessages()
        {
            Messages.Clear();
            _loadedMessagesCount = 0;

            try
            {
                if (CurrentChat != null)
                {
                    var initialMessages = await _hubConnection.InvokeAsync<List<MessageModel>>(
                        "RequestMessageHistory",
                        CurrentChat.Id,
                        0,
                        100
                    );

                    var processedMessages = initialMessages.Select(m => new MessageModel
                    {
                        Id = m.Id,
                        Role = m.Username == _currentUser.Username ? ChatRoleType.Sender :ChatRoleType.Receiver,
                        Username = m.Username,
                        SenderName = m.SenderName,
                        Text = m.Text,
                        SentAt = m.SentAt,
                        FileUrl = m.FileUrl,
                        FileName = m.FileName,
                        FileType = m.FileType,
                        FileSize = m.FileSize
                    }).ToList();

                    if (processedMessages?.Count > 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {

                            var reversedMessages = processedMessages.AsEnumerable().Reverse().ToList();

                            ProcessMessagesHeaders(reversedMessages, Messages.FirstOrDefault());

                            foreach (var message in reversedMessages)
                            {
                                Messages.Add(message);
                            }

                            _loadedMessagesCount = processedMessages.Count;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.WarningGlobal($"Ошибка загрузки чата: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task LoadMoreMessages()
        {
            if (IsLoadingMessages || CurrentChat == null) return;

            IsLoadingMessages = true;
            try
            {
                var additionalMessages = await _hubConnection.InvokeAsync<List<MessageModel>>(
                    "RequestMessageHistory",
                    CurrentChat.Id,
                    _loadedMessagesCount,
                    MessagesBatchSize);

                if (additionalMessages?.Count > 0)
                {
                    var lastMessageBeforeInsert = Messages.FirstOrDefault();
                    var reversedMessages = additionalMessages.AsEnumerable().ToList();

                    ProcessMessagesHeaders(reversedMessages, lastMessageBeforeInsert);

                    foreach (var message in reversedMessages)
                    {
                        Messages.Insert(0, message);
                    }

                    _loadedMessagesCount += additionalMessages.Count;
                }
            }
            finally
            {
                IsLoadingMessages = false;
            }
        }

        private void ProcessMessagesHeaders(List<MessageModel> messages, MessageModel previousMessage = null)
        {
            string lastSender = previousMessage?.SenderName;

            for (int i = 0; i < messages.Count; i++)
            {
                messages[i].IsSenderInfoVisible = messages[i].SenderName != lastSender;
                lastSender = messages[i].SenderName;
            }
        }

        [RelayCommand]
        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageText)) return;
            if (CurrentChat == null) return;

            try
            {
                await _hubConnection.InvokeAsync("SendMessage",
                    CurrentChat.Id,
                    _currentUser.Id,
                    MessageText);

                MessageText = string.Empty;
            }
            catch (Exception ex)
            {
                Growl.WarningGlobal($"{ex.Message}");
            }
        }

        [RelayCommand]
        private async Task SendFile()
        {
            if (CurrentChat == null) return;

            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(filePath);
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    string fileType = GetFileType(fileName);

                    await _hubConnection.InvokeAsync("SendFile",
                        CurrentChat.Id,
                        _currentUser.Id,
                        fileName,
                        fileType,
                        fileBytes);
                }
                catch (Exception ex)
                {
                    Growl.WarningGlobal($"{ex.Message}");
                }
            }
        }

        [RelayCommand]
        public async Task DownloadFile(MessageModel message)
        {
            if (message?.FileUrl == null) return;

            try
            {
                string fileName = message.FileName;
                string fileExtension = Path.GetExtension(fileName);

                var saveDialog = new SaveFileDialog
                {
                    FileName = fileName,
                    Filter = $"{fileExtension} files|*{fileExtension}|All files|*.*"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    byte[] fileData = await _hubConnection.InvokeAsync<byte[]>(
                        "DownloadFile",
                        message.FileUrl);

                    await File.WriteAllBytesAsync(saveDialog.FileName, fileData);
                    Growl.Success("Файл скачен");
                }
            }
            catch (Exception ex)
            {
                Growl.WarningGlobal($"{ex.Message}");
            }
        }

        private string GetFileType(string filename)
        {
            string extension = Path.GetExtension(filename).ToLower();
            return extension switch
            {
                ".jpg" or ".png" or ".jpeg" or ".gif" => "Фото",
                ".mp4" or ".mov" or ".avi" => "Видео",
                _ => "Документ"
            };
        }

        //[RelayCommand]
        //private void ScrollChanged(ScrollChangedEventArgs e)
        //{
        //    if (e.VerticalChange < 0 && e.VerticalOffset <= 100 && !IsLoadingMessages)
        //    {
        //        LoadMoreMessagesCommand.Execute(null);
        //    }
        //}

        [RelayCommand]
        private void OpenImage(MessageModel message)
        {
            if (message?.FileUrl == null || message.FileType != "Фото")
                return;

            try
            {
                new ImageBrowser(new Uri(message.FileUrl))
                {
                    Owner = WindowHelper.GetActiveWindow()
                }.Show();
            }
            catch (Exception ex)
            {
                Growl.WarningGlobal($"{ex.Message}");
            }
        }

        [RelayCommand]
        private void AddNewLine(System.Windows.Controls.TextBox textBox)
        {
            if (textBox == null) return;

            var caretPos = textBox.CaretIndex;
            MessageText = MessageText.Insert(caretPos, Environment.NewLine);
            textBox.CaretIndex = caretPos + Environment.NewLine.Length;
        }
    }
}