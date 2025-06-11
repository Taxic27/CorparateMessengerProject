using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Server.Tools;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Windows;
using Server.Models.User;
using CorparateMessenger.Tools;
using CorparateMessenger.Views;
using System.Net.Http.Json;

namespace CorparateMessenger.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://localhost:7274/api/users/login";

        [ObservableProperty]
        private string username = "";

        [ObservableProperty]
        private string password = "";

        [ObservableProperty]
        private string errorMessage = "";

        [ObservableProperty]
        private bool hasError = false;

        [ObservableProperty]
        private bool isLoading = false;

        [RelayCommand]
        private async Task Login()
        {
            ErrorMessage = string.Empty;
            HasError = false;

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage += "Введите логин!\n";
                HasError = true;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage += "Введите пароль!\n";
                HasError = true;
            }

            if (HasError)
                return;

            try
            {
                IsLoading = true;
                HasError = false;

                var request = new
                {
                    Login = Username,
                    Password = Password
                };

                var response = await _httpClient.PostAsJsonAsync(ApiUrl, request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<UserDB>>();

                    if (result.IsSuccess)
                    {
                        var chatWindow = new ChatView
                        {
                            DataContext = new ChatViewModel(result.Data)
                        };
                        chatWindow.Show();

                        Application.Current.Windows.OfType<LoginView>().FirstOrDefault()?.Close();
                    }
                    else
                    {
                        ErrorMessage = result.ErrorsAsString;
                        HasError = true;
                    }
                }
                else
                {
                    HasError = true;
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}