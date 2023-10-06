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

        public string UserInformation(string login, string password, string checkPassword)
        {
            string pass, checkPass;

            if (string.IsNullOrEmpty(login))
            {
                if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(checkPassword))
                {
                    Log.Warning("Невозможно зарегистрировать пользователя. Отсутствуют данные необходимые для регистрации.\n" +
                        "Логин: отсутсвует\nПароль: отстутсвует\nПотдверждение пароля: отсутсвует");
                    return "Введите логин и пароль! Подтвердите пароль!";
                }
                
                if (string.IsNullOrEmpty(password))
                {
                    checkPass = MaskedPassword(checkPassword);
                    Log.Warning($"Невозможно зарегистрировать пользователя. Отсутствуют данные необходимые для регистрации.\n" +
                        $"Логин: отсутсвует\nПароль: отсутсвует\nПодтверждение пароля: {checkPass}");
                    return "Введите логин и пароль!";
                }
                
                if (string.IsNullOrEmpty(checkPassword))
                {
                    pass = MaskedPassword(password);
                    Log.Warning($"Невозможно зарегистрировать пользователя. Отсутствуют данные необходимые для регистрации.\n" +
                        $"Логин: отсутсвует\nПароль: {pass}\nПотдверждение пароля: отсутствует");
                    return "Введите логин и подтвердите пароль!";
                }

                pass = MaskedPassword(password);
                checkPass = MaskedPassword(checkPassword);
                Log.Warning($"Невозможно зарегистрировать пользователя. Отсутствуют данные необходимые для регистрации.\n" +
                    $"Логин: отсутствует.\nПароль: {pass}\nПодтверждение пароля:{checkPass}");
                return "Введите логин!";
            }
            
            if (string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(checkPassword))
                {
                    Log.Warning($"Невозможно зарегистрировать пользователя. Отсутствуют данные необходимые для регистрации.\n" +
                        $"Логин: {login}\nПароль: отсутствует\nПодтверждение пароля: отсутствует");
                    return "Введите пароль и подтвердите его!";
                }

                checkPass = MaskedPassword(checkPassword);
                Log.Warning($"Невозможно зарегистрировать пользователя. Отстуствуют данные необходимые для регистрации.\n" +
                    $"Логин: {login}\nПароль: отсутствует\nПодтверждение пароля: {checkPass}");
                return "Введите пароль!";
            }
            
            if (string.IsNullOrEmpty(checkPassword))
            {
                pass = MaskedPassword(checkPassword);
                Log.Warning($"Невозможно зарегистрировать пользователя. Отстуствуют данные необходимые для регистрации.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: отсутсвует");
                return "Подтвердите пароль!";
            }

            return null;
        }

        public string LoginVerification(string login, string password, string checkPassword)
        {
            string pass = MaskedPassword(password);
            string checkPass = MaskedPassword(checkPassword);

            if (login.Length < 5)
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return "Логин должен содержать минимум 5 символов";
            }

            if (login.Contains("@") && !(login.Contains(".com") ^ login.Contains(".ru")))
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return "Неверный формат почты. Почта должна быть формата (xxx@xxx.xxx)";
            }

            if (Regex.IsMatch(login, @"^(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[_]).+$"))
            {
                return null;
            }

            if (login.StartsWith("+")) return null;

            Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
            return "Cтроковый логин должен иметь только латиницу, цифры и знак подчеркивания _\n" +
                "Номер телнфона должен быть в формате +7-xxx-xxx-xxxx";

        }

        public string PasswordVerification(string login, string password, string checkPassword)
        {
            string pass = MaskedPassword(password);
            string checkPass = MaskedPassword(checkPassword);

            if (password.Length < 7)
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Пароль не соответсвует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return "Пароль должен содержать минимум 7 символов";
            }
            
            if (!Regex.IsMatch(password, @"^(?=.*[а-я])(?=.*[А-Я])(?=.*\d)(?=.*[@#$%^&+=_.]).+$"))
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Пароль не соответсвует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return "Пароль должен содержать только кириллицу, цифры и спецсимволы.\nОбязательно присутствие минимум одной буквы в верхнем и нижнем регистре, одной цифры и одного спецсимвола @#$%^&+=_.";
            }

            if (password != checkPassword)
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Пароль не подтвержден.\n" +
                    $"Логин:  {login} \nПароль:  {pass} \nПодтверждение пароля: {checkPass}");
                return "Пароль и потдверждение пароля не совпадают";
            }

            return null;
        }

        public string UserValidate(string login, string password, string checkPassword)
        {

            User authUser = null;

            using (ApplicationContext context = new ApplicationContext())
            {
                Log.Debug("Поиск в базе данных пользователя с заданным логином");
                authUser = context.Users.Where(x => x.UserLogin == login).FirstOrDefault();
            }

            if (authUser != null)
            {
                string pass = MaskedPassword(password);
                string checkPass = MaskedPassword(checkPassword);
                Log.Error($"Не удалось зарегестрироваться. Пользователь с заданным логином уже существует\n" +
                    $"Логин:  {login} \nПароль:  {pass} \nПодтверждение пароля: {checkPass}");
                return "Пользователь с таким логином уже существует!";
            }

            return null;
        }


        private void btnSignClick(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = txtPassword.Password;
            string checkPassword = txtCheck.Password;
            string exception;

            exception = UserInformation(login, password, checkPassword);

            if (exception == null)
                exception = UserValidate(login, password, checkPassword);
            if (exception == null)
                exception = LoginVerification(login, password, checkPassword);
            if (exception == null)
                exception = PasswordVerification(login, password, checkPassword);

            if (exception == null)
            {

                Log.Debug("Добавление пользователя в базу данных");

                User user = new User(login, password);
                db.Users.Add(user);
                db.SaveChanges();

                string pass = MaskedPassword(password);
                string checkPass = MaskedPassword(checkPassword);

                MessageBox.Show("True");
                Log.Information($"Успешная регистрация\nЛогин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
            }
            else
            {
                MessageBox.Show($"False\n{exception}");
            }
        }

        public string MaskedPassword(string password)
        {
            string pass;

            using (MD5CryptoServiceProvider maskPass = new MD5CryptoServiceProvider())
            {
                UTF8Encoding uTF8 = new UTF8Encoding();
                byte[] bytes = maskPass.ComputeHash(uTF8.GetBytes(password));
                pass = Convert.ToBase64String(bytes);
            }

            return pass;
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
