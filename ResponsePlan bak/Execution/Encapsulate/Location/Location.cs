using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    abstract class Location
    {
        protected string id = "";
        protected string name = "";
        protected int startMileage = 0;
        protected int endMileage = 0;
        protected string direction = "";
        protected Location upstreamLocation = null;

        /// <summary>
        /// 指定地區編號
        /// </summary>
        public string Id
        {
            get { return id; }
        }
        /// <summary>
        /// 指定地區起點里程
        /// </summary>
        public int StartMileage
        {
            get { return startMileage; }
        }
        /// <summary>
        /// 指定地區終點里程
        /// </summary>
        public int EndMileage
        {
            get { return endMileage; }
        }
        /// <summary>
        /// 上游指定地區
        /// </summary>
        public Location UpstreamLocation
        {
            get { return upstreamLocation; }
        }
        /// <summary>
        /// 指定地區方向
        /// </summary>
        public string Direction
        {
            get { return direction; }
        }
        /// <summary>
        /// 比較是否在本指定地區內
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        abstract public bool compare(object sender);

        public override string ToString()
        {
            return name;
        }
    }

}
