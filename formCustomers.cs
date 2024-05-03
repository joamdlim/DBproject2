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

namespace DBPROJECT
{
    public partial class formCustomers : Form
    {
        DataTable DTable;

        SqlDataAdapter DAdapter;
        SqlCommand Dcommand;
        BindingSource DBindingSource;

        Boolean CancelUpdates;

        int idcolumn = 0;
        public formCustomers()
        {
            InitializeComponent();
        }

       
        private void BindMainGrid()
        {
            this.CancelUpdates = true;
            if (Globals.glOpenSqlConn())
            {
                this.Dcommand = new SqlCommand("spGetAllCustomers", Globals.sqlconn);
                this.DAdapter = new SqlDataAdapter(this.Dcommand);

                this.DTable = new DataTable();

                this.DAdapter.Fill(DTable);

                this.DBindingSource = new BindingSource();
                this.DBindingSource.DataSource = DTable;


                dgvCust.DataSource = DBindingSource;

                this.bindingNavigator1.BindingSource = this.DBindingSource;
            }
            this.CancelUpdates = false;
        }
        private void EditGrid()
        {

            this.dgvCust.Columns["idCustomer"].Visible = false;
            this.dgvCust.Columns["nameCustomer"].HeaderText = "Login Name";
            this.dgvCust.Columns["addressCustomer"].HeaderText = "Address";
            this.dgvCust.Columns["emailCustomer"].HeaderText = "Email";
            this.dgvCust.Columns["contactCustomer"].HeaderText = "Contact";

            this.dgvCust.BackgroundColor = Globals.gGridOddRowColor;
            this.dgvCust.AlternatingRowsDefaultCellStyle.BackColor = Globals.gGridEvenRowColor;

            this.dgvCust.EnableHeadersVisualStyles = false;
            this.dgvCust.ColumnHeadersDefaultCellStyle.BackColor = Globals.gGridHeaderColor;

        }

        private void dgvCust_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(((DataGridView)sender).RowHeadersDefaultCellStyle.ForeColor))

            {

                e.Graphics.DrawString(

                    String.Format("{0,10}", (e.RowIndex + 1).ToString()),

                    e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);

            }
        }

        private void dgvCust_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            int firstDisplayedCellIndex = dgvCust.FirstDisplayedCell.RowIndex;

            int lastDisplayedCellIndex = firstDisplayedCellIndex + dgvCust.DisplayedRowCount(true);



            Graphics Graphics = dgvCust.CreateGraphics();

            int measureFirstDisplayed = (int)(Graphics.MeasureString(firstDisplayedCellIndex.ToString(), dgvCust.Font).Width);

            int measureLastDisplayed = (int)(Graphics.MeasureString(lastDisplayedCellIndex.ToString(), dgvCust.Font).Width);



            int rowHeaderWitdh = System.Math.Max(measureFirstDisplayed, measureLastDisplayed);

            dgvCust.RowHeadersWidth = rowHeaderWitdh + 40;
        }

        private void dgvCust_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            bool cancel = true;

            DataGridViewRow rw = this.dgvCust.CurrentRow;
            String n = rw.Cells["nameCustomer"].Value.ToString().Trim();

            if (rw.Cells[idcolumn].Value != DBNull.Value &&
               csMessageBox.Show("Delete the user:" + n, "Please confirm.",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (Globals.glOpenSqlConn())
                {

                    SqlCommand cmd = new SqlCommand("dbo.spusersDelete", Globals.sqlconn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@rid", Convert.ToInt64(rw.Cells[idcolumn].Value));
                    cmd.ExecuteNonQuery();

                    e.Cancel = false;

                }
                Globals.glCloseSqlConn();
                e.Cancel = false;

            }
            else e.Cancel = true;

        }


        

        private void dgvCust_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }
        private Boolean SearchName(String searchVal)
        {
            bool resultVal = false;
            int rowIndex = -1;

            searchVal = searchVal.Trim().ToUpper();
            if (searchVal != "")
            {
                this.bindingNavigator1.MoveFirstItem.PerformClick();

                foreach (DataGridViewRow row in dgvCust.Rows)
                {
                    try
                    {
                        if (row.Cells["loginname"].Value.ToString().StartsWith(searchVal))
                        {
                            rowIndex = row.Index;
                            dgvCust.Rows[row.Index].Selected = true;
                            resultVal = true;
                            break;
                        }
                        this.bindingNavigator1.MoveNextItem.PerformClick();
                    }
                    catch
                    {
                        break;
                    }
                } // foreach
                if (!resultVal)
                    csMessageBox.Show("Record not found.", "Search Result",
                      MessageBoxButtons.OK, MessageBoxIcon.Warning);

            } // if
            return resultVal;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            {
                String searchVal = custSearch.Text.Trim().ToUpper();

                if (this.SearchName(searchVal))
                {
                    this.custSearch.Clear();
                    this.dgvCust.Focus();

                }
                else
                {
                    this.custSearch.Focus();
                }
            }
        }
        private frmEditUser EditUserfrm;


        private void formCustomers_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void formCustomers_Load(object sender, EventArgs e)
        {
            this.CancelUpdates = true;
            this.BindMainGrid();
            this.EditGrid();
            this.CancelUpdates = false;
        }

        private void dgvCust_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
        
            {

                long customerid = 0;
                long newcustomerid;

                if (this.CancelUpdates == false && this.dgvCust.CurrentRow != null)

                {

                    if (Globals.glOpenSqlConn())

                    {

                        DataGridViewRow row = dgvCust.CurrentRow;

                        String custname = row.Cells["nameCustomer"].Value == DBNull.Value ? ""

                            : row.Cells["nameCustomer"].Value.ToString().ToUpper();

                        String custadd = row.Cells["addressCustomer"].Value == DBNull.Value ? ""

                            : row.Cells["addressCustomer"].Value.ToString();

                        String custemail = row.Cells["emailCustomer"].Value == DBNull.Value ? ""

                            : row.Cells["emailCustomer"].Value.ToString();

                        String custnum = row.Cells["contactCustomer"].Value == DBNull.Value ? ""

                            : row.Cells["contactCustomer"].Value.ToString();




                        if (row.Cells["nameCustomer"].Value == DBNull.Value)

                        {

                            csMessageBox.Show("Please encode a valid user name", "Warning",

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            dgvCust.CancelEdit();

                        }

                        else

                        {

                            try

                            {

                                SqlCommand cmd = new SqlCommand("spCustomersAddEdit", Globals.sqlconn);

                                cmd.CommandType = CommandType.StoredProcedure;


                                cmd.Parameters.AddWithValue("@cid", customerid);

                                cmd.Parameters.AddWithValue("@cname", custname);

                                cmd.Parameters.AddWithValue("@cadd", custadd);

                                cmd.Parameters.AddWithValue("@cemail", custemail);

                                cmd.Parameters.AddWithValue("@cnum", custnum);



                                SqlDataAdapter dAdapt = new SqlDataAdapter(cmd);

                                DataTable dt = new DataTable();

                                dAdapt.Fill(dt);

                                newcustomerid = long.Parse(dt.Rows[0][0].ToString());

                                if (customerid == 0)
                                    row.Cells["idCustomer"].Value = newcustomerid;
                            }

                            catch (Exception ex)

                            {



                                csMessageBox.Show("Exception Error:" + ex.Message,

                                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            }

                        }

                        Globals.glCloseSqlConn();


                    }

                    Globals.glCloseSqlConn();

                }

            
        }
        private frmEditCustomers EditCustomerfrm;
        private void dgvCust_DoubleClick_1(object sender, EventArgs e)
        {
            long customerid;

            DataGridViewRow row = dgvCust.CurrentRow;



            if (row.Cells[this.idcolumn].Value == DBNull.Value)

                customerid = 0;

            else

                customerid = Convert.ToInt64(row.Cells[this.idcolumn].Value);



            if (customerid != 0)

            {

                EditCustomerfrm = new frmEditCustomers(customerid);

                EditCustomerfrm.MdiParent = this.MdiParent;

                EditCustomerfrm.Show();

            }
        }

        private void dgvCust_UserDeletingRow_1(object sender, DataGridViewRowCancelEventArgs e)
        {
            bool cancel = true;

            DataGridViewRow rw = this.dgvCust.CurrentRow;
            String n = rw.Cells["nameCustomer"].Value.ToString().Trim();

            if (rw.Cells[idcolumn].Value != DBNull.Value &&
               csMessageBox.Show("Delete the user:" + n, "Please confirm.",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (Globals.glOpenSqlConn())
                {

                    SqlCommand cmd = new SqlCommand("dbo.spCustomersDelete", Globals.sqlconn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cid", Convert.ToInt64(rw.Cells[idcolumn].Value));
                    cmd.ExecuteNonQuery();

                    e.Cancel = false;

                }
                Globals.glCloseSqlConn();
                e.Cancel = false;

            }
            else e.Cancel = true;
        }

        private void dgvCust_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

}







