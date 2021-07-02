using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Davinchi_Code
{
    static class internetComponent
    {
        public static string getMyIP()
        {
            IPHostEntry IPHost = Dns.GetHostByName(Dns.GetHostName());
            return IPHost.AddressList[0].ToString();
        }

        public static string getServerIP()
        {
            IPHostEntry server = Dns.GetHostByName("muxacarin.iptime.org");
            return server.AddressList[0].ToString();
        }
    }
}
