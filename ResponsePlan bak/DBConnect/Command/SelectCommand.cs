using System;


namespace DBConnect
{
    public class SelectCommand:ICommand
    {
        private string eventID = "";
        private string fieldNames = "";
        private string tblNames = "";
        private string whereCon = "";
        private string orderBy = "";

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

        public string OrderBy
        {
            set { orderBy = value; }
            get { return orderBy; }
        }

        public string getCommand()
        {
            string result = string.Format("select {0} from {1} ", fieldNames, tblNames);
            if (whereCon != "")
                result = string.Format("{0} where {1}", result, whereCon);
            if (orderBy != "")
                result = string.Format("{0} order by {1}", result, orderBy);
            return result;
        }
    }
}
