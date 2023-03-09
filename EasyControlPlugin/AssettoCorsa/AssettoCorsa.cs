using ControllorPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AssettoCorsa
{
    public class AssettoCorsa : InterfacePlugin
    {
        List<Node> moduleList;
        string ip = "127.0.0.1";
        private static Thread _sendThread;
        static Socket server;
        //==============================================
        public string PluginID
        {
            get { return "F4CA1CC5-385A-45BD-9E5D-09FFA321C199"; }
        }

        public bool Open { get; set; }
        public bool Auto { get; set; }

        public event EventHandler ButtonLeftClick;
        public event EventHandler ButtonRightClick;
        public event EventHandler SwitchButtonChange;
        public event EventHandler TextEditorChange;
        public event EventHandler TrackBarChange;
        public event EventHandler CreateUDP;
        public event EventHandler SendUDP;

        public List<Node> GetModuleList()
        {
            return moduleList;
        }

        public string GetName()
        {
            return "Assetto Corsa";
        }

        public void Init()
        {
            Open = true;
            moduleList = new List<Node>();
            List<NodePort> pinList = new List<NodePort>();
            pinList.Add(new NodePort("identifier", PortType.Out, ""));
            pinList.Add(new NodePort("size", PortType.Out, 0));
            pinList.Add(new NodePort("speed_Kmh", PortType.Out, 0f));
            pinList.Add(new NodePort("speed_Mph", PortType.Out, 0f));
            pinList.Add(new NodePort("speed_Ms", PortType.Out, 0f));
            pinList.Add(new NodePort("isAbsEnabled", PortType.Out, 0));
            pinList.Add(new NodePort("isAbsInAction", PortType.Out, 0));
            pinList.Add(new NodePort("isTcInAction", PortType.Out, 0));
            pinList.Add(new NodePort("isTcEnabled", PortType.Out, 0));
            pinList.Add(new NodePort("isInPit", PortType.Out, 0));
            pinList.Add(new NodePort("isEngineLimiterOn", PortType.Out, 0));
            pinList.Add(new NodePort("accG_vertical", PortType.Out, 0f));
            pinList.Add(new NodePort("accG_horizontal", PortType.Out, 0f));
            pinList.Add(new NodePort("accG_frontal", PortType.Out, 0f));
            pinList.Add(new NodePort("lapTime", PortType.Out, ""));
            pinList.Add(new NodePort("lastLap", PortType.Out, ""));
            pinList.Add(new NodePort("bestLap", PortType.Out, ""));
            pinList.Add(new NodePort("lapCount", PortType.Out, 0));
            pinList.Add(new NodePort("gas", PortType.Out, 0f));
            pinList.Add(new NodePort("brake", PortType.Out, 0f));
            pinList.Add(new NodePort("clutch", PortType.Out, 0f));
            pinList.Add(new NodePort("engineRPM", PortType.Out, 0f));
            pinList.Add(new NodePort("steer", PortType.Out, 0f));
            pinList.Add(new NodePort("gear", PortType.Out, ""));
            pinList.Add(new NodePort("cgHeight", PortType.Out, 0f));
            Node test1 = new Node("Car Info A", pinList);
            test1.Info = "This is Car Info A";
            moduleList.Add(test1);
            //----
            List<NodePort> pinList2 = new List<NodePort>();
            pinList2.Add(new NodePort("wheelAngularSpeed1", PortType.Out, 0f));
            pinList2.Add(new NodePort("wheelAngularSpeed2", PortType.Out, 0f));
            pinList2.Add(new NodePort("wheelAngularSpeed3", PortType.Out, 0f));
            pinList2.Add(new NodePort("wheelAngularSpeed4", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipAngle1", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipAngle2", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipAngle3", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipAngle4", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipAngle_ContactPatch1", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipAngle_ContactPatch2", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipAngle_ContactPatch3", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipAngle_ContactPatch4", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipRatio1", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipRatio2", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipRatio3", PortType.Out, 0f));
            pinList2.Add(new NodePort("slipRatio4", PortType.Out, 0f));
            pinList2.Add(new NodePort("tyreSlip1", PortType.Out, 0f));
            pinList2.Add(new NodePort("tyreSlip2", PortType.Out, 0f));
            pinList2.Add(new NodePort("tyreSlip3", PortType.Out, 0f));
            pinList2.Add(new NodePort("tyreSlip4", PortType.Out, 0f));
            pinList2.Add(new NodePort("ndSlip1", PortType.Out, 0f));
            pinList2.Add(new NodePort("ndSlip2", PortType.Out, 0f));
            pinList2.Add(new NodePort("ndSlip3", PortType.Out, 0f));
            pinList2.Add(new NodePort("ndSlip4", PortType.Out, 0f));
            pinList2.Add(new NodePort("load1", PortType.Out, 0f));
            pinList2.Add(new NodePort("load2", PortType.Out, 0f));
            pinList2.Add(new NodePort("load3", PortType.Out, 0f));
            pinList2.Add(new NodePort("load4", PortType.Out, 0f));
            Node test2 = new Node("Car Info B", pinList2);
            test2.Info = "This is Car Info B";
            moduleList.Add(test2);
            //----
            List<NodePort> pinList3 = new List<NodePort>();
            pinList3.Add(new NodePort("Dy1", PortType.Out, 0f));
            pinList3.Add(new NodePort("Dy2", PortType.Out, 0f));
            pinList3.Add(new NodePort("Dy3", PortType.Out, 0f));
            pinList3.Add(new NodePort("Dy4", PortType.Out, 0f));
            pinList3.Add(new NodePort("Mz1", PortType.Out, 0f));
            pinList3.Add(new NodePort("Mz2", PortType.Out, 0f));
            pinList3.Add(new NodePort("Mz3", PortType.Out, 0f));
            pinList3.Add(new NodePort("Mz4", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreDirtyLevel1", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreDirtyLevel2", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreDirtyLevel3", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreDirtyLevel4", PortType.Out, 0f));
            pinList3.Add(new NodePort("camberRAD1", PortType.Out, 0f));
            pinList3.Add(new NodePort("camberRAD2", PortType.Out, 0f));
            pinList3.Add(new NodePort("camberRAD3", PortType.Out, 0f));
            pinList3.Add(new NodePort("camberRAD4", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreRadius1", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreRadius2", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreRadius3", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreRadius4", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreLoadedRadius1", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreLoadedRadius2", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreLoadedRadius3", PortType.Out, 0f));
            pinList3.Add(new NodePort("tyreLoadedRadius4", PortType.Out, 0f));
            pinList3.Add(new NodePort("suspensionHeight1", PortType.Out, 0f));
            pinList3.Add(new NodePort("suspensionHeight2", PortType.Out, 0f));
            pinList3.Add(new NodePort("suspensionHeight3", PortType.Out, 0f));
            pinList3.Add(new NodePort("suspensionHeight4", PortType.Out, 0f));
            pinList3.Add(new NodePort("carPositionNormalized", PortType.Out, 0f));
            pinList3.Add(new NodePort("carSlope", PortType.Out, 0f));
            pinList3.Add(new NodePort("carCoordinates1", PortType.Out, 0f));
            pinList3.Add(new NodePort("carCoordinates2", PortType.Out, 0f));
            pinList3.Add(new NodePort("carCoordinates3", PortType.Out, 0f));
            Node test3 = new Node("Car Info C", pinList3);
            test3.Info = "This is Car Info C";
            moduleList.Add(test3);
            //----
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000));//绑定端口号和IP
            _sendThread = new Thread(
                new ThreadStart(WaitForData));
            _sendThread.IsBackground = true;
            _sendThread.Start();
        }
        private void WaitForData()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(1);
                    EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                    byte[] buffer = new byte[1024];
                    int length = server.ReceiveFrom(buffer, ref point);//接收数据报
                    if (length == 328)//Car Info
                    {

                        var bw = new BinaryReader(new MemoryStream(buffer));
                        moduleList[0].NodePortList[0].ValueString = Encoding.UTF8.GetString(bw.ReadBytes(4));
                        moduleList[0].NodePortList[1].ValueInt64 = bw.ReadInt32();
                        moduleList[0].NodePortList[2].ValueDouble = bw.ReadSingle();
                        moduleList[0].NodePortList[3].ValueDouble = bw.ReadSingle();
                        moduleList[0].NodePortList[4].ValueDouble = bw.ReadSingle();
                        moduleList[0].NodePortList[5].ValueInt64 = bw.ReadByte();
                        moduleList[0].NodePortList[6].ValueInt64 = bw.ReadByte();
                        moduleList[0].NodePortList[7].ValueInt64 = bw.ReadByte();
                        moduleList[0].NodePortList[8].ValueInt64 = bw.ReadByte();
                        bw.ReadByte();
                        bw.ReadByte();
                        moduleList[0].NodePortList[9].ValueInt64 = bw.ReadByte();
                        moduleList[0].NodePortList[10].ValueInt64 = bw.ReadByte();
                        moduleList[0].NodePortList[11].ValueDouble = bw.ReadSingle();
                        moduleList[0].NodePortList[12].ValueDouble = bw.ReadSingle();
                        moduleList[0].NodePortList[13].ValueDouble = bw.ReadSingle();
                        moduleList[0].NodePortList[14].ValueString = TimeSpan.FromMilliseconds(bw.ReadInt32()).ToString("c");
                        moduleList[0].NodePortList[15].ValueString = TimeSpan.FromMilliseconds(bw.ReadInt32()).ToString("c");
                        moduleList[0].NodePortList[16].ValueString = TimeSpan.FromMilliseconds(bw.ReadInt32()).ToString("c");
                        moduleList[0].NodePortList[17].ValueInt64 = bw.ReadInt32();
                        moduleList[0].NodePortList[18].ValueDouble = bw.ReadSingle();
                        moduleList[0].NodePortList[19].ValueDouble = bw.ReadSingle();
                        moduleList[0].NodePortList[20].ValueDouble = bw.ReadSingle();
                        moduleList[0].NodePortList[21].ValueDouble = bw.ReadSingle();
                        moduleList[0].NodePortList[22].ValueDouble = bw.ReadSingle();
                        int gear = bw.ReadInt32();
                        switch (gear)
                        {
                            case 0:
                                moduleList[0].NodePortList[23].ValueString = "R";
                                break;
                            case 1:
                                moduleList[0].NodePortList[23].ValueString = "N";
                                break;
                            default:
                                moduleList[0].NodePortList[23].ValueString = (gear - 1).ToString();
                                break;
                        }
                        moduleList[0].NodePortList[24].ValueDouble = bw.ReadSingle();
                        //----
                        moduleList[1].NodePortList[0].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[1].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[2].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[3].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[4].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[5].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[6].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[7].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[8].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[9].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[10].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[11].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[12].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[13].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[14].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[15].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[16].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[17].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[18].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[19].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[20].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[21].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[22].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[23].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[24].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[25].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[26].ValueDouble = bw.ReadSingle();
                        moduleList[1].NodePortList[27].ValueDouble = bw.ReadSingle();
                        //----
                        moduleList[2].NodePortList[0].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[1].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[2].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[3].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[4].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[5].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[6].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[7].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[8].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[9].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[10].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[11].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[12].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[13].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[14].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[15].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[16].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[17].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[18].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[19].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[20].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[21].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[22].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[23].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[24].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[25].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[26].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[27].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[28].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[29].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[30].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[31].ValueDouble = bw.ReadSingle();
                        moduleList[2].NodePortList[32].ValueDouble = bw.ReadSingle();
                    }
                }
            }
            catch (Exception se)
            {
                Console.WriteLine("error:" + se.Message);
            }
        }
        public void OnReceiveUdpMsg(EndPoint _client, byte[] _msg)
        {
        }
        public void AutoOpen()
        {
            autoLink();
        }
        public void Update()
        {
        }
        public void NodeCloseEvent(int mIndex)
        {
        }
        public void ValueChangeEvent(int mIndex, int pIndex)
        {
        }
        #region UI
        void autoLink()
        {
            EndPoint point = new IPEndPoint(IPAddress.Parse(ip), 9996);
            var buffer = new byte[255];
            var bw = new BinaryWriter(new MemoryStream(buffer));
            bw.Write(1);
            bw.Write(1);
            bw.Write(1);
            server.SendTo(buffer, point);
        }
        public void OnButtonLeftClick(string id)
        {
            if (id.Equals("acSet"))
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
                case "acIP":
                    ip = value;
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
