using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CorparateMessenger.Tools;
using HandyControl.Controls;
using Microsoft.Win32;
using Server.Models.Chat;
using Server.Models.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CorparateMessenger.ViewModels
{
    public partial class GroupCreateViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _groupName;

        [ObservableProperty]
        private byte[] _avatar;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private bool _isCreateMode;

        public ChatDTO _selectedChat;

        public ObservableCollection<UserModel> Users { get; } = new();

        private readonly Guid _currentUserId;
        private readonly HttpClient _httpClient;

        public GroupCreateViewModel(Guid currentUserId)
        {
            _currentUserId = currentUserId;
            _httpClient = new HttpClient();

            IsEditMode = false;
            IsCreateMode = true;

            LoadUsers();
            SetDefaultAvatar();
        }

        public GroupCreateViewModel(ChatDTO selectedChat, Guid currentUserId)
        {
            _selectedChat = selectedChat;
            _currentUserId = currentUserId;
            _httpClient = new HttpClient();

            IsEditMode = true;
            IsCreateMode = false;

            GroupName = selectedChat.Name;
            Avatar = selectedChat.Avatar;

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await LoadUsers();
            await LoadGroupUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7274/api/users/get-users?currentUserId={_currentUserId}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<List<UserDTO>>>();

                    if (result?.IsSuccess == true)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Users.Clear();
                            foreach (var user in result.Data)
                            {
                                Users.Add(new UserModel
                                {
                                    Id = user.Id,
                                    Name = user.Name,
                                    Surname = user.Surname,
                                    Patronymic = user.Patronymic,
                                    CurrentPosition = user.CurrentPosition,
                                    Avatar = user.Avatar,
                                    IsSelected = false
                                });
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка загрузки пользователей: {ex.Message}");
            }
        }

        private async Task LoadGroupUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"https://localhost:7274/api/chats/alluserchats-group?chatId={_selectedChat.Id}");

                if (response.IsSuccessStatusCode)
                {
                    var membersResult = await response.Content.ReadFromJsonAsync<ApiResult<List<Guid>>>();
                    if (membersResult?.IsSuccess == true)
                    {
                        var memberIds = new HashSet<Guid>(membersResult.Data);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            foreach (var user in Users)
                            {
                                user.IsSelected = memberIds.Contains(user.Id);
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка загрузки участников группы: {ex.Message}");
            }
        }


        [RelayCommand]
        private async Task CreateGroup()
        {
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                Growl.Warning("Введите название группы");
                return;
            }

            var selectedUserIds = Users
                .Where(u => u.IsSelected)
                .Select(u => u.Id)
                .ToList();

            if (selectedUserIds.Count < 2)
            {
                Growl.Warning("Выберите хотя бы двух участников");
                return;
            }

            try
            {
                selectedUserIds.Add(_currentUserId);

                var request = new 
                {
                    GroupName = GroupName,
                    Avatar = Avatar,
                    MemberIds = selectedUserIds
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "https://localhost:7274/api/chats/createchat-group",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<Guid>>();
                    if (result?.IsSuccess == true)
                    {
                        Growl.Success("Группа успешно создана");
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Growl.Error($"Ошибка сервера: {error}");
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task EditGroup()
        {
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                Growl.Warning("Введите название группы");
                return;
            }

            var selectedUserIds = Users
                .Where(u => u.IsSelected)
                .Select(u => u.Id)
                .ToList();

            if (selectedUserIds.Count < 1)
            {
                Growl.Warning("Необходимо оставить хотя бы одного участника");
                return;
            }

            try
            {
                selectedUserIds.Add(_currentUserId);

                var request = new
                {
                    ChatId = _selectedChat.Id,
                    GroupName = GroupName,
                    Avatar = Avatar,
                    MemberIds = selectedUserIds
                };

                var response = await _httpClient.PutAsJsonAsync(
                    "https://localhost:7274/api/chats/updatechat-group", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>();
                    if (result?.IsSuccess == true)
                    {
                        Growl.Success("Изменения успешно сохранены");
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Growl.Error($"{error}");
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"Ошибка: {ex.Message}");
            }
        }

        [RelayCommand]
        private void SelectAvatar()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите аватар группы"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Avatar = File.ReadAllBytes(openFileDialog.FileName);
                OnPropertyChanged(nameof(Avatar));
            }
            else
            {
                SetDefaultAvatar();
            }
        }

        private void SetDefaultAvatar()
        {

            string avatarPath = Path.Combine("Assets", "Default_Avatar.jpg");

            string fullPath = Path.Combine(Environment.CurrentDirectory, avatarPath);

            Avatar = File.ReadAllBytes(fullPath);

            OnPropertyChanged(nameof(Avatar));
        }
    }
}
