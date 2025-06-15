using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CorparateMessenger.Tools;
using HandyControl.Controls;
using Microsoft.Win32;
using Server.Models.User;
using Server.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;

namespace CorparateMessenger.ViewModels
{
    public partial class UserManagmentViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _surname;

        [ObservableProperty]
        private string _patronymic;

        [ObservableProperty]
        private string _currentPosition;

        [ObservableProperty]
        private byte[] _avatar;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private bool _isCreateMode;

        public string Password;

        private UserDB _currentUser;
        private readonly HttpClient _httpClient;

        public UserManagmentViewModel()
        {
            _httpClient = new HttpClient();
            IsCreateMode = true;
            IsEditMode = false;
            SetDefaultAvatar();
        }

        public UserManagmentViewModel(UserDB CurrentUser)
        {
            _currentUser = CurrentUser;
            _httpClient = new HttpClient();
            IsCreateMode = false;
            IsEditMode = true;

            Name = _currentUser.Name;
            Surname = _currentUser.Surname;
            Patronymic = _currentUser.Patronymic;
            CurrentPosition = _currentUser.CurrentPosition;
            Avatar = _currentUser.Avatar;
        }

        [RelayCommand]
        private async Task CreateUser()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Surname) || string.IsNullOrWhiteSpace(CurrentPosition) || string.IsNullOrWhiteSpace(Username)
                || string.IsNullOrWhiteSpace(Password))
            {
                Growl.Warning("Заполните обязательные поля");
                return;
            }

            try
            {
                var request = new
                {
                    Username = Username,
                    Password = Password,
                    Name = Name,
                    Surname = Surname,
                    Patronymic = Patronymic,
                    CurrentPosition = CurrentPosition,
                    Avatar = Avatar
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "https://localhost:7274/api/users/create",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<Guid>>();
                    if (result?.IsSuccess == true)
                    {
                        Growl.Success("Пользователь успешно создан");
                        ClearForm();
                        WeakReferenceMessenger.Default.Send(new UserUpdatedMessage());
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
                Growl.Error($"{ex.Message}");
            }
        }

        [RelayCommand]
        private async Task EditUser()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Surname) || string.IsNullOrWhiteSpace(CurrentPosition))
            {
                Growl.Warning("Заполните обязательные поля (Имя и Фамилия)");
                return;
            }

            try
            {
                var request = new
                {
                    UserId = _currentUser.Id,
                    Name = Name,
                    Surname = Surname,
                    Patronymic = Patronymic,
                    CurrentPosition = CurrentPosition,
                    Avatar = Avatar
                };

                var response = await _httpClient.PutAsJsonAsync(
                    "https://localhost:7274/api/users/update",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>();
                    if (result?.IsSuccess == true)
                    {
                        Growl.Success("Изменения успешно сохранены");
                        WeakReferenceMessenger.Default.Send(new UserUpdatedMessage());
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
                Growl.Error($"{ex.Message}");
            }
        }

        [RelayCommand]
        private void SelectAvatar()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите аватар пользователя"
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

        private void ClearForm()
        {
            Name = string.Empty;
            Username = string.Empty;
            Surname = string.Empty;
            Patronymic = string.Empty;
            CurrentPosition = string.Empty;
            Password = string.Empty;
            SetDefaultAvatar();
        }

        [RelayCommand]
        private void PasswordChanged(PasswordBox passwordBox)
        {
            Password = passwordBox.Password;
        }
    }
}