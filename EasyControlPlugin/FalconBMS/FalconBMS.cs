using BmsSimulatorTelemetry;
using ControllorPlugin;
using System;
using System.Collections.Generic;
using System.Net;

namespace FalconBMS
{
    public class FalconBMS : InterfacePlugin
    {
        private SharedMemory _sharedMemory = null;
        private SharedMemory _sharedMemory2 = null;

        //private RadarContact[] _contacts = new RadarContact[40];

        private FlightData _lastFlightData;
        private FlightData2 _lastFlightData2;
        //-------------------------------------------------------------------------
        List<Node> moduleList;
        //===========================================
        public string PluginID
        {
            get
            {
                return "C4CEE5E9-7710-4058-A810-740FC4479C02";
            }
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

        public void DefWndProc(int message)
        {
        }

        public List<Node> GetModuleList()
        {
            return moduleList;
        }

        public string GetName()
        {
            return "Falcon BMS";
        }

        public void Init()
        {
            _sharedMemory = new SharedMemory("FalconSharedMemoryArea");
            _sharedMemory.Open();

            _sharedMemory2 = new SharedMemory("FalconSharedMemoryArea2");
            _sharedMemory2.Open();
            //----------------------------------------------------------------------------------------------------
            moduleList = new List<Node>();
            List<NodePort> portListA = new List<NodePort>();
            portListA.Add(new NodePort("Altimeter-altitude", PortType.Out, 0.0));
            portListA.Add(new NodePort("Altimeter-barometric pressure", PortType.Out, 0.0));
            portListA.Add(new NodePort("Altimeter-indicated altitude", PortType.Out, 0.0));
            portListA.Add(new NodePort("ADI-pitch", PortType.Out, 0.0));
            portListA.Add(new NodePort("ADI-roll", PortType.Out, 0.0));
            portListA.Add(new NodePort("ADI-ils horizontal", PortType.Out, 0.0));
            portListA.Add(new NodePort("ADI-ils vertical", PortType.Out, 0.0));
            portListA.Add(new NodePort("HSI-bearing to beacon", PortType.Out, 0.0));
            portListA.Add(new NodePort("HSI-current heading", PortType.Out, 0.0));
            portListA.Add(new NodePort("HSI-desired course", PortType.Out, 0.0));
            portListA.Add(new NodePort("HSI-desired heading", PortType.Out, 0.0));
            portListA.Add(new NodePort("HSI-nav mode", PortType.Out, 0));

            portListA.Add(new NodePort("HSI-course deviation", PortType.Out, 0.0));

            portListA.Add(new NodePort("HSI-distance to beacon", PortType.Out, 0.0));
            portListA.Add(new NodePort("VVI-vertical velocity", PortType.Out, 0.0));
            portListA.Add(new NodePort("AOA-angle of attack", PortType.Out, 0.0));
            portListA.Add(new NodePort("IAS-mach", PortType.Out, 0.0));
            portListA.Add(new NodePort("IAS-indicated air speed", PortType.Out, 0.0));
            portListA.Add(new NodePort("IAS-true air speed", PortType.Out, 0.0));
            Node newNodeA = new Node("BMS A", portListA);
            moduleList.Add(newNodeA);
            //----------------------------------------------------------------------------------------------------
            List<NodePort> portListB = new List<NodePort>();
            portListB.Add(new NodePort("General-Gs", PortType.Out, 0.0));
            portListB.Add(new NodePort("Engine-nozzle position", PortType.Out, 0.0));
            portListB.Add(new NodePort("Fuel-internal fuel", PortType.Out, 0.0));
            portListB.Add(new NodePort("Fuel-external fuel", PortType.Out, 0.0));
            portListB.Add(new NodePort("Engine-fuel flow", PortType.Out, 0.0));
            portListB.Add(new NodePort("Engine-rpm", PortType.Out, 0.0));
            portListB.Add(new NodePort("Engine-ftit", PortType.Out, 0.0));
            portListB.Add(new NodePort("Landing Gear-position", PortType.Out, 0));
            portListB.Add(new NodePort("General-speed brake position", PortType.Out, 0.0));
            portListB.Add(new NodePort("General-speed brake indicator", PortType.Out, 0));
            portListB.Add(new NodePort("EPU-fuel", PortType.Out, 0.0));
            portListB.Add(new NodePort("Engine-oil pressure", PortType.Out, 0.0));

            portListB.Add(new NodePort("CMDS-chaff remaining", PortType.Out, 0.0));
            portListB.Add(new NodePort("CMDS-flares remaining", PortType.Out, 0.0));

            portListB.Add(new NodePort("Trim-roll trim", PortType.Out, 0.0));
            portListB.Add(new NodePort("Trim-pitch trim", PortType.Out, 0.0));
            portListB.Add(new NodePort("Trim-yaw trim", PortType.Out, 0.0));

            portListB.Add(new NodePort("Tacan-ufc tacan chan", PortType.Out, 0));
            portListB.Add(new NodePort("Tacan-aux tacan chan", PortType.Out, 0));
            portListB.Add(new NodePort("Tacan-ufc tacan band", PortType.Out, 0));
            portListB.Add(new NodePort("Tacan-aux tacan band", PortType.Out, 0));
            portListB.Add(new NodePort("Tacan-ufc tacan mode", PortType.Out, 0));
            portListB.Add(new NodePort("Tacan-aux tacan mode", PortType.Out, 0));

            Node newNodeB = new Node("BMS B", portListB);
            moduleList.Add(newNodeB);
            //----------------------------------------------------------------------------------------------------
            List<NodePort> portListC = new List<NodePort>();
            portListC.Add(new NodePort("HSI-to flag", PortType.Out, 0));
            portListC.Add(new NodePort("HSI-from flag", PortType.Out, 0));

            portListC.Add(new NodePort("HSI-ils warning flag", PortType.Out, 0));
            portListC.Add(new NodePort("HSI-course warning flag", PortType.Out, 0));
            portListC.Add(new NodePort("HSI-off flag", PortType.Out, 0));
            portListC.Add(new NodePort("HSI-init flag", PortType.Out, 0));
            portListC.Add(new NodePort("ADI-off flag", PortType.Out, 0));
            portListC.Add(new NodePort("ADI-aux flag", PortType.Out, 0));
            portListC.Add(new NodePort("ADI-gs flag", PortType.Out, 0));
            portListC.Add(new NodePort("ADI-loc flag", PortType.Out, 0));
            portListC.Add(new NodePort("Backup ADI-off flag", PortType.Out, 0));
            portListC.Add(new NodePort("VVI-off flag", PortType.Out, 0));
            portListC.Add(new NodePort("AOA-off flag", PortType.Out, 0));

            Node newNodeC = new Node("BMS Bits A", portListC);
            moduleList.Add(newNodeC);
            //----------------------------------------------------------------------------------------------------
            List<NodePort> portListD = new List<NodePort>();

            portListD.Add(new NodePort("Left Eyebrow-master caution indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Left Eyebrow-tf-fail indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Right Eyebrow-oxy low indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Right Eyebrow-engine fire indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Right Eyebrow-hydraulic/oil indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Right Eyebrow-canopy indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Right Eyebrow-takeoff landing config indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Right Eyebrow-flcs indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-stores config indicator", PortType.Out, 0));
            portListD.Add(new NodePort("AOA Indexer-above indicator", PortType.Out, 0));
            portListD.Add(new NodePort("AOA Indexer-on indicator", PortType.Out, 0));
            portListD.Add(new NodePort("AOA Indexer-below indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Refuel Indexer-ready indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Refuel Indexer-air/nws indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Refuel Indexer-disconnect indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-flight control system indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-leading edge flaps indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-engine fault indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-equip hot indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-overheat indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-low fuel indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-avionics indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-radar altimeter indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-iff indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-ecm indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-hook indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-nws fail indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Caution-cabin pressure indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Autopilot-on indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Misc-tfs standby indicator", PortType.Out, 0));
            portListD.Add(new NodePort("Test Panel-FLCS channel lamps", PortType.Out, 0));

            Node newNodeD = new Node("BMS Lights A", portListD);
            moduleList.Add(newNodeD);
            //----------------------------------------------------------------------------------------------------
            List<NodePort> portListE = new List<NodePort>();

            portListE.Add(new NodePort("Threat Warning Prime-handoff indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Threat Warning Prime-launch indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Threat Warning Prime-priority mode indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Threat Warning Prime-open mode indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Threat Warning Prime-naval indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Threat Warning Prime-unknown mode indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Threat Warning Prime-target step indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Aux Threat Warning-search indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Aux Threat Warning-activity indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Aux Threat Warning-low altitude indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Aux Threat Warning-power indicator", PortType.Out, 0));

            portListE.Add(new NodePort("CMDS-Go", PortType.Out, 0));
            portListE.Add(new NodePort("CMDS-NoGo", PortType.Out, 0));
            portListE.Add(new NodePort("CMDS-Degr", PortType.Out, 0));
            portListE.Add(new NodePort("CMDS-Rdy", PortType.Out, 0));
            portListE.Add(new NodePort("CMDS-ChaffLo", PortType.Out, 0));
            portListE.Add(new NodePort("CMDS-FlareLo", PortType.Out, 0));

            portListE.Add(new NodePort("ECM-power indicator", PortType.Out, 0));
            portListE.Add(new NodePort("ECM-fail indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Caution-forward fuel low indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Caution-aft fuel low indicator", PortType.Out, 0));
            portListE.Add(new NodePort("EPU-on indicator", PortType.Out, 0));
            portListE.Add(new NodePort("JFS-run indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Caution-second engine compressor indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Caution-oxygen low indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Caution-probe heat indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Caution-seat arm indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Caution-backup fuel control indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Caution-fuel oil hot indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Caution-anti skid indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Misc-tfs engaged indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Gear Handle-handle indicator", PortType.Out, 0));
            portListE.Add(new NodePort("Right Eyebrow-engine indicator", PortType.Out, 0));

            Node newNodeE = new Node("BMS Lights B", portListE);
            moduleList.Add(newNodeE);
            //----------------------------------------------------------------------------------------------------
            List<NodePort> portListF = new List<NodePort>();

            portListF.Add(new NodePort("Electronic-flcs pmg indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Electronic-main gen indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Electronic-standby generator indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Electronic-epu gen indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Electronic-epu pmg indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Electronic-to flcs indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Electronic-flcs rly indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Electronic-bat fail indicator", PortType.Out, 0));
            portListF.Add(new NodePort("EPU-hydrazine indicator", PortType.Out, 0));
            portListF.Add(new NodePort("EPU-air indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Caution-electric bus fail indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Caution-lef fault indicator", PortType.Out, 0));

            portListF.Add(new NodePort("General-on ground", PortType.Out, 0));
            portListF.Add(new NodePort("Flight Control-run light", PortType.Out, 0));
            portListF.Add(new NodePort("Flight Control-fail light", PortType.Out, 0));
            portListF.Add(new NodePort("Right Eyebrow-dbu on indicator", PortType.Out, 0));
            portListF.Add(new NodePort("General-parking brake engaged", PortType.Out, 0));
            portListF.Add(new NodePort("Caution-cadc indicator", PortType.Out, 0));
            portListF.Add(new NodePort("General-speed brake", PortType.Out, 0));

            portListF.Add(new NodePort("Landing Gear-nose gear indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Landing Gear-left gear indicator", PortType.Out, 0));
            portListF.Add(new NodePort("Landing Gear-right gear indicator", PortType.Out, 0));
            portListF.Add(new NodePort("General-power off", PortType.Out, 0));

            Node newNodeF = new Node("BMS Lights C", portListF);
            moduleList.Add(newNodeF);
            //----------------------------------------------------------------------------------------------------
            List<NodePort> portListG = new List<NodePort>();
            portListG.Add(new NodePort("DED-line1", PortType.Out, ""));
            portListG.Add(new NodePort("DED-line2", PortType.Out, ""));
            portListG.Add(new NodePort("DED-line3", PortType.Out, ""));
            portListG.Add(new NodePort("DED-line4", PortType.Out, ""));
            portListG.Add(new NodePort("DED-line5", PortType.Out, ""));
            portListG.Add(new NodePort("DEDInverse-line1", PortType.Out, ""));
            portListG.Add(new NodePort("DEDInverse-line2", PortType.Out, ""));
            portListG.Add(new NodePort("DEDInverse-line3", PortType.Out, ""));
            portListG.Add(new NodePort("DEDInverse-line4", PortType.Out, ""));
            portListG.Add(new NodePort("DEDInverse-line5", PortType.Out, ""));
            portListG.Add(new NodePort("PFL-line1", PortType.Out, ""));
            portListG.Add(new NodePort("PFL-line2", PortType.Out, ""));
            portListG.Add(new NodePort("PFL-line3", PortType.Out, ""));
            portListG.Add(new NodePort("PFL-line4", PortType.Out, ""));
            portListG.Add(new NodePort("PFL-line5", PortType.Out, ""));
            portListG.Add(new NodePort("PFLInverse-line1", PortType.Out, ""));
            portListG.Add(new NodePort("PFLInverse-line2", PortType.Out, ""));
            portListG.Add(new NodePort("PFLInverse-line3", PortType.Out, ""));
            portListG.Add(new NodePort("PFLInverse-line4", PortType.Out, ""));
            portListG.Add(new NodePort("PFLInverse-line5", PortType.Out, ""));
            Node newNodeG = new Node("BMS Text", portListG);
            moduleList.Add(newNodeG);
        }

        public void OnReceiveUdpMsg(EndPoint client, byte[] msg)
        {
        }
        #region UI
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
        public void AutoOpen()
        {

        }
        public void Update()
        {
            if (_sharedMemory != null && _sharedMemory.IsDataAvailable &&
                _sharedMemory2 != null && _sharedMemory2.IsDataAvailable)
            {
                _lastFlightData = (FlightData)_sharedMemory.MarshalTo(typeof(FlightData));
                _lastFlightData2 = (FlightData2)_sharedMemory2.MarshalTo(typeof(FlightData2));


                float altitidue = _lastFlightData.z;
                if (_lastFlightData.z < 0)
                {
                    altitidue = 99999.99f - _lastFlightData.z;
                }
                moduleList[0].NodePortList[0].ValueDouble = altitidue;
                moduleList[0].NodePortList[1].ValueDouble = 29.92;
                moduleList[0].NodePortList[2].ValueDouble = -_lastFlightData2.aauz;
                moduleList[0].NodePortList[3].ValueDouble = _lastFlightData.pitch;
                moduleList[0].NodePortList[4].ValueDouble = _lastFlightData.roll;
                moduleList[0].NodePortList[5].ValueDouble = (_lastFlightData.AdiIlsHorPos / 2.5f) - 1f;
                moduleList[0].NodePortList[6].ValueDouble = (_lastFlightData.AdiIlsVerPos * 2f) - 1f;
                moduleList[0].NodePortList[7].ValueDouble = _lastFlightData.bearingToBeacon;
                moduleList[0].NodePortList[8].ValueDouble = _lastFlightData.currentHeading;
                moduleList[0].NodePortList[9].ValueDouble = _lastFlightData.desiredCourse;
                moduleList[0].NodePortList[10].ValueDouble = _lastFlightData.desiredHeading;
                moduleList[0].NodePortList[11].ValueDouble = _lastFlightData2.navMode;

                float deviation = _lastFlightData.courseDeviation % 180;
                moduleList[0].NodePortList[12].ValueDouble = deviation / _lastFlightData.deviationLimit;

                moduleList[0].NodePortList[13].ValueDouble = _lastFlightData.distanceToBeacon;
                moduleList[0].NodePortList[14].ValueDouble = _lastFlightData.zDot;
                moduleList[0].NodePortList[15].ValueDouble = _lastFlightData.alpha;
                moduleList[0].NodePortList[16].ValueDouble = _lastFlightData.mach;
                moduleList[0].NodePortList[17].ValueDouble = _lastFlightData.kias;
                moduleList[0].NodePortList[18].ValueDouble = _lastFlightData.vt;
                //----------------------------------------------------------------------------------------------------
                moduleList[1].NodePortList[0].ValueDouble = _lastFlightData.gs;
                moduleList[1].NodePortList[1].ValueDouble = _lastFlightData.nozzlePos * 100;
                moduleList[1].NodePortList[2].ValueDouble = _lastFlightData.internalFuel;
                moduleList[1].NodePortList[3].ValueDouble = _lastFlightData.externalFuel;
                moduleList[1].NodePortList[4].ValueDouble = _lastFlightData.fuelFlow;
                moduleList[1].NodePortList[5].ValueDouble = _lastFlightData.rpm;
                moduleList[1].NodePortList[6].ValueDouble = _lastFlightData.ftit * 100;
                moduleList[1].NodePortList[7].ValueInt64 = _lastFlightData.gearPos != 0d ? 1 : 0;
                moduleList[1].NodePortList[8].ValueDouble = _lastFlightData.speedBrake;
                moduleList[1].NodePortList[9].ValueInt64 = _lastFlightData.speedBrake > 0d ? 1 : 0;
                moduleList[1].NodePortList[10].ValueDouble = _lastFlightData.epuFuel;
                moduleList[1].NodePortList[11].ValueDouble = _lastFlightData.oilPressure;

                moduleList[1].NodePortList[12].ValueDouble = _lastFlightData.ChaffCount;
                moduleList[1].NodePortList[13].ValueDouble = _lastFlightData.FlareCount;

                moduleList[1].NodePortList[14].ValueDouble = _lastFlightData.TrimRoll;
                moduleList[1].NodePortList[15].ValueDouble = _lastFlightData.TrimPitch;
                moduleList[1].NodePortList[16].ValueDouble = _lastFlightData.TrimYaw;

                moduleList[1].NodePortList[17].ValueInt64 = _lastFlightData.UFCTChan;
                moduleList[1].NodePortList[18].ValueInt64 = _lastFlightData.AUXTChan;
                moduleList[1].NodePortList[19].ValueInt64 = _lastFlightData2.tacanInfo[(int)TacanSources.UFC].HasFlag(TacanBits.band) ? 1 : 2;
                moduleList[1].NodePortList[20].ValueInt64 = _lastFlightData2.tacanInfo[(int)TacanSources.AUX].HasFlag(TacanBits.mode) ? 2 : 1;
                moduleList[1].NodePortList[21].ValueInt64 = _lastFlightData2.tacanInfo[(int)TacanSources.UFC].HasFlag(TacanBits.band) ? 1 : 2;
                moduleList[1].NodePortList[22].ValueInt64 = _lastFlightData2.tacanInfo[(int)TacanSources.AUX].HasFlag(TacanBits.mode) ? 2 : 1;
                //----------------------------------------------------------------------------------------------------
                moduleList[2].NodePortList[0].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.ToTrue) ? 1 : 0;
                moduleList[2].NodePortList[1].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.FromTrue) ? 1 : 0;

                moduleList[2].NodePortList[2].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.IlsWarning) ? 1 : 0;
                moduleList[2].NodePortList[3].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.CourseWarning) ? 1 : 0;
                moduleList[2].NodePortList[4].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.HSI_OFF) ? 1 : 0;
                moduleList[2].NodePortList[5].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.Init) ? 1 : 0;
                moduleList[2].NodePortList[6].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.ADI_OFF) ? 1 : 0;
                moduleList[2].NodePortList[7].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.ADI_AUX) ? 1 : 0;
                moduleList[2].NodePortList[8].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.ADI_GS) ? 1 : 0;
                moduleList[2].NodePortList[9].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.ADI_LOC) ? 1 : 0;
                moduleList[2].NodePortList[10].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.BUP_ADI_OFF) ? 1 : 0;
                moduleList[2].NodePortList[11].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.VVI) ? 1 : 0;
                moduleList[2].NodePortList[12].ValueInt64 = _lastFlightData.hsiBits.HasFlag(HsiBits.AOA) ? 1 : 0;
                //----------------------------------------------------------------------------------------------------
                moduleList[3].NodePortList[0].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.MasterCaution) ? 1 : 0;
                moduleList[3].NodePortList[1].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.TF) ? 1 : 0;
                moduleList[3].NodePortList[2].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.OXY_BROW) ? 1 : 0;
                moduleList[3].NodePortList[3].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.ENG_FIRE) ? 1 : 0;
                moduleList[3].NodePortList[4].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.HYD) ? 1 : 0;
                moduleList[3].NodePortList[5].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.CAN) ? 1 : 0;
                moduleList[3].NodePortList[6].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.T_L_CFG) ? 1 : 0;
                moduleList[3].NodePortList[7].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.FLCS) ? 1 : 0;
                moduleList[3].NodePortList[8].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.CONFIG) ? 1 : 0;
                moduleList[3].NodePortList[9].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.AOAAbove) ? 1 : 0;
                moduleList[3].NodePortList[10].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.AOAOn) ? 1 : 0;
                moduleList[3].NodePortList[11].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.AOABelow) ? 1 : 0;
                moduleList[3].NodePortList[12].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.RefuelRDY) ? 1 : 0;
                moduleList[3].NodePortList[13].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.RefuelAR) ? 1 : 0;
                moduleList[3].NodePortList[14].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.RefuelDSC) ? 1 : 0;
                moduleList[3].NodePortList[15].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.FltControlSys) ? 1 : 0;
                moduleList[3].NodePortList[16].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.LEFlaps) ? 1 : 0;
                moduleList[3].NodePortList[17].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.EngineFault) ? 1 : 0;
                moduleList[3].NodePortList[18].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.EQUIP_HOT) ? 1 : 0;
                moduleList[3].NodePortList[19].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.Overheat) ? 1 : 0;
                moduleList[3].NodePortList[20].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.FuelLow) ? 1 : 0;
                moduleList[3].NodePortList[21].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.Avionics) ? 1 : 0;
                moduleList[3].NodePortList[22].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.RadarAlt) ? 1 : 0;
                moduleList[3].NodePortList[23].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.IFF) ? 1 : 0;
                moduleList[3].NodePortList[24].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.ECM) ? 1 : 0;
                moduleList[3].NodePortList[25].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.Hook) ? 1 : 0;
                moduleList[3].NodePortList[26].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.NWSFail) ? 1 : 0;
                moduleList[3].NodePortList[27].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.CabinPress) ? 1 : 0;
                moduleList[3].NodePortList[28].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.AutoPilotOn) ? 1 : 0;
                moduleList[3].NodePortList[29].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.TFR_STBY) ? 1 : 0;
                moduleList[3].NodePortList[30].ValueInt64 = _lastFlightData.lightBits.HasFlag(BMSLightBits.Flcs_ABCD) ? 1 : 0;
                //----------------------------------------------------------------------------------------------------
                moduleList[4].NodePortList[0].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.HandOff) ? 1 : 0;
                moduleList[4].NodePortList[1].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.Launch) ? 1 : 0;
                moduleList[4].NodePortList[2].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.PriMode) ? 1 : 0;
                moduleList[4].NodePortList[3].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.AuxPwr) && !_lastFlightData.lightBits2.HasFlag(BMSLightBits2.PriMode) ? 1 : 0;
                moduleList[4].NodePortList[4].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.Naval) ? 1 : 0;
                moduleList[4].NodePortList[5].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.Unk) ? 1 : 0;
                moduleList[4].NodePortList[6].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.TgtSep) ? 1 : 0;
                moduleList[4].NodePortList[7].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.AuxSrch) ? 1 : 0;
                moduleList[4].NodePortList[8].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.AuxAct) ? 1 : 0;
                moduleList[4].NodePortList[9].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.AuxLow) ? 1 : 0;
                moduleList[4].NodePortList[10].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.AuxPwr) ? 1 : 0;

                moduleList[4].NodePortList[11].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.Go) ? 1 : 0;
                moduleList[4].NodePortList[12].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.NoGo) ? 1 : 0;
                moduleList[4].NodePortList[13].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.Degr) ? 1 : 0;
                moduleList[4].NodePortList[14].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.Rdy) ? 1 : 0;
                moduleList[4].NodePortList[15].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.ChaffLo) ? 1 : 0;
                moduleList[4].NodePortList[16].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.FlareLo) ? 1 : 0;

                moduleList[4].NodePortList[17].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.EcmPwr) ? 1 : 0;
                moduleList[4].NodePortList[18].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.EcmFail) ? 1 : 0;
                moduleList[4].NodePortList[19].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.FwdFuelLow) ? 1 : 0;
                moduleList[4].NodePortList[20].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.AftFuelLow) ? 1 : 0;
                moduleList[4].NodePortList[21].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.EPUOn) ? 1 : 0;
                moduleList[4].NodePortList[22].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.JFSOn) ? 1 : 0;
                moduleList[4].NodePortList[23].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.SEC) ? 1 : 0;
                moduleList[4].NodePortList[24].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.OXY_LOW) ? 1 : 0;
                moduleList[4].NodePortList[25].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.PROBEHEAT) ? 1 : 0;
                moduleList[4].NodePortList[26].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.SEAT_ARM) ? 1 : 0;
                moduleList[4].NodePortList[27].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.BUC) ? 1 : 0;
                moduleList[4].NodePortList[28].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.FUEL_OIL_HOT) ? 1 : 0;
                moduleList[4].NodePortList[29].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.ANTI_SKID) ? 1 : 0;
                moduleList[4].NodePortList[30].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.TFR_ENGAGED) ? 1 : 0;
                moduleList[4].NodePortList[31].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.GEARHANDLE) ? 1 : 0;
                moduleList[4].NodePortList[32].ValueInt64 = _lastFlightData.lightBits2.HasFlag(BMSLightBits2.ENGINE) ? 1 : 0;
                //----------------------------------------------------------------------------------------------------
                moduleList[5].NodePortList[0].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.FlcsPmg) ? 1 : 0;
                moduleList[5].NodePortList[1].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.MainGen) ? 1 : 0;
                moduleList[5].NodePortList[2].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.StbyGen) ? 1 : 0;
                moduleList[5].NodePortList[3].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.EpuGen) ? 1 : 0;
                moduleList[5].NodePortList[4].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.EpuPmg) ? 1 : 0;
                moduleList[5].NodePortList[5].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.ToFlcs) ? 1 : 0;
                moduleList[5].NodePortList[6].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.FlcsRly) ? 1 : 0;
                moduleList[5].NodePortList[7].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.BatFail) ? 1 : 0;
                moduleList[5].NodePortList[8].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.Hydrazine) ? 1 : 0;
                moduleList[5].NodePortList[9].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.Air) ? 1 : 0;
                moduleList[5].NodePortList[10].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.Elec_Fault) ? 1 : 0;
                moduleList[5].NodePortList[11].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.Lef_Fault) ? 1 : 0;

                moduleList[5].NodePortList[12].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.OnGround) ? 1 : 0;
                moduleList[5].NodePortList[13].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.FlcsBitRun) ? 1 : 0;
                moduleList[5].NodePortList[14].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.FlcsBitFail) ? 1 : 0;
                moduleList[5].NodePortList[15].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.DbuWarn) ? 1 : 0;
                moduleList[5].NodePortList[16].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.ParkBrakeOn) ? 1 : 0;
                moduleList[5].NodePortList[17].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.cadc) ? 1 : 0;
                moduleList[5].NodePortList[18].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.SpeedBrake) ? 1 : 0;

                moduleList[5].NodePortList[19].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.NoseGearDown) ? 1 : 0;
                moduleList[5].NodePortList[20].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.LeftGearDown) ? 1 : 0;
                moduleList[5].NodePortList[21].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.RightGearDown) ? 1 : 0;
                moduleList[5].NodePortList[22].ValueInt64 = _lastFlightData.lightBits3.HasFlag(BMSLightBits3.Power_Off) ? 1 : 0;
                //----------------------------------------------------------------------------------------------------
                moduleList[6].NodePortList[0].ValueString = _lastFlightData.DED.line1;
                moduleList[6].NodePortList[1].ValueString = _lastFlightData.DED.line2;
                moduleList[6].NodePortList[2].ValueString = _lastFlightData.DED.line3;
                moduleList[6].NodePortList[3].ValueString = _lastFlightData.DED.line4;
                moduleList[6].NodePortList[4].ValueString = _lastFlightData.DED.line5;
                moduleList[6].NodePortList[5].ValueString = _lastFlightData.DEDInverse.line1;
                moduleList[6].NodePortList[6].ValueString = _lastFlightData.DEDInverse.line2;
                moduleList[6].NodePortList[7].ValueString = _lastFlightData.DEDInverse.line3;
                moduleList[6].NodePortList[8].ValueString = _lastFlightData.DEDInverse.line4;
                moduleList[6].NodePortList[9].ValueString = _lastFlightData.DEDInverse.line5;
                moduleList[6].NodePortList[10].ValueString = _lastFlightData.PFL.line1;
                moduleList[6].NodePortList[11].ValueString = _lastFlightData.PFL.line2;
                moduleList[6].NodePortList[12].ValueString = _lastFlightData.PFL.line3;
                moduleList[6].NodePortList[13].ValueString = _lastFlightData.PFL.line4;
                moduleList[6].NodePortList[14].ValueString = _lastFlightData.PFL.line5;
                moduleList[6].NodePortList[15].ValueString = _lastFlightData.PFLInverse.line1;
                moduleList[6].NodePortList[16].ValueString = _lastFlightData.PFLInverse.line2;
                moduleList[6].NodePortList[17].ValueString = _lastFlightData.PFLInverse.line3;
                moduleList[6].NodePortList[18].ValueString = _lastFlightData.PFLInverse.line4;
                moduleList[6].NodePortList[19].ValueString = _lastFlightData.PFLInverse.line5;
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
