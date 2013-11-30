using System;
using System.Collections.Generic;
using System.Text;
using com.calitha.goldparser;

namespace Comm
{
   public  class Protocol:MyParser
    {
        public   string DeviceType;
        string grammarFile;
        public string ip;
       public string version;
        public int port, deviceId;
       private bool CanTest = false;
       public bool Enabled = false;
       private  string ScriptSource;
       private  System.Collections.Hashtable keyhashCommands =System.Collections.Hashtable.Synchronized(  new System.Collections.Hashtable(100));
       private System.Collections.Hashtable func_name_hashCommands =System.Collections.Hashtable.Synchronized( new System.Collections.Hashtable(100));
       public  System.Collections.ArrayList Commands = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(50));

        public  Protocol(string grammarfile):base(grammarfile)
        {
            this.grammarFile = grammarfile;
            this.parser.OnTokenError += new com.calitha.goldparser.LALRParser.TokenErrorHandler(parser_OnTokenError);
            this.parser.OnParseError += new LALRParser.ParseErrorHandler(parser_OnParseError);
           

            
            this.parser.OnAccept += new LALRParser.AcceptHandler(parser_OnAccept);
            
        }
       public static string CPath(string WinPath)
       {


           if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
               return WinPath;
           else
           {
               //  Console.WriteLine("Unix");
               return WinPath.Replace(@"\", @"/");
           }
       }

       public Protocol():this(CPath(AppDomain.CurrentDomain.BaseDirectory+ "V20.cgt"))
       {
         //  Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory + "V20.cgt");
       }
       public new void Parse(string source)
       {
          
          Parse(source,true);
          ScriptSource = source;
       }


       public string calSignature(string source)
       {
          
           string retStr = "";
           System.Security.Cryptography.DES des = System.Security.Cryptography.DES.Create();
           // des.KeySize = 10;
           System.Security.Cryptography.ICryptoTransform trs = des.CreateEncryptor(System.Text.ASCIIEncoding.ASCII.GetBytes("09881638"),System.Text.ASCIIEncoding.ASCII.GetBytes("0935010126"));
           source = source.Replace(" ", "");
           source = source.Replace("\r\n", "");
           byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(source);
           byte[] encdata = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
           for (int i = 0; i < data.Length; i++)
               encdata[i % encdata.Length] ^= data[i];


           encdata = trs.TransformFinalBlock(encdata, 0, encdata.Length);
           for (int i = 0; i < encdata.Length; i++)
               retStr = retStr + string.Format("{0:X2}", encdata[i]);
           return retStr;
       }
       public void Parse(string source, bool bSecurity)
       {

           int inx = 0;
           inx = source.LastIndexOf("@Signature=");
           if (inx == -1)
           {
               if (!bSecurity)
                  base.Parse(source);
               else
                  throw new Exception("無摘要資訊!");
           }
           else
           {
               string temp;
                  
               string signature = source.Substring(inx + "@Signature=".Length).Trim();
               temp = source.Substring(0, inx);
               if (!bSecurity)
                   base.Parse(temp);
               else
               {
                   if (calSignature(temp).ToUpper() == signature.ToUpper())
                       base.Parse(temp);
                   else
                       throw new Exception("摘要資訊錯誤!");
               }
           }

           ScriptSource = source;

       }

      
       //private  void generateCmdHasTable()
       //{
       //}


       public string getScriptSource()
       {
           return ScriptSource;
       }
  public      Cmd getCmd(byte cmd, byte subCmd, CmdType CmdType)
       {
           return(Cmd) keyhashCommands[V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + CmdType];
       }


       public Cmd getCmd(byte cmd, byte protocol_code, byte sub_protocol_code, CmdType cmdtype)  //for 04
       {
           return (Cmd)keyhashCommands[V2DLE.ToHexString(new byte[] { cmd, protocol_code, sub_protocol_code }).Replace(' ', '_') + cmdtype];
       }

       //public Cmd getCmd(byte cmd, byte subcmd,byte distinctInx, CmdType cmdtype)  
       //{
       //    return (Cmd)keyhashCommands[V2DLE.ToHexString(new byte[] { cmd, subcmd, distinctInx }).Replace(' ', '_') + cmdtype];
       //}


   public     Cmd getCmd(string func_name)
       {
           return (Cmd)func_name_hashCommands[func_name];
       }

       void parser_OnAccept(LALRParser parser, AcceptEventArgs args)
       {
           //try
           //{
           //    generateCmdHasTable();
               Enabled = true;
              // Console.WriteLine("Parser Success!");
           //}
           //catch (Exception ex)
           //{
           //    Console.WriteLine("On_accept:" + ex.Message);
           //}
       }

    private   void parser_OnParseError(LALRParser parser, ParseErrorEventArgs args)
       {
           string message;
           message=args.UnexpectedToken.Location+",unexpect token:"+args.UnexpectedToken.Text;
           throw new Exception(message);
       }

     private  void parser_OnTokenError(com.calitha.goldparser.LALRParser parser, com.calitha.goldparser.TokenErrorEventArgs args)
       {
           string message = args.Token.Location + ",token error:" + args.Token.Text;
           throw new Exception(message);
           
       }

       private void  AddCmd(Cmd cmd)
       {
           //try
           //{
           try
           {
               keyhashCommands.Add(cmd.Key, cmd);
           }
           catch (Exception ex)
           {
               System.Console.WriteLine(cmd.Key + ",duplicated");
           }
           //}
           //catch (Exception ex)
           //{
           //    Console.WriteLine(ex.Message);
           //}
           func_name_hashCommands.Add(cmd.cmdName,cmd);
           Commands.Add(cmd);
       }

     NonterminalToken findToken(NonterminalToken tok, string name)
       {
        NonterminalToken t;
           for (int i =0 ; i< tok.Rule.Rhs.Length;i++)
           {
               
               if (tok.Rule.Rhs[i].Name == name)
                   return (NonterminalToken)tok.Tokens[i];
               else if(tok.Tokens[i] is NonterminalToken)
               {
                  t= findToken((NonterminalToken)tok.Tokens[i],name);
                  if (t == null) continue;
                  else
                      return t;
               }


                   
           }
         return null;
       }


        int getTokenInx(NonterminalToken token, string name)
       {
           for (int i = 0; i < token.Rule.Rhs.Length; i++)
           {
               if (name == token.Rule.Rhs[i].Name)
                   return i;
           }
           return -1;
       }
       System.Collections.ArrayList TmpCmdItems = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(100));
       System.Collections.ArrayList TmpSelectValues = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(100));
       System.Collections.ArrayList tmpSendExpress=System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
       System.Collections.ArrayList tmpReturnExpress=System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
       System.Collections.ArrayList tmpTestValues = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
       System.Collections.ArrayList tmpTestExpress =System.Collections.ArrayList.Synchronized( new System.Collections.ArrayList(10));

       protected override Object CreateObject(NonterminalToken token)
       {
           string name;
           int bytes, inx, value;
           CmdItem cmdItem;
           TestValue testval;
           try
           {
               switch (token.Rule.Id)
               {

                   case (int)RuleConstants.RULE_TESTGROUPEXPRESS_TESTEQ:
                       CanTest = false;
                       break;
                   case (int)RuleConstants.RULE_VERSION_VERSIONEQ_FLOAT:
                       this.version = token.Tokens[1].UserObject.ToString();
                       break;
                   case (int)RuleConstants.RULE_TESTGROUPEXPRESS_TESTEQ2:
                       CanTest = true;
                       break;
                   case (int)RuleConstants.RULE_TESTEXPRESS_ATCMD:
                       tmpTestExpress.Add(tmpTestValues);
                       tmpTestValues = new System.Collections.ArrayList(10);
                       break;
                   case (int)RuleConstants.RULE_TESTREPEATITEMS_LBRACE_RBRACE:

                       testval = (TestValue)token.Tokens[0].UserObject;
                       inx = tmpTestValues.IndexOf(testval);
                       for (int i = inx + 1; i < tmpTestValues.Count; i++)
                           testval.subValues.Add(tmpTestValues[i]);
                       for (int i = tmpTestValues.Count - 1; i > inx; i--)
                           tmpTestValues.RemoveAt(i);
                       break;
                   case (int)RuleConstants.RULE_TESTSELECTVALUE_NUMBER:

                       return token.Tokens[0].ToString();
                   case (int)RuleConstants.RULE_TESTSELECTVALUES:
                       return token.Tokens[0].UserObject;


                   case (int)RuleConstants.RULE_TESTSELECTITEM_IDENTIFIER_LPARAN_RPARAN:

                       name = token.Tokens[0].UserObject.ToString();
                       value = Convert.ToInt32(token.Tokens[2].UserObject.ToString());
                       tmpTestValues.Add(testval = new TestValue(name, value));
                       return testval;



                   case (int)RuleConstants.RULE_TESTITEM2:

                       return token.Tokens[0].UserObject;

                   case (int)RuleConstants.RULE_RANGEITEM_IDENTIFIER_LPARAN_RPARAN:
                       NonterminalToken ntok;
                       name = token.Tokens[getTokenInx(token, "Identifier")].UserObject.ToString();
                       ntok = findToken(token, "Bytes");
                       bytes = Convert.ToInt32(ntok.Tokens[0].UserObject.ToString());
                       ntok = findToken(token, "LValue");
                       int lval = Convert.ToInt32(ntok.Tokens[0].UserObject.ToString());
                       ntok = findToken(token, "HValue");
                       int hval = Convert.ToInt32(ntok.Tokens[0].UserObject.ToString());
                       TmpCmdItems.Add(cmdItem = new CmdItem(name, bytes, lval, hval));
                       return cmdItem;


                   case (int)RuleConstants.RULE_SELECTITEM_IDENTIFIER_LPARAN_RPARAN:
                       SelectValue[] selectvalues;
                       name = token.Tokens[0].UserObject.ToString();
                       bytes = Convert.ToInt32(findToken(token, "Bytes").Tokens[0].UserObject.ToString());
                       selectvalues = new SelectValue[TmpSelectValues.Count];
                       for (int i = 0; i < TmpSelectValues.Count; i++)
                       {
                           selectvalues[i] = (SelectValue)TmpSelectValues[i];
                       }
                       TmpCmdItems.Add(cmdItem = new CmdItem(name, bytes, selectvalues));
                       TmpSelectValues.Clear();
                       return cmdItem;

                   case (int)RuleConstants.RULE_SELECTVALUE_NUMBER:
                       SelectValue sVal = new SelectValue();
                       sVal.value = Convert.ToInt32(token.Tokens[0].UserObject.ToString());
                       sVal.valueName = findToken(token, "ValueDescription").Tokens[0].UserObject.ToString();
                       TmpSelectValues.Add(sVal);

                       break;
                   case (int)RuleConstants.RULE_EXPRESSITEM:
                       return token.Tokens[0].UserObject;

                   case (int)RuleConstants.RULE_REPEATEXPRESS_LBRACE_RBRACE:
                       inx = TmpCmdItems.IndexOf(token.Tokens[0].UserObject);
                       for (int i = inx + 1; i < TmpCmdItems.Count; i++)
                           ((CmdItem)TmpCmdItems[inx]).AddSubItems((CmdItem)TmpCmdItems[i]);//reduce repeat item
                       for (int i = TmpCmdItems.Count - 1; i > inx; i--)
                           TmpCmdItems.RemoveAt(i);
                       return TmpCmdItems[inx];
                   case (int)RuleConstants.RULE_SENDEXPRESS_SENDEQ:  //send express
                       tmpSendExpress.Clear();
                       break;
                   case (int)RuleConstants.RULE_SENDEXPRESS_SENDEQ2: //sendexpress
                       tmpSendExpress = (System.Collections.ArrayList)TmpCmdItems.Clone();
                       TmpCmdItems.Clear();
                       break;
                   case (int)RuleConstants.RULE_RETURNEXPRESS_RETURNEQ:
                       tmpReturnExpress.Clear();
                       break;
                   case (int)RuleConstants.RULE_RETURNEXPRESS_RETURNEQ2:
                       tmpReturnExpress = (System.Collections.ArrayList)TmpCmdItems.Clone();
                       TmpCmdItems.Clear();
                       break;
                   case (int)RuleConstants.RULE_DEVICETYPE_DEVICETYPEEQ_IDENTIFIER:
                       this.DeviceType = token.Tokens[1].UserObject.ToString();
                       break;
                   case (int)RuleConstants.RULE_IP_IPEQ_IP:
                       ip = token.Tokens[1].UserObject.ToString();
                       break;
                   case (int)RuleConstants.RULE_PORT_PORTEQ_NUMBER:
                       port = int.Parse(token.Tokens[1].UserObject.ToString());
                       break;
                   case (int)RuleConstants.RULE_DEVICEID_DEVICEIDEQ_DEVICEID:
                       deviceId = Convert.ToInt32(token.Tokens[1].ToString(), 16);
                       break;
                   case (int)RuleConstants.RULE_CMD_CMDEQ_CMD:
                       //Cmd cmd;
                       //if(token.Tokens[2].UserObject==null)
                       //  cmd=new Cmd(Convert.ToByte(token.Tokens[1].UserObject.ToString(),16));
                       //else
                       //cmd=new Cmd(Convert.ToByte(token.Tokens[1].UserObject.ToString(),16),
                       //    Convert.ToByte(token.Tokens[2].UserObject.ToString(),16));
                       //  return cmd;

                       break;
                   case (int)RuleConstants.RULE_COMMAND:

                       //CmdClass cls;
                       //CmdType type;
                       //string desc;
                       //string func_name;
                       //byte cmdcode;
                       Cmd cmd = new Cmd(this);

                       // inx  = getTokenInx(token, "Description");


                       for (int i = 0; i < token.Tokens.Length; i++)
                       {
                           NonterminalToken ttok;
                           ttok = (NonterminalToken)token.Tokens[i];
                           if (token.Rule.Rhs[i].Name == "Description")
                           {
                               cmd.description = ((NonterminalToken)token.Tokens[i]).Tokens[1].UserObject.ToString().Trim(new char[] { '\"' });
                           }
                           else if (token.Rule.Rhs[i].Name == "FuncName")
                           {
                               cmd.cmdName = ((NonterminalToken)token.Tokens[i]).Tokens[1].UserObject.ToString().Trim(new char[] { '\"' });
                           }
                           else if (token.Rule.Rhs[i].Name == "CmdClass")
                           {
                               switch (((NonterminalToken)token.Tokens[i]).Tokens[1].UserObject.ToString()[0])
                               {
                                   case 'A':
                                       cmd.cmdClass = CmdClass.A;
                                       break;
                                   case 'B':
                                       cmd.cmdClass = CmdClass.B;
                                       break;
                                   case 'C':
                                       cmd.cmdClass = CmdClass.C;
                                       break;
                                   case 'D':
                                       cmd.cmdClass = CmdClass.D;
                                       break;
                               }
                           }
                           else if (token.Rule.Rhs[i].Name == "CmdType")
                           {
                               if (((NonterminalToken)token.Tokens[i]).Tokens[1].UserObject.ToString() == "Set")
                                   cmd.cmdType = CmdType.CmdSet;
                               else if (((NonterminalToken)token.Tokens[i]).Tokens[1].UserObject.ToString() == "Query")
                                   cmd.cmdType = CmdType.CmdQuery;
                               else if (((NonterminalToken)token.Tokens[i]).Tokens[1].UserObject.ToString() == "Report")
                                   cmd.cmdType = CmdType.CmdReport;
                               else
                                   cmd.cmdType = CmdType.CmdUnkonwn;
                           }
                           else if (token.Rule.Rhs[i].Name == "Cmd")
                           {
                               cmd.cmd = Convert.ToByte(ttok.Tokens[1].UserObject.ToString(), 16);  //cmd
                               if (((NonterminalToken)ttok.Tokens[2]).Tokens.Length > 0)
                                   cmd.subCmd = Convert.ToByte(((NonterminalToken)ttok.Tokens[2]).Tokens[0].UserObject.ToString(), 16); //subCmd
                           }



                       } //for
                       cmd.SendCmdItems = (System.Collections.ArrayList)tmpSendExpress.Clone();
                       cmd.ReturnCmdItems = (System.Collections.ArrayList)tmpReturnExpress.Clone();
                       cmd.TestGroupValues = tmpTestExpress;
                       cmd.CanTest = CanTest;
                       tmpReturnExpress.Clear();
                       tmpSendExpress.Clear();
                       tmpTestValues.Clear();
                       tmpTestExpress = new System.Collections.ArrayList(5);
                       //try
                       //{
                       if (cmd.cmdType != CmdType.CmdReport)
                           cmd.CheckTestValue();
                       cmd.GenerateCmdDataSet();

                       //}
                       //catch (Exception ex)
                       //{
                       //    Console.WriteLine(ex.StackTrace);
                       //}
                       this.AddCmd(cmd);
                       break;

               }
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.StackTrace);
               throw ex;
           }

           return null;
       }

       //public System.Data.DataSet[] GetETTU_SendDataSet(string func_name)
       //{
       //    Cmd cmd = (Cmd)func_name_hashCommands[func_name];
       //    if (cmd == null) return null;

       //    System.Data.DataSet orgDs = cmd.GetSendCmdTemplateDs();
       //    if (orgDs.Tables[0].Columns.Count > 1 && orgDs.Tables[0].Columns[1].ColumnName == "length")
       //    {
       //        System.Data.DataSet[] dss=new System.Data.DataSet[(int)orgDs.Tables[0].Rows[0]["length"]];

       //        for(int i=0;i<dss.Length;i++)
       //            dss[i]=cmd

       //    }

       //    else
       //        return new System.Data.DataSet[]{orgDs};

       //}

       public System.Data.DataSet GetSendDataSet(string func_name)
       {
           Cmd cmd=(Cmd)func_name_hashCommands[func_name];
           if (cmd == null) return null;
           else
           return cmd.GetSendCmdTemplateDs();
       }
       public System.Data.DataSet GetReturnDataSet(string func_name)
       {
           Cmd cmd = (Cmd)func_name_hashCommands[func_name];
           if (cmd == null) return null;
           else
               return cmd.GetReturnCmdTemplateDs();
       }
       public SendPackage GetReturnPackage(System.Data.DataSet ds, int address)
       {
           Cmd cmd = (Cmd)func_name_hashCommands[ds.Tables["tblMain"].Rows[0]["func_name"].ToString()];
           if (cmd == null)
               return null;
           else
               return cmd.GetReturnSendPackage(ds, address);

       }
       

       public SendPackage GetSendPackage(System.Data.DataSet ds,int address)
       {
           Cmd cmd=(Cmd)  func_name_hashCommands[ds.Tables["tblMain"].Rows[0]["func_name"].ToString()];
           if (cmd == null)
               return null;
           else
               return cmd.GetSendPackage(ds, address);
          
       }

       public SendPackage[] GetETTU_SendPackage(System.Data.DataSet ds, int address)
       {
           Cmd cmd = (Cmd)func_name_hashCommands[ds.Tables["tblMain"].Rows[0]["func_name"].ToString()];
           if (cmd == null)
               return null;

         //  SendPackage orgPkg=
           if (ds.Tables[0].Columns.Count > 1 && ds.Tables[0].Columns[1].ColumnName == "length")
           {
               SendPackage[] pkgs = new SendPackage[System.Convert.ToInt32(ds.Tables[0].Rows[0]["length"])];
               SendPackage orgpkg = cmd.GetSendPackage(ds, address);

               for (int i = 0; i < pkgs.Length; i++)
               {
                   byte[] data=new byte[2+(orgpkg.text.Length-2)/pkgs.Length];
                   data[0]=orgpkg.text[0];
                   data[1]=orgpkg.text[1];
                   System.Array.Copy(orgpkg.text,2+i*(data.Length-2),data,2,data.Length-2);
                   if(i==pkgs.Length-1 && cmd.cmdType== CmdType.CmdQuery)
                       pkgs[i] = new SendPackage(CmdType.CmdQuery, CmdClass.A, address, data);
                   else
                       pkgs[i] = new SendPackage(CmdType.CmdSet, CmdClass.A, address, data);

               }

               return pkgs;
           }
           else
               return  new SendPackage[]{ cmd.GetSendPackage(ds, address)};


       }
       protected  override Object CreateObject(TerminalToken token)
       {
           switch (token.Symbol.Id)
           {

               case (int)com.calitha.goldparser.SymbolConstants.SYMBOL_IDENTIFIER:
                   return token.Text;
               case (int)SymbolConstants.SYMBOL_IP:
                   return token.Text;
               case (int)SymbolConstants.SYMBOL_NUMBER:
                   return token.Text;
               case (int)SymbolConstants.SYMBOL_FLOAT:
                   return token.Text;
               case (int)SymbolConstants.SYMBOL_DEVICEID:
                   return token.Text;
                  
               case (int)SymbolConstants.SYMBOL_CMD:
                   return token.Text;
               case (int)SymbolConstants.SYMBOL_SUBCMD:
                   return token.Text;
               case (int)SymbolConstants.SYMBOL_STRINGLITERAL:
                   return token.Text;
               case (int)SymbolConstants.SYMBOL_CMDCLASS:
                   if (!(token.Text == "A" || token.Text == "B" || token.Text == "C" || token.Text == "D" || token.Text == "N"))
                     throw new SymbolException(token.Location+", must be 'A' or 'B' or 'C' or 'D' or 'N'==>"+token.Text);
                   return token.Text;
               case (int)SymbolConstants.SYMBOL_CMDTYPE:
                   if (!(token.Text == "Set" || token.Text == "Query" || token.Text == "Report" ))
                       throw new SymbolException(token.Location + ", must be 'Set' or 'Query' or 'Report' ==>" + token.Text);
                   return token.Text;
               //case (int)SymbolConstants.SYMBOL_LVALUE:
               //    return token.Text;
                  
               //case (int)SymbolConstants.SYMBOL_HVALUE:
               //    return Convert.ToInt32(token.Text);
               //case (int) SymbolConstants.SYMBOL_BYTES:
                   //return token.Text;
            

                 //  break;

           }
           return null;
       }

     public   System.Data.DataSet GetSendDsByTextPackage(TextPackage txt,CmdType cmdtype)
       {

           Cmd cmd ;

           //if (txt.Cmd == 0x04)
           //    cmd = getCmd((byte)txt.Cmd, txt.Text[1], cmdtype);
           //else
           //    cmd = getCmd((byte)txt.Cmd, (txt.Cmd == 0x0f) ? txt.Text[1] : (byte)0xff, cmdtype);
           cmd = getCmdByTextPackage(txt, cmdtype);
           if (cmd == null)
               return null;
           else
             return  cmd.GetSendDsByTextPackage(txt);
          
         
       }

       public System.Data.DataSet GetETTU_SendDsByReportTextPackage(TextPackage txtpkg)
       {

           Cmd cmd;

           TextPackage txt = new TextPackage();
           txt.Text = txtpkg.ETTU_Text;
           txt.Seq = txtpkg.Seq;
           txt.Err = txtpkg.Err;
           txt.Address = txtpkg.Address;

           //if (txt.Cmd == 0x04)
           //    cmd = getCmd((byte)txt.Cmd, txt.Text[1], cmdtype);
           //else
           //    cmd = getCmd((byte)txt.Cmd, (txt.Cmd == 0x0f) ? txt.Text[1] : (byte)0xff, cmdtype);
           cmd = getCmdByTextPackage(txt, CmdType.CmdReport);
           if (cmd == null)
               return null;
           else
               return cmd.GetSendDsByTextPackage(txt);

       }

       public System.Data.DataSet GetETTU_ReturnDsByTextPackage(TextPackage[] txt)
       {
           Cmd cmd;

         
          
           byte[] data = new byte[txt[0].ETTU_Text.Length + (txt[0].ETTU_Text.Length - 2) * (txt.Length - 1)];
         //  int inx = 0;
           for (int i = 0; i < txt.Length; i++)
           {
               if (i == 0)
                   System.Array.Copy(txt[0].ETTU_Text, data, txt[0].ETTU_Text.Length);
               else
                   System.Array.Copy(txt[i].ETTU_Text, 2, data, txt[0].ETTU_Text.Length + (i - 1) * (txt[0].ETTU_Text.Length - 2), txt[0].ETTU_Text.Length - 2);
           }

           cmd = getCmdByTextPackage(txt[0], CmdType.CmdQuery);
           if (cmd == null)
           {
               Console.WriteLine(txt.ToString() + "look up fail!");
               return null;

           }

           TextPackage txtpkg = new TextPackage();
           txtpkg.Text = data;

           return  cmd.GetETTUReturnDsByTextPackage(txt.Length,txtpkg);

             
       }

       public System.Data.DataSet GetReturnDsByTextPackage(TextPackage txt)
       {
           Cmd cmd;

           //if (txt.Cmd == 0x04)
           //    cmd = getCmd((byte)txt.Cmd, txt.Text[1], CmdType.CmdQuery);
           

           //else
           //    cmd = getCmd((byte)txt.Cmd, (txt.Cmd == 0x0f) ? txt.Text[1] : (byte)0xff, CmdType.CmdQuery);
           cmd = getCmdByTextPackage(txt, CmdType.CmdQuery);
           if (cmd == null)
           {
               Console.WriteLine(txt.ToString() + "look up fail!");
               return null;

           }
           else
               return cmd.GetReturnDsByTextPackage(txt);

       }


       //  查詢命令專用 ,主動回報
       private Cmd getCmdByTextPackage(TextPackage txt,CmdType cmdtype)  //just for receive
       {
           Cmd cmd;
           byte code;
          
               switch (txt.Cmd)
               {
                   case 0x04:
                       code = txt.Text[7];
                       if ((code & 0x0f) != 0x0f)

                           cmd = getCmd((byte)txt.Cmd, txt.Text[7], cmdtype);
                       else
                    
                           cmd = getCmd((byte)txt.Cmd, txt.Text[7], txt.Text[8], cmdtype);
                     
                       
                       break;
                   case 0x52:
                       if (txt.Text.Length == 2)
                           cmd = getCmd((byte)txt.Cmd, (txt.Cmd == 0x0f) ? txt.Text[1] : (byte)0xff, cmdtype);
                       else
                           cmd = getCmd((byte)txt.Cmd, 0x01, cmdtype);
                       break;

                   case 0xb6:
                       if (txt.Text.Length == 2)
                           cmd = getCmd((byte)txt.Cmd, 0xff, cmdtype);
                       else
                           cmd = getCmd((byte)txt.Cmd, 1, cmdtype);
                       break;
                   case 0x5f: // mas 5f 22
                       if (txt.SubCmd==0x22)
                       {
                           if (txt.Text.Length == 4)
                              cmd= getCmd((byte)txt.Cmd, 0x22, cmdtype);
                           else
                              cmd = getCmd((byte)txt.Cmd, 0xfe, cmdtype);
                       }
                       else if (txt.SubCmd == 0x12) //tts
                       {
                           if(txt.Text.Length==4)
                               cmd = getCmd((byte)txt.Cmd, 0x12, cmdtype);
                           else
                               cmd = getCmd((byte)txt.Cmd, 0xfe, cmdtype);
                       }
                       else


                       goto default;
                          break;
                   case 0xAD:
                       if(txt.Text.Length==3)
                            cmd = getCmd((byte)txt.Cmd, (byte)0xff, cmdtype);
                       else
                            cmd = getCmd((byte)txt.Cmd, 0x01, cmdtype);
                        break;
                   case 0xdf:
                       if (txt.Text[1] == 0xd2 && txt.Text.Length == 3)

                           cmd = getCmd((byte)txt.Cmd, txt.Text[1], 0x01, cmdtype);
                       //else if (txt.Text[1] == 0xd5)
                       //{
                       //    if (txt.Text[10] == 0)  //txt
                       //        cmd = getCmd((byte)txt.Cmd, txt.Text[1],(byte) 0x01, CmdType.CmdQuery);
                       //    else
                       //        cmd = getCmd((byte)txt.Cmd, txt.Text[1], CmdType.CmdQuery);
                       //  //      if((this.findCmditem(this.ReturnCmdItems,"data_type").Min!=0 )) //graphic,or default msg
                       //  //     return V2DLE.ToHexString(new byte[] { cmd, subCmd, 0x01 }).Replace(' ', '_') + this.cmdType;
                       //  //else
                       //  //     return V2DLE.ToHexString(new byte[] { cmd, subCmd }).Replace(' ', '_') + this.cmdType;
                       //}
                       else
                           goto default;
                       break;
                   default:
                       if (this.DeviceType != "ETTU")
                           cmd = getCmd((byte)txt.Cmd, ((txt.Cmd & 0x0f) == 0x0f) ? txt.Text[1] : (byte)0xff, cmdtype);
                       else
                           cmd = getCmd((byte)txt.ETTU_Cmd, txt.ETTU_SubCmd, cmdtype);
                       break;
               }
           
         



           return cmd;
        
       }


       //public  SendPackage  Get_SetbackgroundPic30_Package(int device_id, PKG_SetbackgroundPic30 text)
       //{
       //    return new SendPackage(CmdType.CmdSet,CmdClass.A,device_id,text.getSendText());
       //}

    }
}
