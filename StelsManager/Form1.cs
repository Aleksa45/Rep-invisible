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
    public partial class Form1 : Form
    {
        const string ERROR_TEXT = "Отсутствует подключение к серверу";
        const string OK_TEXT = "Соединение установлено";

        private DataContainer _container = DataContainer.Instance;
        public Form1()
        {
            InitializeComponent();
            ToConnect();
            
        }

        private void LoadData()
        {
             _container.Users = ConnectionManager.Instance.GetUsers();
            _container.Teams = ConnectionManager.Instance.GetTeams();

            User[] users = _container.Users;
            Team[] teams = _container.Teams;
            dataGridView1.Rows.Clear();
            foreach(User user in users)
            {
                dataGridView1.Rows.Add(user.GetArrayValues());
            }
            foreach (Team t in teams)
            {
                dataGridView2.Rows.Add(t.GetArrayValues());
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.CurrentRow != null)
            {
                int id = int.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                new Form2(_container.Users.FirstOrDefault(u=> u.Id==id)).ShowDialog();
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            errorLabel.Visible = true;
            errorLabel.Text = ERROR_TEXT;
            errorLabel.ForeColor = Color.Red;
            errorBtn.Visible = true;
        }

        private void errorBtn_Click(object sender, EventArgs e)
        {
            ToConnect();
        }

        private void ToConnect()
        {
            try
            {
                ConnectionManager.Instance.Connect();
                userNameLabel.Text = ConnectionManager.name;
                errorLabel.Text = OK_TEXT;
                errorLabel.ForeColor = Color.Green;
                errorLabel.Visible = true;
                errorBtn.Visible = false;
                LoadData();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.CurrentRow != null)
            {
                int id = int.Parse(dataGridView2.CurrentRow.Cells[0].Value.ToString());

                new Form3(_container.Teams.FirstOrDefault(t=>t.id_teams==id)).ShowDialog();
            }
        }
    }
}
