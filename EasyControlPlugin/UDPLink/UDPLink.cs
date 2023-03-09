using ControllorPlugin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;

namespace UDPLinkPlugin
{
    public struct dataBuffer
    {
        public int nodeIndex;
        public int portIndex;
    }
    public class UDPLink : InterfacePlugin
    {
        string ip = "127.0.0.1";
        int inPort = 7089;
        int outPort = 7099;
        List<dataBuffer> bufferList = new List<dataBuffer>();
        //================================================
        private List<Node> moduleList;
        public string PluginID
        {
            get { return "7DE7C3A2-247F-495E-94D5-DA46FE120307"; }
        }

        public bool Open { get; set; }
        public bool Auto { get; set; }

        public event EventHandler CreateUDP;
        public event EventHandler SendUDP;
        public event EventHandler ButtonLeftClick;
        public event EventHandler ButtonRightClick;
        public event EventHandler SwitchButtonChange;
        public event EventHandler TextEditorChange;
        public event EventHandler TrackBarChange;
        public List<Node> GetModuleList()
        {
            return moduleList;
        }

        public string GetName()
        {
            return "UDP Link";
        }
        public void Init()
        {
            Open = true;
            moduleList = new List<Node>();
            #region 创建模块
            string path = System.Environment.CurrentDirectory + @"\Plugins\Settings\UDPLink.xml";
            XmlModule(path);
            #endregion
        }
        #region Xml
        private List<Node> XmlModule(string path)
        {
            List<Node> newModuleList = new List<Node>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode rootNode = xmlDoc.SelectSingleNode("Plugin");
            if (rootNode != null && rootNode.HasChildNodes)
            {
                XmlNode moduleNode = rootNode.SelectSingleNode("Module");
                if (moduleNode != null && moduleNode.HasChildNodes)
                {
                    foreach (XmlNode node in moduleNode)
                    {
                        #region Node
                        if (node.Name.Equals("Node"))
                        {
                            string nodeName = "";
                            if (GetAttribute(node, "Name", out nodeName) && node.HasChildNodes)
                            {
                                List<NodePort> portList = new List<NodePort>();
                                foreach (XmlNode port in node)
                                {
                                    #region Port
                                    if (port.Name.Equals("Port"))
                                    {
                                        string portName;
                                        PortType portType;
                                        string valueType;
                                        if (GetAttribute(port, "Name", out portName) &&
                                            GetAttribute(port, "Type", out portType) &&
                                            GetAttribute(port, "ValueType", out valueType))
                                        {
                                            #region 处理数据 INT
                                            dataBuffer newBuffer = new dataBuffer();
                                            newBuffer.nodeIndex = moduleList.Count;
                                            newBuffer.portIndex = portList.Count;
                                            switch (valueType)
                                            {
                                                case "Int":
                                                    portList.Add(new NodePort(portName, portType, 0));
                                                    break;
                                                case "Float":
                                                    portList.Add(new NodePort(portName, portType, 0f));
                                                    break;
                                                case "String":
                                                    portList.Add(new NodePort(portName, portType, "...."));
                                                    break;
                                            }
                                            bufferList.Add(newBuffer);
                                            #endregion
                                        }
                                    }
                                    #endregion
                                }
                                Node newNode = new Node(nodeName, portList);
                                string info = "";
                                if (GetAttribute(node, "Info", out info))
                                {
                                    newNode.Info = info;
                                }
                                moduleList.Add(newNode);
                            }
                            return newModuleList;//只处理一个node
                        }
                        #endregion
                    }
                }
            }
            return newModuleList;
        }
        private bool GetAttribute(XmlNode node, string name, out int value)
        {
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    string attriValue = attri.Value.Replace(" ", "");
                    int _temp = 0;
                    if (int.TryParse(attriValue, out _temp))
                    {
                        value = _temp;
                        return true;
                    }
                }
            }
            value = -1;
            return false;
        }
        private bool GetAttribute(XmlNode node, string name, out string value)
        {
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    value = attri.Value;
                    return true;
                }
            }
            value = "";
            return false;
        }
        private bool GetAttribute(XmlNode node, string name, out PortType value)
        {
            string _value = "";
            if (GetAttribute(node, name, out _value))
            {
                PortType type;
                if (PortType.TryParse(_value, out type))
                {
                    value = type;
                    return true;
                }
            }
            value = PortType.In;
            return false;
        }
        #endregion
        public void AutoOpen()
        {
            autoLink();
        }
        void autoLink()
        {
            UdpEventArgs uea = new UdpEventArgs(inPort);
            CreateUDP(this, uea);
            Console.WriteLine("UDP Link : " + ip + " : " + inPort);
        }
        public void Update()
        {
        }
        public void NodeCloseEvent(int mIndex)
        {
        }
        public void ValueChangeEvent(int mIndex, int pIndex)
        {
            string dataList = "EC" + mIndex + "|" + pIndex + ">>";
            NodePort np = moduleList[mIndex].NodePortList[pIndex];
            switch (np.Type)
            {
                case PortValue.Int64:
                    dataList += np.ValueInt64.ToString();
                    break;
                case PortValue.Double:
                    dataList += np.ValueDouble.ToString("f4");
                    break;
                case PortValue.String:
                    dataList += np.ValueString;
                    break;
            }
            UdpEventArgs uea = new UdpEventArgs(inPort);
            uea.Msg = System.Text.Encoding.UTF8.GetBytes(dataList);
            uea.tarClient = new IPEndPoint(IPAddress.Parse(ip), outPort);
            SendUDP(this, uea);
        }

        public void OnReceiveUdpMsg(EndPoint client, byte[] msg)
        {
            if (Encoding.UTF8.GetString(msg, 0, 4).Equals("EC<<"))
            {
                string msgUDP = Encoding.UTF8.GetString(msg, 4, msg.Length - 4);
                string[] pinInfo = msgUDP.Split(',');
                for (int i = 0; i < pinInfo.Length; i++)
                {
                    string[] dataInfo = pinInfo[i].Split('|');
                    if (dataInfo.Length == 3)
                    {
                        int mIndex, pIndex;
                        if (int.TryParse(dataInfo[0], out mIndex) &&
                            int.TryParse(dataInfo[1], out pIndex))
                        {
                            if (mIndex < moduleList.Count &&
                                pIndex < moduleList[mIndex].NodePortList.Count)
                            {
                                NodePort np = moduleList[mIndex].NodePortList[pIndex];
                                switch (np.Type)
                                {
                                    case PortValue.Int64:
                                        Int64 dataInt;
                                        if (Int64.TryParse(dataInfo[2], out dataInt))
                                        {
                                            np.ValueInt64 = dataInt;
                                        }
                                        break;
                                    case PortValue.Double:
                                        double dataDouble;
                                        if (double.TryParse(dataInfo[2], out dataDouble))
                                        {
                                            np.ValueDouble = dataDouble;
                                        }
                                        break;
                                    case PortValue.String:
                                        np.ValueString = dataInfo[2];
                                        break;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("UDP Link : " + msgUDP);
            }
        }
        #region ui事件
        public void OnButtonLeftClick(string id)
        {
            try
            {
                switch (id)
                {
                    case "openUDP":
                        autoLink();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        public void OnButtonRightClick(string id)
        {
        }

        public void OnSwitchButtonChange(string id, bool value)
        {
        }

        public void OnTextEditorChange(string id, string value)
        {
            switch (id)
            {
                case "udpIP":
                    ip = value;
                    break;
                case "udpInPort":
                    int _inPort = 7089;
                    if (int.TryParse(value, out _inPort))
                    {
                        inPort = _inPort;
                    }
                    break;
                case "udpOutPort":
                    int _outPort = 7099;
                    if (int.TryParse(value, out _outPort))
                    {
                        outPort = _outPort;
                    }
                    break;
            }
        }

        public void OnTrackBarChange(string id, int value)
        {
        }
        #endregion
        public void DefWndProc(int message)
        {
        }
    }
}
