using Server.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class InvokeClient
    {
        public string name { get; set; }
        public TcpClient TcpClient { get; set; }
        public InvokeClient(TcpClient tcpClient)
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

                    AddInfoToDB(package);

                    switch (package.Operation)
                    {
                        case (int)OperationInvoke.Connect:
                            {
                                name = package.Name;
                                LogPrint(package.DateTime, "вход в систему");
                                break;
                            }
                        case (int)OperationInvoke.ChangeProcess:
                            {
                                LogPrint(package.DateTime, $"смена процесса {package.Data}");
                                break;
                            }
                        case (int)OperationInvoke.Exit:
                            {
                                LogPrint(DateTime.Now, "выход из системы");
                                isExit = true;
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

        private void AddInfoToDB(Package package)
        {
            using (DBContext context = new DBContext())
            {
                Employee emp = context.Employees.FirstOrDefault(employee => employee.user_name.Equals(package.Name));//null

                Log last_log = context.Logs.Where(l => l.id_employee == emp.id).OrderByDescending(l => l.id).ToArray()[0];
                if(last_log.operation==1)
                    last_log.time = Convert.ToInt32((package.DateTime - last_log.datetime).TotalSeconds);

                Log log = new Log()
                {
                    employee = emp,
                    operation = package.Operation,
                    datetime = package.DateTime,
                    name_process = package.Data,
                    time = 0
                };


                context.Logs.Add(log);
                try
                {
                    context.SaveChanges();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[Invoke][{name} SQL Error] [{DateTime.Now}] {ex}");
                }
            }
        }

        private void LogPrint(DateTime date, string message)
        {
            Console.WriteLine($"[Invoke][{name}] [{date.ToString("dd-MM-yyyy hh:mm:ss")}] {message}");
        }
    }
}
