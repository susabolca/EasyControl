using ControllorPlugin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace FSUIPC
{
    public enum FSUIPCPortType
    {
        None,
        Out
    };
    public struct dataBuffer
    {
        public int nodeIndex;
        public int portIndex;
        public string valueType;
        public string dataGroup;
        public Offset valueOffset;
    }
    public class FSUIPC : InterfacePlugin
    {
        int dwFSReq = 0;              // Any version of FS is OK
        int dwResult = -1;              // Variable to hold returned results
        int token = -1;
        List<dataBuffer> bufferList = new List<dataBuffer>();
        //================================================
        private List<Node> moduleList;
        public string PluginID
        {
            get { return "CD26E39B-767D-410E-AFAB-482ACFCB94BA"; }
        }
        public bool Open
        {
            get { return FSUIPCConnection.IsOpen; }
            set
            {
                try
                {
                    if (value)
                    {
                        if (!FSUIPCConnection.IsOpen)
                        {
                            FSUIPCConnection.Open();
                        }
                    }
                    else
                    {
                        FSUIPCConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FSUIPC Open Error : " + ex.Message);
                }
            }
        }
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
            return "FSUIPC";
        }

        public void Init()
        {
            Open = true;
            moduleList = new List<Node>();
            #region 创建模块
            string path = System.Environment.CurrentDirectory + @"\Plugins\Settings\FSUIPC.xml";
            XmlModule(path);
            #endregion
        }
        #region char2byte
        public static byte[] String2ByteArray(string hex)
        {
            string temp = hex.Replace("0x", "").Replace(",", "").Replace(" ", "").Trim();
            byte[] inString = new byte[temp.Length / 2];
            for (int i = 0; i < temp.Length; i += 2)
            {
                int a = char2byte(temp[i]);
                int b = char2byte(temp[i + 1]);
                int c = (a << 4) + b;
                inString[i / 2] = (byte)c;
            }
            return inString;
        }
        public static byte char2byte(char _num)
        {
            switch (_num)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                case 'a':
                    return 10;
                case 'B':
                case 'b':
                    return 11;
                case 'C':
                case 'c':
                    return 12;
                case 'D':
                case 'd':
                    return 13;
                case 'E':
                case 'e':
                    return 14;
                case 'F':
                case 'f':
                    return 15;
            }
            return 0;
        }
        #endregion
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
                                        FSUIPCPortType portType;
                                        if (EasyXml.GetAttribute(port, "Name", out portName) &&
                                            EasyXml.GetAttribute(port, "Type", out portType))
                                        {
                                            switch (portType)
                                            {
                                                case FSUIPCPortType.Out:
                                                    string valueType;
                                                    string addressStr;
                                                    if (EasyXml.GetAttribute(port, "ValueType", out valueType) &&
                                                        EasyXml.GetAttribute(port, "Address", out addressStr))
                                                    {
                                                        byte[] addressArray = String2ByteArray(addressStr);
                                                        if (addressArray.Length == 2)
                                                        {
                                                            int address = (addressArray[0] << 8) + (addressArray[1]);
                                                            #region 处理数据 Out
                                                            dataBuffer newBuffer = new dataBuffer();
                                                            newBuffer.nodeIndex = moduleList.Count;
                                                            newBuffer.portIndex = portList.Count;
                                                            newBuffer.valueType = valueType;
                                                            switch (valueType)
                                                            {
                                                                case "sbyte":
                                                                    newBuffer.valueOffset = new Offset<sbyte>(address);
                                                                    portList.Add(new NodePort(portName, PortType.Out, 0));
                                                                    break;
                                                                case "byte":
                                                                    newBuffer.valueOffset = new Offset<byte>(address);
                                                                    portList.Add(new NodePort(portName, PortType.Out, 0));
                                                                    break;
                                                                case "ushort":
                                                                    newBuffer.valueOffset = new Offset<ushort>(address);
                                                                    portList.Add(new NodePort(portName, PortType.Out, 0));
                                                                    break;
                                                                case "short":
                                                                    newBuffer.valueOffset = new Offset<short>(address);
                                                                    portList.Add(new NodePort(portName, PortType.Out, 0));
                                                                    break;
                                                                case "int":
                                                                    newBuffer.valueOffset = new Offset<int>(address);
                                                                    portList.Add(new NodePort(portName, PortType.Out, 0));
                                                                    break;
                                                                case "uint":
                                                                    newBuffer.valueOffset = new Offset<uint>(address);
                                                                    portList.Add(new NodePort(portName, PortType.Out, 0));
                                                                    break;
                                                                case "long":
                                                                    newBuffer.valueOffset = new Offset<long>(address);
                                                                    portList.Add(new NodePort(portName, PortType.Out, 0));
                                                                    break;
                                                                case "ulong":
                                                                    newBuffer.valueOffset = new Offset<ulong>(address);
                                                                    portList.Add(new NodePort(portName, PortType.Out, 0));
                                                                    break;
                                                                case "float":
                                                                    newBuffer.valueOffset = new Offset<float>(address);
                                                                    portList.Add(new NodePort(portName, PortType.Out, 0f));
                                                                    break;
                                                                case "double":
                                                                    newBuffer.valueOffset = new Offset<double>(address);
                                                                    portList.Add(new NodePort(portName, PortType.Out, 0f));
                                                                    break;
                                                                case "byte[]":
                                                                    int dwSizeB;
                                                                    if (EasyXml.GetAttribute(port, "dwSize", out dwSizeB))
                                                                    {
                                                                        newBuffer.valueOffset = new Offset<byte[]>(address, dwSizeB);
                                                                        portList.Add(new NodePort(portName, PortType.Out, "...."));
                                                                    }
                                                                    break;
                                                                case "string":
                                                                    int dwSizeS;
                                                                    //string dataG;
                                                                    if (EasyXml.GetAttribute(port, "dwSize", out dwSizeS)/* &&
                                                                        EasyXml.GetAttribute(port, "dataGroup", out dataG)*/)
                                                                    {
                                                                        //newBuffer.dataGroup = dataG;
                                                                        newBuffer.valueOffset = new Offset<byte[]>(/*dataG,*/ address, dwSizeS);
                                                                        portList.Add(new NodePort(portName, PortType.Out, "...."));
                                                                    }
                                                                    break;
                                                                default:
                                                                    Console.WriteLine("FSUIPC ValueType Error : " + nodeName);
                                                                    break;
                                                            }
                                                            bufferList.Add(newBuffer);
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("FSUIPC Address Error : " + nodeName);
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

        }
        public void Update()
        {
            if (Open)
            {
                foreach (dataBuffer buffer in bufferList)
                {
                    if (buffer.nodeIndex >= 0 && buffer.nodeIndex < moduleList.Count)
                    {
                        if (buffer.portIndex >= 0 && buffer.portIndex < moduleList[buffer.nodeIndex].NodePortList.Count)
                        {
                            NodePort np = moduleList[buffer.nodeIndex].NodePortList[buffer.portIndex];
                            switch (buffer.valueType)
                            {
                                case "sbyte":
                                    FSUIPCConnection.Process();
                                    np.ValueInt64 = buffer.valueOffset.GetValue<sbyte>();
                                    break;
                                case "byte":
                                    FSUIPCConnection.Process();
                                    np.ValueInt64 = buffer.valueOffset.GetValue<byte>();
                                    break;
                                case "ushort":
                                    FSUIPCConnection.Process();
                                    np.ValueInt64 = buffer.valueOffset.GetValue<ushort>();
                                    break;
                                case "short":
                                    FSUIPCConnection.Process();
                                    np.ValueInt64 = buffer.valueOffset.GetValue<short>();
                                    break;
                                case "int":
                                    FSUIPCConnection.Process();
                                    np.ValueInt64 = buffer.valueOffset.GetValue<int>();
                                    break;
                                case "uint":
                                    FSUIPCConnection.Process();
                                    np.ValueInt64 = buffer.valueOffset.GetValue<uint>();
                                    break;
                                case "long":
                                    FSUIPCConnection.Process();
                                    np.ValueInt64 = buffer.valueOffset.GetValue<long>();
                                    break;
                                case "ulong":
                                    FSUIPCConnection.Process();
                                    np.ValueInt64 = (long)buffer.valueOffset.GetValue<ulong>();
                                    break;
                                case "float":
                                    FSUIPCConnection.Process();
                                    np.ValueDouble = buffer.valueOffset.GetValue<float>();
                                    break;
                                case "double":
                                    FSUIPCConnection.Process();
                                    np.ValueDouble = buffer.valueOffset.GetValue<double>();
                                    break;
                                case "byte[]":
                                    FSUIPCConnection.Process();
                                    byte[] tempData = buffer.valueOffset.GetValue<byte[]>();
                                    string dataStr = "";
                                    for (int i = 0; i < tempData.Length; i++)
                                    {
                                        dataStr += tempData[i].ToString("x");
                                        if (i < tempData.Length - 1)
                                            dataStr += ",";
                                    }
                                    np.ValueString = dataStr;
                                    break;
                                case "string":
                                    FSUIPCConnection.Process(/*buffer.dataGroup*/);
                                    np.ValueString = buffer.valueOffset.GetValue<string>();
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void NodeCloseEvent(int mIndex)
        {
        }
        public void ValueChangeEvent(int mIndex, int pIndex)
        {
        }
        public void OnReceiveUdpMsg(EndPoint client, byte[] msg)
        {
        }
        #region ui事件
        public void OnButtonLeftClick(string id)
        {
        }

        public void OnButtonRightClick(string id)
        {
        }

        public void OnSwitchButtonChange(string id, bool value)
        {
        }

        public void OnTextEditorChange(string id, string value)
        {
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
