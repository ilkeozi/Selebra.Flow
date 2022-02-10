using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow
{
    public static class Utils
    {
        public static byte[] ToMACBytes(this string mac)
        {
            if (mac.IndexOf(':') > 0)
                mac = mac.Replace(':', '-');
            return PhysicalAddress.Parse(mac).GetAddressBytes();
        }

       
    }
}
