using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DBSYS32
{
    public partial class Login : Form
    {
        public static string user;
        public Login() 
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try {
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-CFPO8DC\\SQLEXPRESS;Initial Catalog=Inventory;Integrated Security=True");
                con.Open();
                
                if (String.IsNullOrEmpty(txtUsername.Text))
                {
                    errorProviderLogin.SetError(txtUsername, "Empty Field!");
                    return;
                }
                if (String.IsNullOrEmpty(txtPassword.Text))
                {
                    errorProviderLogin.SetError(txtPassword, "Empty Field!");
                    return;
                }

                if (txtUsername.Text != "admin" && txtPassword.Text != "admin")
                {
                    string query = "SELECT * FROM signup WHERE username = @username";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string dbPassword = reader["password"].ToString();
                                string hashInputPWD = Convert.ToBase64String(Encoding.UTF8.GetBytes(txtPassword.Text));

                                if (dbPassword == hashInputPWD)
                                {
                                    MessageBox.Show("Login successful!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    user = txtUsername.Text;
                                    timer1.Enabled = true;
                                    timer1.Start();
                                    timer1.Interval = 1;
                                    ProgressBarLogin.Maximum = 200;
                                    timer1.Tick += new EventHandler(timer1_Tick);
                                }
                                else
                                {
                                    // User does not exist or credentials are incorrect
                                    MessageBox.Show("Invalid Account!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            else
                            {
                                // Username doesn't exist
                                MessageBox.Show("Username not found!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    MessageBox.Show("Login successful!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    user = txtUsername.Text;
                    TimerLogin.Enabled = true;
                    TimerLogin.Start();
                    TimerLogin.Interval = 1;
                    ProgressBarLogin.Maximum = 200;
                    TimerLogin.Tick += new EventHandler(timer1_Tick);
                }
                con.Close();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void TimerLogin_Tick(object sender, EventArgs e)
        {
            if (ProgressBarLogin.Value != 200)
            {
                ProgressBarLogin.Value++;
            }
            else
            {
                TimerLogin.Stop();
                ProgressBarLogin.Value = 0;
                Menu m = new Menu();
                m.Show();
                this.Hide();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ProgressBarLogin.Value != 200)
            {
                ProgressBarLogin.Value++;
            }
            else
            {
                timer1.Stop();
                ProgressBarLogin.Value = 0;
                Store s = new Store();
                s.Show();
                this.Hide();
            }
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            Register r = new Register();
            r.Show();
            this.Hide();
        }
    }
}
