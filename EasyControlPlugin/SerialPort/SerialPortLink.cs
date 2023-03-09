using ControllorPlugin;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Xml;

namespace SerialPortLinkPlugin
{
    public struct dataBuffer
    {
        public int nodeIndex;
        public int portIndex;
    }
    public class SerialPortLink : InterfacePlugin
    {
        SerialPort ComDevice;
        int comNum = 1;
        int baudRate = 115200;
        List<dataBuffer> bufferList = new List<dataBuffer>();
        static string log = "";
        //================================================
        private List<Node> moduleList;
        public string PluginID
        {
            get { return "FD3C1671-5706-4DF3-9048-A71ED21D7FA8"; }
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
            return "SerialPort";
        }

        public void Init()
        {
            Open = true;
            moduleList = new List<Node>();
            #region 创建模块
            string path = System.Environment.CurrentDirectory + @"\Plugins\Settings\SerialPortLink.xml";
            XmlModule(path);
            #endregion
            ComDevice = new SerialPort();
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
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                log = sp.ReadExisting();
#if DEBUG
                if (log.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(log);
                    log = "";
                }
#endif
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
        public void AutoOpen()
        {
            autoLink();
        }
        public void Update()
        {
            if (!ComDevice.IsOpen)
                return;
            string dataList = ">>";
            for (int i = 0; i < bufferList.Count; i++)
            {
                dataBuffer buffer = bufferList[i];
                if (buffer.nodeIndex >= 0 && buffer.nodeIndex < moduleList.Count)
                {
                    if (buffer.portIndex >= 0 && buffer.portIndex < moduleList[buffer.nodeIndex].NodePortList.Count)
                    {
                        NodePort np = moduleList[buffer.nodeIndex].NodePortList[buffer.portIndex];
                        switch (np.Type)
                        {
                            case PortValue.Int64:
                                dataList += np.ValueInt64.ToString() + ",";
                                break;
                            case PortValue.Double:
                                dataList += np.ValueDouble.ToString("f4") + ",";
                                break;
                            case PortValue.String:
                                dataList += np.ValueString + ",";
                                break;
                        }
                    }
                }
            }
            dataList += "<<";
            try
            {
                byte[] sendData = System.Text.Encoding.ASCII.GetBytes(dataList);
                ComDevice.Write(sendData, 0, sendData.Length);//发送数据
#if DEBUG
                string log = "";
                for (int i = 0; i < sendData.Length; i++)
                {
                    log += sendData[i].ToString("x2") + ",";
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(log);
#endif
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message + ex.StackTrace);
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
        void autoLink()
        {
            if (!ComDevice.IsOpen)
            {
                string[] comList = SerialPort.GetPortNames();
                foreach (string comName in comList)
                {
                    if (comName.Equals("COM" + comNum))
                    {
                        ComDevice.PortName = comName;
                        ComDevice.BaudRate = baudRate;
                        ComDevice.Parity = Parity.None;
                        ComDevice.DataBits = 8;
                        ComDevice.StopBits = StopBits.One;
                        ComDevice.WriteBufferSize = 1024;
                        ComDevice.ReadBufferSize = 1024;
                        ComDevice.RtsEnable = true;
                        ComDevice.DtrEnable = true;
                        ComDevice.ReceivedBytesThreshold = 1;
                        ComDevice.ReadTimeout = 500;
                        ComDevice.WriteTimeout = 500;
                        ComDevice.Open();
                        ComDevice.DataReceived += Com_DataReceived;
                        if (moduleList.Count == 1)
                        {
                            moduleList[0].Name = ComDevice.PortName;
                        }
                    }
                }
            }
            if (!ComDevice.IsOpen)
            {
                Console.WriteLine("串口 COM" + comNum + " 不存在！！！");
            }
            else
            {
                Console.WriteLine("串口 COM" + comNum + " 已经连接。");
            }
        }
        #region ui事件
        public void OnButtonLeftClick(string id)
        {
            try
            {
                switch (id)
                {
                    case "OpenSerial":
                        autoLink();
                        break;
                    case "CloseSerial":
                        if (ComDevice.IsOpen)
                        {
                            ComDevice.Close();
                            ComDevice.DataReceived -= Com_DataReceived;
                            if (moduleList.Count == 1)
                            {
                                moduleList[0].Name = "已关闭";
                            }
                            if (!ComDevice.IsOpen)
                            {
                                Console.WriteLine("串口 COM" + comNum + " 已经关闭。");
                            }
                        }
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
                case "ComNum":
                    int _comNum;
                    if (int.TryParse(value, out _comNum))
                    {
                        comNum = _comNum;
                    }
                    break;
                case "BaudRate":
                    int _baudRate;
                    if (int.TryParse(value, out _baudRate))
                    {
                        baudRate = _baudRate;
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
