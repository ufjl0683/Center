using System;
using System.Collections.Generic;
using System.Text;


namespace RemoteInterface.MFCC
{
    [Serializable]
    public class RGS_GenericDisplay_Data
    {
        public byte mode;
        public byte graph_code_id;
        public RGS_Generic_ICON_Data[] icons;
        public RGS_Generic_Message_Data[] msgs;
        public RGS_Generic_Section_Data[] sections;
        public string main_display_template;// 2014/4/27  for 
        public string opt_display_template;

        public int alarm_class=-1;

        public RGS_GenericDisplay_Data(byte mode, byte graph_code_id, RGS_Generic_ICON_Data[] icons, RGS_Generic_Message_Data[] msgs, RGS_Generic_Section_Data[] sections)
        {
            this.mode = mode;
            this.icons = icons;
            this.msgs = msgs;
            this.sections = sections;
            this.graph_code_id = graph_code_id;
        }


        public bool Equals(RGS_GenericDisplay_Data data)
        {
            if (mode != data.mode)
            {
               // Console.WriteLine("mode different!");
                return false;
            }
            if (graph_code_id != data.graph_code_id)
            {
               // Console.WriteLine("g_code_id  different!");
                return false;
            }

            if (icons.Length != data.icons.Length)
            {
               // Console.WriteLine("icons.Length different!");
                return false;
            }
            for (int i = 0; i < icons.Length; i++)
                if (!icons[i].Equals(data.icons[i]))
                {
                 //  Console.WriteLine("icon data different!");
                    return false;
                }
            if (msgs.Length != data.msgs.Length)
            {
              //  Console.WriteLine("mesg.Length different!");
                return false;
            }
            for (int i = 0; i < msgs.Length; i++)
                if (!msgs[i].Equals(data.msgs[i]))
                    return false;
            if (sections.Length != data.sections.Length)
            {
               // Console.WriteLine("section.Length different!");
                return false;
            }
            for (int i = 0; i < sections.Length; i++)
                if (!sections[i].Equals(data.sections[i]))
                {
                  //  Console.WriteLine("section data different!");
                    return false;
                }

            return true;
        }

        public override string ToString()
        {
            
             string ret="mode:"+mode +" ";
             ret += "g_codeid:" + this.graph_code_id+" ";;
             foreach (RGS_Generic_ICON_Data icon in icons)
                 ret += icon.ToString() + " ";
             foreach (RGS_Generic_Message_Data mesg in msgs)
                 ret += mesg.ToString() + " ";

             foreach (RGS_Generic_Section_Data sec in sections)
                 ret += sec.ToString() + " ";


             return ret;
            //return base.ToString();
        }

    }
}
