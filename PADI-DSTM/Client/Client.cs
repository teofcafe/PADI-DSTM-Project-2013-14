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
using CoordinatorLibrary;
using ServerLibrary;
using TransactionLibrary;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {

            //TcpChannel channel = new TcpChannel();
            //ChannelServices.RegisterChannel(channel, true);
            //IServer server = (IServer)Activator.GetObject(typeof(IServer), "tcp://localhost:8082/Server");
            //server.CreatePadInt(30, new TimeStamp());

            Library.Library library = new Library.Library();

            library.Init();

            library.TxBegin();

           PadInt padint =  library.CreatePadInt(10);
           
            Console.ReadLine();
        }
    }
}
