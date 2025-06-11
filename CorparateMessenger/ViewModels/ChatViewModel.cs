using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CorparateMessenger.Tools;
using CorparateMessenger.Views;
using HandyControl.Controls;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Server.Models.Chat;
using Server.Models.User;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CorparateMessenger.ViewModels
{
    public partial class ChatViewModel : ObservableObject
    {
        [ObservableProperty]
        private ViewType _currentView = ViewType.Welcome;

        [ObservableProperty]
        private HubConnectionState _connectionState = HubConnectionState.Disconnected;

        [ObservableProperty]
        private ChatDTO _selectedChat;

        private ChatDB _selectedChatDB;

        [ObservableProperty]
        private ChatDTO _selectedUser;

        public ObservableCollection<ChatDTO> Chats { get; } = new();
        public ObservableCollection<ChatDTO> Users { get; } = new();

        private MessagesViewModel _messagesVM;
        public MessagesViewModel MessagesVM
        {
            get => _messagesVM;
            set
            {
                _messagesVM = value;
                OnPropertyChanged();
            }
        }

        private UserManagmentViewModel _userVM;
        public UserManagmentViewModel UserVM
        {
            get => _userVM;
            set
            {
                _userVM = value;
                OnPropertyChanged();
            }
        }

        private GroupCreateViewModel _groupVM;
        public GroupCreateViewModel GroupVM
        {
            get => _groupVM;
            set
            {
                _groupVM = value;
                OnPropertyChanged();
            }
        }

        private HubConnection _hubConnection;

        [ObservableProperty]
        private UserDB _currentUser;

        [ObservableProperty]
        private bool _isLoadingMessages;

        public ChatViewModel(UserDB currentUser)
        {
            InitializeHubConnection();
            _currentUser = currentUser;
            MessagesVM = new MessagesViewModel(_hubConnection, _currentUser);

            LoadAvailableChats();
            LoadAvailableUsers();

        }

        private async void InitializeHubConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7274/chatHub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.Reconnecting += _ =>
            {
                ConnectionState = _hubConnection.State;
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += _ =>
            {
                ConnectionState = _hubConnection.State;
                return Task.CompletedTask;
            };

            _hubConnection.Closed += _ =>
            {
                ConnectionState = _hubConnection.State;
                return Task.CompletedTask;
            };

            try
            {
                await _hubConnection.StartAsync();
                ConnectionState = _hubConnection.State;
            }
            catch (Exception ex)
            {
                Growl.Warning($"Ошибка подключения: {ex.Message}");
            }
        }

        private async void LoadAvailableChats()
        {
            try
            {
                using var httpClient = new HttpClient();

                var response = await httpClient.GetAsync($"https://localhost:7274/api/chats/userchats-group?currentUserId={_currentUser.Id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<List<ChatDTO>>>();

                    if (result?.IsSuccess == true)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Chats.Clear();
                            foreach (var chat in result.Data)
                                Chats.Add(chat);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Warning($"Ошибка: {ex.Message}");
            }
        }

        private async void LoadAvailableUsers()
        {
            try
            {
                using var httpClient = new HttpClient();
                var userResponse = await httpClient.GetAsync("https://localhost:7274/api/users/id");

                if (userResponse.IsSuccessStatusCode)
                {
                    var userResult = await userResponse.Content.ReadFromJsonAsync<ApiResult<List<Guid>>>();

                    if (userResult?.IsSuccess == true)
                    {
                        var request = new
                        {
                            UsersId = userResult.Data.Where(id => id != _currentUser.Id).ToList(),
                            CurrentUserId = _currentUser.Id
                        };

                        var chatResponse = await httpClient.PostAsJsonAsync(
                            "https://localhost:7274/api/chats/userchats-private",
                            request);

                        if (chatResponse.IsSuccessStatusCode)
                        {
                            var chatResult = await chatResponse.Content.ReadFromJsonAsync<ApiResult<List<ChatDTO>>>();

                            if (chatResult.IsSuccess)
                            {
                                Users.Clear();
                                foreach (var userChats in chatResult.Data)
                                    Users.Add(userChats);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Warning($"Ошибка: {ex.Message}");
            }
        }


        [RelayCommand]
        private async Task SelectUser()
        {
            if (SelectedUser == null) return;

            _selectedChatDB = await CreateOrGetPrivateChat(SelectedUser, _currentUser.Id);

            SelectedChat = null;

            CurrentView = ViewType.Messages;


            await MessagesVM.SetCurrentChat(_selectedChatDB);
        }

        [RelayCommand]
        private async Task SelectChat()
        {
            if (SelectedChat == null) return;

            if (SelectedChat.IsGroup)
            {
                SelectedUser = null;
            }

            _selectedChatDB = ConvertToDB(SelectedChat);

            CurrentView = ViewType.Messages;

            await MessagesVM.SetCurrentChat(_selectedChatDB);
        }

        //[RelayCommand]
        //private async Task LoadMoreMessages()
        //{
        //    if (IsLoadingMessages || SelectedChat == null) return;

        //    IsLoadingMessages = true;
        //    try
        //    {
        //        var additionalMessages = await _hubConnection.InvokeAsync<List<MessageModel>>(
        //            "RequestMessageHistory",
        //            SelectedChat.Id,
        //            _loadedMessagesCount,
        //            MessagesBatchSize);

        //        if (additionalMessages?.Count > 0)
        //        {
        //            var reversedMessages = additionalMessages.AsEnumerable().ToList();

        //            foreach (var message in reversedMessages)
        //            {
        //                Messages.Insert(0, message);
        //            }

        //            _loadedMessagesCount += additionalMessages.Count;
        //        }
        //    }
        //    finally
        //    {
        //        IsLoadingMessages = false;
        //    }
        //}

        //[RelayCommand]
        //private async Task SendMessage()
        //{
        //    if (string.IsNullOrWhiteSpace(MessageText))
        //        return;

        //    try
        //    {
        //        await _hubConnection.InvokeAsync("SendMessage",
        //            SelectedChat?.Id ?? Guid.Empty,
        //            _login,
        //            MessageText);

        //        MessageText = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка отправки: {ex.Message}");
        //    }
        //}

        private async Task<ChatDB> CreateOrGetPrivateChat(ChatDTO selectedUser, Guid currentUserId)
        {
            try
            {
                using var httpClient = new HttpClient();

                var checkResponse = await httpClient.GetAsync(
                    $"https://localhost:7274/api/chats/private-exists?&selectedUserId={selectedUser.Id}&currentUserId={currentUserId}");

                if (checkResponse.IsSuccessStatusCode)
                {
                    var existingChat = await checkResponse.Content.ReadFromJsonAsync<ApiResult<ChatDB>>();
                    if (existingChat.IsSuccess)
                        return existingChat.Data;
                }

                var newChatRequest = new
                {
                    SelectedUser = selectedUser,
                    CurrentUserId = currentUserId,
                };

                var createResponse = await httpClient.PostAsJsonAsync(
                    "https://localhost:7274/api/chats/private",
                    newChatRequest);

                if (!createResponse.IsSuccessStatusCode)
                {
                    var error = await createResponse.Content.ReadAsStringAsync();
                    throw new Exception($"{error}");
                }

                var newChat = await createResponse.Content.ReadFromJsonAsync<ApiResult<ChatDB>>();
                if (newChat.IsSuccess)
                    return newChat.Data;

                return null;
            }
            catch (Exception ex)
            {
                Growl.Warning($"{ex.Message}");
                return null;
            }
        }

        private ChatDB ConvertToDB(ChatDTO db)
        {
            return db != null ? new ChatDB
            {
                Id = db.Id,
                Avatar = db.Avatar,
                Name = db.Name,
                IsGroup = db.IsGroup
            } : null;
        }

        [RelayCommand]
        private async Task AddUser()
        {
            if (_currentUser.Role == "Администратор")
                return;

            UserVM = new UserManagmentViewModel();
            CurrentView = ViewType.UserManagement;
        }

        [RelayCommand]
        private async Task EditUser()
        {
            UserVM = new UserManagmentViewModel(_currentUser);
            CurrentView = ViewType.UserManagement;
        }

        [RelayCommand]
        private void CreateGroup()
        {
            GroupVM = new GroupCreateViewModel(_currentUser.Id);

            CurrentView = ViewType.GroupCreate;
        }

        [RelayCommand]
        private void EditGroup()
        {
            if (SelectedChat == null)
                return;

            if (SelectedChat.Creator != _currentUser.Id)
            {
                Growl.Warning("Вы не являетесь создателем группы");
                return;
            }
                


            GroupVM = new GroupCreateViewModel(SelectedChat, _currentUser.Id);

            CurrentView = ViewType.GroupCreate;
        }
    }
}