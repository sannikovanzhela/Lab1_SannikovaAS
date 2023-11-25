using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Serilog;
using System.Text.RegularExpressions;
using RegistrationApp_Test.Database;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using DataBaseFunctional;

namespace RegistrationApp_Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public interface IView
    {
        void ShowResult((string,string) result);
        event Action<(string, string, string)> runCheck;
    }

    public partial class RegistrationWindow : Window, IView
    {

        DatabaseRepository db = new DatabaseRepository();
        public RegistrationWindow()
        {
            InitializeComponent();
            
            // в папке bin/Debug/Log
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "Logs/Log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Verbose("Логгер сконфигурирован");
            Log.Information("Приложение запущено");
        }

        #region txt

        private void textLoginMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtLogin.Focus();
        }

        private void txtLoginTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLogin.Text) && txtLogin.Text.Length > 0)
            {
                textLogin.Visibility = Visibility.Collapsed;

                if (txtLogin.Text.StartsWith("8") || txtLogin.Text.StartsWith("7") || txtLogin.Text.StartsWith("+"))
                    txtLogin.Mask = "+7-000-000-0000";
            }
            else
            {
                textLogin.Visibility = Visibility.Visible;
            }
        }
        private void textPasswordMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword.Focus();
        }

        private void txtPasswordTextChanged(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password != null && txtPassword.Password.Length > 0)
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }

        private void textСheckMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtCheck.Focus();
        }

        private void txtCheckTextChanged(object sender, RoutedEventArgs e)
        {
            if (txtCheck.Password != null && txtCheck.Password.Length > 0)
            {
                textCheck.Visibility = Visibility.Collapsed;

            }
            else
            {
                textCheck.Visibility = Visibility.Visible;
            }
        }
        #endregion

        private void btnLoginMessage(object sender, RoutedEventArgs e)
        {
            btnLogin.Visibility = Visibility.Collapsed;
            btnLoginCollapsed.Visibility = Visibility.Visible;
            LoginMessage.Visibility = Visibility.Visible;
        }

        private void btnPasswordMessage(object sender, RoutedEventArgs e)
        {
            btnPassword.Visibility = Visibility.Collapsed;
            btnPasswordCollapsed.Visibility = Visibility.Visible;
            PasswordMessage.Visibility = Visibility.Visible;
        }


        public event Action<(string, string, string)> runCheck;
        private void btnSignClick(object sender, RoutedEventArgs e)
        {
            runCheck.DynamicInvoke(txtLogin.Text, txtPassword.Password, txtCheck.Password);
        }

        public void ShowResult((string,string) result)
        {
            MessageBox.Show(result.Item1, result.Item2);
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnLoginCollapsedClick(object sender, RoutedEventArgs e)
        {
            btnLogin.Visibility = Visibility.Visible;
            btnLoginCollapsed.Visibility = Visibility.Collapsed;
            LoginMessage.Visibility = Visibility.Collapsed;
        }

        private void btnPasswordCollapsedClick(object sender, RoutedEventArgs e)
        {
            btnPassword.Visibility = Visibility.Visible;
            btnPasswordCollapsed.Visibility = Visibility.Collapsed;
            PasswordMessage.Visibility = Visibility.Collapsed;
        }

        private void txtLoginKeyUp(object sender, KeyEventArgs e)
        {
            if(txtLogin.Text.StartsWith("+"))
            {
                if ((e.Key == Key.Delete) || (e.Key == Key.Back))
                {
                    e.Handled = true;
                    txtLogin.Mask = string.Empty;
                    txtLogin.Text = string.Empty;
                    textLogin.Focus();
                }
            }
        }
    }
}
