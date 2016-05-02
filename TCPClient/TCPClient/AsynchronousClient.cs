using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace TCPClient
{
    class AsynchronousClient
    {
        // The port number for the remote device.
       

        public static int Main(String[] args)
        {
            ChatScreen chat = new ChatScreen();
            chat.ShowDialog();
            return 0;
        }
    }
}
