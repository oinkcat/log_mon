using System;
using System.Windows;
using System.Windows.Input;

namespace LogMon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowHelpers
    {
        private const int ErrorCodeAnyError = 10;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void ToggleLoadingState(bool loading)
        {
            this.Cursor = loading ? Cursors.Wait : Cursors.Arrow;
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
