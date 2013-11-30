using System;
using System.Collections.Generic;
//using SLinqystem.;
using System.ServiceProcess;
using System.Text;

namespace ProcessManagerService
{
    static class Program
    {
       

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new Service1() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
