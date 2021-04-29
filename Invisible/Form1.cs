using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Invisible
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern Int32 GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern UInt32 GetWindowThreadProcessId(Int32 hWnd, out Int32 lpdwProcessId);

        const string ERROR_TEXT = "Отсутствует подключение к серверу";
        const string OK_TEXT = "Соединение установлено";

        public static Thread lookAtThread;
        public static string nameComp = "";

        public static string address = "127.0.0.1";
        public const int port = 8888;

        public static TcpClient client;
        public Form1()
        {
            InitializeComponent();
            //получение имени пользователя
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            ManagementObjectCollection collection = searcher.Get();
            nameComp = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
            nameComp = nameComp.Substring(nameComp.IndexOf("\\") + 1);
            userNameLabel.Text = nameComp;

            //создание и запуск процесса-шпиона
            lookAtThread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Int32 handle = GetForegroundWindow();
                    Int32 ProcessID;
                    GetWindowThreadProcessId(handle, out ProcessID);
                    Process ActiveProcess = Process.GetProcessById(ProcessID);

                    if (!ActiveProcess.ProcessName.Equals(processFocusNameLabel.Text) && this.InvokeRequired)
                        this.Invoke(new Action(() => { OnChangeProcess?.Invoke(ActiveProcess.ProcessName); }));
                    Thread.Sleep(1000);
                }
            }));
            lookAtThread.Start();

            //регистрация обработчика на смену процесса
            OnChangeProcess += OnChangeProcessHandler;

            //Создание и открытие соединения с сервером
            ToConnectServer();
        }




        #region Logic
        private void OnChangeProcessHandler(string processName)
        {
            processFocusNameLabel.Text = processName;

            try
            {
                if (client != null)
                {
                    NetworkStream stream = client.GetStream();

                    Package package = new Package()
                    {
                        Name = nameComp,
                        DateTime = DateTime.Now,
                        Operation = Operation.ChangeProcess,
                        Data = processName
                    };

                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.Unicode.GetBytes(package.ToJson());

                    // отправка сообщения
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage();
            }

        }

        private void ShowErrorMessage()
        {
            //TODO
            MessageBox.Show("Отсутствует соединение с сервером", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Show();
            WindowState = FormWindowState.Normal;

            errorLabel.Visible = true;
            errorLabel.Text = ERROR_TEXT;
            errorLabel.ForeColor = Color.Red;
            errorBtn.Visible = true;

            if (client != null)
            {
                client.Close();
                client = null;
            }

        }
        private void ToConnectServer()
        {
            client = null;
            try
            {
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();

                Package package = new Package() { Name = nameComp, DateTime = DateTime.Now, Operation = Operation.Connect, Data = null };
                // преобразуем сообщение в массив байтов
                byte[] data = Encoding.Unicode.GetBytes(package.ToJson());
                // отправка сообщения
                stream.Write(data, 0, data.Length);
                errorLabel.Visible = true;
                errorLabel.Text = OK_TEXT;
                errorLabel.ForeColor = Color.Green;
                errorBtn.Visible = false;

            }
            catch (Exception ex)
            {
                ShowErrorMessage();
            }
        }
        #endregion
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {

                Hide();
                notifyIcon1.BalloonTipTitle = "Программа была спрятана";
                notifyIcon1.BalloonTipText = "Обратите внимание что программа была спрятана в трей и продолжит свою работу.";
                notifyIcon1.ShowBalloonTip(2000); // Параметром указываем количество миллисекунд, которое будет показываться 
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            lookAtThread.Abort();
            if (client != null)
            {
                try
                {
                    NetworkStream stream = client.GetStream();

                    Package package = new Package() { Name = nameComp, DateTime = DateTime.Now, Operation = Operation.Exit, Data = null };
                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.Unicode.GetBytes(package.ToJson());
                    // отправка сообщения
                    stream.Write(data, 0, data.Length);


                }
                catch (Exception ex)
                {

                    if (client != null)
                    {
                        if (client.Connected)
                            if (client.GetStream() != null)
                                client.GetStream().Close();
                        client.Close();
                    }
                }
            }
        }


        delegate void ChangeProcess(string processName);
        event ChangeProcess OnChangeProcess;

        private void button1_Click(object sender, EventArgs e)
        {
            ToConnectServer();
        }
    }
}
