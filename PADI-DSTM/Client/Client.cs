using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Library;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            Library.Library library = new Library.Library();

            library.Init();

            library.TxBegin();
            
            Console.ReadLine();
        }
    }
}
