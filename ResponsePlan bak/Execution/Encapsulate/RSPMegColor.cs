using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    class RSPMegColor
    {
        private string meg;
        private byte color;
        private System.Drawing.Color forecolor;
        private System.Drawing.Color backColor;

        public RSPMegColor(string meg, byte color)
        {
            this.meg = meg;
            this.color = color;
        }

        public RSPMegColor(string meg, System.Drawing.Color forecolor, System.Drawing.Color backColor)
        {
            this.meg = meg;
            this.forecolor = forecolor;
            this.backColor = backColor;
        }

        public string Message
        {
            get { return meg; }
        }

        public byte Color
        {
            get { return color; }
        }

        public System.Drawing.Color ForeColor
        {
            get { return forecolor; }
        }

        public System.Drawing.Color BackColor
        {
            get { return backColor; }
        }
    }
}
