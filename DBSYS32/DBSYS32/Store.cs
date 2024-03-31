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
using System.Diagnostics;
using System.Drawing.Printing;

namespace DBSYS32
{
    public partial class Store : Form
    {
        string query;
        public Store()
        {
            InitializeComponent();
        }
        private void Store_Load(object sender, EventArgs e)
        {
            try
            {
                txtLoggedName.Text = Login.user;

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
        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                int stock = Convert.ToInt32(txtStock.Text);
                int quantity;
                double subtotal;
                if (txtQuantity.Text == "")
                {
                    quantity = 1;
                }
                else
                {
                    quantity = Convert.ToInt32(txtQuantity.Text);
                }
                if (quantity > stock)
                {
                    MessageBox.Show("Quantity is more than available stock", "Quantity Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    Convert.ToDouble(quantity);

                    double price = Convert.ToDouble(txtPrice.Text);

                    subtotal = price * quantity;

                    txtSubtotal.Text = subtotal.ToString("N2");
                    dgvOrder.Rows.Add(txtItemNo.Text, txtCoffee.Text, txtBeantype.Text, txtPrice.Text, txtQuantity.Text, subtotal.ToString("N2"));
                    Store_Load(sender, e);
                    //clearfields();
                }
            }
            catch
            {
                MessageBox.Show("Enter number of quantity", "Quantity Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-CFPO8DC\\SQLEXPRESS;Initial Catalog=Inventory;Integrated Security=True");
                con.Open();

                double cash = Convert.ToDouble(txtCash.Text);
                txtCash.Text = cash.ToString("N2");
                double total = Convert.ToDouble(txtTotal.Text);
                double change;

                if (total > cash)
                {
                    MessageBox.Show("Your payment is short", "Purchase Fail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    change = cash - total;
                    txtChange.Text = change.ToString("N2");

                    string insertDetailQuery = "INSERT INTO sales_detail (ItemNo, Coffee, BeanType, Price, Quantity, Total) VALUES (@itemNo, @coffee, @beanType, @price, @quantity, @total)";
                    SqlCommand cmdInsertDetail = new SqlCommand(insertDetailQuery, con);

                    string updateStockQuery = "UPDATE stock SET StockAmount = StockAmount - @quantity WHERE ItemCode = @itemNo";
                    SqlCommand cmdUpdateStock = new SqlCommand(updateStockQuery, con);

                    string insertMasterQuery = "INSERT INTO sales_master (CustomerName, Date) VALUES (@customer, @date); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdInsertMaster = new SqlCommand(insertMasterQuery, con);

                    cmdInsertMaster.Parameters.AddWithValue("@customer", txtLoggedName.Text);
                    cmdInsertMaster.Parameters.AddWithValue("@date", DateTime.Now);

                    int orderNo = Convert.ToInt32(cmdInsertMaster.ExecuteScalar());

                    foreach (DataGridViewRow row in dgvOrder.Rows)
                    {
                        string itemNo = row.Cells["ItemNo"].Value.ToString();
                        string coffee = row.Cells["Coffee"].Value.ToString();
                        string beanType = row.Cells["BeanType"].Value.ToString();
                        double price = Convert.ToDouble(row.Cells["Price"].Value);
                        int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                        double totalAmount = Convert.ToDouble(row.Cells["Subtotal"].Value);

                        cmdInsertDetail.Parameters.Clear();
                        cmdInsertDetail.Parameters.AddWithValue("@itemNo", itemNo);
                        cmdInsertDetail.Parameters.AddWithValue("@coffee", coffee);
                        cmdInsertDetail.Parameters.AddWithValue("@beanType", beanType);
                        cmdInsertDetail.Parameters.AddWithValue("@price", price);
                        cmdInsertDetail.Parameters.AddWithValue("@quantity", quantity);
                        cmdInsertDetail.Parameters.AddWithValue("@total", totalAmount);
                        cmdInsertDetail.ExecuteNonQuery();

                        cmdUpdateStock.Parameters.Clear();
                        cmdUpdateStock.Parameters.AddWithValue("@quantity", quantity);
                        cmdUpdateStock.Parameters.AddWithValue("@itemNo", itemNo);
                        cmdUpdateStock.ExecuteNonQuery();
                    }

                    con.Close();
                    Store_Load(sender, e); // Refresh the data in the Store form
                    MessageBox.Show("You have successfully purchased the items", "Purchase Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enter cash payment", "Purchase Fail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void btnTotal_Click(object sender, EventArgs e)
        {
            double total = 0;
            for (int i = 0; i < dgvOrder.RowCount; i++)
            {
                total += Convert.ToDouble(dgvOrder.Rows[i].Cells[5].Value);
            }
            txtTotal.Text = total.ToString("N2");
        }
        private void mainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Menu m = new Menu();
            m.Show();
            this.Hide();
        }
        private void stockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (txtLoggedName.Text == "admin")
            {
                Stock s = new Stock();
                s.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("You dont have permission to access stock", "Access Declined", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login l = new Login();
            l.Show();
            this.Hide();
        }

        private void dgvStock_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtItemNo.Text = dgvStock[0, e.RowIndex].Value.ToString();
            txtCoffee.Text = dgvStock[1, e.RowIndex].Value.ToString();
            txtBeantype.Text = dgvStock[2, e.RowIndex].Value.ToString();
            txtPrice.Text = dgvStock[3, e.RowIndex].Value.ToString();
            txtStock.Text = dgvStock[4, e.RowIndex].Value.ToString();
            txtQuantity.Clear();
        }

        private void dgvOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6 && e.RowIndex >= 0 && dgvOrder.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                dgvOrder.Rows.RemoveAt(e.RowIndex);
            }
        }
        private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                // Graphics object for drawing
                Graphics graphics = e.Graphics;

                // Font for printing
                Font font = new Font("Century Gothic", 12);

                // Brush for text color
                Brush brush = Brushes.Black;

                // Line height
                float lineHeight = font.GetHeight();

                // Starting position for printing
                float x = 50;
                float y = 50;

                // Print header
                string header = "\t\tTop of the Morning Receipt\n" +
                                "============================================\r\n";
                graphics.DrawString(header, font, brush, x - 30, y);

                // Increment y to print date
                y += 2 * lineHeight;
                string date = DateTime.Now.ToString();
                graphics.DrawString(date, font, brush, x, y);

                // Increment y for details
                y += 2 * lineHeight;

                // Iterate through each row in the DataGridView dgvOrder
                foreach (DataGridViewRow row in dgvOrder.Rows)
                {
                    string itemNo = row.Cells[0].Value.ToString();
                    string coffee = row.Cells[1].Value.ToString();
                    string beanType = row.Cells[2].Value.ToString();
                    double price = Convert.ToDouble(row.Cells[3].Value);
                    string quantity = row.Cells[4].Value.ToString();
                    double subtotal = Convert.ToDouble(row.Cells[5].Value);

                    // Print item details
                    graphics.DrawString("Item No:", font, brush, x, y);
                    graphics.DrawString(itemNo, font, brush, x + 100, y);

                    y += lineHeight;
                    graphics.DrawString("Coffee:", font, brush, x, y);
                    graphics.DrawString(coffee, font, brush, x + 100, y);

                    y += lineHeight;
                    graphics.DrawString("Bean Type:", font, brush, x, y);
                    graphics.DrawString(beanType, font, brush, x + 100, y);

                    y += lineHeight;
                    graphics.DrawString("Price:", font, brush, x, y);
                    graphics.DrawString(price.ToString(), font, brush, x + 100, y);

                    y += lineHeight;
                    graphics.DrawString("Quantity:", font, brush, x, y);
                    graphics.DrawString(quantity, font, brush, x + 100, y);

                    y += lineHeight;
                    graphics.DrawString("Subtotal:", font, brush, x, y);
                    graphics.DrawString(subtotal.ToString(), font, brush, x + 100, y);

                    y += 2 * lineHeight; // Add extra line spacing between items
                }

                // Print footer
                y += lineHeight;
                string line = "============================================\r\n";
                graphics.DrawString(line, font, brush, x - 30, y);

                // Increment y for total, cash, and change
                y += lineHeight;
                graphics.DrawString("Total:", font, brush, x, y);
                graphics.DrawString(txtTotal.Text, font, brush, x + 100, y);

                y += lineHeight;
                graphics.DrawString("Cash:", font, brush, x, y);
                graphics.DrawString(txtCash.Text, font, brush, x + 100, y);

                y += lineHeight;
                graphics.DrawString("Change:", font, brush, x, y);
                graphics.DrawString(txtChange.Text, font, brush, x + 100, y);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to print: " + ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a PrintDocument instance
                PrintDocument printDoc = new PrintDocument();

                // Attach the PrintPage event handler
                printDoc.PrintPage += printDoc_PrintPage;

                // Create a PrintPreviewDialog instance
                PrintPreviewDialog previewDialog = new PrintPreviewDialog();

                // Set the document to the preview dialog
                previewDialog.Document = printDoc;

                // Show the print preview dialog
                previewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to print: " + ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printPreview_Load(object sender, EventArgs e)
        {

        }

        
    }
}
