using System;

namespace DBConnect
{
    public class UpdateCommand:ICommand
    {
        private string eventID = "";
        private string fieldNames = "";
        private string tblNames = "";
        private string whereCon = "";

        public string RspID
        {
            set { eventID = value; }
            get { return eventID; }
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
            string result = string.Format("update {0} set {1} ", tblNames, fieldNames);

            if (whereCon != "")
                result = string.Format("{0} where {1}", result, whereCon);

            return result;
        }
    }
}
