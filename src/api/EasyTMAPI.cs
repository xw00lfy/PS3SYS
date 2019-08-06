using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS3System
{
    class EasyTMAPI
    {
        public static uint ProcessID;
        public static uint[] processIDs;
        
        private static string usage;
        public static PS3TMAPI.ConnectStatus connectStatus;
        public static int Target = 0xFF;
        public static bool AssemblyLoaded = true;
        public static PS3TMAPI.ResetParameter resetParameter;


        /// <summary>
        /// Initializes Communication with DLL
        /// </summary>
        public void InitTargetComms()
        {
            PS3TMAPI.InitTargetComms();
        }

        /// <summary>
        /// Closes Communication with DLL
        /// </summary>
        public void CloseTargetComms()
        {
            PS3TMAPI.CloseTargetComms();
        }
        /// <summary>
        /// Directly connects to the target
        /// </summary>
        public void Connect()
        {
            PS3TMAPI.InitTargetComms();
            PS3TMAPI.Connect(0, null);
        }

        public void ProcAttach()
        {
            PS3TMAPI.GetProcessList(0, out processIDs);
            ulong uProcess = processIDs[0];
            ProcessID = Convert.ToUInt32(uProcess);
            PS3TMAPI.ProcessAttach(0, PS3TMAPI.UnitType.PPU, ProcessID);
            PS3TMAPI.ProcessContinue(0, ProcessID);
        }
        /// <summary>
        /// Connects to target with the specified target inside the parameters.
        /// </summary>
        /// <param name="Target"></param>
        public void Connect(int Target = 0)
        {
            PS3TMAPI.InitTargetComms();
            PS3TMAPI.Connect(Target, null);
        }
        /// <summary>
        /// Disconnects from the target
        /// </summary>
        public void Disconnect()
        {
            PS3TMAPI.Disconnect(0);
        }
        /// <summary>
        /// Forces the ps3 to disconnect
        /// </summary>
        public void ForceDisconnect()
        {
            PS3TMAPI.ForceDisconnect(0);
        }

        public void PowerOff(bool Force)
        {
            PS3TMAPI.PowerOff(0, false);
        }

        public void PowerOff(int Target, bool Force)
        {
            PS3TMAPI.PowerOff(Target, Force);
        }

        public void PowerOn(int Target)
        {
            PS3TMAPI.PowerOn(0);
        }

        public void PowerOn()
        {
            PS3TMAPI.PowerOn(0);
        }

        public void GetProcMem(uint Offset, byte[] bytes)
        {
            PS3TMAPI.ProcessGetMemory(0, PS3TMAPI.UnitType.PPU, ProcessID, 0, Offset, ref bytes);
        }

        public void SetProcMem(uint Offset, byte[] bytes)
        {
            PS3TMAPI.ProcessSetMemory(0, PS3TMAPI.UnitType.PPU, ProcessID, 0, Offset, bytes);
        }

        public void GetMACAddress(string Output)
        {
            PS3TMAPI.GetMACAddress(0, out Output);
        }
    }
}
