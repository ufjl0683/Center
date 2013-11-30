using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    class BlockMegAndColor
    {
        private string meg;
        private byte[] color;
        private System.Drawing.Color[] forecolor;
        private System.Drawing.Color[] backColor;

        public BlockMegAndColor(string meg, byte[] color)
        {
            this.meg = meg;
            this.color = color;
            this.forecolor = new System.Drawing.Color[color.Length];
            this.backColor = new System.Drawing.Color[color.Length];
        }

        public BlockMegAndColor(string meg, System.Drawing.Color[] forecolor, System.Drawing.Color[] backColor)
        {
            this.meg = meg;
            this.forecolor = forecolor;
            this.backColor = backColor;
            this.color = new byte[forecolor.Length];
        }

        public string Message
        {
            get { return meg; }
        }

        public byte[] Color
        {
            get { return color; }
        }

        public System.Drawing.Color[] ForeColor
        {
            get { return forecolor; }
        }

        public System.Drawing.Color[] BackColor
        {
            get { return backColor; }
        }

        public System.Drawing.Color[] getSubColors(System.Drawing.Color[] colors, int startIndex, int length)
        {
            System.Drawing.Color[] surColor = new System.Drawing.Color[length];
            int k = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                surColor[k] = colors[i];
                k++;
            }
            return surColor;
        }

        public byte[] getSubColors(byte[] colors, int startIndex, int length)
        {
            byte[] surColor = new byte[length];
            int k = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                surColor[k] = colors[i];
                k++;
            }
            return surColor;
        }

        public bool isLengthEqual()
        {
            if (this.meg.Length == this.forecolor.Length &&
                this.forecolor.Length == this.backColor.Length &&
                this.backColor.Length == this.color.Length)
                return true;
            else
                return false;
        }
    }
}
