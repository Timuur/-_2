using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        //const string ip = "26.9.58.34";
        const int port = 3336;
        IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Any, port);

        Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Thread potok1 = null;
        Socket listener;
        bool isauth = false;

        public delegate void MyDelegate(string iText);

        //CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        public Form1()
        {
            InitializeComponent();
        }

        private void choose(string iText)
        {
            var a = iText.Split('&');
            switch (a[0])
            {
                case "log":
                    log_check(a[1]);
                    break;
                case "unlog":
                    listener.Send(Encoding.UTF8.GetBytes("unlog"));
                    break;
                default:
                    IzmeniElement(iText);
                    break;
            }
        }

        public void IzmeniElement(string iText)
        {
            richTextBox1.Text += iText + "\n"; // изменили текст элемента
            listener.Send(Encoding.UTF8.GetBytes("Успех!" + iText));
        }
        public void log_check(string iText)
        {
            var a = iText.Split(' ');
            string line;
            try
            {
                StreamReader sr = new StreamReader("TextFile1.txt");
                line = sr.ReadLine();
                do
                {
                    var log_pad = line.Split(' ');
                    if (a[0] == log_pad[0] && a[1] == log_pad[1])
                    {
                        isauth = true;
                        break;
                    }
                    else
                    {
                        isauth = false;
                    }
                    line = sr.ReadLine();
                }
                while (line != null);
                if (isauth)
                    listener.Send(Encoding.UTF8.GetBytes("Успех log"));
                else
                    listener.Send(Encoding.UTF8.GetBytes("unУспех log"));
                isauth = false;
                sr.Close();
            }
            catch (Exception e)
            {
                listener.Send(Encoding.UTF8.GetBytes("Exception: " + e.Message));
            }
        }


        public void waait()
        {
            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(10);
            listener = tcpSocket.Accept();
            IPEndPoint clientep = (IPEndPoint)listener.RemoteEndPoint;


            BeginInvoke(new MyDelegate(IzmeniElement), $"Connected with {clientep.Address} at port {clientep.Port}" + "\n");
            while (true)
            {
                var data = new byte[1024];
                var recv = listener.Receive(data);

                if(recv == 0) { break; }
                string mess = Encoding.UTF8.GetString(data, 0, recv);
                BeginInvoke(new MyDelegate(choose), mess);

                //label5.Text = (data.ToString()); // TODO: проверить .ToString

                //listener.Send(Encoding.UTF8.GetBytes("Успех"));
            }

        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Сервер запущен";
            //CancellationToken token = cancelTokenSource.Token;

            //Task task = new Task(() =>
            //{
            //    waait();
            //    //Work
            //    if (token.IsCancellationRequested)
            //    {
            //        //cancel
            //        return;
            //    }
            //    //Work
            //});
            //task.Start();
            potok1 = new Thread(waait);

            potok1.Start();
            button1.Visible = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            listener.Shutdown(SocketShutdown.Both);
            listener.Close();

            //повторное нажатие кнопки вызывает
            //cancelTokenSource.Cancel();
        }
    }

}
