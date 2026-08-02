using System.Windows;

namespace MobileShop
{

    /// <summary>
    /// Interaction logic for MobilePhoneUpdateWindow.xaml
    /// </summary>
    public partial class MobilePhoneUpdateWindow : Window
    {
        public MobilePhoneUpdateModel MobilePhone { get; set; }

        public MobilePhoneUpdateWindow(MobilePhoneUpdateModel mobilePhone)
        {
            InitializeComponent();
            MobilePhone = mobilePhone;
            DataContext = MobilePhone;
        }

        void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        void OpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                MobilePhone.ImagePath = dialog.FileName;
            }
        }
    }
}
