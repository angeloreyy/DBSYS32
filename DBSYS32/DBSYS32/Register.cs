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
//using BCrypt.Net;

namespace DBSYS32
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                //Data Source=.\sqlexpress;Initial Catalog=Inventory;Integrated Security=True
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-CFPO8DC\\SQLEXPRESS;Initial Catalog=Inventory;Integrated Security=True");
                con.Open();

                string password = txtPassword.Text;
                string confirmpass = txtConfirmpass.Text;

                // Check if password matches confirm password
                if (password == confirmpass)
                {
                    // Check if username already exists
                    string checkUsernameQuery = "SELECT COUNT(*) FROM signup WHERE username = @username";
                    SqlCommand checkUsernameCmd = new SqlCommand(checkUsernameQuery, con);
                    checkUsernameCmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    int existingUsernameCount = (int)checkUsernameCmd.ExecuteScalar();

                    if (existingUsernameCount > 0)
                    {
                        MessageBox.Show("Username already exists. Please choose a different username.", "Registration Fail", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        // Generate new ID starting from C-001
                        string newIdQuery = "SELECT 'C-' + RIGHT('000' + CAST(ISNULL(MAX(RIGHT(ID, 3)), 0) + 1 AS VARCHAR(3)), 3) FROM signup";
                        SqlCommand newIdCmd = new SqlCommand(newIdQuery, con);
                        string newId = (string)newIdCmd.ExecuteScalar();

                        // Insert new account with generated ID
                        string hashedpass = Convert.ToBase64String(Encoding.UTF8.GetBytes(txtPassword.Text));
                        string insertQuery = "INSERT INTO [signup] ([ID], [firstname], [lastname], [email], [address], [username], [password]) " +
                                             "VALUES (@ID, @firstname, @lastname, @email, @address, @username, @password)";
                        SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                        insertCmd.Parameters.AddWithValue("@ID", newId);
                        insertCmd.Parameters.AddWithValue("@firstname", txtFirstname.Text);
                        insertCmd.Parameters.AddWithValue("@lastname", txtLastname.Text);
                        insertCmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        insertCmd.Parameters.AddWithValue("@address", txtAddress.Text);
                        insertCmd.Parameters.AddWithValue("@username", txtUsername.Text);
                        insertCmd.Parameters.AddWithValue("@password", hashedpass);
                        insertCmd.ExecuteNonQuery();

                        MessageBox.Show("Account successfully registered with ID: " + newId, "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearfields();
                    }
                }
                else
                {
                    txtConfirmpass.Clear();
                    MessageBox.Show("Password mismatch", "Registration Fail", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void clearfields()
        {
            txtFirstname.Clear();
            txtLastname.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            txtConfirmpass.Clear();
        }
        private bool IsValidEmail(string email)
        {
            // Check if the email contains "@" and "."
            if (!email.Contains("@") || !email.Contains("."))
                return false;

            // Get the index of "@" and "."
            int atIndex = email.IndexOf("@");
            int dotIndex = email.LastIndexOf(".");

            // Ensure "@" comes before "."
            if (atIndex >= dotIndex)
                return false;

            // Ensure there are characters between "@" and "."
            if (dotIndex - atIndex <= 1)
                return false;

            // Ensure there are characters after "."
            if (dotIndex == email.Length - 1)
                return false;

            return true;
        }
        private void btnBacktoLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login l = new Login();
            l.Show();
            this.Hide();
        }
    }
}
