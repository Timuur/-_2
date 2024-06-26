using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Security;
using System.Drawing;
using System;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;

namespace Klient2
{
    public partial class Form1 : Form
    {
        const string ip = "127.0.0.2";
        bool isAuth = false;
        const int port = 3336;
        IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        Thread poto1 = null;
        Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        string login = null;


        public delegate void MyDelegate(string iText);

        public Form1()
        {
            InitializeComponent();

            try
            {
                tcpSocket.Connect(tcpEndPoint);

            }
            catch (SocketException e141)
            {
                MessageBox.Show(e141.Message);
            }
            poto1 = new Thread(waa_it);

            poto1.Start();
        }
        public void IzmeniElement(string iText)
        {
            richTextBox1.Text += iText + "\n"; // �������� ����� ��������
            //listener.Send(Encoding.UTF8.GetBytes("�����!" + iText));
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }

        private string message_check(string message)
        {
            string[] m1 = message.Split(' ');
            if (!isAuth && m1[0] != "log")
            {
                message = message.Insert(0, "unlog&");
                return message;
            }
            else if (!isAuth && m1[0] == "log")
            {
                login = m1[1];
                string password = CreateMD5(m1[2]);

                message = "log&" + login + " " + password;

                return message;
            }
            else
            {
                message = login + ": " + message;
                return message;
            }
        }
        private string answer_check(string message)
        {
            string[] m1 = message.Split('!');
            if (m1[0] == "�����")
            {
                //BeginInvoke(new MyDelegate(IzmeniElement), m1[1] + "\n");
                richTextBox1.Text += m1[1] + "\n";
                return m1[0];
            }
            else
            {
                return message;
            }
        }

        public void waa_it()
        {
            byte[] data = new byte[1024];
            int recv = tcpSocket.Receive(data);
            string answ = Encoding.UTF8.GetString(data, 0, recv);
            BeginInvoke(new MyDelegate(IzmeniElement), answ);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread.Sleep(1000);

            //poto1 = new Thread(waa_it);

            //poto1.Start();

            //label1.Text = "������� ���������:";
            string message = textBox1.Text;
            message = message_check(message);

            var data = Encoding.UTF8.GetBytes(message);

            tcpSocket.Send(data);

            var buffer = new byte[1024];
            var answer = new StringBuilder();
            int size = tcpSocket.Receive(buffer);

            //do
            //{
            answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
            //}
            //while (tcpSocket.Available > 0);
            string answr = answer_check(answer.ToString());

            label2.Text = answr;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            tcpSocket.Shutdown(SocketShutdown.Both);
            tcpSocket.Close();
        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != 8 && !Char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void label2_TextChanged(object sender, EventArgs e)
        {
            if (label2.Text == "����� log")
            {
                isAuth = true;
                label1.Text = $"����, {login}!";
                button1.Text = "�����";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isAuth = false;
            button1.Text = "����";
            label1.Text = "";
        }
    }
}
