using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Davinchi_Code
{
    class sendClass
    {
        NetworkStream stream;
        string msg;

        public void setMsg(NetworkStream st, string ms)
        {
            stream = st;
            msg = ms;
        }

        public void sendMessage()
        {
            byte[] message = new byte[256]; // 인코딩된 message
            message = System.Text.Encoding.Default.GetBytes(msg); // 인코딩 과정
            stream.Write(message, 0, message.Length); // 소켓 스트림으로 전송
            msg = "";
        }
    }
}
