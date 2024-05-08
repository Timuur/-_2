using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        const string ip = "127.0.0.1";
        const int port = 3336;
        IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Thread potok1 = null;
        Socket listener;

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

            MessageBox.Show(a[0]);
        }

        public void IzmeniElement(string iText)
        {
            richTextBox1.Text += iText + "\n"; // �������� ����� ��������
            listener.Send(Encoding.UTF8.GetBytes("�����!"+ iText));
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
                        listener.Send(Encoding.UTF8.GetBytes("����� log"));
                    }
                    else
                    {
                        listener.Send(Encoding.UTF8.GetBytes("un����� log"));
                    }
                    line = sr.ReadLine();
                }
                while (line != null);

                sr.Close();
            }
            catch (Exception e)
            {
                listener.Send(Encoding.UTF8.GetBytes("Exception: " + e.Message));
            }
            //finally
            //{
            //    listener.Send(Encoding.UTF8.GetBytes("Executing finally block."));
            //}

        }


        public void waait()
        {
            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(5);
            listener = tcpSocket.Accept();
            while (true)
            {
                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();

                do
                {
                    size = listener.Receive(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (listener.Available > 0);

                BeginInvoke(new MyDelegate(choose), data.ToString());

                //label5.Text = (data.ToString()); // TODO: ��������� .ToString

                //listener.Send(Encoding.UTF8.GetBytes("�����"));
            }

        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {

        }



        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "������ �������";
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

            // ��� 1 - ����� ��� �� ������� ������
            // �������� ���������� ������
            potok1.Start(); // ������ ������
                            // ��� 2 - ����� ��� ����� ������� ������� ������
                            // ��� 3 - ����� ��� ����� ������� ������� ������
            button1.Visible = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // // ��������� ������
            //try
            //{
            //    potok1.Abort();// Code that is executing when the thread is aborted.  
            //}
            //catch (PlatformNotSupportedException ex)
            //{
            //    // Clean-up code can go here.  
            //    // If there is no Finally clause, ThreadAbortException is  
            //    // re-thrown by the system at the end of the Catch clause.
            //}
            listener.Shutdown(SocketShutdown.Both);
            listener.Close();



            //��������� ������� ������ ��������
            //cancelTokenSource.Cancel();
        }
    }

}
