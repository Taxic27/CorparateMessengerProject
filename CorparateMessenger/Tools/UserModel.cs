using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorparateMessenger.Tools
{
    public class UserModel : ObservableObject
    {
        public Guid Id { get; set; }
        public byte[] Avatar { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string CurrentPosition { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string FullName => $"{Surname} {Name}".Trim();
    }
}
