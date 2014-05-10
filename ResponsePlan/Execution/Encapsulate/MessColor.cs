using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    [Serializable]
    class MessColor
    {
        private int id;
        private string keyMess;
        private System.Drawing.Color forecolor;
        private System.Drawing.Color backColor;

        public MessColor(int id,string keyMess,System.Drawing.Color forecolor,System.Drawing.Color backColor)
        {
            this.id = id;
            this.keyMess = keyMess;
            this.forecolor = forecolor;
            this.backColor = backColor;
        }
        
        public int ID
        {
            get { return id; }
        }

        public string KeyMess
        {
            get { return keyMess; }
        }

        public System.Drawing.Color Forecolor
        {
            get { return forecolor; }
        }

        public System.Drawing.Color BackColor
        {
            get { return backColor; }
        }
    }
}
