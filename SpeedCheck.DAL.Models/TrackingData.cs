using System;
using System.Runtime.InteropServices;

/*
   - Дата и время фиксации 				20.12.2019 14:31:25 
   - Номер транспортного средства 			1234 PP-7
   - Скорость движения км/ч 				65,5
   
*/

namespace SpeedCheck.DAL.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    [Serializable]
    public struct TrackingData
    {
        public DateTime CheckTime; //8

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public string RegistrationNumber;

        //public ReadOnlySpan<char> RegistrationNumber;

        public double Speed; //8
    }
}
