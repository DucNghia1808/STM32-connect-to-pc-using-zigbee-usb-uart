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

namespace Doc_Data
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            timer1.Start();
            this.Text = "LOGIN";
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void LOGGIN_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-99BV9DS\SQLEXPRESS;Initial Catalog=SinhVien;Integrated Security=True");
            // bien ket noi conn
            try
            {
                conn.Open();
                string tk = username.Text;
                string mk = password.Text;
                string sql = "select *from DangNhap where TaiKhoan='" + tk + "' and MatKhau='" + mk + "'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read() == true)
                {
                    MessageBox.Show("Login successfully.", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    DASHBOARD form1_moi = new DASHBOARD();
                    form1_moi.ShowDialog();

                }
                else
                {
                    username.Text = "";
                    password.Text = "";
                    MessageBox.Show("Please check your ID and PassWord !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Connected error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CANCEL_Click(object sender, EventArgs e)
        {
            DialogResult tb = MessageBox.Show("Do you want to exit?", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (tb == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            text_time.Text = DateTime.Now.ToLongTimeString();
            text_day.Text = DateTime.Now.ToString("dd/MM/yyyy");
            //time_label.Text = DateTime.Now.ToString("HH");
        }
    }
}
