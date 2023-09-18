using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
using System.Xml.Linq;
using Xceed.Wpf.Toolkit;
using Serilog;
using System.Text.RegularExpressions;

namespace RegistrationApp_Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("Log/LogFile.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
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

        public void UserInformation()
        {
            if (string.IsNullOrEmpty(txtLogin.Text))
            {
                Log.Information("Логин не введен");
                throw new Exception("Введите логин!");
            }
            else if (txtPassword.Password == null)
            {
                Log.Information("Пароль не введен");
                throw new Exception("Введите пароль!");
            }
            else if (txtCheck.Password == null)
            {
                Log.Information("Пароль не был введен повторно");
                throw new Exception("Повторите пароль!");
            }
            else if (string.IsNullOrEmpty(txtLogin.Text) && txtPassword.Password == null) {
                Log.Information("Пароль и логин не введены");
                throw new Exception("Логин и пароль не введены");
            }
            else if (string.IsNullOrEmpty(txtLogin.Text) && txtCheck.Password == null)
            {
                Log.Information("Логин не введен и пароль не был введен повторно");
                throw new Exception("Логин не введен и пароль не был введен повторно");
            }
            else if (txtPassword.Password == null && txtCheck.Password == null)
            {
                Log.Information("Пароль не введен и пароль не был введен повторно");
                throw new Exception("Пароль не введен и пароль не был введен повторно");
            }
        }

        public void ValidateUser()
        {
            if (txtLogin.Text.Length < 5)
            {
                Log.Information("Слишком маленький логин");
                throw new Exception("Логин должен содержать минимум 5 символов");
            }

            if (Regex.IsMatch(txtLogin.ToString(), @"^[a-zA-Z0-9_]+$"))
            {
                return;
            }

            if (txtLogin.Text.Contains("@"))
            {
                if (!(txtLogin.Text.Contains(".com") ^ txtLogin.Text.Contains(".ru")))
                {
                    Log.Information("Неверный формат почты");
                    throw new Exception("Неверный формат почты!");
                    
                }
            }

            //if (txtPassword.Password.Length < 7 /*|| !Regex.IsMatch(txtPassword.ToString(), @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=_.]).+$")*/)
            //{
            //    Log.Information("Пароль не соответсвует требованиям");
            //    throw new Exception("Пароль должен содержать минимум 7 символов и включать хотя бы одну букву в верхнем и нижнем регистре, одну цифру и один спецсимволыы @#$%^&+=_.");
            //}

            if (txtPassword.ToString() != txtCheck.ToString())
            {
                Log.Information("Пароли не совпадают");
                throw new Exception("Пароли не совпадают");
            }
        }


        private void btnSignClick(object sender, RoutedEventArgs e)
        {

            try
            {
                //UserInformation();
                //ValidateUser();


                string pass = txtPassword.Password;
                System.Windows.MessageBox.Show("True");
                Log.Information("Логин: " + txtLogin.Text + "\nПароль: " + pass + "\nПодтверждение пароля: " + txtCheck.ToString() + "\nУспешная регистрация.");
            }
            catch
            {

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
