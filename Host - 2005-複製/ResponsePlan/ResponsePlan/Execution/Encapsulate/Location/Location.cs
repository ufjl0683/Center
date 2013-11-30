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
        /// ���w�a�Ͻs��
        /// </summary>
        public string Id
        {
            get { return id; }
        }
        /// <summary>
        /// ���w�a�ϰ_�I���{
        /// </summary>
        public int StartMileage
        {
            get { return startMileage; }
        }
        /// <summary>
        /// ���w�a�ϲ��I���{
        /// </summary>
        public int EndMileage
        {
            get { return endMileage; }
        }
        /// <summary>
        /// �W����w�a��
        /// </summary>
        public Location UpstreamLocation
        {
            get { return upstreamLocation; }
        }
        /// <summary>
        /// ���w�a�Ϥ�V
        /// </summary>
        public string Direction
        {
            get { return direction; }
        }
        /// <summary>
        /// ����O�_�b�����w�a�Ϥ�
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
