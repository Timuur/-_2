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

        public void IzmeniElement(string iText)
        {
            richTextBox1.Text += iText + "\n"; // �������� ����� ��������
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

                BeginInvoke(new MyDelegate(IzmeniElement), data.ToString());

                //label5.Text = (data.ToString()); // TODO: ��������� .ToString

                listener.Send(Encoding.UTF8.GetBytes("�����"));
            }
            
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {

        }

       

        private void button1_Click(object sender, EventArgs e)
        {
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
