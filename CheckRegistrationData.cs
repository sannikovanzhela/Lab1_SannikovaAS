using DataBaseFunctional;
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
        public (string, string) CheckRegistration (string login, string password, string checkPassword)
        {
            string pass, checkPass;
            pass = MaskedPassword(password);
            checkPass = MaskedPassword(checkPassword);


            if (string.IsNullOrEmpty(login))
            {
                if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(checkPassword))
                {
                    Log.Warning("Невозможно зарегистрировать пользователя. Отсутствуют данные необходимые для регистрации.\n" +
                        "Логин: отсутсвует\nПароль: отстутсвует\nПотдверждение пароля: отсутсвует");
                    return ("false", "Введите логин и пароль! Подтвердите пароль!");
                }

                if (string.IsNullOrEmpty(password))
                {
                    Log.Warning($"Невозможно зарегистрировать пользователя. Отсутствуют данные необходимые для регистрации.\n" +
                        $"Логин: отсутсвует\nПароль: отсутсвует\nПодтверждение пароля: {checkPass}");
                    return ("false", "Введите логин и пароль!");
                }

                if (string.IsNullOrEmpty(checkPassword))
                {
                    Log.Warning($"Невозможно зарегистрировать пользователя. Отсутствуют данные необходимые для регистрации.\n" +
                        $"Логин: отсутсвует\nПароль: {pass}\nПотдверждение пароля: отсутствует");
                    return ("false", "Введите логин и подтвердите пароль!");
                }

                Log.Warning($"Невозможно зарегистрировать пользователя. Отсутствуют данные необходимые для регистрации.\n" +
                    $"Логин: отсутствует.\nПароль: {pass}\nПодтверждение пароля:{checkPass}");
                return ("false", "Введите логин!");
            }

            if (string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(checkPassword))
                {
                    Log.Warning($"Невозможно зарегистрировать пользователя. Отсутствуют данные необходимые для регистрации.\n" +
                        $"Логин: {login}\nПароль: отсутствует\nПодтверждение пароля: отсутствует");
                    return ("false", "Введите пароль и подтвердите его!");
                }

                Log.Warning($"Невозможно зарегистрировать пользователя. Отстуствуют данные необходимые для регистрации.\n" +
                    $"Логин: {login}\nПароль: отсутствует\nПодтверждение пароля: {checkPass}");
                return ("false", "Введите пароль!");
            }

            if (string.IsNullOrEmpty(checkPassword))
            {
                Log.Warning($"Невозможно зарегистрировать пользователя. Отстуствуют данные необходимые для регистрации.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: отсутсвует");
                return ("false", "Подтвердите пароль!");
            }

            if (login.Length < 5)
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return ("false", "Логин должен содержать минимум 5 символов");
            }

            if (login.Contains("@") && login.Contains("."))
            {
                if (login.Any(c => char.IsWhiteSpace(c)))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                              $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return ("false", "Неверный формат почты. Почта не должна содержать пробелов. Почта должна быть формата xxx@xxx.xxx");
                }

                if (login.Any(x => char.IsLetter(x) && x >= 1072 && x <= 1103))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                              $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return ("false", "Неверный формат почты. Почта должна содержать только латиницу.");
                }

                if (login.Any(c => char.IsPunctuation(c) && c != '.' && c != '@'))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                                $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return ("false", "Неверный формат почты. Почта не должна содержать знаки препинания.");
                }

                if (login.Any(c => char.IsSymbol(c)))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                                $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return ("false", "Неверный формат почты. Почта не должна содержать символов.");
                }

                if (login.Last() == '.')
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                              $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return ("false", "Неверный формат почты. Почта должна содержать домен почты. Почта должна быть формата xxx@xxx.xxx");
                }
                
                if (login.IndexOf('.') < login.IndexOf('@'))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Неверный формат почты.\n" +
                              $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return ("false", "Неверный формат почты. Домен второго уровня должен стоять поже домена первого уровня. Почта должна быть формата xxx@xxx.xxx");
                }
            }

            if (login.Any(x => char.IsWhiteSpace(x)))
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                   $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return ("false", "Логин не должен содержать пробелов");
            }

            if (login.StartsWith("+7"))
            {
                if (!login.Contains("-"))
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return ("false", "Номер телнфона должен быть в формате +7-xxx-xxx-xxxx");
                }

                if (login.Length > 0 && login.Length < 16)
                {
                    Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                    return ("false", "Номер должен состоять из 11 цифр. Количество цифр меньше 11");
                }
            }

            if (!Regex.IsMatch(login, @"^(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[_]).+$"))
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Логин не соотвествует требованиям.\n" +
                        $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return ("false", "Cтроковый логин должен иметь только латиницу, цифры и знак подчеркивания _");
            }

            if (password.Length < 7)
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Пароль не соответсвует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return ("false", "Пароль должен содержать минимум 7 символов");
            }

            if(password.Any(c => char.IsWhiteSpace(c)))
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Пароль не соответсвует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return ("false", "Пароль не должен содержать пробелов");
            }

            if (!Regex.IsMatch(password, @"^(?=.*[а-я])(?=.*[А-Я])(?=.*\d)(?=.*[@#$%^&+=_.]).+$"))
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Пароль не соответсвует требованиям.\n" +
                    $"Логин: {login}\nПароль: {pass}\nПодтверждение пароля: {checkPass}");
                return ("false", "Пароль должен содержать только кириллицу, цифры и спецсимволы.\nОбязательно присутствие минимум одной буквы в верхнем и нижнем регистре, одной цифры и одного спецсимвола @#$%^&+=_.");
            }

            if (password != checkPassword)
            {
                Log.Error($"Невозможно зарегистрировать пользователя. Пароль не подтвержден.\n" +
                    $"Логин:  {login} \nПароль:  {pass} \nПодтверждение пароля: {checkPass}");
                return ("false", "Пароль и потдверждение пароля не совпадают");
            }

            bool authUser;

            DatabaseRepository db = new DatabaseRepository();

            Log.Debug("Поиск в базе данных пользователя с заданным логином");

            authUser = db.GetByLogin(login);

            if (authUser)
            {
                Log.Error($"Не удалось зарегестрироваться. Пользователь с заданным логином уже существует\n" +
                    $"Логин:  {login} \nПароль:  {pass} \nПодтверждение пароля: {checkPass}");
                return ("false", "Пользователь с таким логином уже существует!");
            }

            return ("True", "");
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
