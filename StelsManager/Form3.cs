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
    public partial class Form3 : Form
    {
        Team Team;
        List<Log> logs = new List<Log>();
        public Form3(Team team)
        {
            InitializeComponent();
            Team = team;

            empName.Text = team.team_name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime start = dateTimePicker1.Value;
            DateTime end = dateTimePicker2.Value;
            if (start.Date > end.Date)
            {
                MessageBox.Show("Неверные даты");
            }
            else
            {
                try
                {
                    logs = ConnectionManager.Instance.GetLogRecordPeriod(Team, dateTimePicker1.Value, dateTimePicker2.Value);

                    CalculateWorkPO();
                    CalculateWorkPOSr();
                    CalculateWorkOnDay();
                    CalculateWorkOnDaySr();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка");
                }
            }
        }

        private void CalculateWorkOnDaySr()
        {
            workOnDaySrChart.Series.Clear();

            Dictionary<string, Dictionary<string, double>> dataOnDay = new Dictionary<string,Dictionary<string, double>>();
            List<User> users = DataContainer.Instance.Users.ToList();

            User oldUser = null;
            int oldId = -1;
            foreach (Log log in logs)
            {
                User user = oldId == log.IdEmp ? oldUser : users.FirstOrDefault(u => u.Id == log.IdEmp);

                if (!dataOnDay.Keys.Contains(user.ToString()))
                {
                    dataOnDay.Add(user.ToString(), new Dictionary<string, double>());

                    for (DateTime start = dateTimePicker1.Value; start <= dateTimePicker2.Value; start = start.AddDays(1))
                    {
                        dataOnDay[user.ToString()].Add(start.ToShortDateString(), 0);
                    }
                }

                if ((int)log.Operation == (int)User.OperationUser.ChangeProcess)
                {
                    bool isInstall = false;
                    DataContainer.Instance.GetKeyRecordGroup(log, out isInstall);

                    double hour = log.time / (3600.0 * Team.count_emp);

                    string date = log.DateTime.ToShortDateString();

                    if (isInstall)
                    {
                        dataOnDay[user.ToString()][date] += hour;
                    }
                }
            }
            foreach (string key in dataOnDay.Keys)
            {
                workOnDaySrChart.Series.Add(key);
                int i = workOnDaySrChart.Series.Count - 1;
                workOnDaySrChart.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                workOnDaySrChart.Series[i].Points.DataBindXY(dataOnDay[key].Keys, dataOnDay[key].Values);
            }
            

            workOnDaySrChart.ChartAreas[0].AxisX.Interval = 1;
        }

        private void CalculateWorkOnDay()
        {
            Dictionary<string, double> dataOnDayAll = new Dictionary<string, double>();
            Dictionary<string, double> dataOnDay = new Dictionary<string, double>();

            for (DateTime start = dateTimePicker1.Value; start <= dateTimePicker2.Value; start = start.AddDays(1))
            {
                dataOnDayAll.Add(start.ToShortDateString(), 0);
                dataOnDay.Add(start.ToShortDateString(), 0);
            }
            foreach (Log logRecord in logs)
            {
                if ((int)logRecord.Operation == (int)User.OperationUser.ChangeProcess)
                {
                    bool isInstall = false;
                    DataContainer.Instance.GetKeyRecordGroup(logRecord, out isInstall);

                    double hour = logRecord.time / 3600.0;

                    string date = logRecord.DateTime.ToShortDateString();

                    dataOnDayAll[date] += hour;
                    if (isInstall)
                    {
                        dataOnDay[date] += hour;
                    }
                }
            }

            workOnDayChart.Series[1].Points.DataBindXY(dataOnDayAll.Keys, dataOnDayAll.Values);
            workOnDayChart.Series[0].Points.DataBindXY(dataOnDay.Keys, dataOnDay.Values);

            workOnDayChart.ChartAreas[0].AxisX.Interval = 1;
        }

        private void CalculateWorkPOSr()
        {
            workPOSrChart.Series.Clear();

            List<string> keys = comboBox1.DataSource as List<String>;
            Dictionary<string, Dictionary<string, double>> u_install = new Dictionary<string, Dictionary<string, double>>();

            List<User> users = DataContainer.Instance.Users.ToList();


            User oldUser = null;
            int oldId = -1;
            foreach (Log log in logs)
            {
                User user = oldId==log.IdEmp? oldUser: users.FirstOrDefault(u => u.Id == log.IdEmp);

                if (!u_install.Keys.Contains(user.ToString()))
                {
                    u_install.Add(user.ToString(), new Dictionary<string, double>());

                    foreach (string key in keys)
                    {
                        u_install[user.ToString()].Add(key, 0);
                    }
                }


                if ((int)log.Operation == (int)User.OperationUser.ChangeProcess)
                {
                    bool isInstall = false;
                    string key = DataContainer.Instance.GetKeyRecordGroup(log, out isInstall);

                    double hour = log.time / 3600.0;
                    u_install[user.ToString()][key] += hour;
                }

                oldUser = user;
                oldId = user.Id;
            }


            foreach (string key in u_install.Keys)
            {
                workPOSrChart.Series.Add(key);
                int i = workPOSrChart.Series.Count - 1;
                workPOSrChart.Series[i].Points.DataBindXY(u_install[key].Keys, u_install[key].Values);
            }


            workPOSrChart.ChartAreas[0].AxisX.Interval = 1;
        }

        private void CalculateWorkPO()
        {
            Dictionary<string, double> install = new Dictionary<string, double>();
            Dictionary<string, double> notInstall = new Dictionary<string, double>();

            foreach (Log logRecord in logs)
            {
                if ((int)logRecord.Operation == (int)User.OperationUser.ChangeProcess)
                {
                    bool isInstall = false;
                    string key = DataContainer.Instance.GetKeyRecordGroup(logRecord, out isInstall);

                    double hour = logRecord.time / 3600.0;

                    if (install.ContainsKey(key))
                    {
                        install[key] += isInstall ? hour : 0;
                    }
                    else
                    {
                        install.Add(key, isInstall ? hour : 0);
                    }

                    if (notInstall.ContainsKey(key))
                    {
                        notInstall[key] += !isInstall ? hour : 0;
                    }
                    else
                    {
                        notInstall.Add(key, !isInstall ? hour : 0);
                    }
                }
            }


            workPOChart.Series[0].Points.DataBindXY(install.Keys, install.Values);
            workPOChart.Series[1].Points.DataBindXY(notInstall.Keys, notInstall.Values);

            workPOChart.ChartAreas[0].AxisX.Interval = 1;

            List<string> procceses = install.Keys.ToList<string>();

            comboBox1.DataSource = procceses;
            comboBox1.SelectedIndex = 0;
        }
    }
}
