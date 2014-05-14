using System;
using System.Collections.Generic;
using System.Text;

namespace DBConnect
{
    public class InsertCommand : ICommand
    {
        private string rspID = "";
        private string fieldNames = "";
        private string tblNames = "";
        private string whereCon = "";

        public string RspID
        {
            set { rspID = value; }
            get { return rspID; }
        }

        public string FiledNames
        {
            set { fieldNames = value; }
            get { return fieldNames; }
        }

        public string TblNames
        {
            set { tblNames = value; }
            get { return tblNames; }
        }

        public string WhereCon
        {
            set { whereCon = value; }
            get { return whereCon; }
        }

        public string getCommand()
        {
            string result = "";

            if (!string.IsNullOrEmpty(fieldNames))
                result = string.Format("insert into {0} ({1}) values({2})", tblNames, fieldNames, whereCon);
            else
                result = string.Format("insert into {0} values({1})", tblNames, whereCon);

            return result;
        }
    }
}
