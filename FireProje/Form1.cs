using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net.Mail;
using System.Net;
using System.Data.SqlClient;


//Data Source=DESKTOP-BL458FP;Initial Catalog=FirePro;Integrated Security=True

namespace FireProje
{

    public partial class Form1 : Form
    {
        string[] ayırma;
        string sonuc;
        string dSet;
        int Gaz;
        int ates;
        double Deger;
        int Isı;
        string[] ports = SerialPort.GetPortNames();
        private int borderSize = 1;
        private Size formSize;
        public Form1()
        {
            InitializeComponent();

            this.Padding = new Padding(borderSize);//Border size
            this.BackColor = Color.FromArgb(98, 102, 244);//Border color
        }


        SqlConnection Baglanti = new SqlConnection("Server=.;Database=FirePro; Password=azazazaz09;");




        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void Form1_Load(object sender, EventArgs e)
        {

            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            timer1.Enabled = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }


        public static void SendEmail(string address, string subject, string message, string email, string username, string password, string smtp, int port)
        {
            string to = email; //To address    
            string from = address; //From address    
            MailMessage msg = new MailMessage(from, to);

            string mailbody = message;
            msg.Subject = subject;
            msg.Body = mailbody;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new
            System.Net.NetworkCredential(address, password);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(msg);
               
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x0083;//Standar Title Bar - Snap Window
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020; //Minimize form (Before)
            const int SC_RESTORE = 0xF120; //Restore form (Before)
            const int WM_NCHITTEST = 0x0084;//Win32, Mouse Input Notification: Determine what part of the window corresponds to a point, allows to resize the form.
            const int resizeAreaSize = 10;

            #region Form Resize
            // Resize/WM_NCHITTEST values
            const int HTCLIENT = 1; //Represents the client area of the window
            const int HTLEFT = 10;  //Left border of a window, allows resize horizontally to the left
            const int HTRIGHT = 11; //Right border of a window, allows resize horizontally to the right
            const int HTTOP = 12;   //Upper-horizontal border of a window, allows resize vertically up
            const int HTTOPLEFT = 13;//Upper-left corner of a window border, allows resize diagonally to the left
            const int HTTOPRIGHT = 14;//Upper-right corner of a window border, allows resize diagonally to the right
            const int HTBOTTOM = 15; //Lower-horizontal border of a window, allows resize vertically down
            const int HTBOTTOMLEFT = 16;//Lower-left corner of a window border, allows resize diagonally to the left
            const int HTBOTTOMRIGHT = 17;//Lower-right corner of a window border, allows resize diagonally to the right

            ///<Doc> More Information: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest </Doc>

            if (m.Msg == WM_NCHITTEST)
            { //If the windows m is WM_NCHITTEST
                base.WndProc(ref m);
                if (this.WindowState == FormWindowState.Normal)//Resize the form if it is in normal state
                {
                    if ((int)m.Result == HTCLIENT)//If the result of the m (mouse pointer) is in the client area of the window
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32()); //Gets screen point coordinates(X and Y coordinate of the pointer)                           
                        Point clientPoint = this.PointToClient(screenPoint); //Computes the location of the screen point into client coordinates                          

                        if (clientPoint.Y <= resizeAreaSize)//If the pointer is at the top of the form (within the resize area- X coordinate)
                        {
                            if (clientPoint.X <= resizeAreaSize) //If the pointer is at the coordinate X=0 or less than the resizing area(X=10) in 
                                m.Result = (IntPtr)HTTOPLEFT; //Resize diagonally to the left
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))//If the pointer is at the coordinate X=11 or less than the width of the form(X=Form.Width-resizeArea)
                                m.Result = (IntPtr)HTTOP; //Resize vertically up
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTTOPRIGHT;
                        }
                        else if (clientPoint.Y <= (this.Size.Height - resizeAreaSize)) //If the pointer is inside the form at the Y coordinate(discounting the resize area size)
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize horizontally to the left
                                m.Result = (IntPtr)HTLEFT;
                            else if (clientPoint.X > (this.Width - resizeAreaSize))//Resize horizontally to the right
                                m.Result = (IntPtr)HTRIGHT;
                        }
                        else
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize diagonally to the left
                                m.Result = (IntPtr)HTBOTTOMLEFT;
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize)) //Resize vertically down
                                m.Result = (IntPtr)HTBOTTOM;
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTBOTTOMRIGHT;
                        }
                    }
                }
                return;
            }
            #endregion

            //Remove border and keep snap window
            if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                return;
            }

            //Keep form size when it is minimized and restored. Since the form is resized because it takes into account the size of the title bar and borders.
            if (m.Msg == WM_SYSCOMMAND)
            {
                /// <see cref="https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand"/>
                /// Quote:
                /// In WM_SYSCOMMAND messages, the four low - order bits of the wParam parameter 
                /// are used internally by the system.To obtain the correct result when testing 
                /// the value of wParam, an application must combine the value 0xFFF0 with the 
                /// wParam value by using the bitwise AND operator.
                int wParam = (m.WParam.ToInt32() & 0xFFF0);

                if (wParam == SC_MINIMIZE)  //Before
                    formSize = this.ClientSize;
                if (wParam == SC_RESTORE)// Restored form(Before)
                    this.Size = formSize;
            }
            base.WndProc(ref m);
        }


        private void fire_PanelMFS1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ıconButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void fire_PanelMFS1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void ıconButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string PortName = comboBox1.SelectedItem.ToString();
            serialPort1.PortName = PortName;
            serialPort1.BaudRate = 57600;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Start();
            try
            {
                if (!serialPort1.IsOpen)
                    serialPort1.Open();
            }
            catch
            {
                MessageBox.Show("Seri Port Seçin!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            serialPort1.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (serialPort1.IsOpen)
            {
                
                try
                {

                    sonuc = serialPort1.ReadExisting();//Serial.print kodu ile gelen analog veriyi alıyoruz,string formatında sonuc'a atıyoruz
                    dSet = sonuc.Split('%')[0];
                    if (!string.IsNullOrEmpty(dSet))
                    {

                        ayırma = sonuc.Replace("%", "").Split('*');
                        label24.Text = ayırma[0];
                        label23.Text = ayırma[1];
                        label25.Text = ayırma[2];

                    }


                    serialPort1.DiscardInBuffer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); // basarısız olursa hata verecek.
                    timer1.Stop();
                }
                
                ates = Convert.ToInt32(label23.Text);
                    Gaz = Convert.ToInt32(label24.Text);
               
               if (325 >= ates && ates >= 1 || Gaz >= 460 && Gaz >= 1 )
                  {

                        SendEmail("muhamedsahin378@gmail.com", "Yangın Var!", "Bursa OsmanGazi Süleyman Şah imamhatip orta okulunda yangın çıktı", "omudurlugu@gmail.com", "", "Password is too long (maximum is 72 characters)", "", 587);
                        System.Threading.Thread.Sleep(10000);
                    }
                
            }
        }

        private void panel10_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

      
    }
}
