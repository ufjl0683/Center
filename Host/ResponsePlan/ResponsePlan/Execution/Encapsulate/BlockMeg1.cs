using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    class BlockMeg1
    {
        private int typeid;
        private string meg;
        private bool ifChild;
        public BlockMeg1(int typeid, string meg, bool ifChild)
        {
            this.ifChild = ifChild;
            this.typeid = typeid;
            this.meg = meg;
        }

        public int TypeID
        {
            get { return typeid; }
        }

        public string Message
        {
            get { return meg; }
        }
        public bool IfChild
        {
            get { return ifChild; }
        }
    }
}
