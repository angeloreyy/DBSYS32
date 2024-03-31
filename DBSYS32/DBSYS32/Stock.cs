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
    public partial class Stock : Form
    {
        public Stock()
        {
            InitializeComponent();
        }

        private void Stock_Load(object sender, EventArgs e)
        {
            try {
                txtLogUser.Text = Login.user;

                SqlConnection con = new SqlConnection("Data Source=DESKTOP-CFPO8DC\\SQLEXPRESS;Initial Catalog=Inventory;Integrated Security=True");
                con.Open();

                string query = "SELECT * FROM stock";
                SqlCommand cmd = new SqlCommand(query, con);

                DataTable table = new DataTable();
                SqlDataAdapter data = new SqlDataAdapter(cmd);
                data.Fill(table);

                dgvStock.DataSource = table;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-CFPO8DC\\SQLEXPRESS;Initial Catalog=Inventory;Integrated Security=True");
                con.Open();

                string checkQuery = "SELECT COUNT(*) FROM stock WHERE ItemCode = @ItemCode";
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@ItemCode", txtItemNo.Text);

                int existingCount = (int)checkCmd.ExecuteScalar();

                if (existingCount > 0)
                {
                    MessageBox.Show("Item with the same ItemCode already exists. Please choose a different ItemCode.", "Duplicate ItemCode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string maxItemCodeQuery = "SELECT MAX(ItemCode) FROM stock";
                    SqlCommand maxItemCodeCmd = new SqlCommand(maxItemCodeQuery, con);
                    object result = maxItemCodeCmd.ExecuteScalar();

                    string nextItemCode;
                    if (result != DBNull.Value && !string.IsNullOrEmpty(result.ToString()))
                    {
                        string lastItemCode = result.ToString();
                        int numericPartStartIndex = lastItemCode.IndexOf('-') + 1;
                        string numericPart = lastItemCode.Substring(numericPartStartIndex);
                        int nextItemCodeInt = int.Parse(numericPart) + 1;
                        nextItemCode = "P-" + nextItemCodeInt.ToString("D3");
                    }
                    else
                    {
                        nextItemCode = "P-001";
                    }

                    string query = "INSERT INTO stock (ItemCode, Coffee, BeanType, Price, StockAmount) VALUES(@ItemCode, @Coffee, @BeanType, @Price, @StockAmount)";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@ItemCode", nextItemCode);
                    cmd.Parameters.AddWithValue("@Coffee", cboCoffee.Text);
                    cmd.Parameters.AddWithValue("@BeanType", cboBeanType.Text);
                    cmd.Parameters.AddWithValue("@Price", txtPrice.Text);
                    cmd.Parameters.AddWithValue("@StockAmount", txtStock.Text);

                    cmd.ExecuteNonQuery();

                    // Refresh DataGridView
                    Stock_Load(sender, e);
                    MessageBox.Show("Item successfully added with ID: " + nextItemCode, "Add Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clearfields();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-CFPO8DC\\SQLEXPRESS;Initial Catalog=Inventory;Integrated Security=True");
                con.Open();

                string query = "UPDATE stock SET Coffee = @Coffee, BeanType = @BeanType," +
                                                "Price = @Price, StockAmount = @StockAmount WHERE ItemCode = @ItemCode";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ItemCode", txtItemNo.Text);
                cmd.Parameters.AddWithValue("@Coffee", cboCoffee.Text);
                cmd.Parameters.AddWithValue("@BeanType", cboBeanType.Text);
                cmd.Parameters.AddWithValue("@Price", txtPrice.Text);
                cmd.Parameters.AddWithValue("@StockAmount", txtStock.Text);
                cmd.ExecuteNonQuery();

                Stock_Load(sender, e);
                MessageBox.Show("Item succcessfully updated", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clearfields();

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-CFPO8DC\\SQLEXPRESS;Initial Catalog=Inventory;Integrated Security=True");
                con.Open();

                DialogResult Option = MessageBox.Show("Are you sure you want to delete this record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Option == DialogResult.Yes)
                {
                    string query = "DELETE FROM stock WHERE ItemCode = @ItemCode";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@ItemCode", txtItemNo.Text);
                    cmd.ExecuteNonQuery();

                    Stock_Load(sender, e);
                    MessageBox.Show("Item succcessfully deleted", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dgvStock_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtItemNo.Text = dgvStock[0, e.RowIndex].Value.ToString();
            cboCoffee.Text = dgvStock[1, e.RowIndex].Value.ToString();
            cboBeanType.Text = dgvStock[2, e.RowIndex].Value.ToString();
            txtPrice.Text = dgvStock[3, e.RowIndex].Value.ToString();
            txtStock.Text = dgvStock[4, e.RowIndex].Value.ToString();
        }

        private void btnAddCoffee_Click(object sender, EventArgs e)
        {
            try
            {
                cboCoffee.Items.Add(txtAddCoffee.Text);
                txtAddCoffee.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnAddBeanType_Click(object sender, EventArgs e)
        {
            try
            {
                cboBeanType.Items.Add(txtAddBeanType.Text);
                txtAddBeanType.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemoveCoffee_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboCoffee.Items.Contains(txtAddCoffee.Text))
                {
                    cboCoffee.Items.Remove(txtAddCoffee.Text);
                    txtAddCoffee.Clear();
                }
                else
                {
                    MessageBox.Show("Item cant be found", "Deletion Fail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemoveBeanType_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboBeanType.Items.Contains(txtAddBeanType.Text))
                {
                    cboBeanType.Items.Remove(txtAddBeanType.Text);
                    txtAddBeanType.Clear();
                }
                else
                {
                    MessageBox.Show("Item cant be found", "Deletion Fail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void clearfields()
        {
            txtItemNo.Clear();
            cboCoffee.Text = "";
            cboBeanType.Text = "";
            txtPrice.Clear();
            txtStock.Clear();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login l = new Login();
            l.Show();
            this.Hide();
        }
    }
}
