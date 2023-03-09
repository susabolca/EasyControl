using ControllorPlugin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;

namespace DCS_World
{
    public enum DcsPortType
    {
        None,
        Button,
        Rocker,
        Switch,
        Axis,
        Encoder,
        Out
    };
    public struct inButton
    {
        public string pushMsg;
        public string releaseMsg;
    }
    public struct inRocker
    {
        public bool isPush1;
        public string push1Msg;
        public string release1Msg;
        public string push2Msg;
        public string release2Msg;
    }
    public struct inSwitch
    {
        public List<string> positionList;
    }
    public struct inToggleSwitch
    {
        public string position1Msg;
        public string position2Msg;
    }
    public struct inThreeSwitch
    {
        public string position1Msg;
        public string position2Msg;
        public string position3Msg;
    }
    public struct inAxis
    {
        public float min;
        public float max;
        public string key;
    }
    public struct inEncoder
    {
        public string plusMsg;
        public string lessMsg;
    }
    public struct outData
    {
        public int nodeIndex;
        public int portIndex;
        public string msgValue;
        public string javaScript;
    }
    public class DCS_World : InterfacePlugin
    {
        private List<Node> moduleList = new List<Node>();

        public event EventHandler CreateUDP;
        public event EventHandler SendUDP;
        public event EventHandler ButtonLeftClick;
        public event EventHandler ButtonRightClick;
        public event EventHandler SwitchButtonChange;
        public event EventHandler TextEditorChange;
        public event EventHandler TrackBarChange;
        //=========================================
        string ip = "127.0.0.1";
        int port = 9089;
        EndPoint sendClient;
        //----
        private string _clientID = "";
        int _tokenCount = 0;
        string[] _tokens = new string[1024];
        Dictionary<string, string> valueBuffer = new Dictionary<string, string>();
        List<outData> dataList = new List<outData>();
        Dictionary<string, inButton> buttonList = new Dictionary<string, inButton>();
        Dictionary<string, inRocker> rockerList = new Dictionary<string, inRocker>();
        Dictionary<string, inSwitch> switchList = new Dictionary<string, inSwitch>();
        Dictionary<string, inAxis> axisList = new Dictionary<string, inAxis>();
        Dictionary<string, inEncoder> encoderList = new Dictionary<string, inEncoder>();
        static object lockObj = new object();
        //=========================================
        public bool Open { get; set; }
        public bool Auto { get; set; }

        public string PluginID
        {
            get { return "9EA48E9C-4C9C-4837-9318-D5634BF994ED"; }
        }

        public void DefWndProc(int message)
        {
            //do it
        }

        public List<Node> GetModuleList()
        {
            return moduleList;
        }

        public string GetName()
        {
            return "DCS World";
        }

        public void Init()
        {
            //do it
            Open = true;
            #region 创建模块
            string path = System.Environment.CurrentDirectory + @"\Plugins\Settings\DCS_World.xml";
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
                                        DcsPortType portType;
                                        if (EasyXml.GetAttribute(port, "Name", out portName) &&
                                            EasyXml.GetAttribute(port, "Type", out portType))
                                        {
                                            switch (portType)
                                            {
                                                case DcsPortType.Button:
                                                    #region Button
                                                    string push, release;
                                                    string buttonKey;
                                                    if (EasyXml.GetAttribute(port, "Push", out push) &&
                                                        EasyXml.GetAttribute(port, "Release", out release) &&
                                                        EasyXml.GetAttribute(port, "Key", out buttonKey))
                                                    {
                                                        #region 处理数据 Button
                                                        inButton newButton = new inButton();
                                                        newButton.pushMsg = "C" + buttonKey + "," + push;
                                                        newButton.releaseMsg = "C" + buttonKey + "," + release;
                                                        #endregion
                                                        buttonList.Add(moduleList.Count.ToString() + portList.Count.ToString(), newButton);
                                                        portList.Add(new NodePort(portName, PortType.In, 0));
                                                    }
                                                    break;
                                                #endregion
                                                case DcsPortType.Rocker:
                                                    #region Rocker
                                                    string push1, release1, push2, release2;
                                                    string rockerKey;
                                                    if (EasyXml.GetAttribute(port, "Push1", out push1) &&
                                                        EasyXml.GetAttribute(port, "Release1", out release1) &&
                                                        EasyXml.GetAttribute(port, "Push2", out push2) &&
                                                        EasyXml.GetAttribute(port, "Release2", out release2) &&
                                                        EasyXml.GetAttribute(port, "Key", out rockerKey))
                                                    {
                                                        #region 处理数据 Button
                                                        inRocker newRocker = new inRocker();
                                                        newRocker.isPush1 = true;
                                                        newRocker.push1Msg = "C" + rockerKey + "," + push1 + ",1.0";
                                                        newRocker.release1Msg = "C" + rockerKey + "," + release1 + ",0.0";
                                                        newRocker.push2Msg = "C" + rockerKey + "," + push2 + ",-1.0";
                                                        newRocker.release2Msg = "C" + rockerKey + "," + release2 + ",0.0";
                                                        #endregion
                                                        rockerList.Add(moduleList.Count.ToString() + portList.Count.ToString(), newRocker);
                                                        portList.Add(new NodePort(portName, PortType.In, 0));
                                                    }
                                                    break;
                                                #endregion
                                                case DcsPortType.Switch:
                                                    #region Switch
                                                    string posList;
                                                    string switchKey;
                                                    if (EasyXml.GetAttribute(port, "Position", out posList) &&
                                                        EasyXml.GetAttribute(port, "Key", out switchKey))
                                                    {
                                                        #region 处理数据 Button
                                                        string[] pos = posList.Split(':');
                                                        inSwitch newSwitch = new inSwitch();
                                                        newSwitch.positionList = new List<string>();
                                                        for (int i = 0; i < pos.Length; i++)
                                                        {
                                                            newSwitch.positionList.Add("C" + switchKey + "," + pos[i]);
                                                        }
                                                        #endregion
                                                        switchList.Add(moduleList.Count.ToString() + portList.Count.ToString(), newSwitch);
                                                        portList.Add(new NodePort(portName, PortType.In, 0));
                                                    }
                                                    break;
                                                #endregion
                                                case DcsPortType.Axis:
                                                    #region Axis
                                                    float min, max;
                                                    string axisKey;
                                                    if (EasyXml.GetAttribute(port, "Min", out min) &&
                                                        EasyXml.GetAttribute(port, "Max", out max) &&
                                                        EasyXml.GetAttribute(port, "Key", out axisKey))
                                                    {
                                                        #region 处理数据 Button
                                                        inAxis newAxis = new inAxis();
                                                        newAxis.min = min;
                                                        newAxis.max = max;
                                                        newAxis.key = axisKey;
                                                        #endregion
                                                        axisList.Add(moduleList.Count.ToString() + portList.Count.ToString(), newAxis);
                                                        portList.Add(new NodePort(portName, PortType.In, 0f));
                                                    }
                                                    break;
                                                #endregion
                                                case DcsPortType.Encoder:
                                                    #region Encoder
                                                    string step;
                                                    string encoderKey;
                                                    if (EasyXml.GetAttribute(port, "Step", out step) &&
                                                        EasyXml.GetAttribute(port, "Key", out encoderKey))
                                                    {
                                                        #region 处理数据 Button
                                                        inEncoder newEncoder = new inEncoder();
                                                        newEncoder.plusMsg = "C" + encoderKey + "," + step;
                                                        newEncoder.lessMsg = "C" + encoderKey + ",-" + step;
                                                        #endregion
                                                        encoderList.Add(moduleList.Count.ToString() + portList.Count.ToString(), newEncoder);
                                                        portList.Add(new NodePort(portName, PortType.In, 0));
                                                    }
                                                    break;
                                                #endregion
                                                case DcsPortType.Out:
                                                    #region Out
                                                    string valueType;
                                                    string msgValue;
                                                    if (EasyXml.GetAttribute(port, "ValueType", out valueType) &&
                                                        EasyXml.GetAttribute(port, "Value", out msgValue))
                                                    {
                                                        #region 处理数据 INT
                                                        outData newData = new outData();
                                                        newData.nodeIndex = moduleList.Count;
                                                        newData.portIndex = portList.Count;
                                                        newData.msgValue = msgValue;
                                                        string javaScript;
                                                        if (EasyXml.GetAttribute(port, "Script", out javaScript))
                                                        {
                                                            newData.javaScript = javaScript;
                                                        }
                                                        else
                                                        {
                                                            newData.javaScript = "";
                                                        }
                                                        dataList.Add(newData);
                                                        #endregion
                                                        switch (valueType)
                                                        {
                                                            case "Int":
                                                                portList.Add(new NodePort(portName, PortType.Out, 0));
                                                                break;
                                                            case "Float":
                                                                portList.Add(new NodePort(portName, PortType.Out, 0f));
                                                                break;
                                                            case "String":
                                                                portList.Add(new NodePort(portName, PortType.Out, "...."));
                                                                break;
                                                        }
                                                    }
                                                    break;
                                                    #endregion
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
                foreach (outData buffer in dataList)
                {
                    string[] keyList = buffer.msgValue.Split(',');
                    string msgValue = "";
                    for (int i = 0; i < keyList.Length; i++)
                    {
                        string value = keyList[i];
                        if (value[0].Equals('@'))
                        {
                            value = value.Substring(1);
                            if (valueBuffer.ContainsKey(value))
                            {
                                msgValue += valueBuffer[value];
                            }
                        }
                        else
                        {
                            msgValue += value;
                        }
                    }
                    if (buffer.nodeIndex >= 0 && buffer.nodeIndex < moduleList.Count)
                    {
                        if (buffer.portIndex >= 0 && buffer.portIndex < moduleList[buffer.nodeIndex].NodePortList.Count)
                        {
                            NodePort np = moduleList[buffer.nodeIndex].NodePortList[buffer.portIndex];
                            switch (np.Type)
                            {
                                case PortValue.Int64:
                                    float value;
                                    if (!float.TryParse(msgValue, out value))
                                    {
                                        value = 0;
                                    }
                                    np.ValueInt64 = (int)(value + 0.5f);
                                    break;
                                case PortValue.Double:
                                    float valueF;
                                    if (!float.TryParse(msgValue, out valueF))
                                    {
                                        valueF = 0f;
                                    }
                                    np.ValueDouble = valueF;
                                    break;
                                case PortValue.String:
                                    np.ValueString = msgValue;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("DCS World Error : " + ex.Message + "\n" + ex.StackTrace);
            }
        }
        public void NodeCloseEvent(int mIndex)
        {
        }
        public void ValueChangeEvent(int mIndex, int pIndex)
        {
            string key = mIndex.ToString() + pIndex.ToString();
            #region Button
            if (buttonList.ContainsKey(key))
            {
                inButton button = buttonList[key];
                long value = moduleList[mIndex].NodePortList[pIndex].ValueInt64;
                UdpEventArgs uea = new UdpEventArgs(port);
                if (value == 0)
                {
                    uea.Msg = System.Text.Encoding.UTF8.GetBytes(button.releaseMsg);
                }
                else
                {
                    uea.Msg = System.Text.Encoding.UTF8.GetBytes(button.pushMsg);
                }
                uea.tarClient = sendClient;
                SendUDP(this, uea);
            }
            #endregion
            #region Rocker
            if (rockerList.ContainsKey(key))
            {
                inRocker rocker = rockerList[key];
                long value = moduleList[mIndex].NodePortList[pIndex].ValueInt64;
                UdpEventArgs uea = new UdpEventArgs(port);
                switch (value)
                {
                    case 0:
                        rocker.isPush1 = true;
                        uea.Msg = System.Text.Encoding.UTF8.GetBytes(rocker.push1Msg);
                        break;
                    case 1:
                        if (rocker.isPush1)
                            uea.Msg = System.Text.Encoding.UTF8.GetBytes(rocker.release1Msg);
                        else
                            uea.Msg = System.Text.Encoding.UTF8.GetBytes(rocker.release2Msg);
                        break;
                    case 2:
                        rocker.isPush1 = false;
                        uea.Msg = System.Text.Encoding.UTF8.GetBytes(rocker.push2Msg);
                        break;
                }
                uea.tarClient = sendClient;
                SendUDP(this, uea);
            }
            #endregion
            #region Switch
            if (switchList.ContainsKey(key))
            {
                inSwitch _switch = switchList[key];
                int value = (int)moduleList[mIndex].NodePortList[pIndex].ValueInt64;
                UdpEventArgs uea = new UdpEventArgs(port);
                if (value >= 0 && value < _switch.positionList.Count)
                {
                    uea.Msg = System.Text.Encoding.UTF8.GetBytes(_switch.positionList[value]);
                    uea.tarClient = sendClient;
                    SendUDP(this, uea);
                }
            }
            #endregion
            #region Axis
            if (axisList.ContainsKey(key))
            {
                inAxis axis = axisList[key];
                double value = moduleList[mIndex].NodePortList[pIndex].ValueDouble;
                UdpEventArgs uea = new UdpEventArgs(port);
                double valueMsg = (axis.max - axis.min) * value + axis.min;
                uea.Msg = System.Text.Encoding.UTF8.GetBytes("C" + axis.key + "," + valueMsg.ToString("f4"));
                uea.tarClient = sendClient;
                SendUDP(this, uea);
            }
            #endregion
            #region Encoder
            if (encoderList.ContainsKey(key))
            {
                inEncoder encoder = encoderList[key];
                long value = moduleList[mIndex].NodePortList[pIndex].ValueInt64;
                UdpEventArgs uea = new UdpEventArgs(port);
                switch (value)
                {
                    case 0:
                        uea.Msg = System.Text.Encoding.UTF8.GetBytes(encoder.lessMsg);
                        moduleList[mIndex].NodePortList[pIndex].ValueInt64 = 1;
                        break;
                    case 2:
                        uea.Msg = System.Text.Encoding.UTF8.GetBytes(encoder.plusMsg);
                        moduleList[mIndex].NodePortList[pIndex].ValueInt64 = 1;
                        break;
                    default:
                        moduleList[mIndex].NodePortList[pIndex].ValueInt64 = 1;
                        break;
                }
                uea.tarClient = sendClient;
                SendUDP(this, uea);
            }
            #endregion
        }
        void autoLink()
        {
            UdpEventArgs uea = new UdpEventArgs(port);
            CreateUDP(this, uea);
            Console.WriteLine("DCS World Link : " + ip + " : " + port);
        }
        #region UI回调
        public void OnButtonLeftClick(string id)
        {
            if (id.Equals("DcsSet"))
            {
                autoLink();
            }
        }

        public void OnButtonRightClick(string id)
        {
            //Console.WriteLine("RightClick : " + id);
        }

        public void OnSwitchButtonChange(string id, bool value)
        {
            //Console.WriteLine("SwitchButtonChange : " + id + " - " + value);
        }

        public void OnTextEditorChange(string id, string value)
        {
            //Console.WriteLine("TextEditorChange : " + id + " - " + value);
            switch (id)
            {
                case "dcsIP":
                    ip = value;
                    break;
                case "dcsPort":
                    int _port = 9089;
                    if (int.TryParse(value, out _port))
                    {
                        port = _port;
                    }
                    break;
            }
        }

        public void OnTrackBarChange(string id, int value)
        {
            //Console.WriteLine("TrackBarChange : " + id + " - " + value);
        }
        #endregion
        public void OnReceiveUdpMsg(EndPoint client, byte[] msg)
        {
            sendClient = client;
            if (msg.Length > 12)
            {
                String packetClientID = System.Text.Encoding.UTF8.GetString(msg, 0, 8);
                if (!_clientID.Equals(packetClientID))
                {
                    _clientID = packetClientID;
                    UdpEventArgs uea = new UdpEventArgs(port);
                    uea.Msg = System.Text.Encoding.UTF8.GetBytes("R");
                    uea.tarClient = sendClient;
                    SendUDP(this, uea);
                }
                _tokenCount = 0;
                int parseCount = msg.Length - 1; int lastIndex = 8;
                for (int i = 9; i < parseCount; i++)
                {
                    if (msg[i] == 0x3a || msg[i] == 0x3d)
                    {
                        int size = i - lastIndex - 1;
                        _tokens[_tokenCount++] = Encoding.UTF8.GetString(msg, lastIndex + 1, size);
                        lastIndex = i;
                    }
                }
                _tokens[_tokenCount++] = Encoding.UTF8.GetString(msg, lastIndex + 1, parseCount - lastIndex - 1);
                if (_tokenCount % 1 > 0)
                {
                    _tokenCount--;
                }
                ProcessData();                           //处理数据;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("数据长度异常");
            }
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("DCS World - OnReceiveUdpMsg : " + Encoding.UTF8.GetString(msg) + " - From : " + ip.ToString());
#endif
        }
        private void ProcessData()
        {
            lock (lockObj)
            {
                for (int i = 0; i < _tokenCount; i += 2)
                {
                    string key = _tokens[i];
                    string value = _tokens[i + 1];
                    if (valueBuffer.ContainsKey(key))
                    {
                        valueBuffer[key] = value;
                    }
                    else
                    {
                        valueBuffer.Add(key, value);
                    }
                }
            }
        }
    }
}
