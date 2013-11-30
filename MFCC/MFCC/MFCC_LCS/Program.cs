using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_LCS
{
    class Program
    {
        public static MFCC_LCS mfcc_lcs;
        static void Main(string[] args)
        {

            mfcc_lcs= new MFCC_LCS("MFCC_LCS", "LCS", (int)RemoteInterface.RemotingPortEnum.MFCC_LCS,
               (int)RemoteInterface.NotifyServerPortEnum.MFCC_LCS, (int)RemoteInterface.ConsolePortEnum.MFCC_LCS, "MFCC_LCS", typeof(RemoteObj));
            Console.WriteLine("mfcc stated");

         //   LCS_Test();
        }

        static void LCS_Test()
        {

            int sign_cnt = 32;
            System.Data.DataRow DShow;

            //連Host
            //RemoteInterface.HC.I_HC_FWIS robj_Host = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS)
            //, RemoteInterface.RemoteBuilder.getRemoteUri("192.168.22.89", (int)RemoteInterface.RemotingPortEnum.HOST, "FWIS"));            

            //連Mfcc
            //I_MFCC_LCS robj_Mfcc = (I_MFCC_LCS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(I_MFCC_LCS)
            //        , RemoteBuilder.getRemoteUri("192.168.22.89", (int)RemotingPortEnum.MFCC_LCS, "MFCC_LCS"));            

            System.Data.DataSet DS =  mfcc_lcs.getSendDsByFuncName("set_ctl_sign");
           

            DS.Tables[0].Rows[0]["sign_cnt"] = Convert.ToByte(sign_cnt);

            for (int i = 0; i <= sign_cnt - 1; i++)
            {

                DShow = DS.Tables[1].NewRow();
                DShow["sign_no"] = Convert.ToByte(i);
                DShow["sign_status"] = Convert.ToByte(0);
                DS.Tables[1].Rows.Add(DShow);
            }

            DS.Tables[1].Rows[0]["sign_status"] = Convert.ToByte(2);
            DS.Tables[1].Rows[8]["sign_status"] = Convert.ToByte(2);
            DS.Tables[1].Rows[16]["sign_status"] = Convert.ToByte(2);
            DS.Tables[1].Rows[24]["sign_status"] = Convert.ToByte(2);

            DS.Tables[1].Rows[1]["sign_status"] = Convert.ToByte(8);
            DS.Tables[1].Rows[9]["sign_status"] = Convert.ToByte(8);
            DS.Tables[1].Rows[17]["sign_status"] = Convert.ToByte(8);
            DS.Tables[1].Rows[25]["sign_status"] = Convert.ToByte(8);

            DS.AcceptChanges();

            //SendTC
            //try
            //{ DataSet retds = robj_Mfcc.sendTC("LCS-TEST-234", DS); }
            //catch (Exception ex)
            //{ MessageBox.Show(ex.Message); }   

            //Host            
            //RemoteInterface.HC.LCSOutputData LcsData = new RemoteInterface.HC.LCSOutputData(DS);
            //try
            //{ robj_Host.SetOutput("LCS-TEST-234", LcsData, true); }
            //catch (Exception ex)
            //{ MessageBox.Show(ex.Message); }

            ////Mfcc
            try
            {
                ((Comm.TC.LCSTC)mfcc_lcs.getTcManager()["LCS-N6-W-19.5"]).TC_SetDislay(DS);
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
           
        }
    }
}
