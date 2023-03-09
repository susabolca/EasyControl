﻿using System;
using System.Runtime.InteropServices;

namespace BmsSimulatorTelemetry
{
    #region DATA
    [Flags]
    public enum BMSLightBits : uint
    {
        MasterCaution = 0x1,  // Left eyebrow

        // Brow Lights
        TF = 0x2,   // Left eyebrow
        OXY_BROW = 0x4,   //  repurposed for eyebrow OXY LOW (was OBS, unused)
        EQUIP_HOT = 0x8,   // Caution light; repurposed for cooling fault (was: not used)
        WOW = 0x10,  // True if weight is on wheels: this is not a lamp bit!
        ENG_FIRE = 0x20,  // Right eyebrow; upper half of split face lamp
        CONFIG = 0x40,  // Stores config, caution panel
        HYD = 0x80,  // Right eyebrow; see also OIL (this lamp is not split face)
        Flcs_ABCD = 0x100, // TEST panel FLCS channel lamps; repurposed, was OIL (see HYD; that lamp is not split face)
        FLCS = 0x200, // Right eyebrow; was called DUAL which matches block 25, 30/32 and older 40/42
        CAN = 0x400, // Right eyebrow
        T_L_CFG = 0x800, // Right eyebrow

        // AOA Indexers
        AOAAbove = 0x1000,
        AOAOn = 0x2000,
        AOABelow = 0x4000,

        // Refuel/NWS
        RefuelRDY = 0x8000,
        RefuelAR = 0x10000,
        RefuelDSC = 0x20000,

        // Caution Lights
        FltControlSys = 0x40000,
        LEFlaps = 0x80000,
        EngineFault = 0x100000,
        Overheat = 0x200000,
        FuelLow = 0x400000,
        Avionics = 0x800000,
        RadarAlt = 0x1000000,
        IFF = 0x2000000,
        ECM = 0x4000000,
        Hook = 0x8000000,
        NWSFail = 0x10000000,
        CabinPress = 0x20000000,

        AutoPilotOn = 0x40000000,  // TRUE if is AP on.  NB: This is not a lamp bit!
        TFR_STBY = 0x80000000,  // MISC panel; lower half of split face TFR lamp
    }

    [Flags]
    public enum BMSLightBits2 : uint
    {
        // Threat Warning Prime
        HandOff = 0x1,
        Launch = 0x2,
        PriMode = 0x4,
        Naval = 0x8,
        Unk = 0x10,
        TgtSep = 0x20,

        // EWS
        Go = 0x40,		// On and operating normally
        NoGo = 0x80,     // On but malfunction present
        Degr = 0x100,    // Status message: AUTO DEGR
        Rdy = 0x200,    // Status message: DISPENSE RDY
        ChaffLo = 0x400,    // Bingo chaff quantity reached
        FlareLo = 0x800,    // Bingo flare quantity reached

        // Aux Threat Warning
        AuxSrch = 0x1000,
        AuxAct = 0x2000,
        AuxLow = 0x4000,
        AuxPwr = 0x8000,

        // ECM
        EcmPwr = 0x10000,
        EcmFail = 0x20000,

        // Caution Lights
        FwdFuelLow = 0x40000,
        AftFuelLow = 0x80000,

        EPUOn = 0x100000,  // EPU panel; run light
        JFSOn = 0x200000,  // Eng Jet Start panel; run light

        // Caution panel
        SEC = 0x400000,
        OXY_LOW = 0x800000,
        PROBEHEAT = 0x1000000,
        SEAT_ARM = 0x2000000,
        BUC = 0x4000000,
        FUEL_OIL_HOT = 0x8000000,
        ANTI_SKID = 0x10000000,

        TFR_ENGAGED = 0x20000000,  // MISC panel; upper half of split face TFR lamp
        GEARHANDLE = 0x40000000,  // Lamp in gear handle lights on fault or gear in motion
        ENGINE = 0x80000000,  // Lower half of right eyebrow ENG FIRE/ENGINE lamp
    }

    [Flags]
    public enum BMSLightBits3 : uint
    {
        // Elec panel
        FlcsPmg = 0x1,
        MainGen = 0x2,
        StbyGen = 0x4,
        EpuGen = 0x8,
        EpuPmg = 0x10,
        ToFlcs = 0x20,
        FlcsRly = 0x40,
        BatFail = 0x80,

        // EPU panel
        Hydrazine = 0x100,
        Air = 0x200,

        // Caution panel
        Elec_Fault = 0x400,
        Lef_Fault = 0x800,

        OnGround = 0x1000,   // weight-on-wheels
        FlcsBitRun = 0x2000,   // FLT CONTROL panel RUN light (used to be Multi-engine fire light)
        FlcsBitFail = 0x4000,   // FLT CONTROL panel FAIL light (used to be Lock light Cue; non-F-16)
        DbuWarn = 0x8000,   // Right eyebrow DBU ON cell; was Shoot light cue; non-F16
        NoseGearDown = 0x10000,  // Landing gear panel; on means down and locked
        LeftGearDown = 0x20000,  // Landing gear panel; on means down and locked
        RightGearDown = 0x40000,  // Landing gear panel; on means down and locked
        ParkBrakeOn = 0x100000, // Parking brake engaged; NOTE: not a lamp bit
        Power_Off = 0x200000, // Set if there is no electrical power.  NB: not a lamp bit

        // Caution panel
        cadc = 0x400000,

        // Left Aux console
        SpeedBrake = 0x800000,  // True if speed brake is in anything other than stowed position
    }
    [Flags]
    public enum HsiBits : uint
    {
        ToTrue = 0x01,    // HSI_FLAG_TO_TRUE
        IlsWarning = 0x02,    // HSI_FLAG_ILS_WARN
        CourseWarning = 0x04,    // HSI_FLAG_CRS_WARN
        Init = 0x08,    // HSI_FLAG_INIT
        TotalFlags = 0x10,    // HSI_FLAG_TOTAL_FLAGS; never set
        ADI_OFF = 0x20,    // ADI OFF Flag
        ADI_AUX = 0x40,    // ADI AUX Flag
        ADI_GS = 0x80,    // ADI GS FLAG
        ADI_LOC = 0x100,   // ADI LOC FLAG
        HSI_OFF = 0x200,   // HSI OFF Flag
        BUP_ADI_OFF = 0x400,   // Backup ADI Off Flag
        VVI = 0x800,   // VVI OFF Flag
        AOA = 0x1000,  // AOA OFF Flag
        AVTR = 0x2000,  // AVTR Light
        OuterMarker = 0x4000,  // MARKER beacon light for outer marker
        MiddleMarker = 0x8000,  // MARKER beacon light for middle marker
        FromTrue = 0x10000, // HSI_FLAG_TO_TRUE == 2, FROM
    }
    #endregion
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TextLines
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
        public string line1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
        public string line2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
        public string line3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
        public string line4;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
        public string line5;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FlightData
    {
        public float x;            // Ownship North (Ft)
        public float y;            // Ownship East (Ft)
        public float z;            // Ownship Down (Ft)
        public float xDot;         // Ownship North Rate (ft/sec)
        public float yDot;         // Ownship East Rate (ft/sec)
        public float zDot;         // Ownship Down Rate (ft/sec)
        public float alpha;        // Ownship AOA (Degrees)
        public float beta;         // Ownship Beta (Degrees)
        public float gamma;        // Ownship Gamma (Radians)
        public float pitch;        // Ownship Pitch (Radians)
        public float roll;         // Ownship Pitch (Radians)
        public float yaw;          // Ownship Pitch (Radians)
        public float mach;         // Ownship Mach number
        public float kias;         // Ownship Indicated Airspeed (Knots)
        public float vt;           // Ownship True Airspeed (Ft/Sec)
        public float gs;           // Ownship Normal Gs
        public float windOffset;   // Wind delta to FPM (Radians)
        public float nozzlePos;    // Ownship engine nozzle percent open (0-100)
        public float internalFuel; // Ownship internal fuel (Lbs)
        public float externalFuel; // Ownship external fuel (Lbs)
        public float fuelFlow;     // Ownship fuel flow (Lbs/Hour)
        public float rpm;          // Ownship engine rpm (Percent 0-103)
        public float ftit;         // Ownship Forward Turbine Inlet Temp (Degrees C)
        public float gearPos;      // Ownship Gear position 0 = up, 1 = down;
        public float speedBrake;   // Ownship speed brake position 0 = closed, 1 = 60 Degrees open
        public float epuFuel;      // Ownship EPU fuel (Percent 0-100)
        public float oilPressure;  // Ownship Oil Pressure (Percent 0-100)
        public BMSLightBits lightBits;    // Cockpit Indicator Lights, one bit per bulb. See enum

        // These are inputs. Use them carefully
        // NB: these do not work when TrackIR device is enabled
        public float headPitch;    // Head pitch offset from design eye (radians)
        public float headRoll;     // Head roll offset from design eye (radians)
        public float headYaw;      // Head yaw offset from design eye (radians)

        // new lights
        public BMSLightBits2 lightBits2;   // Cockpit Indicator Lights, one bit per bulb. See enum
        public BMSLightBits3 lightBits3;   // Cockpit Indicator Lights, one bit per bulb. See enum

        // chaff/flare
        public float ChaffCount;   // Number of Chaff left
        public float FlareCount;   // Number of Flare left

        // landing gear
        public float NoseGearPos;  // Position of the nose landinggear; caution: full down values defined in dat files
        public float LeftGearPos;  // Position of the left landinggear; caution: full down values defined in dat files
        public float RightGearPos; // Position of the right landinggear; caution: full down values defined in dat files

        // ADI values
        public float AdiIlsHorPos; // Position of horizontal ILS bar
        public float AdiIlsVerPos; // Position of vertical ILS bar

        // HSI states
        public int courseState;    // HSI_STA_CRS_STATE
        public int headingState;   // HSI_STA_HDG_STATE
        public int totalStates;    // HSI_STA_TOTAL_STATES; never set

        // HSI values
        public float courseDeviation;  // HSI_VAL_CRS_DEVIATION
        public float desiredCourse;    // HSI_VAL_DESIRED_CRS *
        public float distanceToBeacon;    // HSI_VAL_DISTANCE_TO_BEACON *
        public float bearingToBeacon;  // HSI_VAL_BEARING_TO_BEACON *
        public float currentHeading;      // HSI_VAL_CURRENT_HEADING *
        public float desiredHeading;   // HSI_VAL_DESIRED_HEADING *
        public float deviationLimit;      // HSI_VAL_DEV_LIMIT
        public float halfDeviationLimit;  // HSI_VAL_HALF_DEV_LIMIT
        public float localizerCourse;     // HSI_VAL_LOCALIZER_CRS
        public float airbaseX;            // HSI_VAL_AIRBASE_X
        public float airbaseY;            // HSI_VAL_AIRBASE_Y
        public float totalValues;         // HSI_VAL_TOTAL_VALUES; never set

        public float TrimPitch;  // Value of trim in pitch axis, -0.5 to +0.5
        public float TrimRoll;   // Value of trim in roll axis, -0.5 to +0.5
        public float TrimYaw;    // Value of trim in yaw axis, -0.5 to +0.5

        // HSI flags
        public HsiBits hsiBits;      // HSI flags

        //DED Lines
        public TextLines DED;
        public TextLines DEDInverse;

        //PFL Lines
        public TextLines PFL;
        public TextLines PFLInverse;

        //TacanChannel
        public int UFCTChan;
        public int AUXTChan;

        // RWR
        public int RwrObjectCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public int[] RWRsymbol;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public float[] bearing;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public uint[] missileActivity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public uint[] missileLaunch;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public uint[] selected;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public float[] lethality;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public uint[] newDetection;

        //fuel values
        public float fwd;
        public float aft;
        public float total;

        public int VersionNum;

        float headX;        // Head X offset from design eye (feet)
        float headY;        // Head Y offset from design eye (feet)
        float headZ;        // Head Z offset from design eye (feet)

        int MainPower;
    }
    #region DATA2
    public enum TacanSources : int
    {
        UFC = 0,
        AUX,
        NUMBER_OF_SOURCES
    };

    [Flags]
    public enum TacanBits : byte
    {
        band = 0x01,   // true in this bit position if band is X
        mode = 0x02    // true in this bit position if domain is air to air
    };
    #endregion
    [StructLayout(LayoutKind.Sequential)]
    public struct FlightData2
    {
        public float nozzlePos2;   // Ownship engine nozzle2 percent open (0-100)
        public float rpm2;         // Ownship engine rpm2 (Percent 0-103)
        public float ftit2;        // Ownship Forward Turbine Inlet Temp2 (Degrees C)
        public float oilPressure2; // Ownship Oil Pressure2 (Percent 0-100)
        public byte navMode;  // current mode selected for HSI/eHSI (added in BMS4)
        public float aauz; // Ownship barometric altitude given by AAU (depends on calibration)

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)TacanSources.NUMBER_OF_SOURCES)]
        public TacanBits[] tacanInfo;      // Tacan band/mode settings for UFC and AUX COMM

    }
    public class SharedMemory
    {
        private bool _disposed;

        private string _sharedMemoryName;
        private int _checkValue;

        private IntPtr _sharedMemoryHandle = IntPtr.Zero;
        private IntPtr _sharedMemoryAddress = IntPtr.Zero;

        public SharedMemory(string sharedMemoryName)
        {
            _sharedMemoryName = sharedMemoryName;
        }

        public int CheckValue
        {
            get
            {
                return _checkValue;
            }
            set
            {
                _checkValue = value;
            }
        }

        public bool IsOpen
        {
            get
            {
                return (_sharedMemoryAddress != IntPtr.Zero);
            }
        }

        public bool IsDataAvailable
        {
            get
            {
                if (!IsOpen)
                {
                    Open();
                }

                if (IsOpen)
                {
                    int value = Marshal.ReadInt32(_sharedMemoryAddress);
                    if (value != CheckValue)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public object MarshalTo(Type type)
        {
            return Marshal.PtrToStructure(_sharedMemoryAddress, type);
        }

        public object MarshalTo(Type type, Int64 offset)
        {
            return Marshal.PtrToStructure(GetPointer(offset), type);
        }

        public IntPtr GetPointer()
        {
            return _sharedMemoryAddress;
        }

        public IntPtr GetPointer(Int64 offset)
        {
            return new IntPtr(_sharedMemoryAddress.ToInt64() + offset);
        }

        public bool Open()
        {
            if (_sharedMemoryAddress != IntPtr.Zero)
            {
                return true;
            }

            if (_sharedMemoryHandle == IntPtr.Zero)
            {
                _sharedMemoryHandle = NativeMethods.OpenFileMapping(NativeMethods.FileMapAccess.FileMapRead, false, _sharedMemoryName);
            }

            if (_sharedMemoryHandle != IntPtr.Zero)
            {
                _sharedMemoryAddress = NativeMethods.MapViewOfFile(_sharedMemoryHandle, NativeMethods.FileMapAccess.FileMapRead, 0, 0, 0);

                if (_sharedMemoryAddress != IntPtr.Zero)
                {
                    return true;
                }
            }

            return false;
        }

        public void Close()
        {
            if (_sharedMemoryAddress != IntPtr.Zero)
            {
                NativeMethods.UnmapViewOfFile(_sharedMemoryAddress);
                _sharedMemoryAddress = IntPtr.Zero;
            }
            if (_sharedMemoryHandle != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(_sharedMemoryHandle);
                _sharedMemoryHandle = IntPtr.Zero;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Close();
                }
            }
            _disposed = true;
        }

        ~SharedMemory()
        {
            Dispose(false);
        }

        #endregion
    }
    class NativeMethods
    {
        private NativeMethods()
        {
        }

        //aaa
        [Flags]
        public enum FileMapAccess : uint
        {
            FileMapCopy = 0x0001,
            FileMapWrite = 0x0002,
            FileMapRead = 0x0004,
            FileMapAllAccess = 0x001f,
            fileMapExecute = 0x0020,
        }
        //aaa
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenFileMapping(
            FileMapAccess dwDesiredAccess,
            bool bInheritHandle,
            string lpName);

        //aaa
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hHandle);

        //aaa
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject,
            FileMapAccess dwDesiredAccess,
            uint dwFileOffsetHigh,
            uint dwFileOffsetLow,
            uint dwNumberOfBytesToMap);

        //aaa
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

    }
}
