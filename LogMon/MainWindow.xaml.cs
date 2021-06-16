using System;
using System.Windows;

namespace LogMon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUserAlerter
    {
        private const int ErrorCodeAnyError = 10;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowErrorAndExit(Exception e)
        {
            var btnOk = MessageBoxButton.OK;
            var imgError = MessageBoxImage.Error;

            MessageBox.Show(this, e.ToString(), "Error occured", btnOk, imgError);

            this.Close();
            Application.Current.Shutdown(ErrorCodeAnyError);
        }
    }
}
