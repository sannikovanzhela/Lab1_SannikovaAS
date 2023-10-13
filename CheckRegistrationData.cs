using RegistrationApp_Test.Database;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegistrationApp_Test
{
    public class CheckRegistrationData
    {
        ApplicationContext db;
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

            if (login.Contains("@") && login.Contains("."))
            {
                if (login.Any(c => char.IsWhiteSpace(c)))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                              $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return "Неверный формат почты. Почта не должна содержать пробелов. Почта должна быть формата xxx@xxx.xxx";
                }

                if (login.Any(x => char.IsLetter(x) && x >= 1072 && x <= 1103))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                              $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return "Неверный формат почты. Почта должна содержать только латиницу.";
                }

                if (login.Any(c => char.IsPunctuation(c) && c != '.' && c != '@'))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                                $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return "Неверный формат почты. Почта не должна содержать знаки препинания.";
                }

                if (login.Any(c => char.IsSymbol(c)))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                                $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return "Неверный формат почты. Почта не должна содержать символов.";
                }

                if (login.Last() == '.')
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                              $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return "Неверный формат почты. Почта должна содержать домен почты. Почта должна быть формата xxx@xxx.xxx";
                }
                
                if (login.IndexOf('.') < login.IndexOf('@'))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                              $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return "Неверный формат почты. Домен второго уровня должен стоять поже домена первого уровня. Почта должна быть формата xxx@xxx.xxx";
                }
            }

            if (login.Any(x => char.IsWhiteSpace(x)))
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                   $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return "Логин не должен содержать пробелов";
            }

            if (login.StartsWith("+7"))
            {
                if (!login.Contains("-"))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return "Номер телнфона должен быть в формате +7-xxx-xxx-xxxx";
                }

                if (login.Length > 0 && login.Length < 16)
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return "Номер должен состоять из 11 цифр. Количество цифр меньше 11";
                }
            }

            if (Regex.IsMatch(login, @"^(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[_]).+$")) return null;

            Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
            return "Cтроковый логин должен иметь только латиницу, цифры и знак подчеркивания _";

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

            if(password.Any(c => char.IsWhiteSpace(c)))
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Пароль не соответсвует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return "Пароль не должен содержать пробелов";
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
            db = new ApplicationContext();
            db.Users.Load();

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
    }
}
