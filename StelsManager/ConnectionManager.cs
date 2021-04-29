using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Management;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StelsManager
{
    class ConnectionManager
    {
        public static ConnectionManager Instance = new ConnectionManager();
        private const int port = 8887;
        private const string ip_addres = "127.0.0.1";
        public static string name = "";
        private TcpClient TcpClient;
        private NetworkStream Stream;
        public void Connect()
        {
            //получение имени пользователя
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            ManagementObjectCollection collection = searcher.Get();
            name = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
            name = name.Substring(name.IndexOf("\\") + 1);

            TcpClient = new TcpClient(ip_addres, port);
            Stream = TcpClient.GetStream();

            Package package = new Package()
            {
                Name = name,
                DateTime = DateTime.Now,
                Operation = Operation.Connect,
                Data = null,
            };

            // преобразуем сообщение в массив байтов
            byte[] data = Encoding.Unicode.GetBytes(package.ToJson());

            // отправка сообщения
            Stream.Write(data, 0, data.Length);

        }

        internal List<Log> GetLogRecordPeriod(Team team, DateTime startDate, DateTime endDate)
        {
            Package package = new Package()
            {
                Name = name,
                DateTime = DateTime.Now,
                Operation = Operation.GetLogsTeam,
                Data = $"{team.id_teams}@{startDate}@{endDate}",
            };

            // преобразуем сообщение в массив байтов
            byte[] data = Encoding.Unicode.GetBytes(package.ToJson());

            // отправка сообщения
            Stream.Write(data, 0, data.Length);


            data = new byte[64]; // буфер для получаемых данных

            // получаем сообщение
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            string json = builder.ToString();
            package = Package.FromJson(json);
            return Log.FromJsonArray(package.Data).ToList();
        }

        internal Team[] GetTeams()
        {
            Package package = new Package()
            {
                Name = name,
                DateTime = DateTime.Now,
                Operation = Operation.GetTeams,
                Data = null,
            };

            // преобразуем сообщение в массив байтов
            byte[] data = Encoding.Unicode.GetBytes(package.ToJson());

            // отправка сообщения
            Stream.Write(data, 0, data.Length);


            data = new byte[64]; // буфер для получаемых данных

            // получаем сообщение
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            string json = builder.ToString();
            package = Package.FromJson(json);
            return Team.FromJsonArray(package.Data);
        }

        public User[] GetUsers()
        {
            Package package = new Package()
            {
                Name = name,
                DateTime = DateTime.Now,
                Operation = Operation.GetUsers,
                Data = null,
            };

            // преобразуем сообщение в массив байтов
            byte[] data = Encoding.Unicode.GetBytes(package.ToJson());

            // отправка сообщения
            Stream.Write(data, 0, data.Length);


            data = new byte[64]; // буфер для получаемых данных

            // получаем сообщение
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            string json = builder.ToString();
            package = Package.FromJson(json);
            return User.FromJsonArray(package.Data);
        }
        public Log[] GetLogRecordPeriod(User user,DateTime startDate,DateTime endDate)
        {
            Package package = new Package()
            {
                Name = name,
                DateTime = DateTime.Now,
                Operation = Operation.GetUserInfo,
                Data = $"{user.Id}@{startDate}@{endDate}",
            };

            // преобразуем сообщение в массив байтов
            byte[] data = Encoding.Unicode.GetBytes(package.ToJson());

            // отправка сообщения
            Stream.Write(data, 0, data.Length);


            data = new byte[64]; // буфер для получаемых данных

            // получаем сообщение
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            string json = builder.ToString();
            package = Package.FromJson(json);
            return Log.FromJsonArray(package.Data);
        }

    }
}
