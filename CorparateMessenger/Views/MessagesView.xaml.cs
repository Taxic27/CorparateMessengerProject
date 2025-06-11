using CorparateMessenger.Tools;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CorparateMessenger.Views
{
    /// <summary>
    /// Логика взаимодействия для MessagesView.xaml
    /// </summary>
    public partial class MessagesView : UserControl
    {
        public MessagesView()
        {
            InitializeComponent();
        }

        private void ListBoxChat_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.Tag is MessageModel message)
            {
                new ImageBrowser(new Uri(message.FileUrl.ToString()))
                {
                    Owner = WindowHelper.GetActiveWindow()
                }.Show();
            }
        }
    }
}
