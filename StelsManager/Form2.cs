using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StelsManager
{
    public partial class Form2 : Form
    {
        private User user;
        Log[] logs;

        Dictionary<string, double> dataInstall = new Dictionary<string, double>();
        Dictionary<string, double> dataNotInstall = new Dictionary<string, double>();

        Dictionary<string, double> dataOnDayAll = new Dictionary<string, double>();
        Dictionary<string, double> dataOnDay = new Dictionary<string, double>();

        Dictionary<string, string[]> groupProcess = DataContainer.GroupProcess;
        public Form2(User user)
        {
            InitializeComponent();
            hourLabel.Text = "";
            this.user = user;
            empName.Text = user.FullName;
            normaLabel.Text = user.Norma.ToString();

            dateTimePicker1.MaxDate = DateTime.Now;
            dateTimePicker2.MaxDate = DateTime.Now;

            comboBox2.DataSource = DataContainer.Instance.Users.Where(u => u.Id != user.Id).ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime start = dateTimePicker1.Value.Date;
            DateTime end = dateTimePicker2.Value.Date;

            if (start > end)
            {
                MessageBox.Show("Неверные даты");
            }
            else
            {
                try
                {
                    logs = ConnectionManager.Instance.GetLogRecordPeriod(user, start, end.AddDays(1));


                    if (logs.Length > 0)
                    {
                        CalculateTime();
                    }
                    else
                    {
                        MessageBox.Show("Ничего не найдено");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        }
        private void CalculateTime()
        {
            double sum = 0;
            dataInstall = new Dictionary<string, double>();
            dataNotInstall = new Dictionary<string, double>();

            dataOnDayAll = new Dictionary<string, double>();
            dataOnDay = new Dictionary<string, double>();

            DateTime start = dateTimePicker1.Value.Date;
            DateTime end = dateTimePicker2.Value.Date.AddDays(1);

            for (DateTime s = dateTimePicker1.Value; s <= dateTimePicker2.Value; s = s.AddDays(1))
            {
                dataOnDayAll.Add(s.ToShortDateString(), 0);
                dataOnDay.Add(s.ToShortDateString(), 0);
            }

            foreach (Log logRecord in logs)
            {
                if ((int)logRecord.Operation == (int)User.OperationUser.ChangeProcess)
                {
                    bool isInstall = false;
                    string key = DataContainer.Instance.GetKeyRecordGroup(logRecord, out isInstall);

                    double hour = logRecord.time / 3600.0;

                    if (dataInstall.ContainsKey(key))
                    {
                        dataInstall[key] += isInstall ? hour : 0;
                    }
                    else
                    {
                        dataInstall.Add(key, isInstall ? hour : 0);
                    }

                    if (dataNotInstall.ContainsKey(key))
                    {
                        dataNotInstall[key] += !isInstall ? hour : 0;
                    }
                    else
                    {
                        dataNotInstall.Add(key, !isInstall ? hour : 0);
                    }

                    string date = logRecord.DateTime.ToShortDateString();

                    dataOnDayAll[date] += hour;
                    if (isInstall)
                    {
                        dataOnDay[date] += hour;
                    }

                    sum += hour;
                }
            }

            hourLabel.Text = Math.Round(sum, 4).ToString();

            chart1.Series[0].Points.DataBindXY(dataInstall.Keys, dataInstall.Values);
            chart1.Series[1].Points.DataBindXY(dataInstall.Keys, dataNotInstall.Values);

            empWorkOnDay.Series[1].Points.DataBindXY(dataOnDayAll.Keys, dataOnDayAll.Values);
            empWorkOnDay.Series[0].Points.DataBindXY(dataOnDay.Keys, dataOnDay.Values);

            chart1.ChartAreas[0].AxisX.Interval = 1;
            empWorkOnDay.ChartAreas[0].AxisX.Interval = 1;

            List<string> procceses = dataInstall.Keys.ToList<string>();

            comboBox1.DataSource = procceses;
            comboBox1.SelectedIndex = 0;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                //todo сообщение об ошибке MessageBox.Show("ОШИБКА");
            }
            else
            {
                string process = comboBox1.Items[comboBox1.SelectedIndex].ToString();

                Dictionary<string, double> data = new Dictionary<string, double>();

                DateTime d1 = dateTimePicker1.Value, d2 = dateTimePicker2.Value;

                while (d1 <= d2)
                {
                    data.Add(d1.ToString("dd-MM-yyyy"), 0);
                    d1 = d1.AddDays(1);
                }

                if (logs.Length > 0)
                {
                    for (int i = 0; i < logs.Length - 1; i++)
                    {
                        Log log = logs[i];
                        Log next = logs[i + 1];


                        if ((int)log.Operation == (int)User.OperationUser.ChangeProcess)
                        {
                            if (log.DateTime.Day < next.DateTime.Day)
                            {
                                continue;
                            }

                            double seconds = (next.DateTime - log.DateTime).TotalSeconds;

                            string key = log.NameProcess;

                            foreach (string keyD in groupProcess.Keys)
                            {
                                if (groupProcess[keyD].Contains(key))
                                {
                                    key = keyD;
                                    break;
                                }
                            }
                            if (key == process)
                            {
                                key = log.DateTime.ToString("dd-MM-yyyy");
                                data[key] += seconds / 60.0;
                            }


                        }
                    }

                    chart2.Series[0].Points.DataBindXY(data.Keys, data.Values);
                    chart2.ChartAreas[0].AxisX.Interval = 1;
                    chart2.Series["Series1"]["PointWidth"] = "1";

                }
                else
                {
                    MessageBox.Show("Ничего не найдено");
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                MessageBox.Show("Неверные даты");
            }
            else
            {
                try
                {
                    User u = (comboBox2.DataSource as List<User>)[comboBox2.SelectedIndex];
                    Log[] logs = ConnectionManager.Instance.GetLogRecordPeriod(u, dateTimePicker1.Value, dateTimePicker2.Value);


                    if (logs.Length > 0)
                    {
                        CalculateTimeSUser(logs,u.FullName);
                    }
                    else
                    {
                        MessageBox.Show("Ничего не найдено");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        }

        private void CalculateTimeSUser(Log[] logs,string nameUser)
        {
            Dictionary<string, double> dataInstallUser1 = new Dictionary<string, double>();
            Dictionary<string, double> dataNotInstallUser1 = new Dictionary<string, double>();
            Dictionary<string, double> dataInstallUser2 = new Dictionary<string, double>();
            Dictionary<string, double> dataNotInstallUser2 = new Dictionary<string, double>();

            Dictionary<string, double> dataOnDayAllUser1 = new Dictionary<string, double>();
            Dictionary<string, double> dataOnDayUser1 = new Dictionary<string, double>();


            for (DateTime start = dateTimePicker1.Value; start <= dateTimePicker2.Value; start = start.AddDays(1))
            {
                dataOnDayAllUser1.Add(start.ToShortDateString(), 0);
                dataOnDayUser1.Add(start.ToShortDateString(), 0);
            }

            List<string> keys = comboBox1.DataSource as List<string>;
            foreach (string  i in keys)
            {
                dataInstallUser1.Add(i, 0);
                dataNotInstallUser1.Add(i, 0);
                dataInstallUser2.Add(i, 0);
                dataNotInstallUser2.Add(i, 0);
            }
            foreach (Log logRecord in logs)
            {
                if ((int)logRecord.Operation == (int)User.OperationUser.ChangeProcess)
                {
                    bool isInstall = false;
                    string key = DataContainer.Instance.GetKeyRecordGroup(logRecord, out isInstall);

                    if (!keys.Contains(key))
                        keys.Add(key);

                    double hour = logRecord.time / 3600.0;

                    if (dataInstallUser1.ContainsKey(key))
                    {
                        dataInstallUser1[key] += isInstall ? hour : 0;
                    }
                    else
                    {
                        dataInstallUser1.Add(key, isInstall ? hour : 0);
                        dataInstallUser2.Add(key, 0);
                    }

                    if (dataNotInstallUser1.ContainsKey(key))
                    {
                        dataNotInstallUser1[key] += !isInstall ? hour : 0;
                    }
                    else
                    {
                        dataNotInstallUser1.Add(key, !isInstall ? hour : 0);
                        dataNotInstallUser2.Add(key,0);
                    }

                    string date = logRecord.DateTime.ToShortDateString();

                    dataOnDayAllUser1[date] += hour;
                    if (isInstall)
                    {
                        dataOnDayUser1[date] += hour;
                    }
                }
            }

            foreach (string k in dataInstall.Keys)
            {
                if (dataInstallUser2.Keys.Contains(k))
                {
                    dataInstallUser2[k] += dataInstall[k];
                    dataNotInstallUser2[k] += dataNotInstall[k];
                }
                else
                {
                    dataNotInstallUser2.Add(k, dataNotInstall[k]);
                    dataInstallUser2.Add(k, dataInstall[k]);
                    dataNotInstallUser1.Add(k, 0);
                    dataInstallUser1.Add(k, 0);
                }
            }


            sUserProcessChart.Series[0].Points.DataBindXY(dataInstallUser1.Keys, dataInstallUser1.Values);
            sUserProcessChart.Series[2].Points.DataBindXY(dataNotInstallUser1.Keys, dataNotInstallUser1.Values);
            sUserProcessChart.Series[0].Name = $"Допустимые ({nameUser})";
            sUserProcessChart.Series[2].Name = $"Сторонние ({nameUser})";
            sUserProcessChart.Series[1].Points.DataBindXY(dataInstallUser2.Keys, dataInstallUser2.Values);
            sUserProcessChart.Series[3].Points.DataBindXY(dataNotInstallUser2.Keys, dataNotInstallUser2.Values);
            sUserProcessChart.Series[1].Name = $"Допустимые ({user.FullName})";
            sUserProcessChart.Series[3].Name = $"Сторонние ({user.FullName})";

            sUserDayChart.Series[1].Points.DataBindXY(dataOnDayAllUser1.Keys, dataOnDayAllUser1.Values);
            sUserDayChart.Series[0].Points.DataBindXY(dataOnDayUser1.Keys, dataOnDayUser1.Values);
            sUserDayChart.Series[0].Name = $"Общее ({nameUser})";
            sUserDayChart.Series[1].Name = $"Полезное ({nameUser})";
            sUserDayChart.Series[2].Points.DataBindXY(dataOnDayAll.Keys, dataOnDayAll.Values);
            sUserDayChart.Series[3].Points.DataBindXY(dataOnDay.Keys, dataOnDay.Values);
            sUserDayChart.Series[2].Name = $"Общее ({user.FullName})";
            sUserDayChart.Series[3].Name = $"Полезное ({user.FullName})";

            sUserProcessChart.ChartAreas[0].AxisX.Interval = 1;
            sUserDayChart.ChartAreas[0].AxisX.Interval = 1;

        }
    }
}
