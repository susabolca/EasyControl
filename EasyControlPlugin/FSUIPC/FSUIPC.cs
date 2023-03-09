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
        Out,    // 兼容旧配置
        RW,
        R,
        W
    };

    public enum FSUIPCOffsetCan
    {
        None,
        READ = 1 << 0,
        WRITE = 1 << 1, 
    };

    public enum FSUIPCValueType
    {
        None,
        S8,
        U8,
        S16,
        U16,
        S32,
        U32,
        S64,
        U64,
        F32,
        F64,
        BYTES,
        STRING,
    };

    public struct dataBuffer
    {
        public int nodeIndex;
        public int portIndexOut;        // out port index
        public int portIndexIn;         // in port index
        public FSUIPCOffsetCan offsetCan;       // Read / Write
        public FSUIPCValueType valueType;       // S8, U8, S16 ...
        //public string dataGroup;
        public Offset valueOffset;      // hold FSUIPC offset
    };

    public class FSUIPC : InterfacePlugin
    {
        int dwFSReq = 0;              // Any version of FS is OK
        int dwResult = -1;              // Variable to hold returned results
        int token = -1;
        List<dataBuffer> OutList = new List<dataBuffer>();  // buff in read list 
        //List<dataBuffer> InList = new List<dataBuffer>();   // buff in write list
        Dictionary<string, dataBuffer> InList = new Dictionary<string, dataBuffer>();

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
                                        string portType;
                                        string valueType;
                                        string addressStr;
                                        if (EasyXml.GetAttribute(port, "Name", out portName) &&
                                            EasyXml.GetAttribute(port, "Type", out portType) &&
                                            EasyXml.GetAttribute(port, "ValueType", out valueType) &&
                                            EasyXml.GetAttribute(port, "Address", out addressStr))
                                        {
                                            FSUIPCOffsetCan offsetCan = FSUIPCOffsetCan.None;
                                            portType = portType.ToLower();
                                            switch (portType) 
                                            {
                                                case "out": case "r": 
                                                    offsetCan = FSUIPCOffsetCan.READ; 
                                                    break;
                                                case "w": 
                                                    offsetCan = FSUIPCOffsetCan.WRITE; 
                                                    break;
                                                case "rw": 
                                                    offsetCan = FSUIPCOffsetCan.READ | FSUIPCOffsetCan.WRITE; 
                                                    break;
                                                default:
                                                    Console.WriteLine("FSUIPC Type Error : " + nodeName);
                                                    break;
                                            }
                                            
                                            if (offsetCan != FSUIPCOffsetCan.None) {
                                                valueType = valueType.ToLower();

                                                byte[] addressArray = String2ByteArray(addressStr);
                                                if (addressArray.Length == 2)
                                                {
                                                    int address = (addressArray[0] << 8) + (addressArray[1]);
                                                    #region 处理数据 Out
                                                    dataBuffer newBuffer = new dataBuffer();
                                                    newBuffer.nodeIndex = moduleList.Count;
                                                    newBuffer.offsetCan = offsetCan;

                                                    dynamic defValue;
                                                    switch (valueType)
                                                    {
                                                        case "sbyte":
                                                        case "s8":
                                                            newBuffer.valueOffset = new Offset<sbyte>(address);
                                                            newBuffer.valueType = FSUIPCValueType.S8;
                                                            defValue = 0;
                                                            break;
                                                        case "byte":
                                                        case "u8":
                                                            newBuffer.valueOffset = new Offset<byte>(address);
                                                            newBuffer.valueType = FSUIPCValueType.U8;
                                                            defValue = 0;
                                                            break;
                                                        case "ushort":
                                                        case "u16":
                                                            newBuffer.valueOffset = new Offset<ushort>(address);
                                                            newBuffer.valueType = FSUIPCValueType.U16;
                                                            defValue = 0;
                                                            break;
                                                        case "short":
                                                        case "s16":
                                                            newBuffer.valueOffset = new Offset<short>(address);
                                                            newBuffer.valueType = FSUIPCValueType.S16;
                                                            defValue = 0;
                                                            break;
                                                        case "int":
                                                        case "s32":
                                                            newBuffer.valueOffset = new Offset<int>(address);
                                                            newBuffer.valueType = FSUIPCValueType.S32;
                                                            defValue = 0;
                                                            break;
                                                        case "uint":
                                                        case "u32":
                                                            newBuffer.valueOffset = new Offset<uint>(address);
                                                            newBuffer.valueType = FSUIPCValueType.U32;
                                                            defValue = 0;
                                                            break;
                                                        case "long":
                                                        case "s64":
                                                            newBuffer.valueOffset = new Offset<long>(address);
                                                            newBuffer.valueType = FSUIPCValueType.S64;
                                                            defValue = 0;
                                                            break;
                                                        case "ulong":
                                                        case "u64":
                                                            newBuffer.valueOffset = new Offset<ulong>(address);
                                                            newBuffer.valueType = FSUIPCValueType.U64;
                                                            defValue = 0;
                                                            break;
                                                        case "float":
                                                        case "f32":
                                                            newBuffer.valueOffset = new Offset<float>(address);
                                                            newBuffer.valueType = FSUIPCValueType.F32;
                                                            defValue = 0f;
                                                            break;
                                                        case "double":
                                                        case "f64":
                                                            newBuffer.valueOffset = new Offset<double>(address);
                                                            newBuffer.valueType = FSUIPCValueType.F64;
                                                            defValue = 0f;
                                                            break;
                                                        case "byte[]":
                                                        case "bytes":
                                                            int dwSizeB;
                                                            defValue = "...";
                                                            if (EasyXml.GetAttribute(port, "dwSize", out dwSizeB)) {
                                                                newBuffer.valueOffset = new Offset<byte[]>(address, dwSizeB);
                                                                newBuffer.valueType = FSUIPCValueType.BYTES;
                                                            }
                                                            break;
                                                        case "string":
                                                            int dwSizeS;
                                                            defValue = "...";
                                                            //string dataG;
                                                            if (EasyXml.GetAttribute(port, "dwSize", out dwSizeS)) {
                                                                //newBuffer.dataGroup = dataG;
                                                                newBuffer.valueOffset = new Offset<byte[]>(/*dataG,*/ address, dwSizeS);
                                                                newBuffer.valueType = FSUIPCValueType.STRING;
                                                            }
                                                            break;
                                                        default:
                                                            Console.WriteLine("FSUIPC ValueType Error : " + nodeName);
                                                            defValue = 0;
                                                            break;
                                                    }

                                                    if ((newBuffer.offsetCan & FSUIPCOffsetCan.READ) == FSUIPCOffsetCan.READ) {
                                                        newBuffer.portIndexOut = portList.Count;
                                                        OutList.Add(newBuffer);
                                                        portList.Add(new NodePort(portName, PortType.Out, defValue)); 
                                                    }
                                                    if ((newBuffer.offsetCan & FSUIPCOffsetCan.WRITE) == FSUIPCOffsetCan.WRITE) { 
                                                        newBuffer.portIndexIn = portList.Count;
                                                        InList.Add(moduleList.Count.ToString() + portList.Count.ToString(), newBuffer);
                                                        portList.Add(new NodePort(portName, PortType.In, defValue));
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("FSUIPC XML Error: " + nodeName);
                                        } // easyXML
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
                foreach (dataBuffer buffer in OutList)
                {
                    if (buffer.nodeIndex >= 0 && buffer.nodeIndex < moduleList.Count)
                    {
                        if (buffer.portIndexOut >= 0 && buffer.portIndexOut < moduleList[buffer.nodeIndex].NodePortList.Count)
                        {
                            NodePort np = moduleList[buffer.nodeIndex].NodePortList[buffer.portIndexOut];
                            FSUIPCConnection.Process();
                            switch (buffer.valueType)
                            {
                                case FSUIPCValueType.S8: 
                                    np.ValueInt64 = buffer.valueOffset.GetValue<sbyte>();
                                    break;
                                case FSUIPCValueType.U8: 
                                    np.ValueInt64 = buffer.valueOffset.GetValue<byte>();
                                    break;
                                case FSUIPCValueType.U16: 
                                    np.ValueInt64 = buffer.valueOffset.GetValue<ushort>();
                                    break;
                                case FSUIPCValueType.S16: 
                                    np.ValueInt64 = buffer.valueOffset.GetValue<short>();
                                    break;
                                case FSUIPCValueType.S32: 
                                    np.ValueInt64 = buffer.valueOffset.GetValue<int>();
                                    break;
                                case FSUIPCValueType.U32: 
                                    np.ValueInt64 = buffer.valueOffset.GetValue<uint>();
                                    break;
                                case FSUIPCValueType.S64: 
                                    np.ValueInt64 = buffer.valueOffset.GetValue<long>();
                                    break;
                                case FSUIPCValueType.U64: 
                                    np.ValueInt64 = (long)buffer.valueOffset.GetValue<ulong>();
                                    break;
                                case FSUIPCValueType.F32: 
                                    np.ValueDouble = buffer.valueOffset.GetValue<float>();
                                    break;
                                case FSUIPCValueType.F64: 
                                    np.ValueDouble = buffer.valueOffset.GetValue<double>();
                                    break;
                                case FSUIPCValueType.BYTES: 
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
                                case FSUIPCValueType.STRING: 
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
            string key = mIndex.ToString() + pIndex.ToString();
            if (!InList.ContainsKey(key))
            {
                return;
            }

            dataBuffer inBuf = InList[key];
            NodePort inVal = moduleList[mIndex].NodePortList[pIndex];

            switch (inBuf.valueType)
            {
                case FSUIPCValueType.S8:
                    inBuf.valueOffset.SetValue((sbyte)inVal.ValueInt64);
                    break;
                case FSUIPCValueType.U8:
                    inBuf.valueOffset.SetValue((byte)inVal.ValueInt64);
                    break;
                case FSUIPCValueType.S16:
                    inBuf.valueOffset.SetValue((short)inVal.ValueInt64);
                    break;
                case FSUIPCValueType.U16:
                    inBuf.valueOffset.SetValue((ushort)inVal.ValueInt64);
                    break;
                case FSUIPCValueType.S32:
                    inBuf.valueOffset.SetValue((int)inVal.ValueInt64);
                    break;
                case FSUIPCValueType.U32:
                    inBuf.valueOffset.SetValue((uint)inVal.ValueInt64);
                    break;
                case FSUIPCValueType.S64:
                    inBuf.valueOffset.SetValue((long)inVal.ValueInt64);
                    break;
                case FSUIPCValueType.U64:
                    inBuf.valueOffset.SetValue((ulong)inVal.ValueInt64);
                    break;
                case FSUIPCValueType.F32:
                    inBuf.valueOffset.SetValue((float)inVal.ValueDouble);
                    break;
                case FSUIPCValueType.F64:
                    inBuf.valueOffset.SetValue((double)inVal.ValueDouble);
                    break;
                default:
                    Console.WriteLine("FSUIPC In Type Error: " + inBuf.valueType);
                    break;
            }
            FSUIPCConnection.Process();
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
