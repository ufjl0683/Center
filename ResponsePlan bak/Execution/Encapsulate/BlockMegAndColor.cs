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

        public System.Drawing.Color[] getSubColors(System.Drawing.Color[] colors, int startIndex, int length,string Meg)
        {
            System.Drawing.Color[] surColor = new System.Drawing.Color[Meg.Replace("\b",string.Empty).Replace("\f",string.Empty).Length];
            int k = 0;
            for (int i = startIndex; i < startIndex + Meg.Length; i++)
            {
                if (Meg[i - startIndex] != '\b' && Meg[i - startIndex] != '\f')
                {
                    surColor[k] = colors[i];
                    k++;
                }
            }
            return surColor;
        }

        public System.Drawing.Color[] getTmpSubColors(System.Drawing.Color[] colors, int startIndex, int length, string Meg)
        {
            System.Drawing.Color[] surColor = new System.Drawing.Color[Meg.Length];
            int k = 0;
            for (int i = startIndex; i < startIndex + Meg.Length; i++)
            {
                surColor[k] = colors[i];
                k++;
            }
            return surColor;
        }

        public byte[] getSubColors(byte[] colors, int startIndex, int length,string Meg)
        {
            byte[] surColor = new byte[Meg.Replace("\b", string.Empty).Replace("\f", string.Empty).Length];
            int k = 0;
            for (int i = startIndex; i < startIndex + Meg.Length; i++)
            {
                if (Meg[i - startIndex] != '\b' && Meg[i - startIndex] != '\f')
                {
                    surColor[k] = colors[i];
                    k++;
                }
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
