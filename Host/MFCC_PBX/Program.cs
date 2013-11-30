using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using RemoteInterface;

namespace MFCC_PBX
{
    class Program
    {

      //  static DbCmdServer dbServer = new DbCmdServer();
        static System.Timers.Timer tmr = new System.Timers.Timer(1000 * 60);
        static DateTime lastReceiveTime=DateTime.Now;
        static DateTime lastReceiveTimeF601c = DateTime.Now;
        static bool IsInRead = false;
        static bool IsInReadF601z = false;
        public static MFCC_PBX mfcc_pbx;
        static void Main(string[] args)
        {

            mfcc_pbx = new MFCC_PBX("MFCC_PBX", "PBX", (int)9170, (int)3170,
            (int)8170, "MFCC_PBX", typeof(RemoteObj));


            ConsoleServer.WriteLine("MFCC_PBX Start success!");
        //    ConsoleServer.Start(8170);
            tmr.Elapsed += new System.Timers.ElapsedEventHandler(tmr_Elapsed);
            tmr.Start();

            new System.Threading.Thread(start_PBX_F311z).Start();
            new System.Threading.Thread(start_PBX_F601c).Start();
        }


        static void start_PBX_F601c()
        {
            System.Threading.Thread th = null;

            while (true)
            {
                th = new System.Threading.Thread(PBX_F601c_Task);
                th.Start();
                th.Join();
            }
        }
        static void start_PBX_F311z()
        {
            System.Threading.Thread th = null;

            while (true)
            {
                th = new System.Threading.Thread(PBX_Task);
                th.Start();
                th.Join();
            }

        }

        static void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {
                if (tcp.Connected)
                {
                    tcp.GetStream().WriteByte(0x0d);
                    tcp.GetStream().Flush();
                }
                if (tcpf601c.Connected)
                {
                    tcpf601c.GetStream().WriteByte(0x0d);
                    tcpf601c.GetStream().Flush();
                }
            }
           catch { ;}

            if (((TimeSpan)DateTime.Now.Subtract(lastReceiveTime)).TotalMinutes > 10)
                ConsoleServer.WriteLine("Keep silent over 10 min,IsInRead:" + IsInRead);


            if (((TimeSpan)DateTime.Now.Subtract(lastReceiveTimeF601c)).TotalMinutes > 10)
                ConsoleServer.WriteLine("F601c Keep silent over 10 min,IsInRead:" + IsInReadF601z);


            //  throw new Exception("The method or operation is not implemented.");
        }

     static    TcpClient tcp = null;
     static   TcpClient tcpf601c = null;
        static void PBX_F601c_Task()
        {

     
            System.IO.StreamReader stream = null;
            try
            {
                tcpf601c = new TcpClient();
                tcpf601c.ReceiveTimeout = 1000 * 300;
                tcpf601c.Connect("10.21.41.57", 1001);
                ConsoleServer.WriteLine("PBX cconnected");
                stream = new System.IO.StreamReader(tcpf601c.GetStream());
            }
            catch(Exception ex)
            {
                ConsoleServer.WriteLine("PBX Connect err,retrying");
                System.Threading.Thread.Sleep(10000);
                return;
            }


            while (true)
            {

                string data = "";
              
              //  char [] chars=new char[1];
                try
                {

                    IsInReadF601z = true;
                   //int len = stream.Read(chars, 0, chars.Length);
                    
                    data = stream.ReadLine();
                    IsInReadF601z = false;

                  //  for(int i=0;i<len;i++)
                    ConsoleServer.WriteLine(data);
                    if(data.StartsWith("ED5.1"))
                    {
                        F601cRecord record = new F601cRecord(data);
                        if (record.IsDiscard)
                            continue;
                        else
                        {
                            ConsoleServer.WriteLine(record.ToString());
                            mfcc_pbx.ExecuteSql(record.GetSqlInsertStr());
                        }
                    }
                    // ConsoleServer.Write(Util.ToHexString((byte)tcp.GetStream().ReadByte())+" ");

                    // ConsoleServer.WriteLine(Util.ToHexString(System.Text.ASCIIEncoding.ASCII.GetBytes(data)));

                    if (data == null)
                    {
                        Console.WriteLine("null string");
                        tcpf601c.Close();
                        return;
                    }

                    lastReceiveTimeF601c = DateTime.Now;
                    //if (data.Trim().StartsWith("* #") && data.Trim()[7] == '1')
                    //{
                    //    StandardRecord stdRec;
                    //    ConsoleServer.WriteLine(data);
                    //    stdRec = new StandardRecord(data);

                    //    if (stdRec.PartyBType == "3")
                    //    {
                    //        data = stream.ReadLine();
                    //        data = data.Trim();
                    //        if (data.StartsWith("* #") && data[7] == '2')
                    //        {
                    //            stdRec.type2Rec = new Type2Record(data);
                    //            //  Console.WriteLine(stdRec.type2Rec.extNo);
                    //        }
                    //    }

                    //    ConsoleServer.WriteLine(stdRec.ToString());
                    //    dbServer.SendSqlCmd(stdRec.GetSqlInsertStr());
                    //}
                }
                catch (Exception ex)
                {

                    try
                    {
                        ConsoleServer.WriteLine(ex.Message);
                        tcpf601c.Close();
                    }
                    catch { ;}
                    return;
                }


            }

        }

        static void PBX_Task()
        {
         //   TcpClient tcp = null;
            System.IO.StreamReader stream=null;
            try
            {
                tcp = new TcpClient();
                tcp.ReceiveTimeout = 1000 * 60;
                tcp.Connect("192.168.22.231", 1005);
                ConsoleServer.WriteLine("PBX cconnected");
                stream = new System.IO.StreamReader(tcp.GetStream());
            }
            catch 
            {
                ConsoleServer.WriteLine("PBX Connect err,retrying");
                System.Threading.Thread.Sleep(10000);
                return;
            }


            while (true)
            {
               
                string data = "";
                string[] records; 

                try
                {
                    
                    IsInRead = true;
                    data = stream.ReadLine();
                    IsInRead = false;
                    
                 // ConsoleServer.Write(Util.ToHexString((byte)tcp.GetStream().ReadByte())+" ");
               
               // ConsoleServer.WriteLine(Util.ToHexString(System.Text.ASCIIEncoding.ASCII.GetBytes(data)));

                if (data == null)
                {
                    Console.WriteLine("null string");
                    tcp.Close();
                    return;
                }
                lastReceiveTime = DateTime.Now;
                if (data.Trim().StartsWith("* #") && data.Trim()[7] == '1')
                {
                    StandardRecord stdRec;
                    ConsoleServer.WriteLine(data);
                    stdRec=new StandardRecord(data);
                  
                    if(stdRec.PartyBType=="3")
                    {
                       data= stream.ReadLine();
                       data = data.Trim();
                       if (data.StartsWith("* #") && data[7] == '2')
                       {
                           stdRec.type2Rec = new Type2Record(data);
                         //  Console.WriteLine(stdRec.type2Rec.extNo);
                       }
                    }

                    ConsoleServer.WriteLine(stdRec.ToString());
                    mfcc_pbx.ExecuteSql(stdRec.GetSqlInsertStr());
                }
            }
            catch (Exception ex)
            {
               
                try
                {
                    ConsoleServer.WriteLine(ex.Message);
                    tcp.Close();
                }
                catch { ;}
                return;
            }


            }

            



        }

        public class F601cRecord
        {
            string record;
            public F601cRecord(string record)
            {
                this.record = record;
            }

            public string PartyAType
            {
                get
                {
                    if (this.ExtNo == "65535")
                        return "1";
                    else if (ExtNo == "3444")
                    {
                        return "5";
                    }
                    else
                        return "1";
                }
            }

            public string PartyBType
            {
                get
                {
                    if (this.ExtNo == "65535")
                        return "1";
                    else if (ExtNo == "3444")
                    {
                        return "1";
                    }
                    else
                        return "5";
                }
            }

            public string PartyA
            {
                get
                {
                    if(PartyAType=="1" && PartyBType=="1")
                             return record.Substring(6-1,35-6+1).Trim();
                    else
                             return record.Substring(36-1, 65 - 36 + 1).Trim();
                }
            }

            public string PartyB
            {
                get
                {
                    if (PartyAType == "1" && PartyBType == "1")
                            return record.Substring(36-1, 65 - 36 + 1).Trim();
                    else
                            return record.Substring(6-1, 35 - 6 + 1).Trim();

                }
               
            }

            public string ExtNo
            {
                get
                {
                    return record.Substring(212-1, 216 - 212 + 1).Trim();
                }
            }

            public int CallDuration
            {
                get
                {
                    return System.Convert.ToInt32(record.Substring(202-1,211-202+1).Trim());
                }
            }

            public DateTime DateTime
            {
                get
                {
                    int year = System.Convert.ToInt32(record[442-1].ToString() + record[443-1] + record[444-1] + record[445-1]);
                    int mon = System.Convert.ToInt32(record[446-1].ToString() + record[447-1] );
                    int day = System.Convert.ToInt32(record[448-1].ToString() + record[449-1]);
                    int hr = System.Convert.ToInt32(record[451-1].ToString() + record[452-1]);
                    int min = System.Convert.ToInt32(record[454-1].ToString() + record[455-1]);
                    int sec = System.Convert.ToInt32(record[457-1].ToString() + record[458-1]);
                    return new DateTime(year, mon, day, hr, min, sec);
                }
            }

            public string RefNo
            {
                get
                {
                    return record.Substring(349-1, 358 - 349 + 1).Trim();
                }
            }

            public bool IsDiscard
            {

                get
                {
                    if (ExtNo == "65535" && (PartyA == "4010" || PartyA == "0411" || PartyA == "4012" || PartyA == "4013"))
                        return true;
                    else if (PartyA.StartsWith("A"))
                        return true;
                    else
                        return false;
                }

             
            }

            public string GetSqlInsertStr()
            {
                string sqlStr = "insert into tblPBXLog (refno,time,partya,partyb,partyatype,partybtype,duration,extphoneno,ClASSIFY) values('{0}','{1}','{2}','{3}',{4},{5},{6},'{7}','{8}')";

                return string.Format(sqlStr, this.RefNo, DbCmdServer.getTimeStampString(this.DateTime), this.PartyA, this.PartyB, this.PartyAType, this.PartyBType,
                    this.CallDuration,this.ExtNo,"F601C");
            }


            public override string ToString()
            {

                return "refno:" + this.RefNo + "\n" +
                       "time:" + this.DateTime + "\n" +
                       //"type:" + this.Type + "\n" +
                        "PartyA:" + this.PartyA + "\n" +
                        "PartyB:" +this.PartyB+ "\n" +
                         "PartyAType:" + this.PartyAType + "\n" +

                         "PartyBType:" + this.PartyBType + "\n" +
                         "Duration:" + this.CallDuration + "\n" + "extPhoneNo:" + this.ExtNo + "\n";

            }

        }
        public class Type2Record
        {
            string record;
            public Type2Record(string record)
                {
                    this.record = record;
                }


            public string extNo
            {
                get{
                    return record.Substring(8, 20).Trim();
                }
            }
        }

        public class StandardRecord
        {
            string record;

            public Type2Record type2Rec=null;
            public StandardRecord(string record)
            {
                this.record = record;
            }

            public string RefNo
            {
                get
                {
                    return record.Substring(3, 4);
                }
            }

            public char Type
            {
                get
                {
                    return record[7];
                }
            }
            public DateTime DateTime
            {
                get
                {
                    int year = 2000 + System.Convert.ToInt32(record[8].ToString() + record[9]);
                    int month = System.Convert.ToInt32(record[10].ToString() + record[11]);
                    int day = System.Convert.ToInt32(record[12].ToString() + record[13]);
                    int hour = System.Convert.ToInt32(record[14].ToString() + record[15]);
                    int min = System.Convert.ToInt32(record[16].ToString() + record[17]);
                    int sec = System.Convert.ToInt32(record[18].ToString() + record[19]);
                    return new DateTime(year, month, day, hour, min, sec);
                }

            }

            
            public string LocalIdA
            {
                get
                {
                   return record.Substring(20, 12).Trim();
                }
            }

            public string FarEndIdA
            {
                get
                {
                  return  record.Substring(32, 20).Trim();

                }
            }
            public string LocalIdB
            {
                get
                {
                    return record.Substring(52, 12).Trim();
                }
            }

            public string FarEndIdB
            {
                get
                {
                    return record.Substring(64, 20).Trim();

                }
            }

            public string PartyAType
            {
                get
                {
                    return record[84].ToString();
                }
            }

            public string PartyBType
            {
                get
                {
                    return record[85].ToString();
                }
            }
            public int CallDuration
            {
                get
                {
                    return  System.Convert.ToInt32(record.Substring(record.Length-6,6));
                }
            }

            public override string ToString()
            {

                return "refno:" + this.RefNo + "\n" +
                       "time:" + this.DateTime + "\n" +
                       "type:" + this.Type + "\n" +
                        "PartyA:" + this.LocalIdA + " " + this.FarEndIdA + "\n" +
                        "PartyB:" + this.LocalIdB + " " + this.FarEndIdB + "\n" +
                         "PartyAType:" + this.PartyAType + "\n" +
                            
                         "PartyBType:" + this.PartyBType + "\n"+
                         "Duration:" + this.CallDuration + "\n" + ((type2Rec != null) ? "extPhoneNo:" + type2Rec.extNo : "extPhoneNo:" + "\n");




            }

            public string GetSqlInsertStr()
            {
                string sqlStr = "insert into tblPBXLog (refno,time,partya,partyb,partyatype,partybtype,duration,extphoneno,Classify) values('{0}','{1}','{2}','{3}',{4},{5},{6},'{7}','{8}')";

                return string.Format(sqlStr, this.RefNo, DbCmdServer.getTimeStampString(this.DateTime), this.LocalIdA + " " + this.FarEndIdA, this.LocalIdB + " " + this.FarEndIdB, this.PartyAType, this.PartyBType,
                    this.CallDuration, (type2Rec != null) ? type2Rec.extNo : "","F311Z");
            }
        }


        

        
        
    }




}
