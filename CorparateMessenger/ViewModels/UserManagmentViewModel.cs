using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using Microsoft.Win32;

namespace CorparateMessenger.ViewModels
{
    public partial class UserManagmentViewModel : ObservableObject
    {
        [ObservableProperty]
        private byte[] _avatar;

        public UserManagmentViewModel()
        {
            SetDefaultAvatar();
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
