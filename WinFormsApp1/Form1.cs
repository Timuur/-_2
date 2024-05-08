using System.Net.Sockets;
using System.Net;
using System.Text;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        const string ip = "127.0.0.1";
        const int port = 3336;
        IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public Form1()
        {
            InitializeComponent();
            tcpSocket.Connect(tcpEndPoint);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "¬ведите сообщение:";
            var message = textBox1.Text;

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
    }
}
