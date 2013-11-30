using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_SVWS
{
    public class SVWSTC
    {
        Comm.V2DLE dledev;
        Comm.Protocol protocol;

        string ip;
        int port;
        System.Net.Sockets.TcpClient tcp;
        public SVWSTC(Comm.Protocol protocol,string ip,int port)
        {
            this.protocol = protocol;
            new System.Threading.Thread(Communication_Task).Start();
            this.ip = ip;
            this.port = port;
        }
        void Communication_Task()
        {
            while (true)
            {
                try
                {
                    ConsoleServer.WriteLine("Connecting...!");
                    Init_Communication();
                    lock (this)
                        System.Threading.Monitor.Wait(this);



                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message);
                }
                finally
                {
                    System.Threading.Thread.Sleep(1000 * 30);
                }
            }
        }


        void Init_Communication()
        {
            tcp = new System.Net.Sockets.TcpClient();

            tcp.Connect(this.ip, this.port);
            if (dledev != null)
            {
                dledev.OnCommError -= new Comm.OnCommErrHandler(dledev_OnCommError);
                dledev.OnReport -= new Comm.OnTextPackageEventHandler(dledev_OnReport);
                dledev.Close();
            }

            dledev = new Comm.V2DLE("SVWS", tcp.GetStream());

            dledev.OnCommError += new Comm.OnCommErrHandler(dledev_OnCommError);
            dledev.OnReport += new Comm.OnTextPackageEventHandler(dledev_OnReport);
            dledev.OnBeforeAck += new Comm.OnSendingAckNakHandler(dledev_OnBeforeAck);




        }

        void dledev_OnBeforeAck(object sender, ref byte[] data)
        {
            //throw new Exception("The method or operation is not implemented.");
            // data = new byte[0];
        }

        void dledev_OnReport(object sender, Comm.TextPackage txtObj)
        {
            // throw new Exception("The method or operation is not implemented.");

            try
            {
                if (txtObj.Text[0] == 0x10)
                {

                    System.Data.DataSet ds = protocol.GetSendDsByTextPackage(txtObj, Comm.CmdType.CmdReport);
                    string mesg = "";
                    for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                    {
                        mesg += ds.Tables[0].Columns[i].ColumnName + ":" + ds.Tables[0].Rows[0][ds.Tables[0].Columns[i].ColumnName].ToString() + "\t";
                    }
                    ConsoleServer.WriteLine(mesg);
                    SVWS_Status svws_status = new SVWS_Status(ds);
                    Program.mfcc_svws.stausmgr.AddSVWS_Status(svws_status);
                    

                }
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }

            if (txtObj.Text[0] == 0x11)
            {
                System.Data.DataSet ds = protocol.GetSendDsByTextPackage(txtObj, Comm.CmdType.CmdReport);
                string mesg = "";
                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    mesg += ds.Tables[0].Columns[i].ColumnName + ":" + ds.Tables[0].Rows[0][ds.Tables[0].Columns[i].ColumnName].ToString() + "\t";
                }
                ConsoleServer.WriteLine(mesg);
                SVWS_Status svws_status = new SVWS_Status(ds);
                Program.mfcc_svws.stausmgr.AddSVWS_Status(svws_status);
            }
            else

                if (txtObj.Text[0] == 0x12)
                {
                    lock (dledev.GetStream())
                    {
                        byte[] data = dledev.PackData(0xffff, new byte[]{0x12,(byte)(DateTime.Now.Year%100),(byte)DateTime.Now.Month,(byte) DateTime.Now.Day,
                           (byte) DateTime.Now.Hour, (byte) DateTime.Now.Minute, (byte) DateTime.Now.Second}, (byte)txtObj.Seq);
                        dledev.GetStream().Write(data, 0, data.Length);
                    }
                }

        }

        void dledev_OnCommError(object sender, Exception ex)
        {

            lock (this)
            {
               
                try
                {
                    dledev.Close();
                }
                catch
                {
                }
                System.Threading.Monitor.Pulse(this);
            }
            // throw new Exception("The method or operation is not implemented.");
        }
    }
}
