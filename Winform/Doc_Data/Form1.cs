using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO.Ports;
using System.Configuration;
using System.Data.SqlClient;
//using Microsoft.Office.Interop.Excel;
//using app = Microsoft.Office.Interop.Excel.Application;

namespace Doc_Data
{
    public partial class DASHBOARD : Form
    {
        public DASHBOARD()
        {
            InitializeComponent();
            this.Text = "DASHBOARD MANAGER";
            timer1.Start();
            timer2.Start();
            tempProccess.Value = 50;
            humiProgress.Value = 50;
            ////// formboderstyle:
        }
        SqlConnection con= new SqlConnection(@"Data Source=DESKTOP-99BV9DS\SQLEXPRESS;Initial Catalog=CamBien;Integrated Security=True");
        
        
        
        //DESKTOP-99BV9DS\SQLEXPRESS
        private void openCon()
        {
            if(con.State == ConnectionState.Closed)
            {
                con.Open();
            }
        }
        private void closeCon()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        private Boolean Exe(string cmd)
        {
            openCon();
            Boolean kt;
            try
            {
                SqlCommand sc = new SqlCommand(cmd, con);
                kt = true;
            }
            catch (Exception)
            {
                kt = false;
            }
            closeCon();
            return kt;
        }
        private DataTable Red(string cmd)
        {
            openCon();
            DataTable dt = new DataTable();
            try
            {
                SqlCommand sc = new SqlCommand(cmd, con);
                SqlDataAdapter sda = new SqlDataAdapter(sc);
                sda.Fill(dt);
            }
            catch (Exception)
            {
                dt = null;
                throw;
            }
            closeCon();
            return dt;
        }
        private void load() /// load data to grid view
        {
            DataTable dt = Red("SELECT * from CamBien");
            if(dt != null)
            {
                DS_cambien.DataSource = dt; 
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] myport = SerialPort.GetPortNames();
            comboPORT.Items.AddRange(myport);// get port
           // load();
            con.Open();
            hienthi();
        }
        /// HIEN THI LEN DATAGRIDVIEW
        public void hienthi()
        {
            string sqlSelect = "SELECT * FROM CamBien";
            SqlCommand cmd = new SqlCommand(sqlSelect, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            DS_cambien.DataSource=dt;

        }
       
        String Data = "";

        private void Connect_Click(object sender, EventArgs e)
        {
            if (comboPORT.Text == "")
            {
                MessageBox.Show("Please, Select your COM PORT!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboBaudrate.Text == "")
            {
                MessageBox.Show("Please, Select your BaudRate!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                //serialPort1.PortName = comboPORT.Text;
                //serialPort1.BaudRate = Convert.ToInt32(comboBaudrate.Text); // convert to text

                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    comboPORT.Enabled = true;
                    comboBaudrate.Enabled = true;

                    Connect.Text = "Connect";
                    progressBar1.Value = 0;
                    //pictureBox6.Image = tbnv.Properties.Resources.OFF_1;
                    MessageBox.Show("Your Com Port is closed.", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    serialPort1.PortName = comboPORT.Text;
                    serialPort1.BaudRate = int.Parse(comboBaudrate.Text); // Int.Parse(comboBaudrate.Text)
                    comboPORT.Enabled = false;
                    comboBaudrate.Enabled = false;

                    serialPort1.Open();
                    progressBar1.Value = 100;
                    Connect.Text = "Disconnect";
                    MessageBox.Show("Your Com Port is connected and ready for use.", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Your Com Port is not found. Please check your Cable or COM.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            text_time.Text = DateTime.Now.ToLongTimeString();
            text_day.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.Invoke(new EventHandler(ShowData));
        }
        private void ShowData(object sender, EventArgs e)
        {
            //textData.Text = Data;
            Data = serialPort1.ReadLine();
            if (Data != "")
            {
                Invoke(new MethodInvoker(() => listBox1.Items.Add(Data)));
            }

            try // hàm bắt lỗi data json
            {
                var Datajson = JsonConvert.DeserializeObject<dynamic>(Data.Trim());
                //Datajson.a
                string nd = Datajson.ND; // lấy data json của từng nhãn
                string da = Datajson.DA;
                
                tempProccess.Text = nd + " ° C";
                tempProccess.Value =  Int32.Parse(nd); // convert string to int
                humiProgress.Text = da + " % ";
                humiProgress.Value = Int32.Parse(da);

                if (Datajson.TB1 == "0")   // TB1
                {
                    LED1.Text = "ON"; //pictuerbox.Image = "img_location";)
                    pictureBox1.Image = Doc_Data.Properties.Resources.on;
                }
                else
                {
                    LED1.Text = "OFF";
                    pictureBox1.Image = Doc_Data.Properties.Resources.off1;
                }
                if (Datajson.TB2 == "0")  // TB2
                {
                    LED2.Text = "ON";
                    pictureBox2.Image = Doc_Data.Properties.Resources.on;
                }
                else
                {
                    LED2.Text = "OFF";
                    pictureBox2.Image = Doc_Data.Properties.Resources.off1;
                }

                if (Datajson.TB3 == "0")  // TB3
                {
                    LED3.Text = "ON";
                    pictureBox4.Image = Doc_Data.Properties.Resources.snow1;
                }
                else
                {
                    LED3.Text = "OFF";
                    pictureBox4.Image = Doc_Data.Properties.Resources.snow_white;
                }

                if (Datajson.TB4 == "0")  // TB4
                {
                    LED4.Text = "ON";
                   pictureBox3.Image = Doc_Data.Properties.Resources.water_on;
                }
                else
                {
                    LED4.Text = "OFF";
                   pictureBox3.Image = Doc_Data.Properties.Resources.water_off;
                }

                if (Datajson.CB2 == "0")  // cb mua
                {
                    pictureWeather.Image = Doc_Data.Properties.Resources.rain;
                }
                else
                {
                    pictureWeather.Image = Doc_Data.Properties.Resources.sunset;
                }
                if (Datajson.CB1 == "0")  // 
                {
                    pictureDaynight.Image = Doc_Data.Properties.Resources.day;
                }
                else
                {
                    pictureDaynight.Image = Doc_Data.Properties.Resources.moon;
                }
                Data = "";
            }
            catch (Exception)
            {

            }
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            con.Close();
            Application.Exit();
        }
        int dem = 0;
        int timeset_update = 2;
        private void UPDATE_DATA_TIME_Click(object sender, EventArgs e)
        {
            timeset_update = Int32.Parse(text_time_set.Text);
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
           if (serialPort1.IsOpen)
            {
                dem++;
                if(dem == 1) // dem == ? minute
                {
                    string sqlINSERT = "INSERT INTO CamBien VALUES(@NhietDo, @DoAm, @ThoiGian, @NgayThang)";
                    SqlCommand cmd = new SqlCommand(sqlINSERT, con);
                    cmd.Parameters.AddWithValue("NhietDo", tempProccess.Text);
                    cmd.Parameters.AddWithValue("DoAm", humiProgress.Text);
                    cmd.Parameters.AddWithValue("ThoiGian", text_time.Text);
                    cmd.Parameters.AddWithValue("NgayThang", text_day.Text);
                    cmd.ExecuteNonQuery();
                    hienthi();
                    dem = 0;
                }
            }
            listBox1.DataSource = null;
            listBox1.Items.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            string sqlINSERT = "INSERT INTO CamBien VALUES(@NhietDo, @DoAm, @ThoiGian, @NgayThang)";
            SqlCommand cmd = new SqlCommand(sqlINSERT, con);
            cmd.Parameters.AddWithValue("NhietDo", tempProccess.Text);
            cmd.Parameters.AddWithValue("DoAm", humiProgress.Text);
            cmd.Parameters.AddWithValue("ThoiGian", text_time.Text);
            cmd.Parameters.AddWithValue("NgayThang", text_day.Text);
            cmd.ExecuteNonQuery();
            //Thi hành SqlCommand bằng phương thức ExecuteNonQuery nó chỉ trả về kết quả là số dòng dữ liệu bị ảnh hưởng (số dòng xóa, số dòng update ...).
            hienthi();
        }

        private void delete_Click(object sender, EventArgs e)
        {
            DialogResult thongbao= MessageBox.Show("Do you want to delete?", "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if(thongbao == DialogResult.OK)
            {
                string sqlDEL = "DELETE FROM CamBien WHERE ThoiGian ='" + TIME_grid.Text + "'";
                SqlCommand cmd = new SqlCommand(sqlDEL, con);
                cmd.ExecuteNonQuery();
                hienthi();
                DA_grid.Text = "";
                ND_grid.Text = "";
                TIME_grid.Text = "";
                DAY_grid.Text = "";
            }
        }

        private void find_Click(object sender, EventArgs e)
        {
            string sqlfind = "SELECT *FROM CamBien WHERE NhietDo = '" + ND_grid + "', DoAm = '" + DA_grid + "'";
            SqlCommand cmd = new SqlCommand(sqlfind, con);
            cmd.Parameters.AddWithValue("NhietDo", tempProccess.Text);
            cmd.Parameters.AddWithValue("DoAm", humiProgress.Text);
            cmd.Parameters.AddWithValue("ThoiGian", text_time.Text);
            cmd.Parameters.AddWithValue("NgayThang", text_day.Text);
            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            DS_cambien.DataSource = dt;
        }

        private void fix_Click(object sender, EventArgs e)
        {
            string sqlDEL = "UPDATE CamBien SET NhietDo = @NhietDo, DoAm = @Doam, ThoiGian = @ThoiGian, NgayThang = @NgayThang";
            SqlCommand cmd = new SqlCommand(sqlDEL, con);
            cmd.Parameters.AddWithValue("NhietDo", ND_grid.Text);
            cmd.Parameters.AddWithValue("DoAm", DA_grid.Text);
            cmd.Parameters.AddWithValue("ThoiGian", TIME_grid.Text);
            cmd.Parameters.AddWithValue("NgayThang",DAY_grid.Text);
            cmd.ExecuteNonQuery();
            hienthi();
        }

       
        //private void export
        private void ToExcel(DataGridView dataGridView1, string fileName)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook workbook;
            Microsoft.Office.Interop.Excel.Worksheet worksheet;

            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false;

                workbook = excel.Workbooks.Add(Type.Missing);

                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets["Sheet1"];
                worksheet.Name = "DU LIEU TRAM THOI TIET";

                // export header
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    worksheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText;
                }

                // export content
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    }
                }

                // save workbook
                workbook.SaveAs(fileName);
                workbook.Close();
                excel.Quit();
                MessageBox.Show("Your Com Port is connected and ready for use.", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                workbook = null;
                worksheet = null;
            }
        }


        private void CLEAR_listbox_Click(object sender, EventArgs e)
        {
            listBox1.DataSource = null;
            listBox1.Items.Clear();
        }

        private void LOCK_FORM_Click(object sender, EventArgs e)
        {
            this.Hide();
            serialPort1.Close();
            Login form2_moi = new Login();
            form2_moi.ShowDialog();

        }

        private void CLEAR_listbox_Click_1(object sender, EventArgs e)
        {
            listBox1.DataSource = null;
            listBox1.Items.Clear();
        }

        private void DS_cambien_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            ND_grid.Text = DS_cambien.CurrentRow.Cells[0].Value.ToString();
            DA_grid.Text = DS_cambien.CurrentRow.Cells[1].Value.ToString();
            TIME_grid.Text = DS_cambien.CurrentRow.Cells[2].Value.ToString();
            DAY_grid.Text = DS_cambien.CurrentRow.Cells[3].Value.ToString();
        }

        private void LED1_Click_1(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (LED1.Text == "ON")
                {
                    serialPort1.WriteLine("{\"TB1\":\"1\"}");

                    LED1.Text = "ON";
                }
                else
                {
                    serialPort1.WriteLine("{\"TB1\":\"0\"}");
                    LED1.Text = "OFF";
                }
            }
            else
            {
                MessageBox.Show("Please Connect your Com Port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LED2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (LED2.Text == "ON")
                {
                    serialPort1.WriteLine("{\"TB2\":\"1\"}");
                    LED2.Text = "ON";
                }
                else
                {
                    serialPort1.WriteLine("{\"TB2\":\"0\"}");
                    LED2.Text = "OFF";
                }
            }
            else
            {
                MessageBox.Show("Please Connect your Com Port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LED3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (LED3.Text == "ON")
                {
                    serialPort1.WriteLine("{\"TB3\":\"1\"}");
                    LED3.Text = "ON";
                }
                else
                {
                    serialPort1.WriteLine("{\"TB3\":\"0\"}");
                    LED3.Text = "OFF";
                }
            }
            else
            {
                MessageBox.Show("Please Connect your Com Port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LED4_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (LED4.Text == "ON")
                {
                    serialPort1.WriteLine("{\"TB4\":\"1\"}");
                    LED4.Text = "ON";
                }
                else
                {
                    serialPort1.WriteLine("{\"TB4\":\"0\"}");
                    LED4.Text = "OFF";
                }
            }
            else
            {
                MessageBox.Show("Please Connect your Com Port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Export_Click_1(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ToExcel(DS_cambien, saveFileDialog1.FileName);
            }
        }    
    }
}
