using System;
using System.Collections.Generic;
using System.Text;

namespace DBConnect
{
    public delegate object GetReaderDataHandler(DataType type, object dr);
    interface I_Connect
    {
        event GetReaderDataHandler GetReaderData;
        void setSchema(string schema);
        bool Open();
        bool Close();

        object select(DataType type, ICommand selectcmd);
        System.Data.DataTable select(ICommand selectcmd);
        void insert(ICommand insert);
        void update(ICommand updata);
    }
}
