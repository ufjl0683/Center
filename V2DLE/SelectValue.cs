using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
 public   struct SelectValue
    {

        public int value;
        private string _valueName;
        public SelectValue(int value, string valueName)
        {
            this.value = value;
            this._valueName = valueName.Trim(new char[] {'\"' }).Replace(" ", "_");
        }
        public string valueName
        {
            get
            {
                return _valueName;
            }
            set
            {
                _valueName = value.Trim(new char[] { '\"' }).Replace(" ", "_");
            }
        }
    }
}
