using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RemoteInterface.MFCC;

namespace WinConsoleClient
{


  
    public partial class FrmMoniter : Form{
    
        RemoteInterface.EventNotifyClient nclient;
        
     //   System.Net.Sockets.TcpClient tcp;
        string ip;
        int port;
        System.Collections.Queue databuff = new System.Collections.Queue(100);
        RemoteInterface.MFCC.I_MFCC_Base robj;
        string mfccid;
   //     System.Threading.Thread Cthread;
        string devName = "";
        System.IO.StreamWriter sw = null;
        public FrmMoniter(string mfccid, string ip, int Consoleport,string devName)
        {
            
           
            InitializeComponent();
            this.port =Consoleport;
            this.ip = ip;
            this.mfccid = mfccid;
            this.Text =mfccid+ " ip:" + ip + ",port=" + port+"-"+devName;
            this.devName = devName;
             
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           // 
            try
            {
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
              
           
        }

        void nclient_OnEvent(object sender, RemoteInterface.NotifyEventObject objEvent)
        {
          
            try
            {
                if (objEvent.type == RemoteInterface.EventEnumType.MFCC_Comm_Moniter_Event)
                {
                    string comdata = System.Convert.ToString(objEvent.EventObj);
                    if (this.toolStripButton1.Text != "開始")
                        System.IO.File.AppendAllText(txtPathFile.Text, comdata+"\r\n");
                    databuff.Enqueue(comdata);
                    if (databuff.Count > 100)
                        databuff.Dequeue();


                    this.Invoke(new UpdateHandler(updatewindows));
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        void nclient_OnConnect(object sender)
        {
            nclient.OnEvent += new RemoteInterface.NotifyEventHandler(nclient_OnEvent);
            nclient.RegistEvent(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.MFCC_Comm_Moniter_Event,devName,null));
                
          
        }

        //void ThreadWork()
        //{

        //   System.IO.StreamReader rd=new System.IO.StreamReader(tcp.GetStream());
            
        //    while (true)
        //    {
        //        try
        //        {
        //            databuff.Enqueue(rd.ReadLine());
        //            if (databuff.Count > 100)
        //                databuff.Dequeue();


        //            this.Invoke(new UpdateHandler(updatewindows));
        //        }
        //        catch(Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //            try
        //            {
        //                this.Close();
        //            }
        //            catch

        //            { ;}
        //        }
                    
                
               
        //    }
        //}

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
                //if (Cthread != null && Cthread.IsAlive)
                //    Cthread.Abort();
                nclient.close();
                robj.setDeviceCommMointer(this.devName, false);

                //if (tcp != null && tcp.Connected)

                //    tcp.Close();
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

                if (mfccid.StartsWith("MFCC_VD"))
                    mfccid = "MFCC_VD";
                else if (mfccid == "MFCC_CMSRST")
                    mfccid = "MFCC_CMSRST";
                else if (mfccid.StartsWith("MFCC_CMS"))
                    mfccid = "MFCC_CMS";
                else if (mfccid.StartsWith("MFCC_CSLS"))
                    mfccid = "MFCC_CSLS";
             

                robj = (RemoteInterface.MFCC.I_MFCC_Base)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_Base),
                    RemoteInterface.RemoteBuilder.getRemoteUri(ip, port + 1000, mfccid));

                robj.setDeviceCommMointer(devName,true);
                this.nclient = new RemoteInterface.EventNotifyClient(ip, port - 5000, false);

                nclient.OnConnect += new RemoteInterface.OnConnectEventHandler(nclient_OnConnect);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                try
                {
                   
                
                    this.Close();
                    this.Dispose();
                }
                catch (Exception ex1)
                { MessageBox.Show(ex1.Message); }
            }
        }

        
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try{
                if (toolStripButton1.Text == "開始")
                {
                    if (txtPathFile.Text.Trim() == "")
                    {
                        MessageBox.Show("必須要有檔名");
                        return;
                    }

                    toolStripButton1.Text = "結束";

                    this.txtPathFile.Enabled = false;
                }
                else
                {
                    toolStripButton1.Text = "開始";
                    this.txtPathFile.Enabled = true;
                }

                }catch(Exception ex)
               {
                    MessageBox.Show(ex.Message);
                }

        }



    }
}