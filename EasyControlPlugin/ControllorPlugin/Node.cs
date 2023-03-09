using System.Collections.Generic;

namespace ControllorPlugin
{
    public class Node
    {
        public string Name { set; get; }//名字;
        public string Info { set; get; } = "";//插件说明
        public bool Open { set; get; } = true;//模块开关
        public List<NodePort> NodePortList { private set; get; }//端口定义;
        public Node(string _Name, List<NodePort> _PinList)
        {
            Name = _Name;
            if (_PinList == null)
            {
                _PinList = new List<NodePort>();
            }
            else
            {
                NodePortList = _PinList;
            }
        }
    }
}
