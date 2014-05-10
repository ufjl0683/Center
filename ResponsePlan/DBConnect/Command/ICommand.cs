namespace DBConnect
{
    public interface ICommand
    {
        string getCommand();
        string RspID
        {
            set;
            get;
        }

        string FiledNames
        {
            set;
            get;
        }

        string TblNames
        {
            set;
            get;
        }

        string WhereCon
        {
            set;
            get;
        }
    }
}
