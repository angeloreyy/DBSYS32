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
    public partial class Sales : Form
    {
        public Sales()
        {
            InitializeComponent();
        }

        private void Sales_Load(object sender, EventArgs e)
        {
            try
            {
                txtName.Text = Login.user;

                SqlConnection con = new SqlConnection("Data Source=DESKTOP-CFPO8DC\\SQLEXPRESS;Initial Catalog=Inventory;Integrated Security=True");
                con.Open();

                string query = "SELECT * FROM sales_master";
                SqlCommand cmd = new SqlCommand(query, con);

                DataTable table = new DataTable();
                SqlDataAdapter data = new SqlDataAdapter(cmd);
                data.Fill(table);

                dgvSales.DataSource = table;

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
                    string query = "DELETE FROM sales_master WHERE OrderNumber = @orderNo";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@orderNo", txtOrderNo.Text);
                    cmd.ExecuteNonQuery();

                    Sales_Load(sender, e);
                    MessageBox.Show("Item succcessfully deleted", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clearfields();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void clearfields()
        {
            txtCustomerName.Clear();
            txtDateSale.Clear();
            txtOrderNo.Clear();
        }

        private void dgvSales_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtOrderNo.Text = dgvSales[0, e.RowIndex].Value.ToString();
            txtCustomerName.Text = dgvSales[1, e.RowIndex].Value.ToString();
            txtDateSale.Text = dgvSales[2, e.RowIndex].Value.ToString();
        }

        private void btnBacktoStore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Store s = new Store();
            s.Show();
            this.Hide();
        }
    }
}
