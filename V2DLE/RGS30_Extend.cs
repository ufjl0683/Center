using System;
using System.Collections.Generic;
using System.Drawing;
using RemoteInterface.MFCC;
namespace Comm
{
 


   
    public class RGS30_Extend
    {
        const int FRAME_SIZE = 20000*3;
      
        private  static  SendPackage get_SetbackgroundPic_pkg(int address,byte mode, byte g_code_id, byte frame_no, byte frame_id, 
            ushort g_width,ushort g_height,byte [] g_desc,byte[] g_pattern_color)
        {
            byte[] senddata;
            senddata=new byte[1+1+1+1+1+2+2+32+g_pattern_color.Length];
            senddata[0] = 0x97;
            senddata[1] = mode;
            senddata[2] = g_code_id;
            senddata[3] = frame_no;
            senddata[4] = frame_id;
            senddata[5] =(byte)( g_width / 256);
            senddata[6] = (byte)(g_width % 256);
            senddata[7] = (byte)(g_height/256);
            senddata[8] = (byte)(g_height % 256);
            
            Array.Copy(g_desc, 0,senddata,9,32);
            Array.Copy(g_pattern_color, 0, senddata, 9 + 32, g_pattern_color.Length);
            return  new SendPackage( CmdType.CmdSet,CmdClass.A,address, senddata);
        }
        private static SendPackage get_SetIconPic_pkg(int address, byte icon_id, byte frame_no, byte frame_id,
           ushort g_width, ushort g_height, byte[] g_desc, byte[] g_pattern_color)
        {
            byte[] senddata;
            senddata = new byte[1 + 1 + 1 + 1 + 1 + 2 + 2 + 32 + g_pattern_color.Length];
            senddata[0] = 0x5f;
            senddata[1] = 0x51;
            senddata[2] = icon_id;
            senddata[3] = frame_no;
            senddata[4] = frame_id;
            senddata[5] = (byte)(g_width / 256);
            senddata[6] = (byte)(g_width % 256);
            senddata[7] = (byte)(g_height / 256);
            senddata[8] = (byte)(g_height % 256);

            Array.Copy(g_desc, 0, senddata, 9, 32);
            Array.Copy(g_pattern_color, 0, senddata, 9 + 32, g_pattern_color.Length);
            return new SendPackage(CmdType.CmdSet, CmdClass.A, address, senddata);
        }

       // private static SendPackage curr_pkg;
        public static void SetBackgroundPic(I_DLE device,int address,byte mode,byte g_code_id,string desc,Bitmap pic)
        {
              SendPackage curr_pkg;
           byte frame_no, frame_id;
            ushort g_width, g_height;
            byte [] g_desc=new byte[32];
            byte[] g_pattern_color;
            int no_pixels = pic.Width * pic.Height;
            int inx;
#if DEBUG
         //   device.OnReceiveText += new OnTextPackageEventHandler(device_OnReceiveText);
#endif
            g_width =(ushort) pic.Width;
            g_height = (ushort)pic.Height;
            byte[]b=System.Text.Encoding.Unicode.GetBytes(desc);
            Array.Copy(b,g_desc,(b.Length<32)?b.Length:32);
            if (no_pixels*3 % FRAME_SIZE == 0)
                frame_no =(byte)(no_pixels*3 / FRAME_SIZE);
            else
                frame_no = (byte)(no_pixels*3 / FRAME_SIZE + 1);
            inx = 0;

            for (frame_id = 1; frame_id <= frame_no; frame_id++)
            {
                int x,y;
                if (frame_id * FRAME_SIZE <= no_pixels * 3)
                    g_pattern_color = new byte[FRAME_SIZE];
                else
                    g_pattern_color = new byte[(no_pixels * 3) % FRAME_SIZE];


                for (int i = 0; i < g_pattern_color.Length; i+=3)
                {
                     y = inx / pic.Width;
                     x = inx % pic.Width;
                     g_pattern_color[i] = pic.GetPixel(x, y).R;
                     g_pattern_color[i+1] = pic.GetPixel(x, y).G;
                     g_pattern_color[i+2] = pic.GetPixel(x, y).B;
                         inx++;
                }
                
               // if(device!=null)
                SendPackage pkg = get_SetbackgroundPic_pkg(address, mode, g_code_id, (byte)frame_no, (byte)frame_id, g_width, g_height, g_desc, g_pattern_color);
                curr_pkg = pkg;    
                Console.WriteLine(frame_id + "/" + frame_no);
                    device.Send(pkg);
                    if (pkg.result != CmdResult.ACK)
                        throw new Exception("send fail! ");
                 //   System.Threading.Thread.Sleep(3000);
                   // GC.Collect();

                   
              
            }

          //  device.OnReceiveText -= device_OnReceiveText;
        }
        public static void SetIconPic(I_DLE device, int address, byte icon_id, string desc, Bitmap pic)
        {
            byte frame_no, frame_id;
            ushort g_width, g_height;
            byte[] g_desc = new byte[32];
            byte[] g_pattern_color;
            int no_pixels = pic.Width * pic.Height;
            int inx;
             SendPackage curr_pkg;
#if DEBUG
         //   device.OnReceiveText += new OnTextPackageEventHandler(device_OnReceiveText);
#endif
            g_width = (ushort)pic.Width;
            g_height = (ushort)pic.Height;
            byte[] b = System.Text.Encoding.Unicode.GetBytes(desc);
            Array.Copy(b, g_desc, (b.Length < 32 ) ? b.Length : 32);
            if (no_pixels * 3 % FRAME_SIZE == 0)
                frame_no = (byte)(no_pixels * 3 / FRAME_SIZE);
            else
                frame_no = (byte)(no_pixels * 3 / FRAME_SIZE + 1);
            inx = 0;

            for (frame_id = 1; frame_id <= frame_no; frame_id++)
            {
                int x, y;
                if (frame_id * FRAME_SIZE <= no_pixels * 3)
                    g_pattern_color = new byte[FRAME_SIZE];
                else
                    g_pattern_color = new byte[(no_pixels * 3) % FRAME_SIZE];


                for (int i = 0; i < g_pattern_color.Length; i += 3)
                {
                    y = inx / pic.Width;
                    x = inx % pic.Width;
                    g_pattern_color[i] = pic.GetPixel(x, y).R;
                    g_pattern_color[i + 1] = pic.GetPixel(x, y).G;
                    g_pattern_color[i + 2] = pic.GetPixel(x, y).B;
                    inx++;
                }

                // if(device!=null)
                SendPackage pkg = get_SetIconPic_pkg(address, icon_id, (byte)frame_no, (byte)frame_id, g_width, g_height, g_desc, g_pattern_color);
                curr_pkg = pkg;
                Console.WriteLine(frame_id + "/" + frame_no);
                device.Send(pkg);
                if (pkg.result != CmdResult.ACK)
                    throw new Exception("send fail! ");
                //   System.Threading.Thread.Sleep(3000);
                // GC.Collect();



            }

            //  device.OnReceiveText -= device_OnReceiveText;
        }

        //static void device_OnReceiveText(object sender, TextPackage txtObj)
        //{
        //    for (int i = 41; i < txtObj.Text.Length; i++)
        //    {
        //        if(txtObj.Text[i]!=curr_pkg.text[i])
        //            Console.WriteLine("not equal at text["+i+"]");
        //    }

        //    Console.WriteLine("equal!");
        //    //throw new Exception("The method or operation is not implemented.");
        //}


        //public static byte[] ToBig5Bytes(string text)
        //{

        //    byte[] b=System.Text.Encoding.Convert(System.Text.Encoding.Unicode,System.Text.Encoding.GetEncoding("big5"),System.Text.Encoding.Unicode.GetBytes(text));
        //    return b;
       
        //}


        public static Bitmap GetIconPic(I_DLE device, int address, byte icon_id, ref string desc)
        {
         //   RGS_SetBackgroundPic_frame frame = null;
            //System.IO.MemoryStream ms;
            byte[] cmdText = new byte[] { 0x04,0x5f,0x51, icon_id };
            int g_width, g_height;
            SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, address, cmdText);
            device.Send(pkg);
            if (pkg.result != CmdResult.ACK)
                throw new Exception("cmd error:" + pkg.result);


         //   byte frame_no = pkg.ReturnTextPackage.Text[3]; //0x98 frame_no
          //  ms = new System.IO.MemoryStream(1024 * 1024 * 3);


            //for (int i = 1; i <= frame_no; i++)
            //{
            //    cmdText = new byte[] { 0x98, mode, g_code_id, (byte)i };

            //    pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, address, cmdText);

            //    device.Send(pkg);
                

            //    frame = new RGS_SetBackgroundPic_frame(pkg.ReturnTextPackage);

            //    ms.Write(frame.g_pattern_color, 0, frame.g_pattern_color.Length);

            //}

            g_width = pkg.ReturnTextPackage.Text[12] * 256 + pkg.ReturnTextPackage.Text[13];
            g_height = pkg.ReturnTextPackage.Text[14] * 256 + pkg.ReturnTextPackage.Text[15];
            byte[] desc_code = new byte[32];
            Bitmap pic = new Bitmap(g_width, g_height);
            System.Array.Copy(pkg.ReturnTextPackage.Text, 16, desc_code, 0, desc_code.Length);
           // ms.Position = 0;
            desc = System.Text.Encoding.Unicode.GetString(desc_code);

            int inx = 16 + desc_code.Length;
            for (int y = 0; y < g_height; y++)
                for (int x = 0; x < g_width; x++)
                {
                    // int r, g, b;
                    //r = ms.ReadByte();
                    //g = ms.ReadByte();
                    //b = ms.ReadByte();
                    pic.SetPixel(x, y,Color.FromArgb( pkg.ReturnTextPackage.Text[inx],  pkg.ReturnTextPackage.Text[inx+1],  pkg.ReturnTextPackage.Text[inx+2]));
                    inx+=3;
                }

          

          //  ms.Dispose();
            return pic;
        }
        public static Bitmap GetBackgroundPic(I_DLE device, int address, byte mode, byte g_code_id, ref string desc)
        {
            RGS_SetBackgroundPic_frame frame=null;
            System.IO.MemoryStream ms;
            byte[] cmdText = new byte[] { 0x98, mode, g_code_id };
            SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, address, cmdText);
            device.Send(pkg);
            if (pkg.result != CmdResult.ACK)
                throw new Exception("cmd error:" + pkg.result);


            byte frame_no = pkg.ReturnTextPackage.Text[3]; //0x98 frame_no
            ms = new System.IO.MemoryStream(1024 * 1024*3);
            for (int i = 1; i <= frame_no; i++)
            {
                cmdText = new byte[] {0x98,mode,g_code_id,(byte)i };

                pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, address, cmdText);

                device.Send(pkg);

                frame = new RGS_SetBackgroundPic_frame(pkg.ReturnTextPackage);

                ms.Write(frame.g_pattern_color, 0, frame.g_pattern_color.Length);

            }


            Bitmap pic = new Bitmap(frame.g_width, frame.g_height);

            ms.Position = 0;

            for(int y =0;y<frame.g_height;y++)
                for (int x = 0; x < frame.g_width; x++)
                {
                   // int r, g, b;
                    //r = ms.ReadByte();
                    //g = ms.ReadByte();
                    //b = ms.ReadByte();
                    pic.SetPixel(x, y, Color.FromArgb(ms.ReadByte(),ms.ReadByte(), ms.ReadByte()));
                }

           desc= System.Text.Encoding.Unicode.GetString(frame.g_desc);
            

            ms.Dispose();
            return pic;
        }


        public static RGS_GenericDisplay_Data GetGenericDisplayData(byte[] text)
        {
            RGS_GenericDisplay_Data data = null;
            byte mode, g_code_id;
            int inx = 1;
            inx += 7; //skip hw_status,opmode,commstat,opstatus
            mode = text[inx++];
            g_code_id = text[inx++];
            RGS_Generic_ICON_Data[] icons = new RGS_Generic_ICON_Data[text[inx++]];
         //   RGS_Generic_Message_Data[] msgs;
            
            for (int i = 0; i < icons.Length; i++)
            {
                icons[i] = new RGS_Generic_ICON_Data(0, 0, 0);

                icons[i].icon_code_id=text[inx++];
                icons[i].x = (ushort)(text[inx++] * 256);
                icons[i].x += text[inx++];
                icons[i].y = (ushort)(text[inx++] * 256);
                icons[i].y += text[inx++];
            }

            RGS_Generic_Message_Data[] msgs = new RGS_Generic_Message_Data[text[inx++]];
            for (int i = 0; i < msgs.Length; i++)
            {
                msgs[i]=new RGS_Generic_Message_Data("",new Color[0],new Color[0],0,0);
                byte[] code_big5 = new byte[text[inx++]];
                for (int j = 0; j < code_big5.Length; j++)
                    code_big5[j] = text[inx++];
                string message = RemoteInterface.Utils.Util.Big5BytesToString(code_big5);//System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, code_big5));
                Color[] forecolor = new Color[message.Length];
                Color[] backcolor = new Color[message.Length];
                for (int j = 0; j < message.Length; j++)
                {
                    int r,g,b;
                    r=text[inx++];
                    g=text[inx++];
                    b=text[inx++];
                    forecolor[j] = Color.FromArgb(r, g, b);
                    r=text[inx++];
                    g=text[inx++];
                    b=text[inx++];
                    backcolor[j] = Color.FromArgb(r, g, b);

                }

                msgs[i].messgae = message;
                msgs[i].forecolor = forecolor;
                msgs[i].backcolor = backcolor;
                msgs[i].x = (byte)(text[inx++] * 256);
                msgs[i].x += text[inx++];
                msgs[i].y = (byte)(text[inx++] * 256);
                msgs[i].y += text[inx++];


            }


            RGS_Generic_Section_Data[] sections = new RGS_Generic_Section_Data[text[inx++]];
            for (int i = 0; i < sections.Length; i++)
            {
                sections[i] = new RGS_Generic_Section_Data(0, 0);
                sections[i].section_id = text[inx++];
                sections[i].status = text[inx++];
            }
            data = new RGS_GenericDisplay_Data(mode, g_code_id, icons, msgs, sections);

            return data;
        }
        public static RGS_GenericDisplay_Data GetGenericDisplayData(I_DLE device, int address)
        {
            RGS_GenericDisplay_Data data=null;
            SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, address, new byte[] { 0x9d });
            device.Send(pkg);
            if (pkg.result != CmdResult.ACK)
                throw new Exception(pkg.result.ToString());
            byte[] text = pkg.ReturnTextPackage.Text;
            byte mode, g_code_id;
            int inx = 1;
            inx += 7; //skip hw_status,opmode,commstat,opstatus
            mode = text[inx++];
            g_code_id = text[inx++];
            RGS_Generic_ICON_Data[] icons = new RGS_Generic_ICON_Data[text[inx++]];
         //   RGS_Generic_Message_Data[] msgs;
            
            for (int i = 0; i < icons.Length; i++)
            {
                icons[i] = new RGS_Generic_ICON_Data(0, 0, 0);

                icons[i].icon_code_id=text[inx++];
                icons[i].x = (ushort)(text[inx++] * 256);
                icons[i].x += text[inx++];
                icons[i].y = (ushort)(text[inx++] * 256);
                icons[i].y += text[inx++];
            }

            RGS_Generic_Message_Data[] msgs = new RGS_Generic_Message_Data[text[inx++]];
            for (int i = 0; i < msgs.Length; i++)
            {
                msgs[i]=new RGS_Generic_Message_Data("",new Color[0],new Color[0],0,0);
                byte[] code_big5 = new byte[text[inx++]];
                for (int j = 0; j < code_big5.Length; j++)
                    code_big5[j] = text[inx++];
                string message = RemoteInterface.Utils.Util.Big5BytesToString(code_big5);//System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, code_big5));
                Color[] forecolor = new Color[message.Length];
                Color[] backcolor = new Color[message.Length];
                for (int j = 0; j < message.Length; j++)
                {
                    int r,g,b;
                    r=text[inx++];
                    g=text[inx++];
                    b=text[inx++];
                    forecolor[j] = Color.FromArgb(r, g, b);
                    r=text[inx++];
                    g=text[inx++];
                    b=text[inx++];
                    backcolor[j] = Color.FromArgb(r, g, b);

                }

                msgs[i].messgae = message;
                msgs[i].forecolor = forecolor;
                msgs[i].backcolor = backcolor;
                msgs[i].x = (byte)(text[inx++] * 256);
                msgs[i].x += text[inx++];
                msgs[i].y = (byte)(text[inx++] * 256);
                msgs[i].y += text[inx++];


            }


            RGS_Generic_Section_Data[] sections = new RGS_Generic_Section_Data[text[inx++]];
            for (int i = 0; i < sections.Length; i++)
            {
                sections[i] = new RGS_Generic_Section_Data(0, 0);
                sections[i].section_id = text[inx++];
                sections[i].status = text[inx++];
            }
            data = new RGS_GenericDisplay_Data(mode, g_code_id, icons, msgs, sections);

            return data;
        }

        public static void SetGenericDisplay(I_DLE device, int address, RGS_GenericDisplay_Data data)
        {
            System.IO.MemoryStream ms=new System.IO.MemoryStream();
            ms.WriteByte(0x9c);
            ms.WriteByte(data.mode);
            ms.WriteByte(data.graph_code_id);
            ms.WriteByte((byte)data.icons.Length);
            for (int i = 0; i < data.icons.Length; i++)
            {
                ms.WriteByte(data.icons[i].icon_code_id);

                ms.WriteByte((byte)(data.icons[i].x / 256));
                ms.WriteByte((byte)(data.icons[i].x % 256));

                ms.WriteByte((byte)(data.icons[i].y/256));
                ms.WriteByte((byte)(data.icons[i].y%256));
            }
            ms.WriteByte((byte)data.msgs.Length);
            for (int i = 0; i < data.msgs.Length; i++)
            {
                byte[] code_big5 = RemoteInterface.Utils.Util.StringToBig5Bytes(data.msgs[i].messgae);// Comm.RGS30_Extend.ToBig5Bytes(data.msgs[i].messgae);
                
                ms.WriteByte((byte)code_big5.Length);
                ms.Write(code_big5, 0, code_big5.Length);
                for (int j = 0; j < data.msgs[i].messgae.Length; j++)
                {
                    ms.WriteByte(data.msgs[i].forecolor[j].R);
                    ms.WriteByte(data.msgs[i].forecolor[j].G);
                    ms.WriteByte(data.msgs[i].forecolor[j].B);
                    ms.WriteByte(data.msgs[i].backcolor[j].R);
                    ms.WriteByte(data.msgs[i].backcolor[j].G);
                    ms.WriteByte(data.msgs[i].backcolor[j].B);

                }
                ms.WriteByte((byte)(data.msgs[i].x / 256));
                ms.WriteByte((byte)(data.msgs[i].x % 256));
                ms.WriteByte((byte)(data.msgs[i].y / 256));
                ms.WriteByte((byte)(data.msgs[i].y % 256));

            }

            ms.WriteByte((byte)data.sections.Length);
            for (int i = 0; i < data.sections.Length; i++)
            {
                ms.WriteByte(data.sections[i].section_id);
                ms.WriteByte(data.sections[i].status);
            }
            byte[]text=new byte[ms.Position];
            System.Array.Copy(ms.GetBuffer(),text,text.Length);
            SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, address, text);
            device.Send(pkg);
            if (pkg.result != CmdResult.ACK)
                throw new Exception(pkg.result.ToString());
        }

        public static void RGS_setPolygons(I_DLE device, int address, byte g_code_id, RGS_PolygonData polygonData)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.WriteByte(0x9a);
            ms.WriteByte(g_code_id);
            ms.WriteByte((byte)polygonData.polygons.Length);//section_count

            for (byte section_id = 1; section_id <= polygonData.polygons.Length; section_id++)
            {
                ms.WriteByte(section_id);//section_id
                ms.WriteByte((byte)polygonData.polygons[section_id - 1].points.Length);//length
                for (int point_inx = 0; point_inx < polygonData.polygons[section_id - 1].points.Length; point_inx++)
                {
                    ms.WriteByte((byte)(polygonData.polygons[section_id - 1].points[point_inx].X / 256));
                    ms.WriteByte((byte)(polygonData.polygons[section_id - 1].points[point_inx].X % 256));
                    ms.WriteByte((byte)(polygonData.polygons[section_id - 1].points[point_inx].Y / 256));
                    ms.WriteByte((byte)(polygonData.polygons[section_id - 1].points[point_inx].Y % 256));
                }
            }

             byte[]text=new byte[ms.Position];
             System.Array.Copy(ms.GetBuffer(),text,ms.Position);

             SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, address, text);
             device.Send(pkg);

        }


        public static RGS_PolygonData RGS_getPolygons(I_DLE device, int address, byte g_code_id)
        {
            RGS_PolygonData polygonData;

            byte[] cmdtext = new byte[] {0x9b,g_code_id };
           
            SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, address, cmdtext);
            device.Send(pkg);
            if (pkg.result != CmdResult.ACK)
                throw new Exception(pkg.result.ToString());
            byte[] retText = pkg.ReturnTextPackage.Text;
            int inx = 1;
            if (retText[inx++] != g_code_id)
                throw new Exception(" g_code_id is wrong !");

            RGS_Ploygon []polygons=new RGS_Ploygon[retText[inx++]];
          //  sec_id = retText[inx++];
            for (int i = 0; i < polygons.Length; i++)
            {
                polygons[i] = new RGS_Ploygon(retText[inx++]); //no points
                for (int j = 0; j < polygons[i].points.Length; j++)
                {
                    polygons[i].points[j].X = retText[inx++] * 256;
                    polygons[i].points[j].X += retText[inx++];
                    polygons[i].points[j].Y = retText[inx++] * 256;
                    polygons[i].points[j].Y += retText[inx++];
                }
            }

            polygonData = new RGS_PolygonData(polygons);

            return polygonData;
           

        }





     }




     [Serializable]
     public class RGS_SetBackgroundPic_frame
    {
      public  byte mode,frame_no,frame_id,g_code_id;
      public  ushort g_width, g_height;
      public  byte[] g_desc = new byte[32];
      public  byte[] g_pattern_color;

       
        public RGS_SetBackgroundPic_frame(TextPackage txt)
        {
            if (txt.Cmd != 0x97  && txt.Cmd != 0x98)
                throw new Exception("wrong cmd package,0x97,0x98 are required");
            mode = txt.Text[1];
            g_code_id = txt.Text[2];
            frame_no = txt.Text[3];
            frame_id = txt.Text[4];
            g_width =(ushort)( txt.Text[5] * 256 + txt.Text[6]);
            g_height =(ushort)(txt.Text[7] * 256 + txt.Text[8]);
            Array.Copy(txt.Text, 9, g_desc, 0, 32);
            g_pattern_color = new byte[txt.Text.Length - 41];
            Array.Copy(txt.Text, 41, g_pattern_color, 0,g_pattern_color.Length);

        }

       
    }


 
   

   
   

   
}


