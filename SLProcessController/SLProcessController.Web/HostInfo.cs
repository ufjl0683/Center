using System;
using System.Collections.Generic;
using System.Web;

namespace SLProcessController.Web
{
    public class HostInfo
    {
       public  string HostName {get;set;}
       public  string IP { get; set; }
       public override string ToString()
       {

           return HostName;
       }
    }
}