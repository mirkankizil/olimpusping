using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Diagnostics.Eventing;

namespace OLİMPUS_PİNG
{
    public partial class Form1 : Form
    {
        // sql veritabanı bağlantısı oluşturdum --> değişken = connection
        // sql database connection created --> variable = connection
        SqlConnection connection;
        // sql veritabanına komut vermede kullancağım değişken oluşturdum --> değişken = command
        // i've created a variable that will use sql command --> variable = command
        SqlCommand command;
        // sql sorgularını doldurmak için değişken oluşturdum --> değişken = datadapter
        // i've created a variable to complete sql queries --> variable = datadapter
        SqlDataAdapter datadapter;
        public Form1()
        {
            InitializeComponent();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
        }
        public bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;
            nameOrAddress = txtIp.Text;
            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // PingExceptions çalışır ve false döndürür;
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }
        void kayıtGetir()
        {
            // veritabanı bağlantı işlemleri 
            // database connection operations
            connection = new SqlConnection("Data Source=.;Initial Catalog=ping;Integrated Security=True");
            connection.Open();
            datadapter = new SqlDataAdapter("SELECT *FROM ping", connection);
            DataTable tablo = new DataTable();
            datadapter.Fill(tablo);
            dtgList1.DataSource = tablo;
            connection.Close();
            //Seçili kolonu dataGridView den çıkartır.
            //Extracts the selected column from the dataGridView.
            dtgList1.Columns[5].Visible = false;
            dtgList1.Columns[0].Visible = false;
        }
        // verileri tutmak için tablo oluşturdum
        // create a tablo to hold data
        DataTable table = new DataTable();
        private void dtgList1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            kayıtGetir();
            dtgList1.Columns[0].Name = "Id";
            dtgList1.Columns[1].Name = "Ip";
            dtgList1.Columns[2].Name = "Mail";
            dtgList1.Columns[3].Name = "Sunucu Adı";
            dtgList1.Columns[4].Name = "Kullanıcı Mail";
            dtgList1.Columns[5].Name = "Kullanıcı Şifre";
            //table.Columns.Add("Sunucu Adı", typeof(string));
            //table.Columns.Add("Ip", typeof(string));
            //table.Columns.Add("Sunucu Durumu", typeof(string));
            //table.Columns.Add("Gönderici Maili", typeof(string));
            //table.Columns.Add("Mail Sifre", typeof(string));
            dtgList1.AllowUserToAddRows = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Control item in this.Controls)
            {
                if (item is TextBox)
                {
                    TextBox tbox = (TextBox)item;
                    tbox.Clear();
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            // veritabanı sorgusu --> değişken = query
            // database query --> variable = query
            string query = "INSERT INTO ping(ip,mail,kimlik,kulMail,kulSifre) VALUES (@ip,@mail,@kimlik,@kulMail,@kulSifre)";
            command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ip", txtIp.Text);
            command.Parameters.AddWithValue("@mail", txtMail.Text);
            command.Parameters.AddWithValue("@kimlik", txtSunucuAd.Text);
            command.Parameters.AddWithValue("@kulMail", txtKmail.Text);
            command.Parameters.AddWithValue("@kulSifre", txtSifre.Text);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            kayıtGetir();
            dtgList1.DataSource = table;
            if (txtIp.Text == "" || txtMail.Text == "" || txtSunucuAd.Text == "" || txtKmail.Text == "" || txtSifre.Text == "")
            {
                MessageBox.Show("Bu alanlar boş bırakılamaz!");

            }
            else
            { MessageBox.Show("Kayıt işlemi tamamlandı."); }
        }
        private void tbIp_TextChanged(object sender, EventArgs e)
        {
        }
        private void gizleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }
        private void kapatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Form1_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                Ayarlar.ShowBalloonTip(1000, "Ping Atılıyor...", "Arka planda çalışıyor!", ToolTipIcon.Info);
            }
        }
        private void Ayarlar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }
        private void txtMail_TextChanged(object sender, EventArgs e)
        {
        }
        private void txtSunucuAd_TextChanged(object sender, EventArgs e)
        {
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string sorgu = "DELETE FROM ping WHERE id=@id";
            command = new SqlCommand(sorgu, connection);
            command.Parameters.AddWithValue("@id", Convert.ToInt32(txtId.Text));
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            kayıtGetir();
            if (dtgList1.SelectedRows.Count > 0)
            {
                dtgList1.Rows.RemoveAt(dtgList1.SelectedRows[0].Index);
            }
            else
            {
                MessageBox.Show("Lütfen Silinecek Sunucuyu seçin!");
            }
        }
        private void dtgList1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtId.Text = dtgList1.CurrentRow.Cells[0].Value.ToString();
            txtIp.Text = dtgList1.CurrentRow.Cells[1].Value.ToString();
            txtMail.Text = dtgList1.CurrentRow.Cells[2].Value.ToString();
            txtSunucuAd.Text = dtgList1.CurrentRow.Cells[3].Value.ToString();
            txtKmail.Text = dtgList1.CurrentRow.Cells[4].Value.ToString();
            txtSifre.Text = dtgList1.CurrentRow.Cells[5].Value.ToString();
        }
        private void txtIp_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != 46;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string sorgu = "UPDATE ping SET ip=@ip,mail=@mail,kimlik=@kimlik,kulMail=@kulMail,kulSifre=@kulSifre WHERE id=@id";
            command = new SqlCommand(sorgu, connection);
            command.Parameters.AddWithValue("@id", txtId.Text);
            command.Parameters.AddWithValue("@ip", txtIp.Text);
            command.Parameters.AddWithValue("@mail", txtMail.Text);
            command.Parameters.AddWithValue("@kimlik", txtSunucuAd.Text);
            command.Parameters.AddWithValue("@kulMail", txtKmail.Text);
            command.Parameters.AddWithValue("@kulSifre", txtSifre.Text);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            kayıtGetir();
        }
        private void dtgList1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.Value != null)
            {
                e.Value = new String('*', e.Value.ToString().Length);
            }
        }
        public static void WriteLog(string strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            string logFilePath = System.IO.Path.GetDirectoryName(Application.StartupPath.ToString());
            logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            log.Close();
        }
        private void button7_Click(object sender, EventArgs e)
        {


            PingHost(txtIp.Text);
            if (PingHost(txtIp.Text) != false)
            {
                //mail olayları
                MailMessage icerik = new MailMessage();
                SmtpClient istemci = new SmtpClient();
                istemci.Credentials = new System.Net.NetworkCredential(txtKmail.Text, txtSifre.Text);
                istemci.Port = 587;
                istemci.Host = "smtp.outlook.com";
                istemci.EnableSsl = true;
                icerik.To.Add(txtMail.Text);
                icerik.From = new MailAddress(txtKmail.Text);
                icerik.Subject = txtIp.Text + " sunucunuzdan cevap alınamıyor.";
                icerik.Body = txtSunucuAd + " adlı sunucudan 7 dakikadır cevap alınamıyor.";
                istemci.Send(icerik);
                Ping ping = new Ping();
                PingReply reply = ping.Send(txtIp.Text, 1000);
            }


        }
        private void button8_Click(object sender, EventArgs e)
        {
            //her türlü kapatır.
            Environment.Exit(0);
        }
        private void txtKmail_TextChanged(object sender, EventArgs e)
        {
        }
        private void txtSifre_TextChanged(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //    if (checkBox1.Checked)
            //    {
            //        txtSifre.PasswordChar = "\0";
            //    }
            //    else
            //    {
            //        txtSifre.PasswordChar = "*";
            //    }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {
        }
    }
}