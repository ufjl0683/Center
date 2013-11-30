using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Comm
{
    
   public  class Cmd
    {

       public byte cmd;
       public byte subCmd = 0xff;
       public CmdType cmdType;
       public string cmdName;
       public string description;
       public CmdClass cmdClass;
       public bool CanTest = false;
       public System.Collections.ArrayList SendCmdItems=System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
       public System.Collections.ArrayList ReturnCmdItems =System.Collections.ArrayList.Synchronized( new System.Collections.ArrayList());
       public System.Collections.ArrayList TestGroupValues = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
       private DataSet sendDs;
       private DataSet returnDs;
       Protocol protocol;
       public Cmd(Protocol protocol)
       {

           this.protocol = protocol;
       }

       public Cmd(byte cmd)
       {
          this.cmd = cmd;
          // this.CmdType = type;
       }

       public Cmd(byte cmd,byte subCmd)
       {
           this.cmd = cmd;
           this.subCmd = subCmd;
         //  this.CmdType = type;
       }

       internal void AddSendCmdItem(CmdItem item)
       {
           SendCmdItems.Add(item);
       }
       internal void AddReportCmdItem(CmdItem item)
       {
          ReturnCmdItems.Add(item);
       }

       //internal void AddTestItemValue(TestValue value)
       //{
       //    this.TestItemValues.Add(value);
       //}

       public string Key
       {

           get
           {
               switch(this.cmd)
               {
                   case 0x04:

                       //if ((this.cmd & 0x0f) == 0x0f)
                       //{
                           byte protocol_code,sub_protocol_code;

                           System.Collections.IEnumerator ie = this.GetEnumCmds(this.SendCmdItems).GetEnumerator();
                           while (ie.MoveNext())
                           {
                               CmdItem item = (CmdItem)ie.Current;
                               if (item.ItemName == "protocol_code")
                               {

                                   if (item.Bytes == 1)
                                   {
                                       protocol_code = (byte)item.Min;
                                       sub_protocol_code = 0xff;
                                       return V2DLE.ToHexString(new byte[] { this.cmd, (byte)this.findCmditem(this.SendCmdItems, "protocol_code").Min }).Replace(' ', '_') + this.cmdType;
                                   }
                                   else
                                   {
                                       protocol_code = (byte)((item.Min >> 8) & 0xff);
                                       sub_protocol_code = (byte)(item.Min & 0xff);
                                       return V2DLE.ToHexString(new byte[] { this.cmd, protocol_code, sub_protocol_code }).Replace(' ', '_') + this.cmdType;
                                   }
                                 
                                  // ie.MoveNext();

                                 //  sub_protocol_code =(byte) ((CmdItem)ie.Current).Min;
                                 //  return V2DLE.ToHexString(new byte[] { this.cmd, protocol_code, sub_protocol_code }).Replace(' ', '_') + this.cmdType;
                                  
                               }
                           }

                           throw new Exception("Can't find protocol_code or sub_protocol_code!");
                     //  }
                    //   else
                    //       return V2DLE.ToHexString(new byte[] { this.cmd, (byte)this.findCmditem(this.SendCmdItems, "protocol_code").Min }).Replace(' ', '_') + this.cmdType;
                       break;
                   case 0x52:  // cms
                         if(this.SendCmdItems.Count==0)
                             return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                         else
                             return V2DLE.ToHexString(new byte[]{cmd,0x01}).Replace(' ','_')+this.cmdType;
                         break;

                    
                   case 0x5f: //mas 5f+22
                       if (this.subCmd == 0x22)
                       {
                           if (this.SendCmdItems.Count == 1)
                               return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                           else
                               return V2DLE.ToHexString(new byte[] { cmd, 0xFE }).Replace(' ', '_') + this.cmdType;
                       }
                       else if (this.subCmd == 0x27)  //mas 5f 27
                       {
                           if (!(this.findCmditem(this.SendCmdItems, "data_type").Max == 0)) //graphic,or default msg
                               return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                           else
                               return V2DLE.ToHexString(new byte[] { cmd, 0xfd }).Replace(' ', '_') + this.cmdType;
                       }
                       else if (this.subCmd == 0x12  && this.protocol.DeviceType=="TTS")  //tts
                       {
                           if (this.SendCmdItems.Count == 1)
                               return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                           else
                               return V2DLE.ToHexString(new byte[] { cmd, 0XFE }).Replace(' ', '_') + this.cmdType;
                       }
                       else if (this.subCmd == 0x02   &&  (this.protocol.DeviceType =="CMS" || this.protocol.DeviceType =="CMSRST"))
                       {
                           if (!(this.findCmditem(this.SendCmdItems, "data_type").Max == 0)) //graphic,or default msg
                               return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                           else
                               return V2DLE.ToHexString(new byte[] { cmd, 0xfe }).Replace(' ', '_') + this.cmdType;
                       }

                       else
                           goto default;
                   
                  
                   case 0x57:
                       goto case 0x54;
                   case 0x54: //cms

                         if(!(this.findCmditem(this.SendCmdItems,"data_type").Max==0)) //graphic,or default msg
                             return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                         else
                             return V2DLE.ToHexString(new byte[] { cmd, 1}).Replace(' ', '_') + this.cmdType;


                   case 0xAD://RMS
                       if (this.SendCmdItems.Count == 1)
                           return   V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                       else
                           return V2DLE.ToHexString(new byte[] { cmd, 0x01 }).Replace(' ', '_') + this.cmdType;

                        //   break;
                   case 0xb6:
                       if(this.SendCmdItems.Count==0)
                           return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                       else
                           return V2DLE.ToHexString(new byte[] { cmd, 1}).Replace(' ', '_') + this.cmdType;

                   //case 0xd6:
                   //    if (this.SendCmdItems.Count == 0)
                   //        return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                   //    else
                   //        return V2DLE.ToHexString(new byte[] { cmd, 1 }).Replace(' ', '_') + this.cmdType;
                   case 0xDf:
                       if (this.subCmd == 0xd2)
                       {
                           if (this.SendCmdItems.Count == 0)
                               return V2DLE.ToHexString(new byte[] { cmd, subCmd, 0x01 }).Replace(' ', '_') + this.cmdType;
                           else
                               return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;


                       }
                      // else if (this.subCmd==0xd5)
                      // {
                          

                      //     if((this.findCmditem(this.ReturnCmdItems,"data_type").Min!=0 )) //graphic,or default msg
                      //        return V2DLE.ToHexString(new byte[] { cmd, subCmd, 0x01 }).Replace(' ', '_') + this.cmdType;
                      //   else
                      //        return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;




                      //}
                      else if (this.subCmd == 0xd7)
                      {
                          if ((this.findCmditem(this.SendCmdItems, "data_type").RangeType== RangeType.Select)) //graphic,or default msg
                              return V2DLE.ToHexString(new byte[] { cmd, subCmd, 0x01 }).Replace(' ', '_') + this.cmdType;
                          else
                              return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                      }
                      else
                          goto default;
                     
                   default:
                        return V2DLE.ToHexString(new byte[]{cmd,subCmd}).Replace(' ','_')+this.cmdType;
               }
              
               
           }
       }

       public CmdItem getCmdSendItem(string itemName)
       {
         return  this.findCmditem(this.SendCmdItems, itemName);
       }
       public CmdItem getCmdReturnItem(string itemName)
       {
          return  this.findCmditem(this.ReturnCmdItems, itemName);
       }
       private CmdItem findCmditem(System.Collections.ArrayList cmdItems,string itemName)
       {
           foreach (CmdItem item in cmdItems)
           {
               if (item.ItemName == itemName)
                   return item;
               if (item.HasSubItems)
               {
                   CmdItem tmpItem;
                   if ((tmpItem=findCmditem(item.SubItems, itemName)) != null)
                         return tmpItem ;
               }

           }
           return null;
       }

       private TestValue findTestitem(System.Collections.ArrayList cmdItems, string itemName)
       {
           foreach (TestValue item in cmdItems)
           {
               if (item.itemName == itemName)
                   return item;
               if (item.hasSubValues)
               {
                   TestValue tmpItem;
                   if ((tmpItem = findTestitem(item.subValues, itemName)) != null)
                       return tmpItem;
               }

           }
           return null;
       }

       public System.Collections.IEnumerable GetSendCmdEnum()
       {
           return GetEnumCmds(SendCmdItems);
       }
       public System.Collections.IEnumerable GetReturnCmdEnum()
       {
           return GetEnumCmds(ReturnCmdItems);
       }

       private System.Collections.IEnumerable GetEnumCmds(System.Collections.ArrayList cmdItems)
       {
           foreach (CmdItem item in cmdItems)
           {
             
               yield     return item;
               if (item.HasSubItems)
               {
                   foreach (CmdItem tmpItem in item.SubItems)
                   
                    
                           yield return tmpItem;
                   
               }

           }
          // return null;
       }

       public System.Collections.IEnumerable GetTestEnum(int testGroupId)
       {
           return GetEnumTestValues((System.Collections.ArrayList)TestGroupValues[testGroupId]);
       }

       private System.Collections.IEnumerable GetEnumTestValues(System.Collections.ArrayList testItems)
       {
           foreach (TestValue item in testItems)
           {

               yield return item;
               if (item.hasSubValues)
               {
                   foreach (TestValue tmpItem in item.subValues)


                       yield return tmpItem;

               }

           }
           // return null;
       }

       public override string ToString()
       {
           //return base.ToString();
         
          return string.Format("[cmd=0x{0:X2} subcmd={3:X2},type{1},'{2}' ]  ", this.cmd, this.cmdType, this.cmdName,this.subCmd);
           
       }

      internal protected  void CheckTestValue()
       {
         //  bool ret = true;
           TestValue tmp;
           bool find = false;
           foreach (CmdItem item in GetSendCmdEnum())
           {
               
               for (int i = 0; i < TestGroupValues.Count; i++)
               {
                   if ((tmp = findTestitem((System.Collections.ArrayList)TestGroupValues[i], item.ItemName)) == null)
                       throw new Exception("Can not find test name: '" + item.ItemName+"' at "+this);
                   else
                   {
                       tmp.cmdItem = item;
                       switch (item.RangeType)
                       {
                           case RangeType.Rannge:
                               if (tmp.value > item.Max || tmp.value < item.Min)
                                   throw new Exception(tmp.itemName + "value " + tmp.value + " over range,"+cmd);
                               break;
                           case RangeType.Select:
                               find = false;
                               foreach (SelectValue val in item.SelectValues)
                               {
                                   if (val.value == tmp.value)
                                   {
                                       find = true;
                                       break;
                                   }
                               }
                               if(!find)
                               throw new Exception(tmp.itemName + "value " + tmp.value + "not in  range,"+cmd);
                               break;
                       }

                   }
               }//for
               
           }//each
       //    return ret;

       }


       public void FillDsByTestValues(DataSet ds,int inx)
       {

           if (TestGroupValues.Count == 0) throw new Exception("無測試資料");
           System.Collections.IEnumerator ie = this.GetTestEnum(inx).GetEnumerator();
           TestValue tval;
           while (ie.MoveNext())
           {
               tval = (TestValue)ie.Current;
               if (ds.Tables["tblMain"].Columns[tval.itemName].ReadOnly) continue;
               //for (int i = tval.cmdItem.Bytes - 1; i >= 0; i--)
               //    ms.WriteByte((byte)(((tval.value >> (i * 8)) ;& 0x00ff)));
               ds.Tables["tblMain"].Rows[0][tval.itemName] = tval.value;

               if (tval.SubValueCnt > 0)
               {
                 
                   TestValue[] t = new TestValue[tval.subValues.Count];
                   for (int j = 0; j < tval.subValues.Count; j++)
                   {
                       ie.MoveNext();

                       t[j] = (TestValue)ie.Current;

                   }
                   ds.Tables["tbl" + tval.itemName].Clear();
                   for (int k = 0; k < tval.value; k++)
                   {
                       System.Data.DataRow r = ds.Tables["tbl" + tval.itemName].NewRow();

                       for (int j = 0; j < tval.subValues.Count; j++)
                       {
                           r[t[j].itemName] = t[j].value;

                           //for (int i = t.cmdItem.Bytes - 1; i >= 0; i--)
                           //    ms.WriteByte((byte)(((t.value >> (i * 8)) & 0x00ff)));
                       }

                       ds.Tables["tbl" + tval.itemName].Rows.Add(r);

                   }
               }

           }
           
       }

     public   byte[] GetTestTextData(int testGroup)
       {

           System.IO.MemoryStream ms= new System.IO.MemoryStream();
          TestValue tval;
        
           if (TestGroupValues.Count == 0 ||  testGroup>=TestGroupValues.Count) return null;

           ms.WriteByte(this.cmd);
           if (subCmd != 0xff)
               ms.WriteByte(subCmd);

        System.Collections.IEnumerator  ie=this.GetTestEnum(testGroup).GetEnumerator();


        while (ie.MoveNext())
        {

          //  int repeatCnt = 0;
            tval = (TestValue)ie.Current;
            for (int i = tval.cmdItem.Bytes - 1; i >= 0; i--)
                ms.WriteByte((byte)(((tval.value >> (i * 8)) & 0x00ff)));
            
            //ms.WriteByte((byte)tval.value);
            if (tval.SubValueCnt > 0)
            {
                    TestValue[] t = new TestValue[tval.subValues.Count];
                    for (int j = 0; j < tval.subValues.Count; j++)
                    {
                        ie.MoveNext();

                        t[j] = (TestValue)ie.Current;

                    }
                for (int k = 0; k < tval.value; k++)
                {
                    for (int j = 0; j < tval.subValues.Count; j++)
                    {
                        //TestValue t;
                        //ie.MoveNext();
                        //t = (TestValue)ie.Current;
                        for (int i = t[j].cmdItem.Bytes - 1; i >= 0; i--)
                            ms.WriteByte((byte)(((t[j].value >> (i * 8)) & 0x00ff)));
                    }

                }
            }

                 
            //      for(int i=0;i<tval.value;i++)
            //ms.WriteByte((byte)tval

        }
         
       //  byte[] ret=new byte[ms.Length];
       //  System.Array.Copy(ms.GetBuffer(), ret, ms.Length);
            return ms.ToArray();

       }

        internal void GenerateCmdDataSet()
        {
          
                GererateSendDs();
                GenerateReturnDs();
           
           

        }

       private void  GererateSendDs()
       {
           DataTable tblMain = new DataTable("tblMain");
           System.Data.DataSet ds = new DataSet();
       //    DataTable tblRepeat = new DataTable("tblRepeat");
           CmdItem item,subItem;
         //  tblMain.Columns.Add("_cmd", typeof(byte));
          // tblMain.Columns.Add("_subcmd", typeof(byte));
            System.Data.DataColumn c= tblMain.Columns.Add("func_name", typeof(string));
            c.ReadOnly = true;

           ds.Tables.Add(tblMain);
           System.Collections.IEnumerator ie;
           ie= GetSendCmdEnum().GetEnumerator();
           while (ie.MoveNext())
           {
               item = (CmdItem)ie.Current;
               tblMain.Columns.Add(item.ItemName, item.DataType);
               if (item.HasSubItems)
               {
                   DataTable subTbl = new DataTable("tbl" + item.ItemName);
                   for (int i = 0; i < item.SubItemsCnt; i++)
                   {
                       ie.MoveNext();
                       subItem = (CmdItem)ie.Current;
                       subTbl.Columns.Add(((CmdItem)subItem).ItemName, ((CmdItem)subItem).DataType);
                   }
                   ds.Tables.Add(subTbl);
               }
           }
          
           //DataRow row=ds.Tables["tblMain"].NewRow();
           //row["func_name"]=cmdName;
           //ds.Tables["tblMain"].Rows.Add(row);
           //ds.AcceptChanges();
         //  ds.Tables.Add(tblRepeat);
           ds.AcceptChanges();
           sendDs = ds;
           
       }

       
           

       private void GenerateReturnDs()
       {
           if (this.cmdType != CmdType.CmdQuery )
               return;
           System.Data.DataSet ds = new DataSet();
           DataTable tblMain = new DataTable("tblMain");
         //  DataTable tblRepeat = new DataTable("tblRepeat");
           CmdItem item,subitem;
           ds.Tables.Add(tblMain);
           tblMain.Columns.Add("func_name", typeof(string));
           System.Collections.IEnumerator ie;
           ie =this.GetReturnCmdEnum().GetEnumerator();
           while (ie.MoveNext())
           {
               item = (CmdItem)ie.Current;
               tblMain.Columns.Add(item.ItemName, item.DataType);
               if (item.HasSubItems)
               {
                   DataTable subTbl = new DataTable("tbl"+item.ItemName);
                 //  tblRepeat.Columns.Add("RepeatName", typeof(string));
                   for (int i = 0; i < item.SubItemsCnt; i++)
                   {
                       ie.MoveNext();
                       subitem = (CmdItem)ie.Current;
                       subTbl.Columns.Add(((CmdItem)subitem).ItemName, ((CmdItem)subitem).DataType);
                   }
                   ds.Tables.Add(subTbl);
               }
           }


           ds.AcceptChanges();
           returnDs = ds;
       }

       public DataSet GetSendCmdTemplateDs()
       {
           DataSet ds;
          ds=sendDs.Clone();

          DataRow row = ds.Tables["tblMain"].NewRow();
          row["func_name"] = cmdName;
          foreach (System.Data.DataColumn c in row.Table.Columns)
          {
              CmdItem item=this.findCmditem(this.SendCmdItems,c.ColumnName);
              if (item != null && item.RangeType == RangeType.Rannge && item.Max == item.Min )
              {
                  row[c.ColumnName] = item.Min;
                  c.ReadOnly = true;//填上固定常數
              }

          }
          ds.Tables["tblMain"].Rows.Add(row);
          ds.AcceptChanges();
          return ds;
       }

       public DataSet GetReturnCmdTemplateDs()
       {
           if (cmdType == CmdType.CmdQuery)
           {

               DataSet ds=returnDs.Clone();
               DataRow row = ds.Tables["tblMain"].NewRow();
               row["func_name"] = cmdName;
               ds.Tables["tblMain"].Rows.Add(row);
               ds.AcceptChanges();
               return ds;

           }

           else

               return null;

       }

       public SendPackage GetReturnSendPackage(DataSet ds, int address)
       {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
           SendPackage pkg=null;
           if (ds.Tables["tblMain"].Rows[0]["func_name"].ToString() != cmdName)
               throw new Exception("unknow " + ds.Tables["tblMain"].Rows[0]["func_name"].ToString());
           if(ds.Tables["tblMain"].Rows.Count==0)
               throw new Exception("tbleMain  0 row");
           DataRow m_row=ds.Tables["tblMain"].Rows[0];
           ms.WriteByte(cmd);
           if (subCmd != 0xff)
               ms.WriteByte(subCmd);
           System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();
           while (ie.MoveNext())
           {
               CmdItem item = (CmdItem)ie.Current;
               ulong data;
               try
               {
                    data = Convert.ToUInt64(m_row[item.ItemName]);
               }
               catch (Exception ex)
               {
                   Console.WriteLine(item.ItemName + "," + ex.Message);
                   throw ex;
               }
               for(int i=item.Bytes-1;i>=0;i--)
                   ms.WriteByte((byte)((data>>(i*8)) & 0xff));
               if (item.HasSubItems)
               {
                   CmdItem subItem;
                   DataRow s_row;
                    DataTable subtbl=ds.Tables["tbl"+item.ItemName];
                    int repeatCnt = (int)data;
                  //  System.Collections.ArrayList subItems = new System.Collections.ArrayList();
                    for (int inx = 0; inx < item.SubItemsCnt; inx++)
                    {
                        ie.MoveNext();
                      //  subItems.Add(ie.Current);
                    }
                    for (int k = 0; k < repeatCnt; k++)
                    {  
                        s_row = subtbl.Rows[k];
                        for (int inx = 0; inx < item.SubItemsCnt; inx++)
                        {
                           
                          //      ie.MoveNext();
                            subItem = (CmdItem)item.SubItems[inx];
                            data = Convert.ToUInt64(s_row[subItem.ItemName]);
                            for (int i = subItem.Bytes - 1; i >= 0; i--)
                                ms.WriteByte((byte)((data >> (i * 8)) & 0xff));
                        }
                    }


               }

           }
           pkg = new SendPackage(this.cmdType, this.cmdClass, address, ms.ToArray());
           return pkg;//ms.ToArray();
       }

       public SendPackage GetSendPackage(DataSet ds,int address)
       {
           System.IO.MemoryStream ms = new System.IO.MemoryStream();
           SendPackage pkg=null;
           if (ds.Tables["tblMain"].Rows[0]["func_name"].ToString() != cmdName)
               throw new Exception("unknow " + ds.Tables["tblMain"].Rows[0]["func_name"].ToString());
           if(ds.Tables["tblMain"].Rows.Count==0)
               throw new Exception("tbleMain  0 row");
           DataRow m_row=ds.Tables["tblMain"].Rows[0];
           ms.WriteByte(cmd);
           if (subCmd != 0xff)
               ms.WriteByte(subCmd);
           System.Collections.IEnumerator ie = GetSendCmdEnum().GetEnumerator();
           while (ie.MoveNext())
           {
               CmdItem item = (CmdItem)ie.Current;
               ulong data =Convert.ToUInt64( m_row[item.ItemName]);
               for(int i=item.Bytes-1;i>=0;i--)
                   ms.WriteByte((byte)((data>>(i*8)) & 0xff));
               if (item.HasSubItems)
               {
                   CmdItem subItem;
                   DataRow s_row;
                    DataTable subtbl=ds.Tables["tbl"+item.ItemName];
                    int repeatCnt = (int)data;
                  //  System.Collections.ArrayList subItems = new System.Collections.ArrayList();
                    for (int inx = 0; inx < item.SubItemsCnt; inx++)
                    {
                        ie.MoveNext();
                      //  subItems.Add(ie.Current);
                    }
                    for (int k = 0; k < repeatCnt; k++)
                    {  
                        s_row = subtbl.Rows[k];
                        for (int inx = 0; inx < item.SubItemsCnt; inx++)
                        {
                           
                          //      ie.MoveNext();
                            subItem = (CmdItem)item.SubItems[inx];
                            data = Convert.ToUInt64(s_row[subItem.ItemName]);
                            for (int i = subItem.Bytes - 1; i >= 0; i--)
                                ms.WriteByte((byte)((data >> (i * 8)) & 0xff));
                        }
                    }


               }

           }
           pkg = new SendPackage(this.cmdType, this.cmdClass, address, ms.ToArray());
           return pkg;//ms.ToArray();
       }


     internal  DataSet GetSendDsByTextPackage(TextPackage pkg) //主動回報
       {
          try
           {
           switch (pkg.Cmd)
           {
               case 0xDF:
                   if (pkg.Text[1] == 0xda)
                       return process_0xdf_0xda_SendDs(pkg);
                   break;
               case  0X5B:

               case 0x5A:
                   return process_0x5A_SendDs(pkg);
               case 0x5f: //mas 主動回報
                   if (pkg.Text[1] == 0x2a || pkg.Text[1] == 0x25)
                       return process_0x5f_0x2a_report(pkg);
                   break;

               //case  0x04:
               //     if(pkg.Text[7]==0xA4)
               //       return process
           }

           DataSet ds = GetSendCmdTemplateDs();

           System.Collections.IEnumerator ie = this.GetSendCmdEnum().GetEnumerator();

           int inx = 0;
           if (this.subCmd == 0xff)
               inx = 1;  //no subcmd
           else
               inx = 2; //has sub cmd


           while (ie.MoveNext())
           {
               CmdItem item;
               ulong val = 0;
               item = (CmdItem)ie.Current;
               val = 0;
               for (int i = 0; i < item.Bytes; i++)
                   val = val * 256 + pkg.Text[inx++];

               ds.Tables["tblMain"].Rows[0][item.ItemName] = val;


               if (item.HasSubItems)
               {
                   ulong subval = 0;
                   for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                   {
                       System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                       foreach (CmdItem subItem in item.SubItems)  // get subval field
                       {
                           subval = 0;
                           for (int k = 0; k < subItem.Bytes; k++)
                               subval = subval * 256 + pkg.Text[inx++];
                           r[subItem.ItemName] = subval;
                       }

                       ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                   }


                   for (int i = 0; i < (int)item.SubItemsCnt; i++)
                       ie.MoveNext();

               }


           } //while

           if (inx != pkg.Text.Length || ie.MoveNext())
               throw new Exception("text 長度錯誤!");
           ds.AcceptChanges();
           return ds;
       }
       catch (Exception ex)
       {
           Console.WriteLine("主動回報"+ex.Message + "," + pkg.ToString());
           throw ex;
       }

       }

       private DataSet process_0x5f_0x2a_report(TextPackage pkg)
       {

           try
           {
               DataSet ds = this.GetSendCmdTemplateDs();
               int msg_length = 0, CR_cnt = 0;
               ulong data_type = 0;

               System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

               int inx = 0;
               if (this.subCmd == 0xff)
                   inx = 1;  //no subcmd
               else
                   inx = 2; //has sub cmd


               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   if (item.ItemName == "data_type")
                   {
                       // data_type = (int)val;
                       //  for (int i = 0; i < 32 + 1; i++)
                       data_type = val;
                       if (val == 0)  //text
                       {
                           ie.MoveNext(); // skip g_code_id
                           for (int i = 0; i < 32; i++)
                               ie.MoveNext();   // skip g_code_desc 1~32
                           continue;
                       }
                       else if (val == 1)  //g_code
                       {
                           ie.MoveNext();
                           item = (CmdItem)ie.Current;
                           val = 0;
                           for (int i = 0; i < item.Bytes; i++)
                               val = val * 256 + pkg.Text[inx++];

                           ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                           continue;
                       }
                       else  //val==2 speed limit
                       {
                           while (ie.MoveNext())  //skip all util colorcnt
                           {
                               if (((CmdItem)ie.Current).ItemName == "colorcnt")
                                   break;
                           }
                           continue;  //read next item
                       }
                   }


                   if (item.ItemName == "msg_length")
                       msg_length = (int)val;

                   if (item.ItemName == "g_desc32")
                       break;

                   if (item.ItemName == "msgcnt")
                   {
                       val = (ulong)msg_length;
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = msg_length;
                   }

                   if (item.ItemName == "colorcnt")
                   {
                       byte[] codebig5 = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
                       for (int i = 0; i < ds.Tables["tblmsgcnt"].Rows.Count; i++)
                       {
                           codebig5[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);
                       }
                       string s = RemoteInterface.Utils.Util.Big5BytesToString(codebig5);//System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, codebig5));
                       val = (ulong)s.Replace("\r", "").Replace("\n", "").Length; //(ulong)(msg_length-CR_cnt);
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                       // if(data_type==2)
                       break;  //last one for data_type== 0
                   }

                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               //if (item.ItemName == "msgcnt" && subval == 0x0d)
                               //    CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }

               } //while

               ds.AcceptChanges();
               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message +ex.StackTrace+ "," + pkg.ToString());
               return null;
           }
       }

       private DataSet process_0x5f_0x12_returnDs(TextPackage pkg)
       {
           try
           {
               DataSet ds = this.GetReturnCmdTemplateDs();
               int inx = 1;
               System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   val = 0;
                   item = (CmdItem)ie.Current;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;

                   if (pkg.Text.Length == 2 && item.ItemName == "frame_no")
                       return ds;
                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               //if (item.ItemName == "msgcnt" && subval == 0x0d)
                               //    CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();
                   }
               }
               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message +ex.StackTrace+ "," + pkg.ToString());
               return null;
           }
       }
        private DataSet  process_0xd6_return_DS(TextPackage pkg)
        {

            try
            {
             DataSet ds = this.GetReturnCmdTemplateDs();
             int inx = 1;
             System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();
            
                 while (ie.MoveNext())
                 {
                     CmdItem item;
                     ulong val = 0;
                     val = 0;
                     item = (CmdItem)ie.Current;
                     for (int i = 0; i < item.Bytes; i++)
                         val = val * 256 + pkg.Text[inx++];

                     ds.Tables["tblMain"].Rows[0][item.ItemName] = val;

                     if (pkg.Text.Length == 2 && item.ItemName == "frame_no")
                         return ds;
                     if (item.HasSubItems)
                     {
                         int subval = 0;
                         for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                         {
                             System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                             foreach (CmdItem subItem in item.SubItems)  // get subval field
                             {
                                 subval = 0;
                                 for (int k = 0; k < subItem.Bytes; k++)
                                     subval = subval * 256 + pkg.Text[inx++];
                                 r[subItem.ItemName] = subval;
                                 //if (item.ItemName == "msgcnt" && subval == 0x0d)
                                 //    CR_cnt++;
                             }

                             ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                         }


                         for (int i = 0; i < (int)item.SubItemsCnt; i++)
                             ie.MoveNext();
                     }
                 }
                 return ds;
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message +ex.StackTrace+ "," + pkg.ToString());
                 return null;
             }

        }

       private DataSet process_0x04_0x5f_0x2a(TextPackage pkg)
       {

           try
           {
               DataSet ds = this.GetReturnCmdTemplateDs();
               int msg_length = 0, CR_cnt = 0;
               ulong data_type=0;

               System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

               int inx = 0;
               if (this.subCmd == 0xff)
                   inx = 1;  //no subcmd
               else
                   inx = 2; //has sub cmd


               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   if (item.ItemName == "data_type")
                   {
                       // data_type = (int)val;
                       //  for (int i = 0; i < 32 + 1; i++)
                       data_type=val;
                       if (val == 0)  //text
                       {
                           ie.MoveNext(); // skip g_code_id
                           for (int i = 0; i < 32; i++)
                               ie.MoveNext();   // skip g_code_desc 1~32
                           continue;
                       }
                       else if (val == 1)  //g_code
                       {
                           ie.MoveNext();
                           item = (CmdItem)ie.Current;
                           val = 0;
                           for (int i = 0; i < item.Bytes; i++)
                               val = val * 256 + pkg.Text[inx++];

                           ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                           continue;
                       }
                       else  //val==2 speed limit
                       {
                           while (ie.MoveNext())  //skip all util colorcnt
                           {
                               if (((CmdItem)ie.Current).ItemName == "colorcnt")
                                   break;
                           }
                           continue;  //read next item
                       }
                   }


                   if (item.ItemName == "msg_length")
                       msg_length = (int)val;

                   if (item.ItemName == "g_desc32")
                       break;

                   if (item.ItemName == "msgcnt")
                   {
                       val = (ulong)msg_length;
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = msg_length;
                   }

                   if (item.ItemName == "colorcnt")
                   {
                       byte[] codebig5 = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
                       for (int i = 0; i < ds.Tables["tblmsgcnt"].Rows.Count; i++)
                       {
                           codebig5[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);
                       }
                       string s = RemoteInterface.Utils.Util.Big5BytesToString(codebig5);//System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, codebig5));
                       val = (ulong)s.Replace("\r", "").Replace("\n", "").Length; //(ulong)(msg_length-CR_cnt);
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                      // if(data_type==2)
                       break;  //last one for data_type== 0
                   }

                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               //if (item.ItemName == "msgcnt" && subval == 0x0d)
                               //    CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }

               } //while


               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message+ex.StackTrace + "," + pkg.ToString());
               return null;
           }
       }

       internal DataSet GetETTUReturnDsByTextPackage(int cnt, TextPackage pkg)
       {
           try{
            DataSet ds =GetReturnCmdTemplateDs();

           System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

           int inx = 0;
           if (this.subCmd == 0xff)
               inx = 1;  //no subcmd
           else
               inx = 2; //has sub cmd

          
               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];


                

                    if(((CmdItem)ie.Current).ItemName=="length")
                       val =(ulong) cnt;
                    ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                                   
                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl"+item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }
                   
                  
               } //while

               if (inx != pkg.Text.Length || ie.MoveNext())
                   throw new Exception("text 長度錯誤!");

               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message+ex.StackTrace + "," + pkg.ToString());
               throw ex;
           }


       }
      internal DataSet GetReturnDsByTextPackage(TextPackage pkg)
       {
           try
           {
           switch (pkg.Cmd)
           {
               case 0x5f:
                   if (pkg.Text[1] == 0x55)
                        return  process_0x04_0x5f_0x57_returnDs(pkg);
                    if (pkg.Text[1] == 0x25)
                        return process_0x5f_0x25_returnDs(pkg);
                   //else if (pkg.Text[1] == 0x12) //tts
                   //     return process_0xd6_return_DS(pkg);
                  break;
               case 0x55:
                   return process_0x55_returnDs(pkg);
               //case 0xAD:
               //    return process_0xAD_returnDs(pkg);
               case 0xdf:
                    if(pkg.Text[1]==0xd5)
                        return process_0x04_0x54_returnDs(pkg);
                   break;
                  
               case 0x96:
                   return process_0x96_returnDs(pkg);
                   break;

               case 0xd6:

                   return process_0xd6_return_DS(pkg);
                   break;
            
               case 0x04:
                   if (pkg.Text[7] == 0x50 ||  pkg.Text[7]==0x5f && pkg.Text[8]==0x20)
                       return process_0x04_0x50_returnDs(pkg);
                   else if (pkg.Text[7] == 0x54 || pkg.Text[7] == 0x57 || pkg.Text[7] == 0xdf && pkg.Text[8] == 0xd4 || pkg.Text[7] == 0xdf && pkg.Text[8] == 0xd7 || pkg.Text[7]==0x5f && pkg.Text[8]==0x02)
                       return process_0x04_0x54_returnDs(pkg);
                   else if (pkg.Text[7] == 0xA4)
                       return process_0x04_0xA4_returnDs(pkg);
                   else if (pkg.Text[7] == 0xdf && pkg.Text[8] == 0xd0)
                       return process_0x04_0xdf_0xd0_returnDs(pkg);
                   else if (pkg.Text[7] == 0x5f && pkg.Text[8] == 0x57)
                       return process_0x04_0x5f_0x57_returnDs(pkg);
                   else if (pkg.Text[7] == 0x5f && pkg.Text[8] == 0x27)
                       return process_0x04_0x54_returnDs(pkg);

                   else if (pkg.Text[7] == 0x5f && pkg.Text[8] == 0x2a)
                       return process_0x04_0x5f_0x2a(pkg);

                   //else if(pkg.Text[7]==0x0df && pkg.Text[8]==0xd1)
                   //    return process_0x04_0xdf_0xd1_returnDs(pkg);
                 
                   break;
           }
           DataSet ds =GetReturnCmdTemplateDs();

           System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

           int inx = 0;
           if (this.subCmd == 0xff)
               inx = 1;  //no subcmd
           else
               inx = 2; //has sub cmd

          
               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;

                                   
                   if (item.HasSubItems)
                   {
                       ulong subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl"+item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }
                   
                  
               } //while

               if (inx != pkg.Text.Length || ie.MoveNext())
                   throw new Exception("text 長度錯誤!");

               return ds;
           }
           catch (Exception ex)
           {
             //  Console.WriteLine(ex.Message+ex.StackTrace + "," + pkg.ToString());
               throw ex;
           }

       }





       private   DataSet process_0x04_0xdf_0xd0_returnDs(TextPackage pkg)
       {
           try
           {
               int row = 0, col = 0;

               DataSet ds = GetReturnCmdTemplateDs();

               System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

               int inx = 0;
               if (this.subCmd == 0xff)
                   inx = 1;  //no subcmd
               else
                   inx = 2; //has sub cmd


               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   if (item.ItemName == "row")
                       row = (int)val;
                   if (item.ItemName == "column")
                       col = (int)val;
                   if (item.ItemName == "pattern_cnt")
                   {
                       val = (ulong)(row * col / 8);

                   }
                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;


                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }


               } //while


               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message + ex.StackTrace+"," + pkg.ToString());
               return null;
           }
       }
        private DataSet process_0x04_0x50_returnDs(TextPackage pkg)
        {
            try
            {
                int row=0, col=0;
               
                DataSet ds = GetReturnCmdTemplateDs();

                System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

                int inx = 0;
                if (this.subCmd == 0xff)
                    inx = 1;  //no subcmd
                else
                    inx = 2; //has sub cmd


                while (ie.MoveNext())
                {
                    CmdItem item;
                    ulong val = 0;
                    item = (CmdItem)ie.Current;
                    val = 0;
                    for (int i = 0; i < item.Bytes; i++)
                        val = val * 256 + pkg.Text[inx++];

                    if (item.ItemName == "row")
                        row =(int) val;
                    if (item.ItemName == "column")
                        col = (int)val;
                    if (item.ItemName == "pattern_cnt")
                    {
                        val =(ulong)( row * col / 8);

                    }
                    ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                    

                    if (item.HasSubItems)
                    {
                        int subval = 0;
                        for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                        {
                            System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                            foreach (CmdItem subItem in item.SubItems)  // get subval field
                            {
                                subval = 0;
                                for (int k = 0; k < subItem.Bytes; k++)
                                    subval = subval * 256 + pkg.Text[inx++];
                                r[subItem.ItemName] = subval;
                            }

                            ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                        }


                        for (int i = 0; i < (int)item.SubItemsCnt; i++)
                            ie.MoveNext();

                    }


                } //while


                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+ex.StackTrace + "," + pkg.ToString());
                return null;
            }
        }

       private DataSet process_0x96_returnDs(TextPackage pkg)
       {
           try
           {
               DataSet ds = this.GetReturnCmdTemplateDs();
               int msg_length = 0;

               System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

               int inx = 0;
               if (this.subCmd == 0xff)
                   inx = 1;  //no subcmd
               else
                   inx = 2; //has sub cmd


               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   //if (item.ItemName == "data_type")
                   //{
                   //    // data_type = (int)val;
                   //    //  for (int i = 0; i < 32 + 1; i++)
                   //    if (val == 0)  //text
                   //    {
                   //        ie.MoveNext(); // skip g_code_id
                   //        continue;
                   //    }
                   //    else
                   //    {
                   //        ie.MoveNext();
                   //        item = (CmdItem)ie.Current;
                   //        val = 0;
                   //        for (int i = 0; i < item.Bytes; i++)
                   //            val = val * 256 + pkg.Text[inx++];

                   //        ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   //        break;
                   //    }
                   //}


                   if (item.ItemName == "msg_length")
                       msg_length = (int)val;

                   //if (item.ItemName == "g_desc32")
                   //    break;

                   if (item.ItemName == "msgcnt")
                   {
                       val = (ulong)msg_length;
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = msg_length;
                   }

                   if (item.ItemName == "colorcnt")
                   {
                       byte[] codebig5 = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
                       for (int i = 0; i < ds.Tables["tblmsgcnt"].Rows.Count; i++)
                       {
                           codebig5[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);
                       }
                       string s = RemoteInterface.Utils.Util.Big5BytesToString(codebig5);//System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, codebig5));
                       val = (ulong)s.Replace("\r", "").Replace("\n", "").Length; //(ulong)(msg_length-CR_cnt);
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   }

                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               //if (item.ItemName == "msgcnt" && subval == 0x0d)
                               //    CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }

               } //while


               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message+ex.StackTrace + "," + pkg.ToString());
               return null;
           }

       }


       private DataSet process_0x04_0x5f_0x57_returnDs(TextPackage pkg)
       {
           try
           {
               DataSet ds = this.GetReturnCmdTemplateDs();
               int msg_length = 0, CR_cnt = 0;

               System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

               int inx = 0;
               if (this.subCmd == 0xff)
                   inx = 1;  //no subcmd
               else
                   inx = 2; //has sub cmd


               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   //if (item.ItemName == "data_type")
                   //{
                   //    // data_type = (int)val;
                   //    //  for (int i = 0; i < 32 + 1; i++)
                   //    if (val == 0)  //text
                   //    {
                   //        ie.MoveNext(); // skip g_code_id
                   //        continue;
                   //    }
                   //    else
                   //    {
                   //        ie.MoveNext();
                   //        item = (CmdItem)ie.Current;
                   //        val = 0;
                   //        for (int i = 0; i < item.Bytes; i++)
                   //            val = val * 256 + pkg.Text[inx++];

                   //        ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   //        break;
                   //    }
                  // }


                   if (item.ItemName == "msg_length")
                       msg_length = (int)val;

                   if (item.ItemName == "g_desc32")
                       break;

                   if (item.ItemName == "msgcnt")
                   {
                       val = (ulong)msg_length;
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = msg_length;
                   }

                   if (item.ItemName == "colorcnt")
                   {
                       byte[] codebig5 = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
                       for (int i = 0; i < ds.Tables["tblmsgcnt"].Rows.Count; i++)
                       {
                           codebig5[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);
                       }
                       string s = RemoteInterface.Utils.Util.Big5BytesToString(codebig5); // System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, codebig5));
                       val = (ulong)s.Replace("\r", "").Replace("\n", "").Length; //(ulong)(msg_length-CR_cnt);
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   }

                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               //if (item.ItemName == "msgcnt" && subval == 0x0d)
                               //    CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }

               } //while


               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message+ex.StackTrace + "," + pkg.ToString());
               return null;
           }
       }

       private DataSet process_0x04_0x54_returnDs(TextPackage pkg)
       {
           try
           {
               DataSet ds = this.GetReturnCmdTemplateDs();
           int msg_length = 0, CR_cnt = 0;

           System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

           int inx = 0;
           if (this.subCmd == 0xff)
               inx = 1;  //no subcmd
           else
               inx = 2; //has sub cmd

           
               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   if (item.ItemName == "data_type")  
                   {
                       // data_type = (int)val;
                       //  for (int i = 0; i < 32 + 1; i++)
                       if (val == 0)  //text
                       {
                           ie.MoveNext(); // skip g_code_id
                           continue;
                       }
                       else
                       {
                           ie.MoveNext();
                           item = (CmdItem)ie.Current;
                           val = 0;
                           for (int i = 0; i < item.Bytes; i++)
                               val = val * 256 + pkg.Text[inx++];

                           ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                           break;
                       }
                   }
                  
                   
                   if (item.ItemName == "msg_length")
                       msg_length = (int)val;

                   if (item.ItemName == "g_desc32")
                       break;

                   if (item.ItemName == "msgcnt")
                   {
                       val = (ulong)msg_length;
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = msg_length;
                   }

                   if (item.ItemName == "colorcnt")
                   {
                       byte[] codebig5 = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
                       for(int i=0;i< ds.Tables["tblmsgcnt"].Rows.Count;i++)
                       {
                           codebig5[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);
                       }
                       string s = RemoteInterface.Utils.Util.Big5BytesToString(codebig5);   //System.Text.Encoding.Unicode.GetString( System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, codebig5));
                       val = (ulong)s.Replace("\r", "").Replace("\n", "").Length; //(ulong)(msg_length-CR_cnt);
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   }

                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               //if (item.ItemName == "msgcnt" && subval == 0x0d)
                               //    CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }

               } //while


               return ds;
           }
           catch (Exception ex)
           {
               //Console.WriteLine(ex.Message +ex.StackTrace+ "," + pkg.ToString());
               //return null;
               throw new Exception(ex.Message+ "," + pkg.ToString());
           }

       }
       private DataSet process_0x04_0xA4_returnDs(TextPackage pkg)
       {
           try
           {
               DataSet ds = this.GetReturnCmdTemplateDs();
               int sgmcnt = 0, CR_cnt = 0;

               System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

               int inx = 0;
               if (this.subCmd == 0xff)
                   inx = 1;  //no subcmd
               else
                   inx = 2; //has sub cmd


               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   if (item.ItemName == "data_type" && val == 0)  //text
                   {
                       // data_type = (int)val;
                       for (int i = 0; i < 32 + 1; i++)
                           ie.MoveNext();
                       continue;
                   }
                   if (item.ItemName == "sgmcnt")
                       sgmcnt = (int)val;

                   //if (item.ItemName == "g_desc32")
                   //    break;

                   if (item.ItemName == "scnt")
                   {
                       val = (ulong)sgmcnt;
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = sgmcnt;
                   }

                   //if (item.ItemName == "colorcnt")
                   //{
                   //    val = (ulong)(msg_length - CR_cnt);
                   //    ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   //}

                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               //if (item.ItemName == "msgcnt" && subval == 0x0d)
                               //    CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }

               } //while


               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message+ex.StackTrace + "," + pkg.ToString());
               return null;
           }

       }
       private DataSet process_0xdf_0xda_SendDs(TextPackage pkg)
       {
           try
           {

               DataSet ds = this.GetSendCmdTemplateDs();
               int msg_length = 0, CR_cnt = 0;

               System.Collections.IEnumerator ie = this.GetSendCmdEnum().GetEnumerator();

               int inx = 0;
               if (this.subCmd == 0xff)
                   inx = 1;  //no subcmd
               else
                   inx = 2; //has sub cmd


               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   if (item.ItemName == "data_type" && val == 0)  //text
                   {
                       // data_type = (int)val;
                       //for (int i = 0; i < 32 + 1; i++)
                          ie.MoveNext();
                       continue;
                   }
                   if (item.ItemName == "msg_length")
                       msg_length = (int)val;

                   if (item.ItemName == "g_desc32")
                       break;

                   if (item.ItemName == "msgcnt")
                   {
                       val = (ulong)msg_length;
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = msg_length;
                   }

                   if (item.ItemName == "colorcnt")
                   {
                       //val = (ulong)(msg_length-CR_cnt);
                       //ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                       byte[] codebig5 = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
                       for (int i = 0; i < ds.Tables["tblmsgcnt"].Rows.Count; i++)
                       {
                           codebig5[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);
                       }
                       string s = RemoteInterface.Utils.Util.Big5BytesToString(codebig5);//System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, codebig5));
                       val = (ulong)s.Replace("\r", "").Replace("\n", "").Length; //(ulong)(msg_length-CR_cnt);
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   }

                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               if (item.ItemName == "msgcnt" && subval == 0x0d)
                                   CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }

               } //while


               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message + ex.StackTrace + "," + pkg.ToString());
               return null;
           }
       }
       private DataSet process_0x5A_SendDs(TextPackage pkg)
       {
           try
           {
         
           DataSet ds = this.GetSendCmdTemplateDs();
           int msg_length = 0, CR_cnt = 0;

           System.Collections.IEnumerator ie = this.GetSendCmdEnum().GetEnumerator();

           int inx = 0;
           if (this.subCmd == 0xff)
               inx = 1;  //no subcmd
           else
               inx = 2; //has sub cmd

          
               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   if (item.ItemName == "data_type" && val == 0)  //text
                   {
                       // data_type = (int)val;
                       for (int i = 0; i < 32 + 1; i++)
                           ie.MoveNext();
                       continue;
                   }
                   if (item.ItemName == "msg_length")
                       msg_length = (int)val;

                   if (item.ItemName == "g_desc32")
                       break;

                   if (item.ItemName == "msgcnt")
                   {
                       val = (ulong)msg_length;
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = msg_length;
                   }

                   if (item.ItemName == "colorcnt")
                   {
                       //val = (ulong)(msg_length-CR_cnt);
                       //ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                       byte[] codebig5 = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
                       for (int i = 0; i < ds.Tables["tblmsgcnt"].Rows.Count; i++)
                       {
                           codebig5[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);
                       }
                       string s = RemoteInterface.Utils.Util.Big5BytesToString(codebig5);//System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, codebig5));
                       val = (ulong)s.Replace("\r", "").Replace("\n", "").Length; //(ulong)(msg_length-CR_cnt);
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   }

                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               if (item.ItemName == "msgcnt" && subval == 0x0d)
                                   CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }

               } //while


               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message+ex.StackTrace + "," + pkg.ToString());
               return null;
           }
       }

       //private DataSet process_0xAD_returnDs(TextPackage pkg)
       //{
       //    DataSet ds = GetReturnCmdTemplateDs();
       //    System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

       //    int inx = 0;
       //    if (this.subCmd == 0xff)
       //        inx = 1;  //no subcmd
       //    else
       //        inx = 2; //has sub cmd;
       //}


       private DataSet process_0x5f_0x25_returnDs(TextPackage pkg)
       {
           DataSet ds = GetReturnCmdTemplateDs();
           int msg_length = 0, CR_cnt = 0;

           System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

           int inx = 0;
           if (this.subCmd == 0xff)
               inx = 1;  //no subcmd
           else
               inx = 2; //has sub cmd

           try
           {
               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   if (item.ItemName == "data_type" && val == 0)  //text
                   {
                       // data_type = (int)val;
                       for (int i = 0; i < 32 + 1; i++)
                           ie.MoveNext();
                       continue;
                   }
                   else if (item.ItemName == "data_type" && val == 2)
                   {
                       bool isFound = false;
                       while(true)
                       {
                         
                          if(! ie.MoveNext())
                              throw new Exception("Can't find item colorcnt!");
                          item = (CmdItem)ie.Current;
                          if (item.ItemName == "color")
                          {
                              isFound = true;
                              break;
                          }
                       }


                       if(isFound)
                          continue;
                   }


                   if (item.ItemName == "msg_length")
                       msg_length = (int)val;

                   if (item.ItemName == "g_desc32")
                       break;

                   if (item.ItemName == "msgcnt")
                   {
                       val = (ulong)msg_length;
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = msg_length;
                   }

                   if (item.ItemName == "colorcnt")
                   {
                       //val = (ulong)msg_length;
                       //ds.Tables["tblMain"].Rows[0][item.ItemName] = CR_cnt;
                       byte[] codebig5 = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
                       for (int i = 0; i < ds.Tables["tblmsgcnt"].Rows.Count; i++)
                       {
                           codebig5[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);
                       }
                       string s = RemoteInterface.Utils.Util.Big5BytesToString(codebig5);//System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, codebig5));
                       val = (ulong)s.Replace("\r", "").Replace("\n", "").Length; //(ulong)(msg_length-CR_cnt);
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                       
                   }

                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               if (item.ItemName == "msgcnt" && subCmd == 0x0d)
                                   CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();


                       if (item.ItemName == "colorcnt")
                           break;

                   }

               } //while


               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message+ex.StackTrace + "," + pkg.ToString());
               return null;
           }
       }
       private DataSet process_0x55_returnDs(TextPackage pkg)
       {
           DataSet ds = GetReturnCmdTemplateDs();
           int msg_length = 0, CR_cnt = 0; 

           System.Collections.IEnumerator ie = this.GetReturnCmdEnum().GetEnumerator();

           int inx = 0;
           if (this.subCmd == 0xff)
               inx = 1;  //no subcmd
           else
               inx = 2; //has sub cmd

           try
           {
               while (ie.MoveNext())
               {
                   CmdItem item;
                   ulong val = 0;
                   item = (CmdItem)ie.Current;
                   val = 0;
                   for (int i = 0; i < item.Bytes; i++)
                       val = val * 256 + pkg.Text[inx++];

                   ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   if (item.ItemName == "data_type"  && val==0)  //text
                   {
                      // data_type = (int)val;
                       for (int i = 0; i < 32+1; i++)
                           ie.MoveNext();
                       continue;
                   }
                   if (item.ItemName == "msg_length")
                       msg_length = (int)val;

                   if (item.ItemName == "g_desc32")
                       break;

                   if (item.ItemName == "msgcnt")
                   {
                       val = (ulong)msg_length;
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = msg_length;
                   }

                   if (item.ItemName == "colorcnt")
                   {
                       //val = (ulong)msg_length;
                       //ds.Tables["tblMain"].Rows[0][item.ItemName] = CR_cnt;
                       byte[] codebig5 = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
                       for (int i = 0; i < ds.Tables["tblmsgcnt"].Rows.Count; i++)
                       {
                           codebig5[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);
                       }
                       string s = RemoteInterface.Utils.Util.Big5BytesToString(codebig5);// System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, codebig5));
                       val = (ulong)s.Replace("\r", "").Replace("\n", "").Length; //(ulong)(msg_length-CR_cnt);
                       ds.Tables["tblMain"].Rows[0][item.ItemName] = val;
                   }

                   if (item.HasSubItems)
                   {
                       int subval = 0;
                       for (int i = 0; i < (int)val; i++)  //val=repeat cnt
                       {
                           System.Data.DataRow r = ds.Tables["tbl" + item.ItemName].NewRow();
                           foreach (CmdItem subItem in item.SubItems)  // get subval field
                           {
                               subval = 0;
                               for (int k = 0; k < subItem.Bytes; k++)
                                   subval = subval * 256 + pkg.Text[inx++];
                               r[subItem.ItemName] = subval;
                               if (item.ItemName == "msgcnt" && subCmd==0x0d)
                                   CR_cnt++;
                           }

                           ds.Tables["tbl" + item.ItemName].Rows.Add(r);

                       }


                       for (int i = 0; i < (int)item.SubItemsCnt; i++)
                           ie.MoveNext();

                   }
                  
               } //while


               return ds;
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message +ex.StackTrace+ "," + pkg.ToString());
               return null;
           }
       }




 }
}
