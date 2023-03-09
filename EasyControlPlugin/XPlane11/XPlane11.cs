using ControllorPlugin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;

namespace XPlane11
{
    public enum XPPortType
    {
        None,
        Command,
        Out,
        In,
    };
    public struct dataBuffer
    {
        public int nodeIndex;
        public int portIndex;
        public string dataRef;
    }
    public struct sendData
    {
        public string dataRef;
        public XPPortType type;
    }
    public class XPlane11 : InterfacePlugin
    {
        string ip = "127.0.0.1";
        int inPort = 9089;
        int outPort = 49000;
        List<dataBuffer> bufferList = new List<dataBuffer>();
        Dictionary<string, sendData> sendDataList = new Dictionary<string, sendData>();
        Dictionary<string, Byte[]> dataRefList = new Dictionary<string, Byte[]>();
        static object lockObj = new object();
        //================================================
        private List<Node> moduleList;
        public string PluginID
        {
            get { return "ED259808-E7F9-40F1-A1A6-05FC5C434CA4"; }
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
            return "X-Plane 11";
        }

        public void Init()
        {
            Open = true;
            moduleList = new List<Node>();
            #region 创建模块
            string path = System.Environment.CurrentDirectory + @"\Plugins\Settings\XPlane11.xml";
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
                            if (EasyXml.GetAttribute(node, "Name", out nodeName) && node.HasChildNodes)
                            {
                                List<NodePort> portList = new List<NodePort>();
                                foreach (XmlNode port in node)
                                {
                                    #region Port
                                    if (port.Name.Equals("Port"))
                                    {
                                        string portName;
                                        XPPortType portType;
                                        if (EasyXml.GetAttribute(port, "Name", out portName) &&
                                            EasyXml.GetAttribute(port, "Type", out portType))
                                        {
                                            switch (portType)
                                            {
                                                case XPPortType.Command:
                                                    string command;
                                                    if (EasyXml.GetAttribute(port, "Command", out command))
                                                    {
                                                        #region 处理数据 Command
                                                        sendData newCommand = new sendData();
                                                        newCommand.dataRef = command;
                                                        newCommand.type = XPPortType.Command;
                                                        sendDataList.Add(moduleList.Count.ToString() + portList.Count.ToString(), newCommand);
                                                        portList.Add(new NodePort(portName, PortType.In, 0));
                                                        #endregion
                                                    }
                                                    break;
                                                case XPPortType.Out:
                                                    string valueOutType;
                                                    string dataRefOut;
                                                    if (EasyXml.GetAttribute(port, "ValueType", out valueOutType) &&
                                                        EasyXml.GetAttribute(port, "DataRef", out dataRefOut))
                                                    {
                                                        #region 处理数据 Out
                                                        bool outAdd = false;
                                                        dataBuffer newBuffer = new dataBuffer();
                                                        newBuffer.nodeIndex = moduleList.Count;
                                                        newBuffer.portIndex = portList.Count;
                                                        newBuffer.dataRef = dataRefOut;
                                                        bufferList.Add(newBuffer);
                                                        switch (valueOutType)
                                                        {
                                                            case "Int":
                                                                //portList.Add(new NodePort(portName, PortType.Out, 0));
                                                                break;
                                                            case "Float":
                                                                portList.Add(new NodePort(portName, PortType.Out, 0f));
                                                                break;
                                                            case "String":
                                                                //portList.Add(new NodePort(portName, PortType.Out, "...."));
                                                                break;
                                                        }
                                                        #endregion
                                                    }
                                                    break;
                                                case XPPortType.In:
                                                    string valueInType;
                                                    string dataRefIn;
                                                    if (EasyXml.GetAttribute(port, "ValueType", out valueInType) &&
                                                        EasyXml.GetAttribute(port, "DataRef", out dataRefIn))
                                                    {
                                                        #region 处理数据 Out
                                                        sendData newSend = new sendData();
                                                        newSend.dataRef = dataRefIn;
                                                        newSend.type = XPPortType.In;
                                                        sendDataList.Add(moduleList.Count.ToString() + portList.Count.ToString(), newSend);
                                                        switch (valueInType)
                                                        {
                                                            case "Int":
                                                                //portList.Add(new NodePort(portName, PortType.In, 0));
                                                                break;
                                                            case "Float":
                                                                portList.Add(new NodePort(portName, PortType.In, 0f));
                                                                break;
                                                            case "String":
                                                                //portList.Add(new NodePort(portName, PortType.In, "...."));
                                                                break;
                                                        }
                                                        #endregion
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                Node newNode = new Node(nodeName, portList);
                                string info = "";
                                if (EasyXml.GetAttribute(node, "Info", out info))
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
        #endregion
        public void AutoOpen()
        {
            autoLink();
        }
        public void Update()
        {
            try
            {
                foreach (dataBuffer buffer in bufferList)
                {
                    if (dataRefList.ContainsKey(buffer.dataRef))
                    {
                        if (buffer.nodeIndex >= 0 && buffer.nodeIndex < moduleList.Count)
                        {
                            if (buffer.portIndex >= 0 && buffer.portIndex < moduleList[buffer.nodeIndex].NodePortList.Count)
                            {
                                NodePort np = moduleList[buffer.nodeIndex].NodePortList[buffer.portIndex];
                                switch (np.Type)
                                {
                                    case PortValue.Int64:
                                        np.ValueInt64 = BitConverter.ToInt32(dataRefList[buffer.dataRef], 0);
                                        break;
                                    case PortValue.Double:
                                        np.ValueDouble = BitConverter.ToSingle(dataRefList[buffer.dataRef], 0);
                                        break;
                                    case PortValue.String:
                                        np.ValueString = System.Text.Encoding.UTF8.GetString(dataRefList[buffer.dataRef]);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("X-Plane 11 Error : " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void NodeCloseEvent(int mIndex)
        {
        }
        public void ValueChangeEvent(int mIndex, int pIndex)
        {
            if (sendDataList.ContainsKey(mIndex.ToString() + pIndex.ToString()))
            {
                sendData data = sendDataList[mIndex.ToString() + pIndex.ToString()];
                switch (data.type)
                {
                    case XPPortType.Command:
                        long value = moduleList[mIndex].NodePortList[pIndex].ValueInt64;
                        if (value != 0)
                        {
                            UdpEventArgs ueaCmd = new UdpEventArgs(inPort);
                            ueaCmd.Msg = System.Text.Encoding.UTF8.GetBytes("CMND0" + sendDataList[mIndex.ToString() + pIndex.ToString()].dataRef);
                            ueaCmd.tarClient = new IPEndPoint(IPAddress.Parse(ip), outPort);
                            SendUDP(this, ueaCmd);
                        }
                        break;
                    case XPPortType.In:
                        byte[] valueByte = new byte[4];
                        switch (moduleList[mIndex].NodePortList[pIndex].Type)
                        {
                            case PortValue.Int64:
                                //valueByte = BitConverter.GetBytes((int)moduleList[mIndex].NodePortList[pIndex].ValueInt64);
                                break;
                            case PortValue.Double:
                                valueByte = BitConverter.GetBytes((float)moduleList[mIndex].NodePortList[pIndex].ValueDouble);
                                break;
                            case PortValue.String:
                                //暂不支持
                                break;
                        }
                        string dataRef = sendDataList[mIndex.ToString() + pIndex.ToString()].dataRef;
                        if (dataRef.Length < 500)
                        {
                            for (int i = dataRef.Length; i < 500; i++)
                            {
                                dataRef += '\0';
                            }
                        }
                        byte[] dataRefByte = System.Text.Encoding.UTF8.GetBytes(dataRef);
                        UdpEventArgs ueaIn = new UdpEventArgs(inPort);
                        byte[] newMsg = new byte[509];
                        System.Text.Encoding.UTF8.GetBytes("DREF+").CopyTo(newMsg, 0);
                        valueByte.CopyTo(newMsg, 5);
                        dataRefByte.CopyTo(newMsg, 9);
                        ueaIn.Msg = newMsg;
                        ueaIn.tarClient = new IPEndPoint(IPAddress.Parse(ip), outPort);
                        SendUDP(this, ueaIn);
                        break;
                }
            }
        }

        public void OnReceiveUdpMsg(EndPoint client, byte[] msg)
        {
            string tip = Encoding.UTF8.GetString(msg, 0, 5);
            if (tip.Equals("DREF+"))
            {
                Byte[] byteForValue = { msg[5], msg[6], msg[7], msg[8] };
                string dataPath = Encoding.UTF8.GetString(msg, 9, msg.Length - 9);
                dataPath = dataPath.Replace('\0', ' ').Trim();
                if (dataRefList.ContainsKey(dataPath))
                {
                    dataRefList[dataPath] = byteForValue;
                }
                else
                {
                    lock (lockObj)
                    {
                        dataRefList.Add(dataPath, byteForValue);
                    }
                }
            }
        }
        void autoLink()
        {
            UdpEventArgs uea = new ControllorPlugin.UdpEventArgs(inPort);
            CreateUDP(this, uea);
            Console.WriteLine("X-Plane 11 Link : " + ip + " : " + inPort);
        }
        #region ui事件
        public void OnButtonLeftClick(string id)
        {
            if (id.Equals("XPSet"))
            {
                autoLink();
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
                case "xpIP":
                    ip = value;
                    break;
                case "xpInPort":
                    int _inPort = 9089;
                    if (int.TryParse(value, out _inPort))
                    {
                        inPort = _inPort;
                    }
                    break;
                case "xpOutPort":
                    int _outPort = 49000;
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
