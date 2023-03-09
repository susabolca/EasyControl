using ControllorPlugin;
using LockheedMartin.Prepar3D.SimConnect;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace Prepar3Dv4
{
    public class Prepar3Dv4 : InterfacePlugin
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        //=========================================
        private List<Node> moduleList = new List<Node>();

        public event EventHandler ButtonLeftClick;
        public event EventHandler ButtonRightClick;
        public event EventHandler SwitchButtonChange;
        public event EventHandler TextEditorChange;
        public event EventHandler TrackBarChange;
        public event EventHandler CreateUDP;
        public event EventHandler SendUDP;
        //=========================================
        // User-defined win32 event 
        const int WM_USER_SIMCONNECT = 0x0402;

        // SimConnect object 
        SimConnect simconnect = null;

        enum DEFINITIONS
        {
            Struct1,
        }

        enum DATA_REQUESTS
        {
            REQUEST_1,
        };

        // this is how you declare a data structure so that 
        // simconnect knows how to fill it/read it. 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct Struct1
        {
            // this is how you declare a fixed size string 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String title;
            public double latitude;
            public double longitude;
            public double altitude;
        };
        //=========================================
        public bool Open { get; set; }
        public bool Auto { get; set; }

        public string PluginID
        {
            get { return "8816ED29-E221-4A22-B78C-B72CCA4151FD"; }
        }

        public void DefWndProc(int message)
        {
            if (message == WM_USER_SIMCONNECT)
            {
                if (simconnect != null)
                {
                    simconnect.ReceiveMessage();
                }
            }
        }

        public List<Node> GetModuleList()
        {
            return moduleList;
        }

        public string GetName()
        {
            return "Prepar3D v4";
        }

        public void Init()
        {
            moduleList = new List<Node>();
            //----------------------------------------------------------------------------
            List<NodePort> pinList = new List<NodePort>();
            pinList.Add(new NodePort("title", PortType.Out, ""));
            pinList.Add(new NodePort("latitude", PortType.Out, 0f));
            pinList.Add(new NodePort("longitude", PortType.Out, 0f));
            pinList.Add(new NodePort("altitude", PortType.Out, 0f));
            Node test1 = new Node("Prepar3Dv4 Test", pinList);
            test1.Info = "This is Prepar3Dv4 Test";
            moduleList.Add(test1);
        }

        private void closeConnection()
        {
            if (simconnect != null)
            {
                // Dispose serves the same purpose as SimConnect_Close() 
                simconnect.Dispose();
                simconnect = null;
                Console.WriteLine("Prepar3D connection closed");
            }
        }
        void simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Console.WriteLine("Connected to Prepar3D");
        }

        // The case where the user closes Prepar3D 
        void simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Console.WriteLine("Prepar3D has exited");
            closeConnection();
        }

        void simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Console.WriteLine("Exception received: " + data.dwException);
        }
        void simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            switch ((DATA_REQUESTS)data.dwRequestID)
            {
                case DATA_REQUESTS.REQUEST_1:
                    Struct1 s1 = (Struct1)data.dwData[0];

                    moduleList[0].NodePortList[0].ValueString = s1.title;
                    moduleList[0].NodePortList[1].ValueDouble = (float)s1.latitude;
                    moduleList[0].NodePortList[2].ValueDouble = (float)s1.longitude;
                    moduleList[0].NodePortList[3].ValueDouble = (float)s1.altitude;
                    break;

                default:
                    Console.WriteLine("Unknown request ID: " + data.dwRequestID);
                    break;
            }
        }
        void autoLink()
        {
            if (simconnect == null)
            {
                try
                {
                    simconnect = new SimConnect("Managed Data Request", GetForegroundWindow(), WM_USER_SIMCONNECT, null, 0);
                    // listen to connect and quit msgs 
                    simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(simconnect_OnRecvOpen);
                    simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);

                    // listen to exceptions 
                    simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);

                    // define a data structure 
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                    // IMPORTANT: register it with the simconnect managed wrapper marshaller 
                    // if you skip this step, you will only receive a uint in the .dwData field. 
                    simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

                    // catch a simobject data request 
                    simconnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(simconnect_OnRecvSimobjectDataBytype);
                }
                catch (COMException ex)
                {
                    Console.WriteLine("Plugin Prepar3Dv4 Error : " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Prepar3D connected");
            }
        }
        #region UI
        public void OnButtonLeftClick(string id)
        {
            switch (id)
            {
                case "P3dOpen":
                    autoLink();
                    break;
                case "P3dClose":
                    closeConnection();
                    break;
            }
        }

        public void OnButtonRightClick(string id)
        {
        }

        public void OnReceiveUdpMsg(EndPoint client, byte[] msg)
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
        public void AutoOpen()
        {
            autoLink();
        }
        public void Update()
        {
            try
            {
                if (simconnect != null)
                    simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Updata Error : " + ex.Message);
            }
        }

        public void NodeCloseEvent(int mIndex)
        {
        }
        public void ValueChangeEvent(int mIndex, int pIndex)
        {
        }
    }
}
