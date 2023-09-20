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

        public void UserInformation()
        {
            if (string.IsNullOrEmpty(txtLogin.Text))
            {
                Log.Warning("Невозможно зарегистрировать пользователя. Отстуствуют данные необходимые для регистрации. Логин не введен");
                throw new Exception("Введите логин!");
            }
            else if (txtPassword.Password == null)
            {
                Log.Warning("Невозможно зарегистрировать пользователя. Отстуствуют данные необходимые для регистрации. Пароль не введен");
                throw new Exception("Введите пароль!");
            }
            else if (txtCheck.Password == null)
            {
                Log.Warning("Невозможно зарегистрировать пользователя. Отстуствуют данные необходимые для регистрации. Пароль не был введен повторно");
                throw new Exception("Повторите пароль!");
            }
            else if (string.IsNullOrEmpty(txtLogin.Text) && txtPassword.Password == null) {
                Log.Warning("Невозможно зарегистрировать пользователя. Отстуствуют данные необходимые для регистрации.Пароль и логин не введены");
                throw new Exception("Введите логин и пароль!");
            }
            else if (string.IsNullOrEmpty(txtLogin.Text) && txtCheck.Password == null)
            {
                Log.Warning("Невозможно зарегистрировать пользователя. Отстуствуют данные необходимые для регистрации. Логин не введен и пароль не был введен повторно");
                throw new Exception("Введите логин и повторите пароль!");
            }
            else if (txtPassword.Password == null && txtCheck.Password == null)
            {
                Log.Warning("Невозможно зарегистрировать пользователя. Отстуствуют данные необходимые для регистрации. Пароль не введен и пароль не был введен повторно");
                throw new Exception("Введите пароль и повторите его!");
            }
        }

        public void UserVerification()
        {
            if (txtLogin.Text.Length < 5)
            {
                Log.Error("Невозможно зарегистрировать пользователя. Данные не соотвествуют требованиям. Слишком маленький логин");
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
                    Log.Error("Невозможно зарегистрировать пользователя. Данные не соотвествуют требованиям. Неверный формат почты");
                    throw new Exception("Неверный формат почты. Почта должна быть формата (xxx@xxx.xxx)");
                    
                }
            }

            if (txtPassword.Password.Length < 7)
            {
                Log.Error("Невозможно зарегистрировать пользователя. Данные не соотвествуют требованиям. Пароль не соответсвует требованиям");
                throw new Exception("Пароль должен содержать минимум 7 символов");
            }
            else if (!Regex.IsMatch(txtPassword.Password, @"^(?=.*[а-я])(?=.*[А-Я])(?=.*\d)(?=.*[@#$%^&+=_.]).+$"))
            {
                Log.Error("Невозможно зарегистрировать пользователя. Данные не соотвествуют требованиям. Пароль не соответсвует требованиям");
                throw new Exception("Пароль должен содержать только кириллицу, цифры и спецсимволы.\nОбязательно присутствие минимум одной буквы в верхнем и нижнем регистре, одной цифры и одного спецсимвола @#$%^&+=_.");
            }

            if (txtPassword.Password != txtCheck.Password)
            {
                Log.Error("Невозможно зарегистрировать пользователя. Пароль не подтвержден");
                throw new Exception("Пароль и потдверждение пароля не совпадают");
            }
        }

        public void UserValidate(string login)
        {
            User authUser = null;

            using (ApplicationContext context = new ApplicationContext())
            {
                Log.Debug("Поиск в базе данных пользователя с заданным логином");
                authUser = context.Users.Where(x => x.UserLogin == login).FirstOrDefault();
            }

            if (authUser != null)
            {
                Log.Error("Не удалось зарегестрироваться. Пользователь с заданным логином уже существует");
                throw new Exception("Пользователь с таким логином уже существует!");
            }
        }


        private void btnSignClick(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();
            string check = txtCheck.Password.Trim();

            try
            {
                UserInformation();
                UserValidate(login);
                UserVerification();

                Log.Debug("Добавление пользователя в базу данных");
                User user = new User(txtLogin.Text.Trim(), txtPassword.Password.Trim());
                db.Users.Add(user);
                db.SaveChanges();

                string pass, checkPass;

                using (MD5CryptoServiceProvider maskPass = new MD5CryptoServiceProvider())
                {
                    UTF8Encoding uTF8 = new UTF8Encoding();
                    byte[] bytes = maskPass.ComputeHash(uTF8.GetBytes(password));
                    pass = Convert.ToBase64String(bytes);
                }

                using (MD5CryptoServiceProvider maskPass = new MD5CryptoServiceProvider())
                {
                    UTF8Encoding uTF8 = new UTF8Encoding();
                    byte[] bytes = maskPass.ComputeHash(uTF8.GetBytes(check));
                    checkPass = Convert.ToBase64String(bytes);
                }


                MessageBox.Show("True");
                Log.Information($"Успешная регистрация\nЛогин:{login}\nПароль:{pass}\nПодтверждение пароля:{checkPass}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"False\n{ex.Message}");
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
