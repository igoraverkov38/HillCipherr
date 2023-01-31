using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography;

namespace HillCipher
{
    class Database
    {
        private string dbPath = @"users.db";//путь к БД                       
        public Database()
        {
            if (!File.Exists(dbPath))//Проверка существования файла БД
            {
                Initialize();
            }
        }
        //Если не существует - создаем
        private void Initialize()
        {
            File.Create(dbPath).Close();
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", dbPath));
            //Если находим таблицу, то удаляем ее, чтобы не было ошибок перезаписи
            SQLiteCommand command = new SQLiteCommand("DROP TABLE IF EXISTS users; CREATE TABLE usersInfo(id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT,password TEXT);", connection);
            connection.Open();
            command.ExecuteNonQuery(); //запись в БД
            connection.Close();
        }
        public bool Valid(string username, string password)
        {
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", dbPath));
            connection.Open();
            //проходим по всем значениям в БД
            SQLiteCommand command1 = new SQLiteCommand("SELECT * FROM 'usersInfo'; ", connection);
            SQLiteDataReader reader = command1.ExecuteReader(); //заносим в reader все значения
            string tempUsername = "";
            string tempPassword = "";
            foreach (DbDataRecord record in reader) //перебор значений поочереди из reader
            {
                tempUsername = record["username"].ToString();
                tempPassword = record["password"].ToString();
                //проверка соответствия введенных данных с данными из БД
                if (username == tempUsername && password == tempPassword)
                {
                    connection.Close();
                    return true;
                }
            }
            connection.Close();
            return false;
        }
        public bool Exists(string username)
        {
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0}; ", dbPath));
            connection.Open();
            SQLiteCommand command1 = new SQLiteCommand("SELECT * FROM 'usersInfo'; ", connection);
            SQLiteDataReader reader = command1.ExecuteReader();
            string tempUsername;
            foreach (DbDataRecord record in reader)
            {
                tempUsername = record["username"].ToString();
                if (username == tempUsername)
                {
                    connection.Close();
                    return true;
                }
            }
            connection.Close();
            return false;
        }
        public void Create(string username, string password)
        {
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0}; ", dbPath));
            connection.Open();
            //добавляем в таблицу значения ..., которые равны ...
            SQLiteCommand command = new SQLiteCommand("INSERT INTO 'usersInfo' ('username', 'password') VALUES(@username, @password); ", connection);
            command.Parameters.Add("@username", DbType.String).Value = username;
            command.Parameters.Add("@password", DbType.String).Value = password;
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    class User
    {
        string username, password, conf;
        Database database = new Database();
        public User(string username, string password, string confirm = null)
        {
            Username = username;
            Password = password;
            Conf = confirm;
        }
        // Проверяет правильность введенных данных в форме входа
        public bool CorrectUser()
        {
            if (String.IsNullOrEmpty(username))
                throw new Exception("Поле для имени пользователя пусто.");
            if (String.IsNullOrEmpty(password))
                throw new Exception("Поле для пароля пусто.");
            if (username.Length < 8)
                throw new Exception("Длина имени пользователя меньше 8 символов.");
            if (password.Length < 8)
                throw new Exception("Длина пароля меньше 8 символов.");
            if (conf != null && password != conf)
                throw new Exception("Пароль и подтверждение пароля не совпадают.");
            return true;
        }
        //хэширование функции
        private string getHash()
        {
            using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes("new key")))
            {
                return
               Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
        //создание пользователя
        public void CreateUser()
        {
            database.Create(username, getHash());
        }
        public bool UsernameExist()
        {
            return database.Exists(username);
        }
        public bool UserValid()
        {
            return database.Valid(username, getHash()); //передается имя и зашифрованный  пароль
        }
        //Свойства переменных
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Conf { get => conf; set => conf = value; }
    }
}