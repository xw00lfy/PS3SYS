using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;


namespace PS3System
{
    public class CCAPI
    {
        [DllImport("kernel32.dll")]
        static extern uint GetLastError();
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllName);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll")]
        static extern bool FreeLibrary(IntPtr a);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIConnectConsole_t(string targetIP);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIDisconnectConsole_t();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetConnectionStatus_t(ref int status);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetConsoleInfo_t(int index, IntPtr ptrN, IntPtr ptrI);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetDllVersion_t();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetFirmwareInfo_t(ref int firmware, ref int ccapi, ref int consoleType);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetNumberOfConsoles_t();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetProcessList_t(ref uint numberProcesses, IntPtr processIdPtr);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetMemory_t(uint processID, ulong offset, uint size, byte[] buffOut);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetProcessName_t(uint processID, IntPtr strPtr);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIGetTemperature_t(ref int cell, ref int rsx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIVshNotify_t(int mode, string msgWChar);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIRingBuzzer_t(int type);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPISetBootConsoleIds_t(int idType, int on, byte[] ID);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPISetConsoleIds_t(int idType, byte[] consoleID);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPISetConsoleLed_t(int color, int status);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPISetMemory_t(uint processID, ulong offset, uint size, byte[] buffIn);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CCAPIShutdown_t(int mode);

        private CCAPIConnectConsole_t connectConsole;
        private CCAPIDisconnectConsole_t disconnectConsole;
        private CCAPIGetConnectionStatus_t getConnectionStatus;
        private CCAPIGetConsoleInfo_t getConsoleInfo;
        private CCAPIGetDllVersion_t getDllVersion;
        private CCAPIGetFirmwareInfo_t getFirmwareInfo;
        private CCAPIGetNumberOfConsoles_t getNumberOfConsoles;
        private CCAPIGetProcessList_t getProcessList;
        private CCAPIGetMemory_t getProcessMemory;
        private CCAPIGetProcessName_t getProcessName;
        private CCAPIGetTemperature_t getTemperature;
        private CCAPIVshNotify_t notify;
        private CCAPIRingBuzzer_t ringBuzzer;
        private CCAPISetBootConsoleIds_t setBootConsoleIds;
        private CCAPISetConsoleIds_t setConsoleIds;
        private CCAPISetConsoleLed_t setConsoleLed;
        private CCAPISetMemory_t setProcessMemory;
        private CCAPIShutdown_t shutdown;

        private IntPtr libModule = IntPtr.Zero;
        private List<IntPtr> CCAPIFunctionsList = new List<IntPtr>();

        private enum CCAPIFunctions
        {
            ConnectConsole,
            DisconnectConsole,
            GetConnectionStatus,
            GetConsoleInfo,
            GetDllVersion,
            GetFirmwareInfo,
            GetNumberOfConsoles,
            GetProcessList,
            GetMemory,
            GetProcessName,
            GetTemperature,
            VshNotify,
            RingBuzzer,
            SetBootConsoleIds,
            SetConsoleIds,
            SetConsoleLed,
            SetMemory,
            ShutDown
        }

        public CCAPI()
        {
            RegistryKey Key = Registry
                .CurrentUser
                .OpenSubKey(@"Software\FrenchModdingTeam\CCAPI\InstallFolder");

            if (Key != null)
            {
                string Path = Key.GetValue("path") as String;
                if (!string.IsNullOrEmpty(Path))
                {
                    string DllUrl = Path + @"\CCAPI.dll";
                    if (File.Exists(DllUrl))
                    {
                        if (libModule == IntPtr.Zero)
                            libModule = LoadLibrary(DllUrl);

                        if (libModule != IntPtr.Zero)
                        {
                            CCAPIFunctionsList.Clear();
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIConnectConsole"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIDisconnectConsole"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIGetConnectionStatus"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIGetConsoleInfo"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIGetDllVersion"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIGetFirmwareInfo"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIGetNumberOfConsoles"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIGetProcessList"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIGetMemory"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIGetProcessName"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIGetTemperature"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIVshNotify"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIRingBuzzer"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPISetBootConsoleIds"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPISetConsoleIds"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPISetConsoleLed"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPISetMemory"));
                            CCAPIFunctionsList.Add(GetProcAddress(libModule, "CCAPIShutdown"));

                            if (IsCCAPILoaded())
                            {
                                connectConsole = (CCAPIConnectConsole_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.ConnectConsole), typeof(CCAPIConnectConsole_t));
                                disconnectConsole = (CCAPIDisconnectConsole_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.DisconnectConsole), typeof(CCAPIDisconnectConsole_t));
                                getConnectionStatus = (CCAPIGetConnectionStatus_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.GetConnectionStatus), typeof(CCAPIGetConnectionStatus_t));
                                getConsoleInfo = (CCAPIGetConsoleInfo_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.GetConsoleInfo), typeof(CCAPIGetConsoleInfo_t));
                                getDllVersion = (CCAPIGetDllVersion_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.GetDllVersion), typeof(CCAPIGetDllVersion_t));
                                getFirmwareInfo = (CCAPIGetFirmwareInfo_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.GetFirmwareInfo), typeof(CCAPIGetFirmwareInfo_t));
                                getNumberOfConsoles = (CCAPIGetNumberOfConsoles_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.GetNumberOfConsoles), typeof(CCAPIGetNumberOfConsoles_t));
                                getProcessList = (CCAPIGetProcessList_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.GetProcessList), typeof(CCAPIGetProcessList_t));
                                getProcessMemory = (CCAPIGetMemory_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.GetMemory), typeof(CCAPIGetMemory_t));
                                getProcessName = (CCAPIGetProcessName_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.GetProcessName), typeof(CCAPIGetProcessName_t));
                                getTemperature = (CCAPIGetTemperature_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.GetTemperature), typeof(CCAPIGetTemperature_t));
                                notify = (CCAPIVshNotify_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.VshNotify), typeof(CCAPIVshNotify_t));
                                ringBuzzer = (CCAPIRingBuzzer_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.RingBuzzer), typeof(CCAPIRingBuzzer_t));
                                setBootConsoleIds = (CCAPISetBootConsoleIds_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.SetBootConsoleIds), typeof(CCAPISetBootConsoleIds_t));
                                setConsoleIds = (CCAPISetConsoleIds_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.SetConsoleIds), typeof(CCAPISetConsoleIds_t));
                                setConsoleLed = (CCAPISetConsoleLed_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.SetConsoleLed), typeof(CCAPISetConsoleLed_t));
                                setProcessMemory = (CCAPISetMemory_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.SetMemory), typeof(CCAPISetMemory_t));
                                shutdown = (CCAPIShutdown_t)Marshal.GetDelegateForFunctionPointer(GetCCAPIFunctionPtr(CCAPIFunctions.ShutDown), typeof(CCAPIShutdown_t));
                            }
                            else
                            {
                                MessageBox.Show("Impossible to load CCAPI 2.80+", "This CCAPI.dll is not compatible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Impossible to load CCAPI 2.80+", "CCAPI.dll cannot be loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("You need to install CCAPI 2.80+ to use this library.", "CCAPI.dll not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid CCAPI folder, please re-install it.", "CCAPI not installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("You need to install CCAPI 2.80+ to use this library.", "CCAPI not installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public enum IdType
        {
            IDPS,
            PSID
        }

        public enum NotifyIcon
        {
            INFO,
            CAUTION,
            FRIEND,
            SLIDER,
            WRONGWAY,
            DIALOG,
            DIALOGSHADOW,
            TEXT,
            POINTER,
            GRAB,
            HAND,
            PEN,
            FINGER,
            ARROW,
            ARROWRIGHT,
            PROGRESS,
            TROPHY1,
            TROPHY2,
            TROPHY3,
            TROPHY4
        }

        public enum ConsoleType
        {
            CEX = 1,
            DEX = 2,
            TOOL = 3
        }

        public enum ProcessType
        {
            VSH,
            SYS_AGENT,
            CURRENTGAME
        }

        public enum BootMode
        {
            ShutDown = 1,
            SoftReboot = 2,
            HardReboot = 3
        }

        public enum BuzzerMode
        {
            Continuous,
            Single,
            Double,
            Triple
        }

        public enum LedColor
        {
            Green = 1,
            Red = 2
        }

        public enum LedMode
        {
            Off,
            On,
            Blink
        }

        private TargetInfo pInfo = new TargetInfo();

        private IntPtr ReadDataFromUnBufPtr<T>(IntPtr unBuf, ref T storage)
        {
            storage = (T)Marshal.PtrToStructure(unBuf, typeof(T));
            return new IntPtr(unBuf.ToInt64() + Marshal.SizeOf((T)storage));
        }

        private class System
        {
            public static int
                connectionID = -1;
            public static uint
                processID = 0;
            public static uint[]
                processIDs;
        }

        /// <summary>Get informations from your target.</summary>
        public class TargetInfo
        {
            public int
                Firmware = 0,
                CCAPI = 0,
                ConsoleType = 0,
                TempCell = 0,
                TempRSX = 0;
            public ulong
                SysTable = 0;
        }

        /// <summary>Get Info for targets.</summary>
        public class ConsoleInfo
        {
            public string
                Name,
                Ip;
        }

        private IntPtr GetCCAPIFunctionPtr(CCAPIFunctions Function)
        {
            return CCAPIFunctionsList.ElementAt((int)Function);
        }

        private bool IsCCAPILoaded()
        {
            for (int i = 0; i < CCAPIFunctionsList.Count; i++)
                if (CCAPIFunctionsList.ElementAt(i) == IntPtr.Zero)
                    return false;
            return true;
        }

        private void CompleteInfo(ref TargetInfo Info, int fw, int ccapi, ulong sysTable, int consoleType, int tempCELL, int tempRSX)
        {
            Info.Firmware = fw;
            Info.CCAPI = ccapi;
            Info.SysTable = sysTable;
            Info.ConsoleType = consoleType;
            Info.TempCell = tempCELL;
            Info.TempRSX = tempRSX;
        }

        /// <summary>Return true if a ccapi function return a good integer.</summary>
        public bool SUCCESS(int Void)
        {
            if (Void == 0)
                return true;
            else return false;
        }

        public bool Connect()
        {
            return new PS3API.ConsoleList(new PS3API(SelectAPI.CCAPI)).Show();
        }


        /// <summary>Connect your console by ip address.</summary>
        public int Connect(string targetIP)
        {
            int code = connectConsole(targetIP);
            return code;
        }

        public int Disconnect()
        {
            return disconnectConsole();
        }

        public int GetProcList(out uint[] processIds)
        {
            uint numOfProcs = 64; int result = -1;
            IntPtr ptr = Marshal.AllocHGlobal((int)(4 * 0x40));
            result = getProcessList(ref numOfProcs, ptr);
            processIds = new uint[numOfProcs];
            if (SUCCESS(result))
            {
                IntPtr unBuf = ptr;
                for (uint i = 0; i < numOfProcs; i++)
                    unBuf = ReadDataFromUnBufPtr<uint>(unBuf, ref processIds[i]);
            }
            Marshal.FreeHGlobal(ptr);
            return result;
        }

        /// <summary>Get the process name of your choice.</summary>
        public int GetProcName(uint processId, out string name)
        {
            IntPtr ptr = Marshal.AllocHGlobal((int)(0x211)); int result = -1;
            result = getProcessName(processId, ptr);
            name = String.Empty;
            if (SUCCESS(result))
                name = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
            return result;
        }

        public int ProcAttach()
        {
            int result = -1; System.processID = 0;
            result = GetProcList(out System.processIDs);
            if (SUCCESS(result) && System.processIDs.Length > 0)
            {
                for (int i = 0; i < System.processIDs.Length; i++)
                {
                    string name = String.Empty;
                    result = GetProcName(System.processIDs[i], out name);
                    if (!SUCCESS(result))
                        break;
                    if (!name.Contains("flash"))
                    {
                        System.processID = System.processIDs[i];
                        break;
                    }
                    else result = -1;
                }
                if (System.processID == 0)
                    System.processID = System.processIDs[System.processIDs.Length - 1];
            }
            else result = -1;
            return result;
        }

        /// <summary>Attach your desired process.</summary>
        public int ProcAttach(ProcessType procType)
        {
            int result = -1; System.processID = 0;
            result = GetProcList(out System.processIDs);
            if (result >= 0 && System.processIDs.Length > 0)
            {
                for (int i = 0; i < System.processIDs.Length; i++)
                {
                    string name = String.Empty;
                    result = GetProcName(System.processIDs[i], out name);
                    if (result < 0)
                        break;
                    if (procType == ProcessType.VSH && name.Contains("vsh"))
                    {
                        System.processID = System.processIDs[i]; break;
                    }
                    else if (procType == ProcessType.SYS_AGENT && name.Contains("agent"))
                    {
                        System.processID = System.processIDs[i]; break;
                    }
                    else if (procType == ProcessType.CURRENTGAME && !name.Contains("flash"))
                    {
                        System.processID = System.processIDs[i]; break;
                    }
                }
                if (System.processID == 0)
                    System.processID = System.processIDs[System.processIDs.Length - 1];
            }
            else result = -1;
            return result;
        }

        public int ProcAttach(uint process)
        {
            int result = -1;
            uint[] procs = new uint[64];
            result = GetProcList(out procs);
            if (SUCCESS(result))
            {
                for (int i = 0; i < procs.Length; i++)
                {
                    if (procs[i] == process)
                    {
                        result = 0;
                        System.processID = process;
                        break;
                    }
                    else result = -1;
                }
            }
            procs = null;
            return result;
        }

        /// <summary>Return the current process attached. Use this function only if you called AttachProcess before.</summary>
        public uint GetAttachedProcess()
        {
            return System.processID;
        }

        public int Buzzer(BuzzerMode Mode)
        {
            return ringBuzzer((int)Mode);
        }

        public int Notify(NotifyIcon Icon, string Message)
        {
            return notify((int)Icon, Message);
        }

        public int Notify(int Icon, string Message)
        {
            return notify(Icon, Message);
        }

        public int BootFunctions(BootMode flag)
        {
            return shutdown((int)flag);
        }

        public int SetProcMem(uint offset, byte[] buffer)
        {
            return setProcessMemory(System.processID, (ulong)offset, (uint)buffer.Length, buffer);
        }

        /// <summary>Set memory to offset (ulong).</summary>
        public int SetProcMem(ulong offset, byte[] buffer)
        {
            return setProcessMemory(System.processID, offset, (uint)buffer.Length, buffer);
        }

        /// <summary>Set memory to offset (string hex).</summary>
        public int SetProcMem(ulong offset, string hexadecimal, EndianType Type = EndianType.BigEndian)
        {
            byte[] Entry = StringToByteArray(hexadecimal);
            if (Type == EndianType.LittleEndian)
                Array.Reverse(Entry);
            return setProcessMemory(System.processID, offset, (uint)Entry.Length, Entry);
        }

        /// <summary>Get memory from offset (uint).</summary>
        public int GetProcMem(uint offset, byte[] buffer)
        {
            return getProcessMemory(System.processID, (ulong)offset, (uint)buffer.Length, buffer);
        }

        /// <summary>Get memory from offset (ulong).</summary>
        public int GetProcMem(ulong offset, byte[] buffer)
        {
            return getProcessMemory(System.processID, offset, (uint)buffer.Length, buffer);
        }

        public byte[] GetBytes(uint offset, uint length)
        {
            byte[] buffer = new byte[length];
            GetProcMem(offset, buffer);
            return buffer;
        }

        /// <summary>Like Get memory but this function return directly the buffer from the offset (ulong).</summary>
        public byte[] GetBytes(ulong offset, uint length)
        {
            byte[] buffer = new byte[length];
            GetProcMem(offset, buffer);
            return buffer;
        }

        public int ConsoleLED(LedColor Color, LedMode Mode)
        {
            return setConsoleLed((int)Color, (int)Mode);
        }

        private int GetTargetInfo()
        {
            int result = -1; int[] sysTemp = new int[2];
            int fw = 0, ccapi = 0, consoleType = 0; ulong sysTable = 0;
            result = getFirmwareInfo(ref fw, ref ccapi, ref consoleType);
            if (result >= 0)
            {
                result = getTemperature(ref sysTemp[0], ref sysTemp[1]);
                if (result >= 0)
                    CompleteInfo(ref pInfo, fw, ccapi, sysTable, consoleType, sysTemp[0], sysTemp[1]);
            }

            return result;
        }

        /// <summary>Get informations of your console and store them into TargetInfo class.</summary>
        public int GetTargetInfo(out TargetInfo Info)
        {
            Info = new TargetInfo();
            int result = -1; int[] sysTemp = new int[2];
            int fw = 0, ccapi = 0, consoleType = 0; ulong sysTable = 0;
            result = getFirmwareInfo(ref fw, ref ccapi, ref consoleType);
            if (result >= 0)
            {
                result = getTemperature(ref sysTemp[0], ref sysTemp[1]);
                if (result >= 0)
                {
                    CompleteInfo(ref Info, fw, ccapi, sysTable, consoleType, sysTemp[0], sysTemp[1]);
                    CompleteInfo(ref pInfo, fw, ccapi, sysTable, consoleType, sysTemp[0], sysTemp[1]);
                }
            }
            return result;
        }

        /// <summary>Return the current firmware of your console in string format.</summary>
        public string GetFirmwareVersion()
        {
            if (pInfo.Firmware == 0)
                GetTargetInfo();

            string ver = pInfo.Firmware.ToString("X8");
            string char1 = ver.Substring(1, 1) + ".";
            string char2 = ver.Substring(3, 1);
            string char3 = ver.Substring(4, 1);
            return char1 + char2 + char3;
        }

        /// <summary>Return the current temperature of your system in string.</summary>
        public string GetCELLTemp()
        {
            if (pInfo.TempCell == 0)
                GetTargetInfo(out pInfo);

            return pInfo.TempCell.ToString() + " °C";
        }

        /// <summary>Return the current temperature of your system in string.</summary>
        public string GetRSXTemp()
        {
            if (pInfo.TempRSX == 0)
                GetTargetInfo(out pInfo);
            return pInfo.TempRSX.ToString() + " °C";
        }

        /// <summary>Return the type of your firmware in string format.</summary>
        public string GetFirmwareType()
        {
            if (pInfo.ConsoleType == 0)
                GetTargetInfo(out pInfo);
            string type = "UNK";
            if (pInfo.ConsoleType == (int)ConsoleType.CEX)
                type = "CEX";
            else if (pInfo.ConsoleType == (int)ConsoleType.DEX)
                type = "DEX";
            else if (pInfo.ConsoleType == (int)ConsoleType.TOOL)
                type = "DECR";
            return type;
        }

        public void ClearTargetInfo()
        {
            pInfo = new TargetInfo();
        }

        /// <summary>Set a new ConsoleID in real time. (string)</summary>
        public int SetConsoleID(string consoleID)
        {
            if (string.IsNullOrEmpty(consoleID))
            {
                MessageBox.Show("This is an empty value, it's invalid.", "Empty or null console id", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            string newCID = String.Empty;
            if (consoleID.Length >= 32)
                newCID = consoleID.Substring(0, 32);
            return SetConsoleID(StringToByteArray(newCID));
        }

        /// <summary>Set a new ConsoleID in real time. (bytes)</summary>
        public int SetConsoleID(byte[] consoleID)
        {
            if (consoleID.Length <= 0)
            {
                MessageBox.Show("This is an empty value, it's invalid.", "Empty or null console id", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            return setConsoleIds((int)IdType.IDPS, consoleID);
        }

        /// <summary>Set a new PSID in real time. (string)</summary>
        public int SetPSID(string PSID)
        {
            if (string.IsNullOrEmpty(PSID))
            {
                MessageBox.Show("This is an empty value, it's invalid.", "Empty or null psid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            string PS_ID = String.Empty;
            if (PSID.Length >= 32)
                PS_ID = PSID.Substring(0, 32);
            return SetPSID(StringToByteArray(PS_ID));
        }

        /// <summary>Set a new PSID in real time. (bytes)</summary>
        public int SetPSID(byte[] consoleID)
        {
            if (consoleID.Length <= 0)
            {
                MessageBox.Show("This is an empty value, it's invalid.", "Empty or null psid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            return setConsoleIds((int)IdType.PSID, consoleID);
        }

        /// <summary>Set a console ID when the console is running. (string)</summary>
        public int SetBootConsoleID(string consoleID, IdType Type = IdType.IDPS)
        {
            string newCID = String.Empty;
            if (consoleID.Length >= 32)
                newCID = consoleID.Substring(0, 32);
            return SetBootConsoleID(StringToByteArray(consoleID), Type);
        }

        /// <summary>Set a console ID when the console is running. (bytes)</summary>
        public int SetBootConsoleID(byte[] consoleID, IdType Type = IdType.IDPS)
        {
            return setBootConsoleIds((int)Type, 1, consoleID);
        }

        /// <summary>Reset a console ID when the console is running.</summary>
        public int ResetBootConsoleID(IdType Type = IdType.IDPS)
        {
            return setBootConsoleIds((int)Type, 0, null);
        }

        public int GetDllVersion()
        {
            return getDllVersion();
        }

        /// <summary>Return a list of informations for each console available.</summary>
        public List<ConsoleInfo> GetConsoleList()
        {
            List<ConsoleInfo> data = new List<ConsoleInfo>();
            int targetCount = getNumberOfConsoles();
            IntPtr name = Marshal.AllocHGlobal((int)(512)),
                   ip = Marshal.AllocHGlobal((int)(512));
            for (int i = 0; i < targetCount; i++)
            {
                ConsoleInfo Info = new ConsoleInfo();
                getConsoleInfo(i, name, ip);
                Info.Name = Marshal.PtrToStringAnsi(name);
                Info.Ip = Marshal.PtrToStringAnsi(ip);
                data.Add(Info);
            }
            Marshal.FreeHGlobal(name);
            Marshal.FreeHGlobal(ip);
            return data;
        }

        internal static byte[] StringToByteArray(string hex)
        {
            try
            {
                string replace = hex.Replace("0x", "");
                string Stringz = replace.Insert(replace.Length - 1, "0");

                int Odd = replace.Length;
                bool Nombre;
                if (Odd % 2 == 0)
                    Nombre = true;
                else
                    Nombre = false;
                if (Nombre == true)
                {
                    return Enumerable.Range(0, replace.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(replace.Substring(x, 2), 16))
                    .ToArray();
                }
                else
                {
                    return Enumerable.Range(0, replace.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(Stringz.Substring(x, 2), 16))
                    .ToArray();
                }
            }
            catch
            {
                MessageBox.Show("Incorrect value (empty)", "StringToByteArray Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new byte[1];
            }
        }

    }
}
