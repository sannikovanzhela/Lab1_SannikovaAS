using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using System.Collections;

namespace DataBaseFunctional
{
    public class DatabaseRepository
    {
        private readonly string connectionString;
        public DatabaseRepository()
        {
            connectionString = "Data Source = LAPTOP-1BQG2FKL\\SQLEXPRESS; Initial Catalog = UsersDB; Integrated Security=true;";
        }

        #region table Users
        public bool GetByLogin(string login)
        {
            bool flag;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "select UserLogin from Users where UserLogin =@login";
                    command.Parameters.Add("@login", SqlDbType.NVarChar).Value = login;
                    flag = command.ExecuteScalar() == null ? false : true;
                    connection.Close();
                }
            }

            return flag;
        }

        public void AddUser(string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "insert into Users values (@login, @password)";
                    command.Parameters.Add("@login", SqlDbType.NVarChar).Value = login;
                    command.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                    connection.Close();
                }
            }
        }
        #endregion


        #region table RegistrationResult
        public string[] GetData(string login, string password, string checkPass )
        {
            string[] data;

            if (string.IsNullOrEmpty(login))
                return null;
            if (string.IsNullOrEmpty(password))
                return null;
            if (string.IsNullOrEmpty(checkPass))
                return null;

            bool flag;
            string result, message;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "select UserLogin from RegistrationResult where UserLogin = @login and UserPassword = @pass and CheckPassword = @check";
                    command.Parameters.Add("@login", SqlDbType.NVarChar).Value = login;
                    command.Parameters.Add("@pass", SqlDbType.NVarChar).Value = password;
                    command.Parameters.Add("@check", SqlDbType.NVarChar).Value = checkPass;
                    flag = command.ExecuteScalar() == null ? false : true;

                    if (!flag) return null;

                    command.CommandText = "select Result from RegistrationResult where UserLogin = @login and UserPassword = @pass and CheckPassword = @check";
                    result = command.ExecuteScalarAsync().Result.ToString();

                    command.CommandText = "select Message from RegistrationResult where UserLogin = @login and UserPassword = @pass and CheckPassword = @check";
                    message = command.ExecuteScalarAsync().Result.ToString();

                    connection.Close();
                }
            }

            data = new string[4];
            data[0] = login;
            data[1] = password;
            data[2] = checkPass;
            data[3] = result;
            data[4] = message;

            return data;
        }

        public string AddData(string login, string password, string checkPass, bool result, string message)
        {
            if (string.IsNullOrEmpty(login))
                return "login is empty";
            if (string.IsNullOrEmpty(password))
                return "password is empty";
            if (string.IsNullOrEmpty(checkPass))
                return "check password is empty";
            if (string.IsNullOrEmpty(message))
                return "message is empty";

            bool flag;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "select UserLogin from RegistrationResult where UserLogin = @login and UserPassword = @pass and CheckPassword = @check";
                    command.Parameters.Add("@login", SqlDbType.NVarChar).Value = login;
                    command.Parameters.Add("@pass", SqlDbType.NVarChar).Value = password;
                    command.Parameters.Add("@check", SqlDbType.NVarChar).Value = checkPass;
                    flag = command.ExecuteScalar() == null ? false : true;

                    if (flag) return "data is exist";

                    command.CommandText = "insert into RegistrationResult values (@login, @password, @check, @result, @message)";
                    command.Parameters.Add("@result", SqlDbType.Bit).Value = result;
                    command.Parameters.Add("@message", SqlDbType.NVarChar).Value = message;
                    connection.Close();
                }
            }

            return "Data is added";
        }

        public string DeleteData(string login, string password, string checkPass)
        {
            if (string.IsNullOrEmpty(login))
                return "login is empty";
            if (string.IsNullOrEmpty(password))
                return "password is empty";
            if (string.IsNullOrEmpty(checkPass))
                return "check password is empty";

            bool flag;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "select UserLogin from RegistrationResult where UserLogin = @login and UserPassword = @pass and CheckPassword = @check";
                    command.Parameters.Add("@login", SqlDbType.NVarChar).Value = login;
                    command.Parameters.Add("@pass", SqlDbType.NVarChar).Value = password;
                    command.Parameters.Add("@check", SqlDbType.NVarChar).Value = checkPass;
                    flag = command.ExecuteScalar() == null ? false : true;

                    if (!flag) return "data isn't exist";

                    command.CommandText = "delete from RegistrationResult where UserLogin = @login and UserPassword = @pass and CheckPassword = @check";
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }

            return "User is deleted";

        }

        #endregion
    }
}
