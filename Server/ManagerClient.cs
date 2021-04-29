using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ManagerClient
    {
        private const string CONNECTION_STRING = @"Data Source=DESKTOP-L4A77DQ;Initial Catalog=LogEmployees;Integrated Security=True";
        public string name { get; set; }
        public TcpClient TcpClient { get; set; }
        public ManagerClient(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        public void Process()
        {
            name = "aaa";
            NetworkStream stream = null;
            bool isExit = false;
            try
            {
                stream = TcpClient.GetStream();
                byte[] data = new byte[64]; // буфер для получаемых данных
                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string json = builder.ToString();

                    if (string.IsNullOrEmpty(json))
                    {
                        break;
                    }

                    Package package = Package.FromJson(json);

                    switch (package.Operation)
                    {
                        case (int)OperationManager.Connect:
                            {
                                name = package.Name;
                                LogPrint(package.DateTime, "вход в систему");
                                break;
                            }
                        case (int)OperationManager.GetUsers:
                            {
                                GetUsers(package);
                                LogPrint(package.DateTime, $"получение списка сотрудников");
                                break;
                            }
                        case (int)OperationManager.GetTeams:
                            {
                                GetTeams(package);
                                LogPrint(package.DateTime, $"получение списка команд");
                                break;
                            }
                        case (int)OperationManager.GetUserInfo:
                            {
                                LogPrint(package.DateTime, $"получение лога сотрудника id={package.Data.Split('@')[0]}");
                                GetUserInfo(package);
                                break;
                            }
                        case (int)OperationManager.Exit:
                            {
                                LogPrint(DateTime.Now, "выход из системы");
                                isExit = true;
                                break;
                            }
                        case (int)OperationManager.GetLogsTeam:
                            {
                                LogPrint(package.DateTime, $"получение лога команды id={package.Data.Split('@')[0]}");
                                GetLogsTeam(package);
                                break;
                            }
                    }

                    if (isExit)
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (!isExit)
                {
                    LogPrint(DateTime.Now, "выход из системы (отключение)");
                }
                if (stream != null)
                    stream.Close();
                if (TcpClient != null)
                    TcpClient.Close();
            }
        }

        private void GetLogsTeam(Package package)
        {
            List<Log> logs = new List<Log>();
            byte[] data;
            int id = int.Parse(package.Data.Split('@')[0]);
            DateTime start = DateTime.Parse(package.Data.Split('@')[1]);
            DateTime end = DateTime.Parse(package.Data.Split('@')[2]).AddDays(0);

            using (DBContext context = new DBContext())
            {
                logs = context.Logs.Include(t => t.employee.team).Where(l=>l.employee.team.id_teams==id 
                    && l.datetime>start && l.datetime<end
                ).ToList();
                package.Data = Log.ToJsonList(logs);
                string json = package.ToJson();
                data = Encoding.Unicode.GetBytes(json);
                TcpClient.GetStream().Write(data, 0, data.Length);
            }
        }

        private void GetTeams(Package package)
        {
            List<Team> teams = new List<Team>();
            byte[] data;
            using (DBContext context = new DBContext())
            {
                teams = context.Teams.Include(t => t.employees).ToList();
                package.Data = Team.ToJsonList(teams);
                string json = package.ToJson();
                data = Encoding.Unicode.GetBytes(json);
                TcpClient.GetStream().Write(data, 0, data.Length);

            }

        }

        private void GetUsers(Package package)
        {
            List<User> users = new List<User>();

            using (DBContext context = new DBContext())
            {
                users = context.Users.ToList();
                package.Data = User.ToJsonList(users);
                string json = package.ToJson();
                byte[] data = Encoding.Unicode.GetBytes(json);
                TcpClient.GetStream().Write(data, 0, data.Length);
            }


        }
        private void GetUserInfo(Package package)
        {
            string[] args = package.Data.Split('@');
            List<Log> logs = new List<Log>();
            using (DBContext context = new DBContext())
            {
                int id_emp = int.Parse(args[0]);
                DateTime startDate = DateTime.Parse(args[1]);
                DateTime endDate = DateTime.Parse(args[2]);
                logs = context.Logs.Where(l => l.id_employee == id_emp && l.datetime >= startDate && l.datetime <= endDate).ToList();
                package.Data = Log.ToJsonList(logs);
                string json = package.ToJson();
                byte[] data = Encoding.Unicode.GetBytes(json);
                TcpClient.GetStream().Write(data, 0, data.Length);
            }



        }
        private void LogPrint(DateTime date, string message)
        {
            Console.WriteLine($"[Manager][{name}] [{date.ToString("dd-MM-yyyy hh:mm:ss")}] {message}");
        }
    }
}
