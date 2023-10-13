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

namespace RegistrationApp_Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        ApplicationContext db;
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

            db = new ApplicationContext();
            db.Users.Load();
            this.DataContext = db.Users.Local.ToBindingList();
            Log.Information("Получение данных из базы данных");

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



        private void btnSignClick(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = txtPassword.Password;
            string checkPassword = txtCheck.Password;
            string exception;

            CheckRegistrationData check = new CheckRegistrationData();

            exception = check.UserInformation(login, password, checkPassword);

            if (exception == null)
                exception = check.UserValidate(login, password, checkPassword);
            if (exception == null)
                exception = check.LoginVerification(login, password, checkPassword);
            if (exception == null)
                exception = check.PasswordVerification(login, password, checkPassword);

            if (exception == null)
            {

                Log.Debug("Добавление пользователя в базу данных");

                User user = new User(login, password);
                db.Users.Add(user);
                db.SaveChanges();

                string pass = check.MaskedPassword(password);
                string checkPass = check.MaskedPassword(checkPassword);

                MessageBox.Show("True");
                Log.Information($"Успешная регистрация\nЛогин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
            }
            else
            {
                MessageBox.Show($"False\n{exception}");
            }
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
