using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace MemoryMatchingGame
{
    public partial class Form2 : Form
    {
        // used to communicate
        Socket sckCommunication;
        EndPoint epLocal, epRemote;

        // buffer to receive info
        byte[] buffer;

        public Form2()
        {
            InitializeComponent();
            button2.Enabled = false;
        }

        // return the own ip
        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // bind socket                        
            epLocal = new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox3.Text));
            sckCommunication.Bind(epLocal);

            // connect to remote ip and port 
            epRemote = new IPEndPoint(IPAddress.Parse(textBox2.Text), Convert.ToInt32(textBox4.Text));
            sckCommunication.Connect(epRemote);

            // starts to listen to an specific port
            buffer = new byte[1464];
            sckCommunication.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                    ref epRemote, new AsyncCallback(OperatorCallBack), buffer);

            // release button to send message
            button2.Enabled = true;
            button1.Enabled = false;
        }

        private void OperatorCallBack(IAsyncResult ar)
        {
            try
            {
                int size = sckCommunication.EndReceiveFrom(ar, ref epRemote);

                // check if theres actually information
                if (size > 0)
                {
                    // used to help us on getting the data
                    byte[] aux = new byte[1464];

                    // gets the data
                    aux = (byte[])ar.AsyncState;

                    // converts from data[] to string
                    System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                    string msg = enc.GetString(aux);

                    // adds to listbox
                    listBox1.Items.Add("Friend: " + msg);
                }

                // starts to listen again
                buffer = new byte[1464];
                sckCommunication.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                        ref epRemote, new AsyncCallback(OperatorCallBack), buffer);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            // set up socket
            sckCommunication = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sckCommunication.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // get own ip
            textBox1.Text = GetLocalIP();
            textBox2.Text = GetLocalIP();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // converts from string to byte[]
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] msg = new byte[1464];
            msg = enc.GetBytes(textBox5.Text);

            // sending the message
            sckCommunication.Send(msg);

            // add to listbox
            listBox1.Items.Add("You: " + textBox5.Text);

            // clear txtMessage
            textBox5.Clear();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // bind socket                        
            epLocal = new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox3.Text));
            sckCommunication.Bind(epLocal);

            // connect to remote ip and port 
            epRemote = new IPEndPoint(IPAddress.Parse(textBox2.Text), Convert.ToInt32(textBox4.Text));
            sckCommunication.Connect(epRemote);

            // starts to listen to an specific port
            buffer = new byte[1464];
            sckCommunication.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                    ref epRemote, new AsyncCallback(OperatorCallBack), buffer);

            // release button to send message
            button2.Enabled = true;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // converts from string to byte[]
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] msg = new byte[1464];
            msg = enc.GetBytes(textBox5.Text);

            // sending the message
            sckCommunication.Send(msg);

            // add to listbox
            listBox1.Items.Add("You: " + textBox5.Text);

            // clear txtMessage
            textBox5.Clear();
        }

        
    }
}
