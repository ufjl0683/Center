using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinConsoleClient
{


    delegate  void UpdateHandler();
    public partial class Form2 : Form
    {
        System.Net.Sockets.TcpClient tcp;
        string ip;
        int port;
        System.Collections.Queue databuff = new System.Collections.Queue(100);

        string mfccid;
        System.Threading.Thread Cthread;
        public Form2(string mfccid,string ip,int port)
        {
            
           
            InitializeComponent();
            this.port =port;
            this.ip = ip;
            this.mfccid = mfccid;
            this.Text =mfccid+ " ip:" + ip + ",port=" + port;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

           
        }

        void ThreadWork()
        {

            System.IO.StreamReader rd=new System.IO.StreamReader(tcp.GetStream());
            
            while (true)
            {
                try
                {
                    databuff.Enqueue(rd.ReadLine());
                    if (databuff.Count > 100)
                        databuff.Dequeue();


                    this.Invoke(new UpdateHandler(updatewindows));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    try
                    {
                        this.Close();
                    }
                    catch

                    { ;}
                }
                    
                
               
            }
        }

        void updatewindows()
        {
            string data="";

            object[] buf =databuff.ToArray();


            foreach (object s in buf)
                data += s.ToString() + "\r\n";
            textBox1.Text = data;
            textBox1.SelectionStart = textBox1.Text.Length - 1;
            textBox1.ScrollToCaret();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
             //   this.Dispose();
                if (Cthread != null && Cthread.IsAlive)
                    Cthread.Abort();
                

                if (tcp != null && tcp.Connected)

                    tcp.Close();
            }
            catch { ;}
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form2_Activated(object sender, EventArgs e)
        {
           
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            try
            {
                tcp = new System.Net.Sockets.TcpClient();
                tcp.Connect(ip, port);
                Cthread = new System.Threading.Thread(ThreadWork);
                Cthread.Start();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                try
                {
                    if (Cthread != null)
                        Cthread.Abort();
                    this.Close();
                    this.Dispose();
                }
                catch (Exception ex1)
                { MessageBox.Show(ex1.Message); }
            }
        }



    }
}