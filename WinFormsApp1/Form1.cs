using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        const string ip = "127.0.0.1";
        bool isAuth = false;
        const int port = 3336;
        IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public Form1()
        {
            InitializeComponent();
            tcpSocket.Connect(tcpEndPoint);

        }

        private string message_check(string message)
        {
            string[] m1 = message.Split(' ');
            if (!isAuth && m1[0] != "log")
            {
                message = message.Insert(0, "unlog&");
                return message;
            }
            else if (m1[0] == "log")
            {
                message = message.Remove(3, 1);

                message = message.Insert(3, "&");
                return message;
            }
            else
            {
                //m.Insert(0, "");
                return message;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "¬ведите сообщение:";
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

            label2.Text = answer.ToString();
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
            if (label2.Text == "”спех log")
            {
                isAuth = true;
            }
        }
    }
}
