using ControllorPlugin;
using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Collections.Generic;
using System.Net;

namespace ETS2
{
    public class ETS2 : InterfacePlugin
    {
        List<Node> moduleList;
        //===========================================
        public string PluginID
        {
            get
            {
                return "CB290B27-3C13-4CEF-B29D-E4D744E5B234";
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

        private SCSSdkTelemetry Telemetry;

        public void DefWndProc(int message)
        {
        }

        public List<Node> GetModuleList()
        {
            return moduleList;
        }

        public string GetName()
        {
            return "Euro Truck Simulator 2";
        }

        public void Init()
        {
            moduleList = new List<Node>();
            //----------------------------------------------------------------------------
            List<NodePort> pinList = new List<NodePort>();//1-2
            pinList.Add(new NodePort("SdkActive", PortType.Out, 0));
            pinList.Add(new NodePort("Paused", PortType.Out, 0));
            pinList.Add(new NodePort("Timestamp", PortType.Out, 0));
            pinList.Add(new NodePort("SimulationTimestamp", PortType.Out, 0));
            pinList.Add(new NodePort("RenderTimestamp", PortType.Out, 0));

            pinList.Add(new NodePort("DllVersion", PortType.Out, 0));
            pinList.Add(new NodePort("GameVersion", PortType.Out, ""));
            pinList.Add(new NodePort("Game", PortType.Out, ""));
            pinList.Add(new NodePort("TelemetryVersion", PortType.Out, ""));

            pinList.Add(new NodePort("GameTime", PortType.Out, 0));
            pinList.Add(new NodePort("ForwardGearCount", PortType.Out, 0));
            pinList.Add(new NodePort("ReverseGearCount", PortType.Out, 0));
            pinList.Add(new NodePort("RetarderStepCount", PortType.Out, 0));
            pinList.Add(new NodePort("WheelsCount", PortType.Out, 0));
            pinList.Add(new NodePort("MotorSelectorCount", PortType.Out, 0));
            //--
            pinList.Add(new NodePort("JobDeliveryTime", PortType.Out, 0));
            pinList.Add(new NodePort("JobRemainingDeliveryTime", PortType.Out, 0));
            //--
            pinList.Add(new NodePort("MaxTrailerCount", PortType.Out, 0));
            pinList.Add(new NodePort("CargoUnitCount", PortType.Out, 0));
            pinList.Add(new NodePort("PlannedDistanceKm", PortType.Out, 0));

            pinList.Add(new NodePort("GearHShifterSlot", PortType.Out, 0));
            pinList.Add(new NodePort("BrakeRetarderLevel", PortType.Out, 0));
            pinList.Add(new NodePort("LightsAuxFront", PortType.Out, ""));
            pinList.Add(new NodePort("LightsAuxRoof", PortType.Out, ""));
            pinList.Add(new NodePort("WheelsSubstance", PortType.Out, ""));

            pinList.Add(new NodePort("MotorSlotHandlePosition", PortType.Out, ""));
            pinList.Add(new NodePort("MotorSlotSelectors", PortType.Out, ""));

            pinList.Add(new NodePort("JobDeliveredDeliveryTime", PortType.Out, 0));
            pinList.Add(new NodePort("JobDeliveredStarted", PortType.Out, 0));
            pinList.Add(new NodePort("JobDeliveredFinished", PortType.Out, 0));

            Node test1 = new Node("ETS2 Data A", pinList);
            test1.Info = "This is ETS2 Data A";
            moduleList.Add(test1);
            //----------------------------------------------------------------------------
            List<NodePort> pinList1 = new List<NodePort>();//3
            pinList1.Add(new NodePort("NextRestStop", PortType.Out, 0));

            pinList1.Add(new NodePort("GearSelected", PortType.Out, 0));
            pinList1.Add(new NodePort("DashboardGearDashboards", PortType.Out, 0));
            pinList1.Add(new NodePort("MotorSlotGear", PortType.Out, ""));

            pinList1.Add(new NodePort("JobDeliveredEarnedXp", PortType.Out, 0));

            pinList1.Add(new NodePort("TrailerAttached0", PortType.Out, 0));
            pinList1.Add(new NodePort("TrailerAttached1", PortType.Out, 0));
            pinList1.Add(new NodePort("TrailerAttached2", PortType.Out, 0));
            pinList1.Add(new NodePort("TrailerAttached3", PortType.Out, 0));
            pinList1.Add(new NodePort("TrailerAttached4", PortType.Out, 0));
            pinList1.Add(new NodePort("TrailerAttached5", PortType.Out, 0));
            pinList1.Add(new NodePort("TrailerAttached6", PortType.Out, 0));
            pinList1.Add(new NodePort("TrailerAttached7", PortType.Out, 0));
            pinList1.Add(new NodePort("TrailerAttached8", PortType.Out, 0));
            pinList1.Add(new NodePort("TrailerAttached9", PortType.Out, 0));

            Node test2 = new Node("ETS2 Data B", pinList1);
            test2.Info = "This is ETS2 Data B";
            moduleList.Add(test2);
            //----------------------------------------------------------------------------
            List<NodePort> pinList2 = new List<NodePort>();//4
            pinList2.Add(new NodePort("Scale", PortType.Out, 0f));

            pinList2.Add(new NodePort("CapacityFuel", PortType.Out, 0f));
            pinList2.Add(new NodePort("WarningFactorFuel", PortType.Out, 0f));
            pinList2.Add(new NodePort("CapacityAdBlue", PortType.Out, 0f));
            pinList2.Add(new NodePort("WarningFactorAdBlue", PortType.Out, 0f));
            pinList2.Add(new NodePort("WarningFactorAirPressure", PortType.Out, 0f));
            pinList2.Add(new NodePort("WarningFactorAirPressureEmergency", PortType.Out, 0f));
            pinList2.Add(new NodePort("WarningFactorOilPressure", PortType.Out, 0f));
            pinList2.Add(new NodePort("WarningFactorWaterTemperature", PortType.Out, 0f));
            pinList2.Add(new NodePort("WarningFactorBatteryVoltage", PortType.Out, 0f));
            pinList2.Add(new NodePort("MotorEngineRpmMax", PortType.Out, 0f));
            pinList2.Add(new NodePort("MotorDifferentialRation", PortType.Out, 0f));
            pinList2.Add(new NodePort("JobCargoMass", PortType.Out, 0f));
            pinList2.Add(new NodePort("WheelsRadius", PortType.Out, ""));
            pinList2.Add(new NodePort("MotorGearRatiosForward", PortType.Out, ""));
            pinList2.Add(new NodePort("MotorGearRatiosReverse", PortType.Out, ""));
            pinList2.Add(new NodePort("JobCargoUnitMass", PortType.Out, 0f));

            pinList2.Add(new NodePort("DashboardSpeed", PortType.Out, 0f));
            pinList2.Add(new NodePort("DashboardRPM", PortType.Out, 0f));
            pinList2.Add(new NodePort("InputSteering", PortType.Out, 0f));
            pinList2.Add(new NodePort("InputThrottle", PortType.Out, 0f));
            pinList2.Add(new NodePort("InputBrake", PortType.Out, 0f));
            pinList2.Add(new NodePort("InputClutch", PortType.Out, 0f));
            pinList2.Add(new NodePort("GameSteering", PortType.Out, 0f));
            pinList2.Add(new NodePort("GameThrottle", PortType.Out, 0f));
            pinList2.Add(new NodePort("GameBrake", PortType.Out, 0f));
            pinList2.Add(new NodePort("GameClutch", PortType.Out, 0f));
            pinList2.Add(new NodePort("DashboardCruiseControlSpeed", PortType.Out, 0f));
            pinList2.Add(new NodePort("MotorBrakeAirPressure", PortType.Out, 0f));
            pinList2.Add(new NodePort("MotorBrakeTemperature", PortType.Out, 0f));
            pinList2.Add(new NodePort("DashboardFuelAmount", PortType.Out, 0f));
            pinList2.Add(new NodePort("DashboardFuelAverageConsumption", PortType.Out, 0f));
            pinList2.Add(new NodePort("DashboardFuelRange", PortType.Out, 0f));
            pinList2.Add(new NodePort("DashboardAdBlue", PortType.Out, 0f));
            pinList2.Add(new NodePort("DashboardOilPressure", PortType.Out, 0f));
            pinList2.Add(new NodePort("DashboardOilTemperature", PortType.Out, 0f));
            pinList2.Add(new NodePort("DashboardWaterTemperature", PortType.Out, 0f));
            pinList2.Add(new NodePort("DashboardBatteryVoltage", PortType.Out, 0f));
            pinList2.Add(new NodePort("LightsDashboardBacklight", PortType.Out, 0f));
            pinList2.Add(new NodePort("DamageEngine", PortType.Out, 0f));
            pinList2.Add(new NodePort("DamageTransmission", PortType.Out, 0f));
            pinList2.Add(new NodePort("DamageCabin", PortType.Out, 0f));
            pinList2.Add(new NodePort("DamageChassis", PortType.Out, 0f));
            pinList2.Add(new NodePort("DamageWheelsAvg", PortType.Out, 0f));

            pinList2.Add(new NodePort("DashboardOdometer", PortType.Out, 0f));
            pinList2.Add(new NodePort("NavigationDistance", PortType.Out, 0f));
            pinList2.Add(new NodePort("NavigationTime", PortType.Out, 0f));
            pinList2.Add(new NodePort("NavigationSpeedLimit", PortType.Out, 0f));
            pinList2.Add(new NodePort("WheelsSuspDeflection", PortType.Out, ""));
            pinList2.Add(new NodePort("WheelsVelocity", PortType.Out, ""));
            pinList2.Add(new NodePort("WheelsSteering", PortType.Out, ""));
            pinList2.Add(new NodePort("WheelsRotation", PortType.Out, ""));
            pinList2.Add(new NodePort("WheelsLift", PortType.Out, ""));
            pinList2.Add(new NodePort("WheelsLiftOffset", PortType.Out, ""));

            pinList2.Add(new NodePort("JobDeliveredCargoDamage", PortType.Out, 0f));
            pinList2.Add(new NodePort("JobDeliveredDistanceKm", PortType.Out, 0f));
            pinList2.Add(new NodePort("RefuelEventAmount", PortType.Out, 0f));
            pinList2.Add(new NodePort("JobCargoCargoDamage", PortType.Out, 0f));

            Node test3 = new Node("ETS2 Data C", pinList2);
            test3.Info = "This is ETS2 Data C";
            moduleList.Add(test3);
            //----------------------------------------------------------------------------
            List<NodePort> pinList2B = new List<NodePort>();//5
            pinList2B.Add(new NodePort("WheelsSteerable", PortType.Out, ""));
            pinList2B.Add(new NodePort("WheelsSimulated", PortType.Out, ""));
            pinList2B.Add(new NodePort("WheelsPowered", PortType.Out, ""));
            pinList2B.Add(new NodePort("WheelsLiftable", PortType.Out, ""));

            pinList2B.Add(new NodePort("JobCargoLoaded", PortType.Out, 0));
            pinList2B.Add(new NodePort("JobSpecialJob", PortType.Out, 0));

            pinList2B.Add(new NodePort("ParkingBrake", PortType.Out, 0));
            pinList2B.Add(new NodePort("MotorBrake", PortType.Out, 0));
            pinList2B.Add(new NodePort("WarningAirPressure", PortType.Out, 0));
            pinList2B.Add(new NodePort("WarningAirPressureEmergency", PortType.Out, 0));

            pinList2B.Add(new NodePort("WarningFuelW", PortType.Out, 0));
            pinList2B.Add(new NodePort("WarningAdBlue", PortType.Out, 0));
            pinList2B.Add(new NodePort("WarningOilPressure", PortType.Out, 0));
            pinList2B.Add(new NodePort("WarningWaterTemperature", PortType.Out, 0));
            pinList2B.Add(new NodePort("WarningBatteryVoltage", PortType.Out, 0));
            pinList2B.Add(new NodePort("ElectricEnabled", PortType.Out, 0));
            pinList2B.Add(new NodePort("EngineEnabled", PortType.Out, 0));
            pinList2B.Add(new NodePort("DashboardWipers", PortType.Out, 0));
            pinList2B.Add(new NodePort("LightsBlinkerLeftActive", PortType.Out, 0));
            pinList2B.Add(new NodePort("LightsBlinkerRightActive", PortType.Out, 0));
            pinList2B.Add(new NodePort("LightsBlinkerLeftOn", PortType.Out, 0));
            pinList2B.Add(new NodePort("LightsBlinkerRightOn", PortType.Out, 0));
            pinList2B.Add(new NodePort("LightsParking", PortType.Out, 0));
            pinList2B.Add(new NodePort("LightsBeamLow", PortType.Out, 0));
            pinList2B.Add(new NodePort("LightsBeamHigh", PortType.Out, 0));
            pinList2B.Add(new NodePort("LightsBeacon", PortType.Out, 0));
            pinList2B.Add(new NodePort("LightsBrake", PortType.Out, 0));
            pinList2B.Add(new NodePort("LightsReverse", PortType.Out, 0));
            pinList2B.Add(new NodePort("DashboardCruiseControl", PortType.Out, 0));
            pinList2B.Add(new NodePort("WheelsOnGround", PortType.Out, ""));
            pinList2B.Add(new NodePort("GearHShifterSelector", PortType.Out, ""));

            pinList2B.Add(new NodePort("JobDeliveredAutoParked", PortType.Out, 0));
            pinList2B.Add(new NodePort("JobDeliveredAutoLoaded", PortType.Out, 0));

            Node test3B = new Node("ETS2 Data D", pinList2B);
            test3B.Info = "This is ETS2 Data D";
            moduleList.Add(test3B);
            //----------------------------------------------------------------------------
            List<NodePort> pinList3 = new List<NodePort>();//6
            pinList3.Add(new NodePort("PositioningCabin", PortType.Out, ""));
            pinList3.Add(new NodePort("PositioningHead", PortType.Out, ""));
            pinList3.Add(new NodePort("PositioningHook", PortType.Out, ""));

            pinList3.Add(new NodePort("WheelsPosition0", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition1", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition2", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition3", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition4", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition5", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition6", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition7", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition8", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition9", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition10", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition11", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition12", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition13", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition14", PortType.Out, ""));
            pinList3.Add(new NodePort("WheelsPosition15", PortType.Out, ""));

            pinList3.Add(new NodePort("AccelerationLinearVelocity", PortType.Out, ""));
            pinList3.Add(new NodePort("AccelerationAngularVelocity", PortType.Out, ""));
            pinList3.Add(new NodePort("AccelerationLinearAcceleration", PortType.Out, ""));
            pinList3.Add(new NodePort("AccelerationAngularAcceleration", PortType.Out, ""));
            pinList3.Add(new NodePort("AccelerationCabinAngularVelocity", PortType.Out, ""));
            pinList3.Add(new NodePort("AccelerationCabinAngularAcceleration", PortType.Out, ""));

            Node test4 = new Node("ETS2 Data E", pinList3);
            test4.Info = "This is ETS2 Data E";
            moduleList.Add(test4);
            //----------------------------------------------------------------------------
            List<NodePort> pinList4 = new List<NodePort>();//7-9
            pinList4.Add(new NodePort("PositioningCabinOffsetPosition", PortType.Out, ""));
            pinList4.Add(new NodePort("PositioningCabinOffsetOrientation", PortType.Out, ""));
            //--
            pinList4.Add(new NodePort("PositioningHeadOffsetPosition", PortType.Out, ""));
            pinList4.Add(new NodePort("PositioningHeadOffsetOrientation", PortType.Out, ""));
            //--
            pinList4.Add(new NodePort("PositioningTruckPosition", PortType.Out, ""));
            pinList4.Add(new NodePort("PositioningTruckOrientation", PortType.Out, ""));
            //--

            pinList4.Add(new NodePort("ConstantsBrandId", PortType.Out, ""));
            pinList4.Add(new NodePort("ConstantsBrand", PortType.Out, ""));
            pinList4.Add(new NodePort("ConstantsId", PortType.Out, ""));
            pinList4.Add(new NodePort("ConstantsName", PortType.Out, ""));
            pinList4.Add(new NodePort("CargoId", PortType.Out, ""));
            pinList4.Add(new NodePort("CargoName", PortType.Out, ""));
            pinList4.Add(new NodePort("CityDestinationId", PortType.Out, ""));
            pinList4.Add(new NodePort("CityDestination", PortType.Out, ""));
            pinList4.Add(new NodePort("CompanyDestinationId", PortType.Out, ""));
            pinList4.Add(new NodePort("CompanyDestination", PortType.Out, ""));
            pinList4.Add(new NodePort("CitySourceId", PortType.Out, ""));
            pinList4.Add(new NodePort("CitySource", PortType.Out, ""));
            pinList4.Add(new NodePort("CompanySourceId", PortType.Out, ""));
            pinList4.Add(new NodePort("CompanySource", PortType.Out, ""));
            pinList4.Add(new NodePort("MotorShifterType", PortType.Out, ""));

            pinList4.Add(new NodePort("ConstantsLicensePlate", PortType.Out, ""));
            pinList4.Add(new NodePort("ConstantsLicensePlateCountryId", PortType.Out, ""));
            pinList4.Add(new NodePort("ConstantsLicensePlateCountry", PortType.Out, ""));

            pinList4.Add(new NodePort("JobMarket", PortType.Out, ""));

            pinList4.Add(new NodePort("FinedEventOffence", PortType.Out, ""));

            pinList4.Add(new NodePort("FerryEventSourceName", PortType.Out, ""));
            pinList4.Add(new NodePort("FerryEventTargetName", PortType.Out, ""));
            pinList4.Add(new NodePort("FerryEventSourceId", PortType.Out, ""));
            pinList4.Add(new NodePort("FerryEventTargetId", PortType.Out, ""));
            pinList4.Add(new NodePort("TrainEventSourceName", PortType.Out, ""));
            pinList4.Add(new NodePort("TrainEventTargetName", PortType.Out, ""));
            pinList4.Add(new NodePort("TrainEventSourceId", PortType.Out, ""));
            pinList4.Add(new NodePort("TrainEventTargetId", PortType.Out, ""));

            Node test5 = new Node("ETS2 Data F", pinList4);
            test5.Info = "This is ETS2 Data F";
            moduleList.Add(test5);
            //----------------------------------------------------------------------------
            List<NodePort> pinList5 = new List<NodePort>();//10-12
            pinList5.Add(new NodePort("JobIncome", PortType.Out, 0));

            pinList5.Add(new NodePort("JobCancelledPenalty", PortType.Out, 0));
            pinList5.Add(new NodePort("JobDeliveredRevenue", PortType.Out, 0));
            pinList5.Add(new NodePort("FinedEventAmount", PortType.Out, 0));
            pinList5.Add(new NodePort("TollgateEventPayAmount", PortType.Out, 0));
            pinList5.Add(new NodePort("FerryEventPayAmount", PortType.Out, 0));
            pinList5.Add(new NodePort("TrainEventPayAmount", PortType.Out, 0));

            pinList5.Add(new NodePort("SpecialEventsOnJob", PortType.Out, 0));
            pinList5.Add(new NodePort("SpecialEventsJobFinished", PortType.Out, 0));

            pinList5.Add(new NodePort("SpecialEventsJobCancelled", PortType.Out, 0));
            pinList5.Add(new NodePort("SpecialEventsJobDelivered", PortType.Out, 0));
            pinList5.Add(new NodePort("SpecialEventsFined", PortType.Out, 0));
            pinList5.Add(new NodePort("SpecialEventsTollgate", PortType.Out, 0));
            pinList5.Add(new NodePort("SpecialEventsFerry", PortType.Out, 0));
            pinList5.Add(new NodePort("SpecialEventsTrain", PortType.Out, 0));

            pinList5.Add(new NodePort("SpecialEventsRefuel", PortType.Out, 0));
            pinList5.Add(new NodePort("SpecialEventsRefuelPayed", PortType.Out, 0));

            Node test6 = new Node("ETS2 Data G", pinList5);
            test6.Info = "This is ETS2 Data G";
            moduleList.Add(test6);
            //Telemetry--------------------------------------------------------------------------------------------
            Telemetry = new SCSSdkTelemetry();
        }
        public void AutoOpen()
        {

        }

        public void Update()
        {
            try
            {
                if (Telemetry.LinkReady)
                {
                    SCSTelemetry ets2Data = Telemetry.Update();
                    //------------------------------------------------------------------------------------------------------
                    moduleList[0].NodePortList[0].ValueInt64 = ets2Data.SdkActive ? 1 : 0;

                    moduleList[0].NodePortList[1].ValueInt64 = ets2Data.Paused ? 1 : 0;
                    moduleList[0].NodePortList[2].ValueInt64 = (long)ets2Data.Timestamp;
                    moduleList[0].NodePortList[3].ValueInt64 = (long)ets2Data.SimulationTimestamp;
                    moduleList[0].NodePortList[4].ValueInt64 = (long)ets2Data.RenderTimestamp;

                    moduleList[0].NodePortList[5].ValueInt64 = ets2Data.DllVersion;
                    moduleList[0].NodePortList[6].ValueString = ets2Data.GameVersion.Major + "." + ets2Data.GameVersion.Minor;
                    moduleList[0].NodePortList[7].ValueString = ets2Data.Game.ToString();
                    moduleList[0].NodePortList[8].ValueString = ets2Data.TelemetryVersion.Major + "." + ets2Data.TelemetryVersion.Minor;

                    moduleList[0].NodePortList[9].ValueInt64 = ets2Data.CommonValues.GameTime.Value;

                    moduleList[0].NodePortList[10].ValueInt64 = ets2Data.TruckValues.ConstantsValues.MotorValues.ForwardGearCount;
                    moduleList[0].NodePortList[11].ValueInt64 = ets2Data.TruckValues.ConstantsValues.MotorValues.ReverseGearCount;
                    moduleList[0].NodePortList[12].ValueInt64 = ets2Data.TruckValues.ConstantsValues.MotorValues.RetarderStepCount;

                    moduleList[0].NodePortList[13].ValueInt64 = ets2Data.TruckValues.ConstantsValues.WheelsValues.Count;
                    moduleList[0].NodePortList[14].ValueInt64 = ets2Data.TruckValues.ConstantsValues.MotorValues.SelectorCount;

                    moduleList[0].NodePortList[15].ValueInt64 = ets2Data.JobValues.DeliveryTime.Value;
                    moduleList[0].NodePortList[16].ValueInt64 = ets2Data.JobValues.RemainingDeliveryTime.Value;
                    moduleList[0].NodePortList[17].ValueInt64 = ets2Data.MaxTrailerCount;
                    moduleList[0].NodePortList[18].ValueInt64 = ets2Data.JobValues.CargoValues.UnitCount;
                    moduleList[0].NodePortList[19].ValueInt64 = ets2Data.JobValues.PlannedDistanceKm;

                    moduleList[0].NodePortList[20].ValueInt64 = ets2Data.TruckValues.CurrentValues.MotorValues.GearValues.HShifterSlot;
                    moduleList[0].NodePortList[21].ValueInt64 = ets2Data.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;
                    moduleList[0].NodePortList[22].ValueString = ets2Data.TruckValues.CurrentValues.LightsValues.AuxFront.ToString();
                    moduleList[0].NodePortList[23].ValueString = ets2Data.TruckValues.CurrentValues.LightsValues.AuxRoof.ToString();
                    moduleList[0].NodePortList[24].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.CurrentValues.WheelsValues.Substance.Length; i++)
                    {
                        moduleList[0].NodePortList[24].ValueString += ets2Data.TruckValues.CurrentValues.WheelsValues.Substance[i] + ",";
                    }
                    moduleList[0].NodePortList[25].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.ConstantsValues.MotorValues.SlotHandlePosition.Length; i++)
                    {
                        moduleList[0].NodePortList[25].ValueString += ets2Data.TruckValues.ConstantsValues.MotorValues.SlotHandlePosition[i] + ",";
                    }
                    moduleList[0].NodePortList[26].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.ConstantsValues.MotorValues.SlotSelectors.Length; i++)
                    {
                        moduleList[0].NodePortList[26].ValueString += ets2Data.TruckValues.ConstantsValues.MotorValues.SlotSelectors[i] + ",";
                    }
                    moduleList[0].NodePortList[27].ValueInt64 = ets2Data.GamePlay.JobDelivered.DeliveryTime.Value;
                    moduleList[0].NodePortList[28].ValueInt64 = ets2Data.GamePlay.JobCancelled.Started.Value;
                    moduleList[0].NodePortList[29].ValueInt64 = ets2Data.GamePlay.JobCancelled.Finished.Value;
                    //------------------------------------------------------------------------------------------------------
                    moduleList[1].NodePortList[0].ValueInt64 = ets2Data.CommonValues.NextRestStop.Value;
                    moduleList[1].NodePortList[1].ValueInt64 = ets2Data.TruckValues.CurrentValues.MotorValues.GearValues.Selected;
                    moduleList[1].NodePortList[2].ValueInt64 = ets2Data.TruckValues.CurrentValues.DashboardValues.GearDashboards;
                    moduleList[1].NodePortList[3].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.ConstantsValues.MotorValues.SlotGear.Length; i++)
                    {
                        moduleList[1].NodePortList[3].ValueString += ets2Data.TruckValues.ConstantsValues.MotorValues.SlotGear[i] + ",";
                    }

                    moduleList[1].NodePortList[4].ValueInt64 = ets2Data.GamePlay.JobDelivered.EarnedXp;
                    moduleList[1].NodePortList[5].ValueInt64 = ets2Data.TrailerValues[0].Attached ? 1 : 0;
                    moduleList[1].NodePortList[6].ValueInt64 = ets2Data.TrailerValues[1].Attached ? 1 : 0;
                    moduleList[1].NodePortList[7].ValueInt64 = ets2Data.TrailerValues[2].Attached ? 1 : 0;
                    moduleList[1].NodePortList[8].ValueInt64 = ets2Data.TrailerValues[3].Attached ? 1 : 0;
                    moduleList[1].NodePortList[9].ValueInt64 = ets2Data.TrailerValues[4].Attached ? 1 : 0;
                    moduleList[1].NodePortList[10].ValueInt64 = ets2Data.TrailerValues[5].Attached ? 1 : 0;
                    moduleList[1].NodePortList[11].ValueInt64 = ets2Data.TrailerValues[6].Attached ? 1 : 0;
                    moduleList[1].NodePortList[12].ValueInt64 = ets2Data.TrailerValues[7].Attached ? 1 : 0;
                    moduleList[1].NodePortList[13].ValueInt64 = ets2Data.TrailerValues[8].Attached ? 1 : 0;
                    moduleList[1].NodePortList[14].ValueInt64 = ets2Data.TrailerValues[9].Attached ? 1 : 0;
                    //------------------------------------------------------------------------------------------------------
                    moduleList[2].NodePortList[0].ValueDouble = ets2Data.CommonValues.Scale;

                    moduleList[2].NodePortList[1].ValueDouble = ets2Data.TruckValues.ConstantsValues.CapacityValues.Fuel;
                    moduleList[2].NodePortList[2].ValueDouble = ets2Data.TruckValues.ConstantsValues.WarningFactorValues.Fuel;
                    moduleList[2].NodePortList[3].ValueDouble = ets2Data.TruckValues.ConstantsValues.CapacityValues.AdBlue;
                    moduleList[2].NodePortList[4].ValueDouble = ets2Data.TruckValues.ConstantsValues.WarningFactorValues.AdBlue;
                    moduleList[2].NodePortList[5].ValueDouble = ets2Data.TruckValues.ConstantsValues.WarningFactorValues.AirPressure;
                    moduleList[2].NodePortList[6].ValueDouble = ets2Data.TruckValues.ConstantsValues.WarningFactorValues.AirPressureEmergency;
                    moduleList[2].NodePortList[7].ValueDouble = ets2Data.TruckValues.ConstantsValues.WarningFactorValues.OilPressure;
                    moduleList[2].NodePortList[8].ValueDouble = ets2Data.TruckValues.ConstantsValues.WarningFactorValues.WaterTemperature;
                    moduleList[2].NodePortList[9].ValueDouble = ets2Data.TruckValues.ConstantsValues.WarningFactorValues.BatteryVoltage;
                    moduleList[2].NodePortList[10].ValueDouble = ets2Data.TruckValues.ConstantsValues.MotorValues.EngineRpmMax;
                    moduleList[2].NodePortList[11].ValueDouble = ets2Data.TruckValues.ConstantsValues.MotorValues.DifferentialRation;
                    moduleList[2].NodePortList[12].ValueDouble = ets2Data.JobValues.CargoValues.Mass;
                    moduleList[2].NodePortList[13].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.ConstantsValues.WheelsValues.Radius.Length; i++)
                    {
                        moduleList[2].NodePortList[13].ValueString += ets2Data.TruckValues.ConstantsValues.WheelsValues.Radius[i].ToString("f4") + ",";
                    }
                    moduleList[2].NodePortList[14].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.ConstantsValues.MotorValues.GearRatiosForward.Length; i++)
                    {
                        moduleList[2].NodePortList[14].ValueString += ets2Data.TruckValues.ConstantsValues.MotorValues.GearRatiosForward[i].ToString("f4") + ",";
                    }
                    moduleList[2].NodePortList[15].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.ConstantsValues.MotorValues.GearRatiosReverse.Length; i++)
                    {
                        moduleList[2].NodePortList[15].ValueString += ets2Data.TruckValues.ConstantsValues.MotorValues.GearRatiosReverse[i].ToString("f4") + ",";
                    }
                    moduleList[2].NodePortList[16].ValueDouble = ets2Data.JobValues.CargoValues.UnitMass;

                    moduleList[2].NodePortList[17].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.Speed.Value;
                    moduleList[2].NodePortList[18].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.RPM;
                    moduleList[2].NodePortList[19].ValueDouble = ets2Data.ControlValues.InputValues.Steering;
                    moduleList[2].NodePortList[20].ValueDouble = ets2Data.ControlValues.InputValues.Throttle;
                    moduleList[2].NodePortList[21].ValueDouble = ets2Data.ControlValues.InputValues.Brake;
                    moduleList[2].NodePortList[22].ValueDouble = ets2Data.ControlValues.InputValues.Clutch;
                    moduleList[2].NodePortList[23].ValueDouble = ets2Data.ControlValues.GameValues.Steering;
                    moduleList[2].NodePortList[24].ValueDouble = ets2Data.ControlValues.GameValues.Throttle;
                    moduleList[2].NodePortList[25].ValueDouble = ets2Data.ControlValues.GameValues.Brake;
                    moduleList[2].NodePortList[26].ValueDouble = ets2Data.ControlValues.GameValues.Clutch;
                    moduleList[2].NodePortList[27].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.CruiseControlSpeed.Value;
                    moduleList[2].NodePortList[28].ValueDouble = ets2Data.TruckValues.CurrentValues.MotorValues.BrakeValues.AirPressure;
                    moduleList[2].NodePortList[29].ValueDouble = ets2Data.TruckValues.CurrentValues.MotorValues.BrakeValues.Temperature;
                    moduleList[2].NodePortList[30].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
                    moduleList[2].NodePortList[31].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.FuelValue.AverageConsumption;
                    moduleList[2].NodePortList[32].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range;
                    moduleList[2].NodePortList[33].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.AdBlue;
                    moduleList[2].NodePortList[34].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.OilPressure;
                    moduleList[2].NodePortList[35].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.OilTemperature;
                    moduleList[2].NodePortList[36].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.WaterTemperature;
                    moduleList[2].NodePortList[37].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.BatteryVoltage;
                    moduleList[2].NodePortList[38].ValueDouble = ets2Data.TruckValues.CurrentValues.LightsValues.DashboardBacklight;
                    moduleList[2].NodePortList[39].ValueDouble = ets2Data.TruckValues.CurrentValues.DamageValues.Engine;
                    moduleList[2].NodePortList[40].ValueDouble = ets2Data.TruckValues.CurrentValues.DamageValues.Transmission;
                    moduleList[2].NodePortList[41].ValueDouble = ets2Data.TruckValues.CurrentValues.DamageValues.Cabin;
                    moduleList[2].NodePortList[42].ValueDouble = ets2Data.TruckValues.CurrentValues.DamageValues.Chassis;
                    moduleList[2].NodePortList[43].ValueDouble = ets2Data.TruckValues.CurrentValues.DamageValues.WheelsAvg;

                    moduleList[2].NodePortList[44].ValueDouble = ets2Data.TruckValues.CurrentValues.DashboardValues.Odometer;
                    moduleList[2].NodePortList[45].ValueDouble = ets2Data.NavigationValues.NavigationDistance;
                    moduleList[2].NodePortList[46].ValueDouble = ets2Data.NavigationValues.NavigationTime;
                    moduleList[2].NodePortList[47].ValueDouble = ets2Data.NavigationValues.SpeedLimit.Value;
                    moduleList[2].NodePortList[48].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.CurrentValues.WheelsValues.SuspDeflection.Length; i++)
                    {
                        moduleList[2].NodePortList[48].ValueString += ets2Data.TruckValues.CurrentValues.WheelsValues.SuspDeflection[i].ToString("f4") + ",";
                    }
                    moduleList[2].NodePortList[49].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.CurrentValues.WheelsValues.Velocity.Length; i++)
                    {
                        moduleList[2].NodePortList[49].ValueString += ets2Data.TruckValues.CurrentValues.WheelsValues.Velocity[i].ToString("f4") + ",";
                    }
                    moduleList[2].NodePortList[50].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.CurrentValues.WheelsValues.Steering.Length; i++)
                    {
                        moduleList[2].NodePortList[50].ValueString += ets2Data.TruckValues.CurrentValues.WheelsValues.Steering[i].ToString("f4") + ",";
                    }
                    moduleList[2].NodePortList[51].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.CurrentValues.WheelsValues.Rotation.Length; i++)
                    {
                        moduleList[2].NodePortList[51].ValueString += ets2Data.TruckValues.CurrentValues.WheelsValues.Rotation[i].ToString("f4") + ",";
                    }
                    moduleList[2].NodePortList[52].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.CurrentValues.WheelsValues.Lift.Length; i++)
                    {
                        moduleList[2].NodePortList[52].ValueString += ets2Data.TruckValues.CurrentValues.WheelsValues.Lift[i].ToString("f4") + ",";
                    }
                    moduleList[2].NodePortList[53].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.CurrentValues.WheelsValues.LiftOffset.Length; i++)
                    {
                        moduleList[2].NodePortList[53].ValueString += ets2Data.TruckValues.CurrentValues.WheelsValues.LiftOffset[i].ToString("f4") + ",";
                    }

                    moduleList[2].NodePortList[54].ValueDouble = ets2Data.GamePlay.JobDelivered.CargoDamage;
                    moduleList[2].NodePortList[55].ValueDouble = ets2Data.GamePlay.JobDelivered.DistanceKm;
                    moduleList[2].NodePortList[56].ValueDouble = ets2Data.GamePlay.RefuelEvent.Amount;
                    moduleList[2].NodePortList[57].ValueDouble = ets2Data.JobValues.CargoValues.CargoDamage;
                    //------------------------------------------------------------------------------------------------------
                    moduleList[3].NodePortList[0].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.ConstantsValues.WheelsValues.Steerable.Length; i++)
                    {
                        moduleList[3].NodePortList[0].ValueString += ets2Data.TruckValues.ConstantsValues.WheelsValues.Steerable[i] + ",";
                    }
                    moduleList[3].NodePortList[1].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.ConstantsValues.WheelsValues.Simulated.Length; i++)
                    {
                        moduleList[3].NodePortList[1].ValueString += ets2Data.TruckValues.ConstantsValues.WheelsValues.Simulated[i] + ",";
                    }
                    moduleList[3].NodePortList[2].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.ConstantsValues.WheelsValues.Powered.Length; i++)
                    {
                        moduleList[3].NodePortList[2].ValueString += ets2Data.TruckValues.ConstantsValues.WheelsValues.Powered[i] + ",";
                    }
                    moduleList[3].NodePortList[3].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.ConstantsValues.WheelsValues.Liftable.Length; i++)
                    {
                        moduleList[3].NodePortList[3].ValueString += ets2Data.TruckValues.ConstantsValues.WheelsValues.Liftable[i] + ",";
                    }

                    moduleList[3].NodePortList[4].ValueInt64 = ets2Data.JobValues.CargoLoaded ? 1 : 0;
                    moduleList[3].NodePortList[5].ValueInt64 = ets2Data.JobValues.SpecialJob ? 1 : 0;

                    moduleList[3].NodePortList[6].ValueInt64 = ets2Data.TruckValues.CurrentValues.MotorValues.BrakeValues.ParkingBrake ? 1 : 0;
                    moduleList[3].NodePortList[7].ValueInt64 = ets2Data.TruckValues.CurrentValues.MotorValues.BrakeValues.MotorBrake ? 1 : 0;
                    moduleList[3].NodePortList[8].ValueInt64 = ets2Data.TruckValues.CurrentValues.DashboardValues.WarningValues.AirPressure ? 1 : 0;
                    moduleList[3].NodePortList[9].ValueInt64 = ets2Data.TruckValues.CurrentValues.DashboardValues.WarningValues.AirPressureEmergency ? 1 : 0;

                    moduleList[3].NodePortList[10].ValueInt64 = ets2Data.TruckValues.CurrentValues.DashboardValues.WarningValues.FuelW ? 1 : 0;
                    moduleList[3].NodePortList[11].ValueInt64 = ets2Data.TruckValues.CurrentValues.DashboardValues.WarningValues.AdBlue ? 1 : 0;
                    moduleList[3].NodePortList[12].ValueInt64 = ets2Data.TruckValues.CurrentValues.DashboardValues.WarningValues.OilPressure ? 1 : 0;
                    moduleList[3].NodePortList[13].ValueInt64 = ets2Data.TruckValues.CurrentValues.DashboardValues.WarningValues.WaterTemperature ? 1 : 0;
                    moduleList[3].NodePortList[14].ValueInt64 = ets2Data.TruckValues.CurrentValues.DashboardValues.WarningValues.BatteryVoltage ? 1 : 0;
                    moduleList[3].NodePortList[15].ValueInt64 = ets2Data.TruckValues.CurrentValues.ElectricEnabled ? 1 : 0;
                    moduleList[3].NodePortList[16].ValueInt64 = ets2Data.TruckValues.CurrentValues.EngineEnabled ? 1 : 0;
                    moduleList[3].NodePortList[17].ValueInt64 = ets2Data.TruckValues.CurrentValues.DashboardValues.Wipers ? 1 : 0;
                    moduleList[3].NodePortList[18].ValueInt64 = ets2Data.TruckValues.CurrentValues.LightsValues.BlinkerLeftActive ? 1 : 0;
                    moduleList[3].NodePortList[19].ValueInt64 = ets2Data.TruckValues.CurrentValues.LightsValues.BlinkerRightActive ? 1 : 0;
                    moduleList[3].NodePortList[20].ValueInt64 = ets2Data.TruckValues.CurrentValues.LightsValues.BlinkerLeftOn ? 1 : 0;
                    moduleList[3].NodePortList[21].ValueInt64 = ets2Data.TruckValues.CurrentValues.LightsValues.BlinkerRightOn ? 1 : 0;
                    moduleList[3].NodePortList[22].ValueInt64 = ets2Data.TruckValues.CurrentValues.LightsValues.Parking ? 1 : 0;
                    moduleList[3].NodePortList[23].ValueInt64 = ets2Data.TruckValues.CurrentValues.LightsValues.BeamLow ? 1 : 0;
                    moduleList[3].NodePortList[24].ValueInt64 = ets2Data.TruckValues.CurrentValues.LightsValues.BeamHigh ? 1 : 0;
                    moduleList[3].NodePortList[25].ValueInt64 = ets2Data.TruckValues.CurrentValues.LightsValues.Beacon ? 1 : 0;
                    moduleList[3].NodePortList[26].ValueInt64 = ets2Data.TruckValues.CurrentValues.LightsValues.Brake ? 1 : 0;
                    moduleList[3].NodePortList[27].ValueInt64 = ets2Data.TruckValues.CurrentValues.LightsValues.Reverse ? 1 : 0;
                    moduleList[3].NodePortList[28].ValueInt64 = ets2Data.TruckValues.CurrentValues.DashboardValues.CruiseControl ? 1 : 0;

                    moduleList[3].NodePortList[29].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.CurrentValues.WheelsValues.OnGround.Length; i++)
                    {
                        moduleList[3].NodePortList[29].ValueString += ets2Data.TruckValues.CurrentValues.WheelsValues.OnGround[i] + ",";
                    }
                    moduleList[3].NodePortList[30].ValueString = "";
                    for (int i = 0; i < ets2Data.TruckValues.CurrentValues.MotorValues.GearValues.HShifterSelector.Length; i++)
                    {
                        moduleList[3].NodePortList[30].ValueString += ets2Data.TruckValues.CurrentValues.MotorValues.GearValues.HShifterSelector[i] + ",";
                    }

                    moduleList[3].NodePortList[31].ValueInt64 = ets2Data.GamePlay.JobDelivered.AutoParked ? 1 : 0;
                    moduleList[3].NodePortList[32].ValueInt64 = ets2Data.GamePlay.JobDelivered.AutoLoaded ? 1 : 0;
                    //------------------------------------------------------------------------------------------------------
                    moduleList[4].NodePortList[0].ValueString = ets2Data.TruckValues.Positioning.Cabin.X.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.Cabin.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.Cabin.Z.ToString("f4");
                    moduleList[4].NodePortList[1].ValueString = ets2Data.TruckValues.Positioning.Head.X.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.Head.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.Head.Z.ToString("f4");
                    moduleList[4].NodePortList[2].ValueString = ets2Data.TruckValues.Positioning.Hook.X.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.Hook.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.Hook.Z.ToString("f4");

                    moduleList[4].NodePortList[3].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[0].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[0].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[0].Z.ToString("f4");
                    moduleList[4].NodePortList[4].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[1].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[1].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[1].Z.ToString("f4");
                    moduleList[4].NodePortList[5].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[2].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[2].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[2].Z.ToString("f4");
                    moduleList[4].NodePortList[6].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[3].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[3].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[3].Z.ToString("f4");
                    moduleList[4].NodePortList[7].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[4].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[4].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[4].Z.ToString("f4");
                    moduleList[4].NodePortList[8].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[5].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[5].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[5].Z.ToString("f4");
                    moduleList[4].NodePortList[9].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[6].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[6].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[6].Z.ToString("f4");
                    moduleList[4].NodePortList[10].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[7].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[7].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[7].Z.ToString("f4");
                    moduleList[4].NodePortList[11].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[8].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[8].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[8].Z.ToString("f4");
                    moduleList[4].NodePortList[12].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[9].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[9].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[9].Z.ToString("f4");
                    moduleList[4].NodePortList[13].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[10].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[10].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[10].Z.ToString("f4");
                    moduleList[4].NodePortList[14].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[11].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[11].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[11].Z.ToString("f4");
                    moduleList[4].NodePortList[15].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[12].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[12].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[12].Z.ToString("f4");
                    moduleList[4].NodePortList[16].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[13].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[13].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[13].Z.ToString("f4");
                    moduleList[4].NodePortList[17].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[14].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[14].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[14].Z.ToString("f4");
                    moduleList[4].NodePortList[18].ValueString = ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[15].X.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[15].Y.ToString("f4") + "," +
                        ets2Data.TruckValues.ConstantsValues.WheelsValues.PositionValues[15].Z.ToString("f4");

                    moduleList[4].NodePortList[19].ValueString = ets2Data.TruckValues.CurrentValues.AccelerationValues.LinearVelocity.X.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.LinearVelocity.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.LinearVelocity.Z.ToString("f4");
                    moduleList[4].NodePortList[20].ValueString = ets2Data.TruckValues.CurrentValues.AccelerationValues.AngularVelocity.X.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.AngularVelocity.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.AngularVelocity.Z.ToString("f4");
                    moduleList[4].NodePortList[21].ValueString = ets2Data.TruckValues.CurrentValues.AccelerationValues.LinearAcceleration.X.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.LinearAcceleration.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.LinearAcceleration.Z.ToString("f4");
                    moduleList[4].NodePortList[22].ValueString = ets2Data.TruckValues.CurrentValues.AccelerationValues.AngularAcceleration.X.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.AngularAcceleration.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.AngularAcceleration.Z.ToString("f4");
                    moduleList[4].NodePortList[23].ValueString = ets2Data.TruckValues.CurrentValues.AccelerationValues.CabinAngularVelocity.X.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.CabinAngularVelocity.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.CabinAngularVelocity.Z.ToString("f4");
                    moduleList[4].NodePortList[24].ValueString = ets2Data.TruckValues.CurrentValues.AccelerationValues.CabinAngularAcceleration.X.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.CabinAngularAcceleration.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.AccelerationValues.CabinAngularAcceleration.Z.ToString("f4");


                    //------------------------------------------------------------------------------------------------------
                    moduleList[5].NodePortList[0].ValueString = ets2Data.TruckValues.Positioning.CabinOffset.Position.X.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.CabinOffset.Position.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.CabinOffset.Position.Z.ToString("f4");
                    moduleList[5].NodePortList[1].ValueString = ets2Data.TruckValues.Positioning.CabinOffset.Orientation.Heading.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.CabinOffset.Orientation.Pitch.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.CabinOffset.Orientation.Roll.ToString("f4");
                    moduleList[5].NodePortList[2].ValueString = ets2Data.TruckValues.Positioning.HeadOffset.Position.X.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.HeadOffset.Position.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.HeadOffset.Position.Z.ToString("f4");
                    moduleList[5].NodePortList[3].ValueString = ets2Data.TruckValues.Positioning.HeadOffset.Orientation.Heading.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.HeadOffset.Orientation.Pitch.ToString("f4") + "," +
                        ets2Data.TruckValues.Positioning.HeadOffset.Orientation.Roll.ToString("f4");
                    moduleList[5].NodePortList[4].ValueString = ets2Data.TruckValues.CurrentValues.PositionValue.Position.X.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.PositionValue.Position.Y.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.PositionValue.Position.Z.ToString("f4");
                    moduleList[5].NodePortList[5].ValueString = ets2Data.TruckValues.CurrentValues.PositionValue.Orientation.Heading.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.PositionValue.Orientation.Pitch.ToString("f4") + "," +
                        ets2Data.TruckValues.CurrentValues.PositionValue.Orientation.Roll.ToString("f4");

                    moduleList[5].NodePortList[6].ValueString = ets2Data.TruckValues.ConstantsValues.BrandId;
                    moduleList[5].NodePortList[7].ValueString = ets2Data.TruckValues.ConstantsValues.Brand;
                    moduleList[5].NodePortList[8].ValueString = ets2Data.TruckValues.ConstantsValues.Id;
                    moduleList[5].NodePortList[9].ValueString = ets2Data.TruckValues.ConstantsValues.Name;
                    moduleList[5].NodePortList[10].ValueString = ets2Data.JobValues.CargoValues.Id;
                    moduleList[5].NodePortList[11].ValueString = ets2Data.JobValues.CargoValues.Name;
                    moduleList[5].NodePortList[12].ValueString = ets2Data.JobValues.CityDestinationId;
                    moduleList[5].NodePortList[13].ValueString = ets2Data.JobValues.CityDestination;
                    moduleList[5].NodePortList[14].ValueString = ets2Data.JobValues.CompanyDestinationId;
                    moduleList[5].NodePortList[15].ValueString = ets2Data.JobValues.CompanyDestination;
                    moduleList[5].NodePortList[16].ValueString = ets2Data.JobValues.CitySourceId;
                    moduleList[5].NodePortList[17].ValueString = ets2Data.JobValues.CitySource;
                    moduleList[5].NodePortList[18].ValueString = ets2Data.JobValues.CompanySourceId;
                    moduleList[5].NodePortList[19].ValueString = ets2Data.JobValues.CompanySource;

                    moduleList[5].NodePortList[20].ValueString = ets2Data.TruckValues.ConstantsValues.MotorValues.ShifterTypeValue.ToString();

                    moduleList[5].NodePortList[21].ValueString = ets2Data.TruckValues.ConstantsValues.LicensePlate;
                    moduleList[5].NodePortList[22].ValueString = ets2Data.TruckValues.ConstantsValues.LicensePlateCountryId;
                    moduleList[5].NodePortList[23].ValueString = ets2Data.TruckValues.ConstantsValues.LicensePlateCountry;

                    moduleList[5].NodePortList[24].ValueString = ets2Data.JobValues.Market.ToString();
                    moduleList[5].NodePortList[25].ValueString = ets2Data.GamePlay.FinedEvent.Offence.ToString();

                    moduleList[5].NodePortList[26].ValueString = ets2Data.GamePlay.FerryEvent.SourceName;
                    moduleList[5].NodePortList[27].ValueString = ets2Data.GamePlay.FerryEvent.TargetName;
                    moduleList[5].NodePortList[28].ValueString = ets2Data.GamePlay.FerryEvent.SourceId;
                    moduleList[5].NodePortList[29].ValueString = ets2Data.GamePlay.FerryEvent.TargetId;
                    moduleList[5].NodePortList[30].ValueString = ets2Data.GamePlay.TrainEvent.SourceName;
                    moduleList[5].NodePortList[31].ValueString = ets2Data.GamePlay.TrainEvent.TargetName;
                    moduleList[5].NodePortList[32].ValueString = ets2Data.GamePlay.TrainEvent.SourceId;
                    moduleList[5].NodePortList[33].ValueString = ets2Data.GamePlay.TrainEvent.TargetId;
                    //------------------------------------------------------------------------------------------------------
                    moduleList[6].NodePortList[0].ValueInt64 = (long)ets2Data.JobValues.Income;

                    moduleList[6].NodePortList[1].ValueInt64 = ets2Data.GamePlay.JobCancelled.Penalty;
                    moduleList[6].NodePortList[2].ValueInt64 = ets2Data.GamePlay.JobDelivered.Revenue;
                    moduleList[6].NodePortList[3].ValueInt64 = ets2Data.GamePlay.FinedEvent.Amount;
                    moduleList[6].NodePortList[4].ValueInt64 = ets2Data.GamePlay.TollgateEvent.PayAmount;
                    moduleList[6].NodePortList[5].ValueInt64 = ets2Data.GamePlay.FerryEvent.PayAmount;
                    moduleList[6].NodePortList[6].ValueInt64 = ets2Data.GamePlay.TrainEvent.PayAmount;

                    moduleList[6].NodePortList[7].ValueInt64 = ets2Data.SpecialEventsValues.OnJob ? 1 : 0;
                    moduleList[6].NodePortList[8].ValueInt64 = ets2Data.SpecialEventsValues.JobFinished ? 1 : 0;

                    moduleList[6].NodePortList[9].ValueInt64 = ets2Data.SpecialEventsValues.JobCancelled ? 1 : 0;
                    moduleList[6].NodePortList[10].ValueInt64 = ets2Data.SpecialEventsValues.JobDelivered ? 1 : 0;
                    moduleList[6].NodePortList[11].ValueInt64 = ets2Data.SpecialEventsValues.Fined ? 1 : 0;
                    moduleList[6].NodePortList[12].ValueInt64 = ets2Data.SpecialEventsValues.Tollgate ? 1 : 0;
                    moduleList[6].NodePortList[13].ValueInt64 = ets2Data.SpecialEventsValues.Ferry ? 1 : 0;
                    moduleList[6].NodePortList[14].ValueInt64 = ets2Data.SpecialEventsValues.Train ? 1 : 0;

                    moduleList[6].NodePortList[15].ValueInt64 = ets2Data.SpecialEventsValues.Refuel ? 1 : 0;
                    moduleList[6].NodePortList[16].ValueInt64 = ets2Data.SpecialEventsValues.RefuelPayed ? 1 : 0;
                    //------------------------------------------------------------------------------------------------------
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
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
    }
}
