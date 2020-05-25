using System;
using System.Runtime.InteropServices;

namespace SpeedCheck.DAL.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    [Serializable]
    public struct TrackingData
    {
        public DateTime CheckTime; //8

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string RegistrationNumber;

        public double Speed; //8
    }
}
